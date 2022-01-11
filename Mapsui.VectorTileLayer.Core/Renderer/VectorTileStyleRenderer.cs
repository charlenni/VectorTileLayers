using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using Mapsui.VectorTileLayer.Core.Extensions;
using Mapsui.VectorTileLayer.Core.Primitives;
using Mapsui.VectorTileLayer.Core.Styles;
using RBush;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mapsui.VectorTileLayer.Core.Renderer
{
    public class VectorTileStyleRenderer : ISkiaStyleRenderer
    {
#if DEBUG
        SKPaint testPaintRect = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 4 ,Color = SKColors.Red };
        SKPaint testPaintTextStroke = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 4, TextSize = 40, Color = SKColors.White };
        SKPaint testPaintTextFill = new SKPaint { Style = SKPaintStyle.StrokeAndFill, TextSize = 40, Color = SKColors.Red };
#endif

        private SKRect clipRect = new SKRect(0, 0, 512, 512);

        public VectorTileStyleRenderer()
        {
        }

        public bool Draw(SKCanvas canvas, IReadOnlyViewport viewport, ILayer layer, IFeature feature, IStyle style, ISymbolCache symbolCache)
        {
            try
            {
                var vectorTileFeature = (VectorTileFeature)feature;
                var tree = new RBush<Symbol>(9);

                foreach (var vectorStyle in ((VectorTileStyle)style).VectorTileStyles)
                {
                    foreach (var tile in vectorTileFeature.Tiles)
                    {
                        var vectorTile = vectorTileFeature.Cache.Find(tile.Index);

                        if (vectorTile == null)
                            continue;

                        if (!vectorTile.Buckets.ContainsKey(vectorStyle))
                            continue;

                        var extent = tile?.Extent.ToMRect();

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

                        canvas.ClipRect(clipRect);

                        var index = tile.Index;

                        var context = new EvaluationContext((float)viewport.Resolution.ToZoomLevel(), 1f / scale);

                        vectorStyle.Update(context);

                        var bucket = vectorTile.Buckets[vectorStyle];

                        if (bucket is LineBucket lineBucket)
                        {
                            foreach (var paint in vectorStyle.Paints)
                            {
                                var skPaint = paint.CreatePaint(context);

                                canvas.DrawPath(lineBucket.Path, skPaint);
                            }
                        }
                        if (bucket is FillBucket fillBucket)
                        {
                            foreach (var paint in vectorStyle.Paints)
                            {
                                var skPaint = paint.CreatePaint(context);

                                if (skPaint.IsStroke)
                                {
                                    canvas.DrawPath(fillBucket.Path, skPaint);
                                }
                                else
                                {
                                    foreach (var path in fillBucket.Paths)
                                    {
                                        canvas.DrawPath(path, skPaint);
                                    }
                                }
                            }
                        }

#if DEBUG
                        canvas.DrawRect(clipRect, testPaintRect);
                        canvas.DrawText($"Tile {index.Col}/{index.Row}/{index.Level}", new SKPoint(20, 50), testPaintTextStroke);
                        canvas.DrawText($"Tile {index.Col}/{index.Row}/{index.Level}", new SKPoint(20, 50), testPaintTextFill);
#endif

                        // Remove clipping for symbols
                        canvas.Restore();
                    }
                }

                // Now draw symbols

                // Check SymbolBuckts in reverse order, because the last is the most important
                foreach (var vectorStyle in ((VectorTileStyle)style).VectorTileStyles.Reverse())
                {
                    foreach (var tile in vectorTileFeature.Tiles)
                    {
                        var vectorTile = vectorTileFeature.Cache.Find(tile.Index);

                        if (vectorTile == null)
                            continue;

                        if (!vectorTile.Buckets.ContainsKey(vectorStyle))
                            continue;

                        if (!(vectorTile.Buckets[vectorStyle] is SymbolBucket symbolBucket))
                            continue;

                        if (symbolBucket.Symbols.Count == 0)
                            continue;

                        var extent = tile?.Extent.ToMRect();

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

                        var context = new EvaluationContext((float)viewport.Resolution.ToZoomLevel(), 1f / scale);

                        vectorStyle.Update(context);

                        foreach (var symbol in symbolBucket.Symbols)
                        {
                            // Calculate envelope
                            symbol.CalcEnvelope(scale, (float)viewport.Rotation);
                            // Check symbol, if there is another one already occuping place
                            var result = symbol.TreeSearch(tree);
                            if (result != null)
                            {
                                result.Draw(canvas, context);
                                result.AddEnvelope(tree);
                            }
                        }

                        canvas.Restore();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message, ex);

                return false;
            }
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
