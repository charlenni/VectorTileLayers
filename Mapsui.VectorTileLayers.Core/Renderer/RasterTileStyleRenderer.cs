using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia;
using Mapsui.Rendering.Skia.Extensions;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayers.Core.Styles;
using SkiaSharp;
using System;
using System.Collections.Generic;
using static Mapsui.Rendering.Skia.MapRenderer;

namespace Mapsui.VectorTileLayers.Core.Renderer
{
    public class RasterTileStyleRenderer : ISkiaStyleRenderer
    {
        private readonly IDictionary<object, BitmapInfo?> _tileCache =
            new Dictionary<object, BitmapInfo?>(new IdentityComparer<object>());

        public bool Draw(SKCanvas canvas, IReadOnlyViewport viewport, ILayer layer, IFeature feature, IStyle style, ISymbolCache symbolCache)
        {
            try
            {
                var rasterTileStyle = style as RasterTileStyle;
                var rasterFeature = feature as RasterFeature;

                if (rasterFeature == null)
                    return false;

                var raster = rasterFeature.Raster;

                BitmapInfo? bitmapInfo;

                if (!_tileCache.Keys.Contains(raster))
                {
                    bitmapInfo = BitmapHelper.LoadBitmap(raster.Data);
                    _tileCache[raster] = bitmapInfo;
                }
                else
                {
                    bitmapInfo = _tileCache[raster];
                }

                if (bitmapInfo == null)
                    return false;

                _tileCache[raster] = bitmapInfo;

                var extent = feature.Extent;

                if (extent == null)
                    return false;

                if (bitmapInfo.Bitmap == null)
                    return false;

                var context = new EvaluationContext((float)viewport.Resolution.ToZoomLevel());

                if (viewport.IsRotated)
                {
                    var priorMatrix = canvas.TotalMatrix;

                    var matrix = CreateRotationMatrix(viewport, extent, priorMatrix);

                    canvas.SetMatrix(matrix);

                    var destination = new MRect(0.0, 0.0, extent.Width, extent.Height);
                    foreach (var paint in rasterTileStyle.StyleLayer.Paints)
                    {
                        canvas.DrawImage(bitmapInfo.Bitmap, destination.ToSkia(), paint.CreatePaint(context));
                        canvas.DrawRect(destination.ToSkia(), new SKPaint { Color = SKColors.Blue, Style = SKPaintStyle.Stroke, StrokeWidth = 2 });
                    }

                    canvas.SetMatrix(priorMatrix);
                }
                else
                {
                    var destination = WorldToScreen(viewport, extent);
                    foreach (var paint in rasterTileStyle.StyleLayer.Paints)
                    {
                        canvas.DrawImage(bitmapInfo.Bitmap, destination.ToSkia(), paint.CreatePaint(context));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message, ex);

                return false;
            }

            return true;
        }

        private static SKMatrix CreateRotationMatrix(IReadOnlyViewport viewport, MRect rect, SKMatrix priorMatrix)
        {
            // The front-end sets up the canvas with a matrix based on screen scaling (e.g. retina).
            // We need to retain that effect by combining our matrix with the incoming matrix.

            // We'll create four matrices in addition to the incoming matrix. They perform the
            // zoom scale, focal point offset, user rotation and finally, centering in the screen.

            var userRotation = SKMatrix.CreateRotationDegrees((float)viewport.Rotation);
            var focalPointOffset = SKMatrix.CreateTranslation(
                (float)(rect.Left - viewport.Center.X),
                (float)(viewport.Center.Y - rect.Top));
            var zoomScale = SKMatrix.CreateScale((float)(1.0 / viewport.Resolution), (float)(1.0 / viewport.Resolution));
            var centerInScreen = SKMatrix.CreateTranslation((float)(viewport.Width / 2.0), (float)(viewport.Height / 2.0));

            // We'll concatenate them like so: incomingMatrix * centerInScreen * userRotation * zoomScale * focalPointOffset

            var matrix = SKMatrix.Concat(zoomScale, focalPointOffset);
            matrix = SKMatrix.Concat(userRotation, matrix);
            matrix = SKMatrix.Concat(centerInScreen, matrix);
            matrix = SKMatrix.Concat(priorMatrix, matrix);

            return matrix;
        }

        private static MRect WorldToScreen(IReadOnlyViewport viewport, MRect rect)
        {
            var first = viewport.WorldToScreen(rect.Min.X, rect.Min.Y);
            var second = viewport.WorldToScreen(rect.Max.X, rect.Max.Y);
            return new MRect
            (
                Math.Min(first.X, second.X),
                Math.Min(first.Y, second.Y),
                Math.Max(first.X, second.X),
                Math.Max(first.Y, second.Y)
            );
        }

        private static MRect RoundToPixel(MRect boundingBox)
        {
            return new MRect(
                (float)Math.Round(boundingBox.Left),
                (float)Math.Round(Math.Min(boundingBox.Top, boundingBox.Bottom)),
                (float)Math.Round(boundingBox.Right),
                (float)Math.Round(Math.Max(boundingBox.Top, boundingBox.Bottom)));
        }
    }
}
