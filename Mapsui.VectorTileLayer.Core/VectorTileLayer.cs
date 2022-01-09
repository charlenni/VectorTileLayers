using BruTile;
using BruTile.Cache;
using Mapsui.Extensions;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Rendering;
using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Primitives;
using Mapsui.VectorTileLayer.Core.Styles;
using Mapsui.VectorTileLayer.Core.Utilities;
using System;
using System.IO;
using System.IO.Compression;

namespace Mapsui.VectorTileLayer.Core
{
    /// <summary>
    /// Layer, which displays a map consisting of individual vector tiles
    /// </summary>
    public class VectorTileLayer
    {
        public static TileLayer CreateVectorTileLayer(VectorTileStyle style, ITileSource tileSource, int minTiles = 200, int maxTiles = 300,
            IDataFetchStrategy? dataFetchStrategy = null, IRenderFetchStrategy? renderFetchStrategy = null,
            int minExtraTiles = -1, int maxExtraTiles = -1, Func<TileInfo, IFeature?>? fetchTileAsFeature = null, ITileDataParser tileDataParser = null)
        {
            var vectorTileLayer = new VectorTileLayer(style, null, tileDataParser);
            var tileLayer = new TileLayer(tileSource, minTiles, maxTiles, dataFetchStrategy, renderFetchStrategy, minExtraTiles, maxExtraTiles, vectorTileLayer.FetchTileAsFeature);

            vectorTileLayer.TileLayer = tileLayer;

            return tileLayer;
        }

        public VectorTileLayer(VectorTileStyle style, TileLayer tileLayer, ITileDataParser tileDataParser = null)
        {
            _style = style;
            _tileLayer = new WeakReference<TileLayer>(tileLayer);
            _tileDataParser = tileDataParser ?? throw new ArgumentNullException("TileDataParser must not be null");
            _memoryCache = new Utilities.MemoryCache<IFeature>(200, 300);
        }

        private VectorTileStyle _style;

        private ITileDataParser _tileDataParser;

        private readonly WeakReference<TileLayer> _tileLayer;

        private Utilities.MemoryCache<IFeature> _memoryCache;

        internal TileLayer TileLayer
        {
            get
            {
                if (_tileLayer.TryGetTarget(out TileLayer tileLayer))
                    return tileLayer;
                return null;
            }
            set
            {
                _tileLayer.SetTarget(value);
            }
        }

        public int TileSize { get; } = 512;

        public IFeature? FetchTileAsFeature(TileInfo tileInfo)
        {
            if (TileLayer?.TileSource == null)
                return null;

            if (_memoryCache.TryGet(tileInfo.Index, out IFeature feature))
                return feature;

            // We don't create tiles with zoom higher than TileSource could provide
            if (tileInfo.Index.Level > TileLayer?.TileSource.Schema.Resolutions.Count - 1)
                return null;

            // Get binary data from tile source as byte[]
            var (tileData, overzoom) = GetTileData(tileInfo);

            // We find data in the tile source for this tile, perhaps on lower levels
            if (tileData != null) // && overzoom == Overzoom.None)
                feature = ToVectorTileFeature(tileInfo, overzoom, ref tileData);

            _memoryCache.Add(tileInfo.Index, feature);

            return feature;
        }

