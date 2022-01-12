using BruTile;
using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Primitives;
using Mapsui.VectorTileLayer.Core.Utilities;
using Mapsui.VectorTileLayer.OpenMapTiles.Pbf;
using ProtoBuf;
using System.IO;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Parser
{
    public class MGLTileParser : ITileDataParser
    {
        /// <summary>
        /// Parses a unzipped tile in Mapbox format
        /// </summary>
        /// <param name="tileInfo">TileInfo of this tile</param>
        /// <param name="stream">Stream containing tile data in Pbf format</param>
        /// <param name="scale">Factor for scaling of coordinates because of overzooming</param>
        /// <param name="overzoom">Offset in X direction because of overzooming</param>
        /// <param name="offsetY">Offset in Y direction because of overzooming</param>
        /// <returns>List of VectorTileLayers, which contain Name and VectorTilesFeatures of each layer, this tile containing</returns>
        public void Parse(TileInfo tileInfo, Stream stream, ITileDataSink sink, Overzoom overzoom, TileClipper clipper = null)
        {
            // Get tile information from Pbf format
            var tile = Serializer.Deserialize<Tile>(stream);

            VectorElement vectorElement = new VectorElement(clipper, tileInfo.Index, 4096);

            foreach (var layer in tile.Layers)
            {
                // Convert all features
                foreach (var feature in layer.Features)
                {
                    FeatureParser.Parse(vectorElement, tileInfo, layer.Name, feature, layer.Keys, layer.Values, layer.Extent, overzoom);

                    if (vectorElement != null)
                        sink.Process(vectorElement);
                }
            }

            sink.Completed(QueryResult.Succes);
        }
    }
}