using Mapsui.VectorTileLayers.Core.Enums;

namespace Mapsui.VectorTileLayers.OpenMapTiles.Extensions
{
    public static class MapAlignmentExtension
    {
        public static MapAlignment ToMapAlignment(this string alignment)
        {
            switch (alignment.ToLower())
            {
                case "map":
                    return MapAlignment.Map;
                case "viewport":
                    return MapAlignment.Viewport;
                case "auto":
                    return MapAlignment.Auto;
                default:
                    return MapAlignment.Map;
            }
        }
    }
}
