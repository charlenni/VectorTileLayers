using BruTile;
using BruTile.Cache;
using ExCSS;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Tiling.Extensions;
using Mapsui.Tiling.Fetcher;
using Mapsui.Tiling.Layers;
using Mapsui.Tiling.Rendering;
using Mapsui.Tiling.Utilities;
using Mapsui.VectorTileLayers.Core;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Interfaces;
using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayers.Core.Styles;
using Mapsui.VectorTileLayers.Core.Utilities;
using Mapsui.VectorTileLayers.OpenMapTiles.Parser;
using RBush;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    /// <summary>
    /// Layer, which displays a map consisting of individual tiles
    /// </summary>
    public class OMTVectorTileLayer : BaseLayer, IVectorTileLayer, IAsyncDataFetcher, IDisposable
    {
        private const int TileSizeOfData = 4096;

        private readonly ITileSource _tileSource;
        private readonly IRenderFetchStrategy _renderFetchStrategy;
        private readonly int _minExtraTiles;
        private readonly int _maxExtraTiles;
        private int _numberTilesNeeded;
        private readonly TileFetchDispatcher _tileFetchDispatcher;
        private readonly MRect? _extent;
        private readonly HttpClient _httpClient = new();
        private int _treeZoomLevel = -1;
        private int _treeMinCol = int.MaxValue;
        private int _treeMaxCol = int.MinValue;
        private int _treeMinRow = int.MaxValue;
        private int _treeMaxRow = int.MinValue;
        private readonly ITileDataParser _tileDataParser = new MGLTileParser();
        private CancellationTokenSource _cancelToken;

        /// <summary>
        /// Create vector tile layer for given tile source
        /// </summary>
        /// <param name="tileSource">Tile source to use for this layer</param>
        /// <param name="minTiles">Minimum number of tiles to cache</param>
        /// <param name="maxTiles">Maximum number of tiles to cache</param>
        /// <param name="dataFetchStrategy">Strategy to get list of tiles for given extent</param>
        /// <param name="renderFetchStrategy"></param>
        /// <param name="minExtraTiles">Number of minimum extra tiles for memory cache</param>
        /// <param name="maxExtraTiles">Number of maximum extra tiles for memory cache</param>
        /// <param name="fetchTileAsFeature">Fetch tile as feature</param>
        // ReSharper disable once UnusedParameter.Local // Is public and won't break this now
        public OMTVectorTileLayer(IEnumerable<IVectorStyle> vectorStyles, ITileSource tileSource, int minTiles = 200, int maxTiles = 300,
            IDataFetchStrategy? dataFetchStrategy = null, IRenderFetchStrategy? renderFetchStrategy = null,
            int minExtraTiles = -1, int maxExtraTiles = -1, Func<TileInfo, Task<IFeature?>>? fetchTileAsFeature = null)
        {
            _tileSource = tileSource ?? throw new ArgumentException($"{tileSource} can not null");
            MemoryCache = new MemoryCache<IFeature?>(minTiles, maxTiles);
            Style = new VectorTileStyle(0, 24, vectorStyles);
            Attribution.Text = _tileSource.Attribution.Text;
            Attribution.Url = _tileSource.Attribution.Url;
            _extent = _tileSource.Schema?.Extent.ToMRect();
            dataFetchStrategy ??= new DataFetchStrategy(3);
            _renderFetchStrategy = renderFetchStrategy ?? new RenderFetchStrategy();
            _minExtraTiles = minExtraTiles;
            _maxExtraTiles = maxExtraTiles;
            _tileFetchDispatcher = new TileFetchDispatcher(MemoryCache, _tileSource.Schema, fetchTileAsFeature ?? FetchTileAsVectorTile, dataFetchStrategy);
            _tileFetchDispatcher.DataChanged += TileFetchDispatcherOnDataChanged;
            _tileFetchDispatcher.PropertyChanged += TileFetchDispatcherOnPropertyChanged;
            // There should be a way to override the application wide default user agent.
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", HttpClientTools.GetDefaultApplicationUserAgent());
        }

        /// <summary>
        /// Tile size for this type of layer is always 512 x 512 (OpenMapTiles and Mapbox GL)
        /// </summary>
        public int TileSize { get; } = 512;

        /// <summary>
        /// Tree of all visible symbols in the viewable part of this layer
        /// </summary>
        public RBush<Symbol> Tree { get; private set; }

        /// <summary>
        /// TileSource</summary>
        public ITileSource TileSource => _tileSource;

        /// <summary>
        /// Memory cache for this layer
        /// </summary>
        private MemoryCache<IFeature?> MemoryCache { get; }

        /// <inheritdoc />
        public override IReadOnlyList<double> Resolutions => _tileSource.Schema.Resolutions.Select(r => r.Value.UnitsPerPixel).ToList();

        /// <inheritdoc />
        public override MRect? Extent => _extent;

        /// <inheritdoc />
        public override IEnumerable<IFeature> GetFeatures(MRect extent, double resolution)
        {
            if (_tileSource.Schema == null) return [];
            UpdateMemoryCacheMinAndMax();
            var features = _renderFetchStrategy.Get(extent, resolution, _tileSource.Schema, MemoryCache);

            // Check, if tree should be updated
            CheckForTreeUpdate(features, resolution);

            return features;
        }

        /// <inheritdoc />
        public void AbortFetch()
        {
            _tileFetchDispatcher.StopFetching();
        }

        /// <inheritdoc />
        public void ClearCache()
        {
            MemoryCache.Clear();
        }

        /// <inheritdoc />
        public void RefreshData(FetchInfo fetchInfo)
        {
            if (Enabled
                && fetchInfo.Extent?.GetArea() > 0
                && MaxVisible >= fetchInfo.Resolution
                && MinVisible <= fetchInfo.Resolution)
            {
                _tileFetchDispatcher.RefreshData(fetchInfo);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                MemoryCache.Dispose();
                _httpClient.Dispose();
            }

            base.Dispose(disposing);
        }

        private void TileFetchDispatcherOnPropertyChanged(object? sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(Busy))
                Busy = _tileFetchDispatcher.Busy;
        }

        private void UpdateMemoryCacheMinAndMax()
        {
            if (_minExtraTiles < 0 || _maxExtraTiles < 0) return;
            if (_numberTilesNeeded == _tileFetchDispatcher.NumberTilesNeeded) return;

            _numberTilesNeeded = _tileFetchDispatcher.NumberTilesNeeded;
            MemoryCache.MinTiles = _numberTilesNeeded + _minExtraTiles;
            MemoryCache.MaxTiles = _numberTilesNeeded + _maxExtraTiles;
        }

        private void TileFetchDispatcherOnDataChanged(object? sender, Exception? ex)
        {
            OnDataChanged(new DataChangedEventArgs(ex, Name));
        }

        private void CheckForTreeUpdate(IEnumerable<IFeature> features, double resolution)
        {
            var zoomLevel = (int)resolution.ToZoomLevel();

            var minCol = int.MaxValue;
            var minRow = int.MaxValue;
            var maxCol = int.MinValue;
            var maxRow = int.MinValue;

            var tiles = new List<TileInfo>();

            foreach (VectorTileFeature feature in features)
            {
                tiles.Add(feature.TileInfo);

                // Only update, if all tiles belong to the same zoom level
                //if (vectorTileFeature.TileInfo.Index.Level == zoomLevel)
                {
                    minCol = Math.Min(feature.TileInfo.Index.Col, minCol);
                    minRow = Math.Min(feature.TileInfo.Index.Row, minRow);
                    maxCol = Math.Max(feature.TileInfo.Index.Col, minCol);
                    maxRow = Math.Max(feature.TileInfo.Index.Row, minRow);
                }
            }

            if (zoomLevel != _treeZoomLevel || minCol != _treeMinCol || minRow != _treeMinRow || maxCol != _treeMaxCol || maxRow != _treeMaxRow)
            {
                // Something changed, so the tree should be updated
                RefreshTree(features, zoomLevel, minCol, minRow, maxCol, maxRow);
            }
        }

        private void RefreshTree(IEnumerable<IFeature> vectorTiles, int zoomLevel, int minCol, int minRow, int maxCol, int maxRow)
        {
            if (_cancelToken != null)
            {
                // There is already a layouter running, so cancel it
                _cancelToken.Cancel();
                _cancelToken = null;
            }

            _cancelToken = new CancellationTokenSource();
            var token = _cancelToken.Token;

            var task = new Task(() =>
            {
                var watch = new System.Diagnostics.Stopwatch();

                watch.Start();

                var tree = OMTSymbolLayouter.Layout(((VectorTileStyle)Style).StyleLayers, vectorTiles, zoomLevel, minCol, minRow, token);

                if (!token.IsCancellationRequested)
                {
                    watch.Stop();

                    Tree = tree;

                    _treeMinCol = minCol;
                    _treeMinRow = minRow;
                    _treeMaxCol = maxCol;
                    _treeMaxRow = maxRow;

                    _treeZoomLevel = zoomLevel;

#if DEBUG
                    Logger.Log(LogLevel.Information, $"Created a new tree in {watch.ElapsedMilliseconds} ms");
#endif

                    OnDataChanged(new DataChangedEventArgs(Name));
                }
            }, token);

            task.Start();
        }

        public async Task<IFeature> FetchTileAsVectorTile(TileInfo tileInfo)
        {
            if (TileSource == null)
                return null;

            var vectorTileFeature = (VectorTileFeature)MemoryCache.Find(tileInfo.Index);

            if (vectorTileFeature != null)
                return vectorTileFeature;

            // We don't create tiles with zoom higher than TileSource could provide
            //if (tileInfo.Index.Level > _tileSource.Schema.Resolutions.Count - 1)
            //    return null;

            // Get binary data from tile source as byte[]
            var (tileData, overzoom) = GetTileData(tileInfo);

            // We find data in the tile source for this tile, perhaps on lower levels
            if (tileData != null) // && overzoom == Overzoom.None)
                vectorTileFeature = ToVectorTile(tileInfo, overzoom, ref tileData);

            MemoryCache.Add(tileInfo.Index, vectorTileFeature);

            return vectorTileFeature;
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
            var offsetY = 0f;
            var offsetFactor = TileSizeOfData;

            // Check MinZoom of source. MaxZoom isn't checked, because of overzoom
            if (zoom < 0)
                return (null, Overzoom.None);

#if DEBUG
            Logger.Log(Logging.LogLevel.Information, $"Before GetTile from source at {DateTime.Now.Ticks}: {tileInfo.Index.Col}/{tileInfo.Index.Row}/{tileInfo.Index.Level}");
#endif

            // Get byte data for this tile
            byte[] tileData;

            if (TileSource is ILocalTileSource)
                tileData = ((ILocalTileSource)_tileSource).GetTileAsync(tileInfo).Result;
            else
                tileData = ((IHttpTileSource)_tileSource).GetTileAsync(_httpClient, tileInfo).Result;

#if DEBUG
            Logger.Log(Logging.LogLevel.Information, $"After GetTile from source at {DateTime.Now.Ticks}: {tileInfo.Index.Col}/{tileInfo.Index.Row}/{tileInfo.Index.Level}");
#endif

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
                offsetY = offsetY + (row % 2) * offsetFactor * (TileSource.Schema.YAxis == YAxis.TMS ? +1f : -1f);
                zoom--;
                row >>= 1;
                col >>= 1;
                offsetFactor <<= 1;
                //info.Extent = new Extent(minX, minY, minX + halfWidth, minY + halfHeight);
                info.Index = new TileIndex(col, row, zoom);

                if (TileSource is ILocalTileSource)
                    tileData = ((ILocalTileSource)_tileSource).GetTileAsync(info).Result;
                else
                    tileData = ((IHttpTileSource)_tileSource).GetTileAsync(_httpClient, info).Result;
            }

            if (zoom < 0)
                return (null, Overzoom.None);

            offsetY = offsetFactor - offsetY + (TileSource.Schema.YAxis == YAxis.TMS ? -TileSizeOfData : 0f);

            var overzoom = new Overzoom(scale, offsetX, offsetY);

            return (tileData, overzoom);
        }

        private VectorTileFeature ToVectorTile(TileInfo tileInfo, Overzoom overzoom, ref byte[] tileData)
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

            var sink = new VectorTileFeature(tileInfo, TileSize, TileSizeOfData, Style);

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
                Logger.Log(LogLevel.Error, $"Exception while parsing tile {tileInfo.Index.Col}/{tileInfo.Index.Row}/{tileInfo.Index.Level}", e);
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
