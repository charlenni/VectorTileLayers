using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayers.Core.Styles;
using RBush;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mapsui.VectorTileLayers.Core.Renderer
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
                var zoomLevel = (int)viewport.Resolution.ToZoomLevel();

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
                        var index = tile.Index;

                        canvas.Save();

                        var scale = CreateMatrix(canvas, viewport, extent);

                        canvas.ClipRect(clipRect);

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
                    if (!vectorStyle.IsVisible || vectorStyle.MinZoom > zoomLevel || vectorStyle.MaxZoom < zoomLevel)
                        continue;

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

                        canvas.Save();

                        var scale = CreateMatrix(canvas, viewport, extent);

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
