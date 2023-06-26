using BruTile;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using Mapsui.Tiling.Extensions;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Interfaces;
using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayers.Core.Styles;
using SkiaSharp;
using System;

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

        public bool Draw(SKCanvas canvas, Viewport viewport, ILayer layer, IFeature feature, IStyle style, IRenderCache renderCache, long iteration)
        {
            try
            {
                var vectorTileFeature = (VectorTileFeature)feature;
                var styleLayers = ((VectorTileStyle)style).StyleLayers;
                var vectorTileLayer = (IVectorTileLayer)layer;
                var extent = vectorTileFeature.TileInfo.Extent.ToMRect();
                var index = vectorTileFeature.TileInfo.Index;

                var scale = CreateMatrix(canvas, viewport, extent);
                var context = new EvaluationContext((float)viewport.Resolution.ToZoomLevel(), 1f / scale, (float)viewport.Rotation);

                canvas.Save();

                DrawVector(canvas, context, index, vectorTileLayer, vectorTileFeature, styleLayers, renderCache, iteration, scale);
                DrawSymbol(canvas, context, index, vectorTileLayer, vectorTileFeature, styleLayers, renderCache, iteration);

                canvas.Restore();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message, ex);

                return false;
            }
        }

        /// <summary>
        /// Draw all vector data
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="context"></param>
        /// <param name="index">Tile index</param>
        /// <param name="layer"></param>
        /// <param name="feature"></param>
        /// <param name="styleLayers"></param>
        /// <param name="symbolCache"></param>
        /// <param name="iteration"></param>
        /// <param name="scale"></param>
        public void DrawVector(SKCanvas canvas, EvaluationContext context, TileIndex index, IVectorTileLayer layer, VectorTileFeature feature, System.Collections.Generic.IEnumerable<IStyleLayer> styleLayers, ISymbolCache symbolCache, long iteration, float scale)
        {
            var strokeLimit = 1 / scale;

            canvas.ClipRect(clipRect);

            foreach (var styleLayer in styleLayers)
            {
                if (!styleLayer.Enabled)
                    continue;

                if (!feature.Buckets.ContainsKey(styleLayer))
                    continue;

                styleLayer.Update(context);

                var bucket = feature.Buckets[styleLayer];

                if (bucket is LineBucket lineBucket)
                {
                    if (!lineBucket.Path.Bounds.IntersectsWith(canvas.LocalClipBounds))
                        continue;

                    foreach (var paint in styleLayer.Paints)
                    {
                        var skPaint = paint.CreatePaint(context);

                        if (skPaint.StrokeWidth < strokeLimit)
                            continue;

                        canvas.DrawPath(lineBucket.Path, skPaint);
                    }
                }
                if (bucket is FillBucket fillBucket)
                {
                    foreach (var paint in styleLayer.Paints)
                    {
                        var skPaint = paint.CreatePaint(context);

                        if (skPaint.IsStroke)
                        {
                            if (skPaint.StrokeWidth < strokeLimit)
                                continue;

                            if (fillBucket.Path.Bounds.IntersectsWith(canvas.LocalClipBounds))
                            {
                                canvas.DrawPath(fillBucket.Path, skPaint);
                            }
                        }
                        else
                        {
                            foreach (var path in fillBucket.Paths)
                            {
                                if (path.Bounds.IntersectsWith(canvas.LocalClipBounds))
                                {
                                    canvas.DrawPath(path, skPaint);
                                }
                            }
                        }
                    }
                }
            }

#if DEBUG
            canvas.DrawRect(clipRect, testPaintRect);
            canvas.DrawText($"Tile {index.Col}/{index.Row}/{index.Level}", new SKPoint(20, 50), testPaintTextStroke);
            canvas.DrawText($"Tile {index.Col}/{index.Row}/{index.Level}", new SKPoint(20, 50), testPaintTextFill);
#endif

            return;
        }

        /// <summary>
        /// Draw all symbols, which contained in Layer.Tree
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="context"></param>
        /// <param name="index"></param>
        /// <param name="layer"></param>
        /// <param name="feature"></param>
        /// <param name="styleLayer"></param>
        /// <param name="symbolCache"></param>
        /// <param name="iteration"></param>
        public void DrawSymbol(SKCanvas canvas, EvaluationContext context, TileIndex index, IVectorTileLayer layer, VectorTileFeature feature, System.Collections.Generic.IEnumerable<IStyleLayer> styleLayer, ISymbolCache symbolCache, long iteration)
        {
            // Now draw symbols
            var tree = layer.Tree;

            if (tree == null || tree?.Count == 0)
                return;

            var symbols = tree.Search();

            foreach (var symbol in symbols)
            {
                // Is the symbols style layer still enabled
                if (!symbol.Style.Enabled)
                    continue;

                // Draw only symbols that belong to this feature
                if (index != symbol.Index)
                    continue;

                symbol.Draw(canvas, context);
            }
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
