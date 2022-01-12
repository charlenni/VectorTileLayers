using Mapsui.VectorTileLayer.Core.Enums;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Extensions
{
    public static class OrientationExtensions
    {
        public static Orientation ToOrientation(this string orientation)
        {
            switch (orientation.ToLower())
            {
                case "horizontal":
                    return Orientation.Horizontal;
                case "vertical":
                    return Orientation.Vertical;
                default:
                    return Orientation.Horizontal;
            }
        }
    }
}
