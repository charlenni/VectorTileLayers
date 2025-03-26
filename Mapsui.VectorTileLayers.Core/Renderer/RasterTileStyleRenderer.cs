using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Rendering.Skia.Cache;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayers.Core.Styles;
using SkiaSharp;
using System;

namespace Mapsui.VectorTileLayers.Core.Renderer
{
    public class RasterTileStyleRenderer : ISkiaStyleRenderer
    {
        public bool Draw(SKCanvas canvas, Viewport viewport, ILayer layer, IFeature feature, IStyle style, RenderService renderService, long currentIteration)
        {
            try
            {
                var rasterTileStyle = style as RasterTileStyle;
                var rasterFeature = feature as RasterFeature;

                if (rasterFeature == null)
                    return false;

                var raster = rasterFeature.Raster;

                var tileCache = renderService.TileCache;
                tileCache.UpdateCache(currentIteration);

                var tile = tileCache.GetOrCreate(raster, currentIteration);
                if (tile is null)
                    return false;

                var extent = feature.Extent;

                if (extent == null)
                    return false;

                var scale = CreateMatrix(canvas, viewport, extent);

                var context = new EvaluationContext((float)viewport.Resolution.ToZoomLevel(), scale);

                foreach (var paint in rasterTileStyle.StyleLayer.Paints)
                {
                    canvas.DrawImage(tile.SKObject as SKImage, 0, 0, paint.CreatePaint(context));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message, ex);

                return false;
            }

            return true;
        }

        private float CreateMatrix(SKCanvas canvas, Viewport viewport, MRect extent)
        {
            var destinationTopLeft = viewport.WorldToScreen(extent.TopLeft);
            var destinationTopRight = viewport.WorldToScreen(extent.TopRight);

            var dx = destinationTopRight.X - destinationTopLeft.X;
            var dy = destinationTopRight.Y - destinationTopLeft.Y;

            var scale = (float)Math.Sqrt(dx * dx + dy * dy) / 512f;

            canvas.Translate((float)destinationTopLeft.X, (float)destinationTopLeft.Y);
            if (viewport.IsRotated())
                canvas.RotateDegrees((float)viewport.Rotation);
            canvas.Scale(scale);

            return scale;
        }
    }
}