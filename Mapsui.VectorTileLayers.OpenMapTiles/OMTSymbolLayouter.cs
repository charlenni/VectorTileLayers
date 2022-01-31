using BruTile;
using Mapsui.VectorTileLayers.Core;
using Mapsui.VectorTileLayers.Core.Interfaces;
using Mapsui.VectorTileLayers.Core.Primitives;
using RBush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    public static class OMTSymbolLayouter
    {
        public static RBush<Symbol> Layout(IEnumerable<IVectorTileStyle> vectorTileStyles, IEnumerable<IFeature> vectorTiles, int zoomLevel, int minCol, int minRow, CancellationToken cancelToken)
        {
            RBush<Symbol> tree = new RBush<Symbol>(9);
            Dictionary<TileIndex, MPoint> offsets = new Dictionary<TileIndex, MPoint>();

            // Create a dictionary with all positions of the tiles relative to the left top one
            foreach (var feature in vectorTiles)
            {
                var vectorTileFeature = (VectorTileFeature)feature;
                offsets[vectorTileFeature.TileInfo.Index] = new MPoint((vectorTileFeature.TileInfo.Index.Col - minCol) * 512, (vectorTileFeature.TileInfo.Index.Row - minRow) * 512);
            }

            if (cancelToken.IsCancellationRequested)
            {
                tree = null;
                return null;
            }

            // Now go trough all style layers from top to bottom and look for symbols
            foreach (var style in vectorTileStyles.Reverse())
            {
                if (!style.IsVisible || style.MinZoom > zoomLevel || style.MaxZoom < zoomLevel)
                    continue;

                List<Symbol> symbols = new List<Symbol>();

                foreach (var feature in vectorTiles)
                {
                    var vectorTileFeature = (VectorTileFeature)feature;
                    if (vectorTileFeature.Buckets.ContainsKey(style) && vectorTileFeature.Buckets[style] is SymbolBucket symbolBucket)
                    {
                        symbols.AddRange(symbolBucket.Symbols);
                    }
                    if (cancelToken.IsCancellationRequested)
                    {
                        tree = null;
                        return null;
                    }
                }

                if (symbols.Count == 0)
                    continue;

                // Now we have all symbols in this style layer
                // So sort them, update them and check, if there is space to display them
                foreach (var symbol in symbols.OrderBy((s) => s.Rank))
                {
                    var scale = zoomLevel <= symbol.Index.Level ? 0.5f : 1 << (zoomLevel - symbol.Index.Level - 1);
                    var context = new EvaluationContext(zoomLevel, scale);

                    if (!offsets.ContainsKey(symbol.Index))
                        continue;

                    var offset = offsets[symbol.Index];

                    symbol.Update(context);

                    if (symbol.Alignment == Core.Enums.MapAlignment.Map)
                        // It could be rotated, so use the biggest possible envelope
                        symbol.CalcEnvelope(scale, 45, offset);
                    else
                        symbol.CalcEnvelope(scale, 0, offset);

                    var result = symbol.TreeSearch(tree);
                    if (result != null)
                    {
                        result.AddEnvelope(tree);
                    }
                    if (cancelToken.IsCancellationRequested)
                    {
                        tree = null;
                        return null;
                    }
                }
            }

            return tree;
        }
    }
}
