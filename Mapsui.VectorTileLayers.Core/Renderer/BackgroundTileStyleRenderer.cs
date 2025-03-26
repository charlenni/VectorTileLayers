using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia.Cache;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using Mapsui.Tiling.Extensions;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayers.Core.Styles;
using SkiaSharp;
using System;

namespace Mapsui.VectorTileLayers.Core.Renderer
{
    public class BackgroundTileStyleRenderer : ISkiaStyleRenderer
    {
        private SKRect tileRect = new SKRect(0, 0, 512, 512);

        public BackgroundTileStyleRenderer()
        {
        }

        public bool Draw(SKCanvas canvas, Viewport viewport, ILayer layer, IFeature feature, IStyle style, RenderService renderService, long iteration)
        {
            try
            {
                var backgroundStyle = (BackgroundTileStyle)style;
                var context = new EvaluationContext((int)viewport.Resolution.ToZoomLevel(), 1f);
                var paint = backgroundStyle.Paint.CreatePaint(context);

                foreach (var tile in ((BackgroundTileFeature)feature).Tiles)
                {
                    var extent = tile.Extent.ToMRect();

                    if (extent == null)
                        continue;

                    canvas.Save();

                    var scale = CreateMatrix(canvas, viewport, extent);

                    canvas.DrawRect(tileRect, paint);

                    canvas.Restore();
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
