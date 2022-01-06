using SkiaSharp;

namespace Mapsui.VectorTileLayer.Core.Extensions
{
    public static class SKPointExtension
    {
        public static MPoint ToPoint(this SKPoint point)
        {
            return new MPoint(point.X, point.Y);
        }

        public static SKPoint ToSKPoint(this MPoint point)
        {
            return new SKPoint((float)point.X, (float)point.Y);
        }

        public static MPoint[] ToPoints(this SKPoint[] points)
        {
            MPoint[] result = new MPoint[points.Length];
            int i = 0;

            foreach (var point in points)
                result[i++] = point.ToPoint();

            return result;
        }

        public static SKPoint[] ToSKPoints(this MPoint[] points)
        {
            SKPoint[] result = new SKPoint[points.Length];
            int i = 0;

            foreach (var point in points)
                result[i++] = point.ToSKPoint();

            return result;
        }
    }
}
