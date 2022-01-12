using SkiaSharp;
using Mapsui.VectorTileLayers.OpenMapTiles.Extensions;

namespace Mapsui.VectorTileLayers.OpenMapTiles.Expressions
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