        /// <summary>
        /// Get data for tile
        /// </summary>
        /// <remarks>
        /// If this tile couldn't be found, than we try to get tile data for a tile with lower zoom level
        /// </remarks>
        /// <param name="tileInfo">Tile info for tile to get data for</param>
        /// <returns>Raw tile data, factor for enlargement for this data and offsets for parts of this data, which to use</returns>
        private (byte[], Overzoom) GetTileData(TileInfo tileInfo)
        {
            var zoom = tileInfo.Index.Level;
            var scale = 1;
            var offsetX = 0f;
            var offsetY = 0f;  //(Source.Schema.YAxis == YAxis.TMS ? -4096f : 0f);
            var offsetFactor = 4096;

            // Check MinZoom of source. MaxZoom isn't checked, because of overzoom
            if (zoom < 0)
                return (null, Overzoom.None);

            Logging.Logger.Log(Logging.LogLevel.Information, $"Before GetTile from source at {System.DateTime.Now.Ticks}: {tileInfo.Index.Col}/{tileInfo.Index.Row}/{tileInfo.Index.Level}");

            // Get byte data for this tile
            var tileData = TileLayer.TileSource.GetTile(tileInfo);

            Logging.Logger.Log(Logging.LogLevel.Information, $"After GetTile from source at {System.DateTime.Now.Ticks}: {tileInfo.Index.Col}/{tileInfo.Index.Row}/{tileInfo.Index.Level}");

            if (tileData != null)
                return (tileData, Overzoom.None);

            // We only create overzoom tiles when zoom is between min and max zoom
            //if (zoom < MinZoom || zoom > MaxZoom)
            //    return (null, Overzoom.None);

            var info = new TileInfo { Index = new TileIndex(tileInfo.Index.Col, tileInfo.Index.Row, tileInfo.Index.Level) };
            var row = info.Index.Row;
            var col = info.Index.Col;

            while (tileData == null && zoom >= 0)
            {
                scale <<= 1;
                offsetX = offsetX + (col % 2) * offsetFactor;
                offsetY = offsetY + (row % 2) * offsetFactor * (TileLayer.TileSource.Schema.YAxis == YAxis.TMS ? +1f : -1f);
                var doubleWidth = info.Extent.Width * 2.0;
                var doubleHeight = info.Extent.Height * 2.0;
                //var minX = info.Extent.MinX  ((col % 2) * halfWidth);
                //var minY = info.Extent.MinY + ((row % 2) * halfHeight);
                zoom--;
                row >>= 1;
                col >>= 1;
                offsetFactor <<= 1;
                //info.Extent = new Extent(minX, minY, minX + halfWidth, minY + halfHeight);
                info.Index = new TileIndex(col, row, zoom);
                tileData = TileLayer.TileSource.GetTile(info);
            }

            if (zoom < 0)
                return (null, Overzoom.None);

            offsetY = offsetFactor - offsetY + (TileLayer.TileSource.Schema.YAxis == YAxis.TMS ? -4096f : 0f);

            var overzoom = new Overzoom(scale, offsetX, offsetY);

            return (tileData, overzoom);
        }

        private VectorTile? ToVectorTileFeature(TileInfo tileInfo, Overzoom overzoom, ref byte[]? tileData)
        {
            // A TileSource may return a byte array that is null. This is currently only implemented
            // for MbTilesTileSource. It is to indicate that the tile is not present in the source,
            // although it should be given the tile schema. It does not mean the tile could not
            // be accessed because of some temporary reason. In that case it will throw an exception.
            // For Mapsui this is important because it will not try again and again to fetch it. 
            // Here we return the geometry as null so that it will be added to the tile cache. 
            // TileLayer.GetFeatureInView will have to return only the non null geometries.

            if (tileData == null) 
                return null;

            var sink = new VectorTile(tileInfo, TileSize, ref _style, ref tileData, new MRect(0, 0, 0, 0));

            // Parse tile and convert it to a feature list
            Stream stream = new MemoryStream(tileData);

            if (IsGZipped(stream))
                stream = new GZipStream(stream, CompressionMode.Decompress);

            try
            {
                _tileDataParser.Parse(tileInfo, stream, sink, overzoom, new TileClipper(new MRect(-8, -8, TileSize + 8, TileSize + 8)));
            }
            catch (Exception e)
            {
                var test = e.Message;
            }

            return sink;
        }

        /// <summary>
        /// Check, if stream contains gzipped data 
        /// </summary>
        /// <param name="stream">Stream to check</param>
        /// <returns>True, if the stream is gzipped</returns>
        private static bool IsGZipped(Stream stream)
        {
            return IsZipped(stream, 3, "1F-8B-08");
        }

        /// <summary>
        /// Check, if stream contains zipped data
        /// </summary>
        /// <param name="stream">Stream to check</param>
        /// <param name="signatureSize">Length of bytes to check for signature</param>
        /// <param name="expectedSignature">Signature to check</param>
        /// <returns>True, if the stream is zipped</returns>
        private static bool IsZipped(Stream stream, int signatureSize = 4, string expectedSignature = "50-4B-03-04")
        {
            if (stream.Length < signatureSize)
                return false;

            byte[] signature = new byte[signatureSize];
            int bytesRequired = signatureSize;
            int index = 0;

            while (bytesRequired > 0)
            {
                int bytesRead = stream.Read(signature, index, bytesRequired);
                bytesRequired -= bytesRead;
                index += bytesRead;
            }

            stream.Seek(0, SeekOrigin.Begin);

            string actualSignature = BitConverter.ToString(signature);
            if (actualSignature == expectedSignature)
                return true;

            return false;
        }
    }
}