using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Primitives;
using Mapsui.VectorTileLayers.Core.Styles;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.Core.Renderer
{
    public class RasterTileStyleRenderer : ISkiaStyleRenderer
    {
        private readonly IDictionary<object, BitmapInfo> _tileCache = new Dictionary<object, BitmapInfo>(new IdentityComparer<object>());

        public bool Draw(SKCanvas canvas, Viewport viewport, ILayer layer, IFeature feature, IStyle style, IRenderCache renderCache, long iteration)
        {
            try
            {
                var rasterTileStyle = style as RasterTileStyle;
                var rasterFeature = feature as RasterFeature;

                if (rasterFeature == null)
                    return false;

                var raster = rasterFeature.Raster;

                BitmapInfo bitmapInfo;

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

                var scale = CreateMatrix(canvas, viewport, extent);

                var context = new EvaluationContext((float)viewport.Resolution.ToZoomLevel(), scale);

                foreach (var paint in rasterTileStyle.StyleLayer.Paints)
                {
                    canvas.DrawImage(bitmapInfo.Bitmap, 0, 0, paint.CreatePaint(context));
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