using Mapsui.VectorTileLayer.Core.Enums;

namespace Mapsui.VectorTileLayer.MapboxGL.Extensions
{
    public static class TextTransformExtensions
    {
        public static TextTransform ToTextTransform(this string transform)
        {
            switch (transform)
            {
                case "none":
                    return TextTransform.None;
                case "uppercase":
                    return TextTransform.Uppercase;
                case "lowercase":
                    return TextTransform.Lowercase;
                default:
                    return TextTransform.None;
            }
        }
    }
}
