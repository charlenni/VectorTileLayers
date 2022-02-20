using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Interfaces;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.Core.Primitives
{
    public class FillBucket : IBucket
    {
        public FillBucket()
        {
            Paths = new List<SKPath>();
            Path = new SKPath();
        }

        public List<SKPath> Paths { get; }
        public SKPath Path { get; }

        public void AddElement(VectorElement element)
        {
            if (element.Type == GeometryType.Polygon)
            {
                var path = element.CreatePath();
                if (path?.PointCount > 0)
                {
                    Paths.Add(path);
                    Path.AddPath(path);
                }
            }
        }

        public void SimplifyPaths()
        {
            Path.Simplify(Path);

            foreach (var path in Paths)
            {
                path.Simplify(path);
            }
        }

        public void Dispose()
        {
            foreach (var path in Paths)
                path.Dispose();

            Path.Dispose();
        }
    }
}
