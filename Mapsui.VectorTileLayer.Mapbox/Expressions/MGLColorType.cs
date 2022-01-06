using SkiaSharp;
using Mapsui.VectorTileLayer.MapboxGL.Extensions;

namespace Mapsui.VectorTileLayer.MapboxGL.Expressions
{
    internal class MGLColorType : MGLType
    {
        public MGLColorType(string v)
        {
            Value = v.FromString();
        }

        public MGLColorType(SKColor v)
        {
            Value = v;
        }

        public SKColor Value { get; }

        public override string ToString()
        {
            return "color";
        }
    }
}
