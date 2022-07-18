using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Interfaces;
using SkiaSharp;

namespace Mapsui.VectorTileLayers.Core.Primitives
{
    public class LineBucket : IBucket
    {
        public LineBucket()
        {
            Path = new SKPath();
        }

        public SKPath Path { get; private set; }

        public void AddElement(VectorElement element)
        {
            if (element.Type == GeometryType.LineString)
                element.AddToPath(Path);
        }

        public void SimplifyPath()
        {
            // TODO: Path.Simplyfy doesn't work correct
            // Path.Simplify(Path);
        }

        public void Dispose()
        {
            Path.Dispose();
        }
    }
}
