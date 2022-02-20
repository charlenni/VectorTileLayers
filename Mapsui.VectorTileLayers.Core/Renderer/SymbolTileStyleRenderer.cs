using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Interfaces;
using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayers.Core.Styles;
using SkiaSharp;
using System;

namespace Mapsui.VectorTileLayers.Core.Renderer
{
    public class SymbolTileStyleRenderer : ISkiaStyleRenderer
    {
        public SymbolTileStyleRenderer()
        {
        }

        public bool Draw(SKCanvas canvas, IReadOnlyViewport viewport, ILayer layer, IFeature feature, IStyle style, ISymbolCache symbolCache)
        {
            try
            {
                var vectorTileFeature = (VectorTileFeature)feature;
                var symbolTileStyle = (SymbolTileStyle)style;
                var vectorTileLayer = (IVectorTileLayer)layer;
                var zoomLevel = (int)viewport.Resolution.ToZoomLevel();
                var extent = vectorTileFeature.TileInfo.Extent.ToMRect();
                var index = vectorTileFeature.TileInfo.Index;

                canvas.Save();

                var scale = CreateMatrix(canvas, viewport, extent);

                var context = new EvaluationContext((float)viewport.Resolution.ToZoomLevel(), 1f / scale, (float)viewport.Rotation);

                // Now draw symbols
                var tree = vectorTileLayer.Tree;

                if (tree == null || tree?.Count == 0)
                    return true;

                var symbols = tree.Search();

                foreach (var symbol in symbols)
                {
                    // Draw only symbols that belong to this feature
                    if (index != symbol.Index)
                        continue;

                    symbol.Draw(canvas, context);
                }

                canvas.Restore();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message, ex);

                return false;
            }

            return true;
        }

        private float CreateMatrix(SKCanvas canvas, IReadOnlyViewport viewport, MRect extent)
        {
            var destinationTopLeft = viewport.WorldToScreen(extent.TopLeft);
            var destinationTopRight = viewport.WorldToScreen(extent.TopRight);

            var dx = destinationTopRight.X - destinationTopLeft.X;
            var dy = destinationTopRight.Y - destinationTopLeft.Y;

            var scale = (float)Math.Sqrt(dx * dx + dy * dy) / 512f;

            canvas.Translate((float)destinationTopLeft.X, (float)destinationTopLeft.Y);
            if (viewport.IsRotated)
                canvas.RotateDegrees((float)viewport.Rotation);
            canvas.Scale(scale);

            return scale;
        }
    }
}
