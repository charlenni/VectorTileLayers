using Mapsui.VectorTileLayer.Core.Enums;

namespace Mapsui.VectorTileLayer.MapboxGL.Extensions
{
    public static class ZOrderExtensions
    {
        public static ZOrder ToZOrder(this string zOrder)
        {
            switch (zOrder.ToLower())
            {
                case "auto":
                    return ZOrder.Auto;
                case "viewport-y":
                    return ZOrder.ViewportY;
                case "source":
                    return ZOrder.Source;
                default:
                    return ZOrder.Auto;
            }
        }
    }
}
