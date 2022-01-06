using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Interfaces;
using SkiaSharp;

namespace Mapsui.VectorTileLayer.Core.Primitives
{
    public class LineBucket : IBucket
    {
        public LineBucket()
        {
            Path = new SKPath();
        }

        public SKPath Path { get; }

        public void AddElement(VectorElement element)
        {
            if (element.Type == GeometryType.LineString)
                element.AddToPath(Path);
        }
    }
}
