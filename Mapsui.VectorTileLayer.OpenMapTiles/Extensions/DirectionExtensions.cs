using Mapsui.VectorTileLayer.Core.Enums;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Extensions
{
    public static class DirectionExtensions
    {
        public static Direction ToDirection(this string direction)
        {
            switch (direction.ToLower())
            {
                case "center":
                    return Direction.Center;
                case "left":
                    return Direction.Left;
                case "right":
                    return Direction.Right;
                case "top":
                    return Direction.Top;
                case "bottom":
                    return Direction.Bottom;
                case "top-left":
                    return Direction.TopLeft;
                case "top-right":
                    return Direction.TopRight;
                case "bottom-left":
                    return Direction.BottomLeft;
                case "bottom-right":
                    return Direction.BottomRight;
                default:
                    return Direction.Center;
            }
        }
    }
}
