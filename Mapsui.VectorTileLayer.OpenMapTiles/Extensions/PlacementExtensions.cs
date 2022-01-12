using Mapsui.VectorTileLayer.Core.Enums;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Extensions
{
    public static class PlacementExtensions
    {
        public static Placement ToPlacement(this string placement)
        {
            switch (placement.ToLower())
            {
                case "point":
                    return Placement.Point;
                case "line":
                    return Placement.Line;
                case "line-center":
                    return Placement.LineCenter;
                default:
                    return Placement.Point;
            }
        }
    }
}
