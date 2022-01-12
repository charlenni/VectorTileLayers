using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
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

        public bool Draw(SKCanvas canvas, IReadOnlyViewport viewport, ILayer layer, IFeature feature, IStyle style, ISymbolCache symbolCache)
        {
            try
            {
                var backgroundStyle = (BackgroundTileStyle)style;
                var context = new EvaluationContext((int)viewport.Resolution.ToZoomLevel(), 1f);
                var paint = backgroundStyle.Paint.CreatePaint(context);

                foreach (var tile in ((VectorTileFeature)feature).Tiles)
                {
                    var extent = tile.Extent.ToMRect();

                    if (extent == null)
                        continue;

                    MRect destination;

                    if (viewport.IsRotated)
                    {
                        var matrix = CreateRotationMatrix(viewport, extent, canvas.TotalMatrix);

                        canvas.SetMatrix(matrix);

                        destination = new MRect(0.0, 0.0, extent.Width, extent.Height);
                    }
                    else
                    {
                        destination = WorldToScreen(viewport, extent);
                    }

                    var scale = (float)(destination.MaxX - destination.MinX) / 512;

                    canvas.Save();

                    canvas.Translate((float)destination.MinX, (float)destination.MinY);
                    canvas.Scale(scale);

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
