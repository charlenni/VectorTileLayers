using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Interfaces;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.Core.Primitives
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

        public void Dispose()
        {
            foreach (var path in Paths)
                path.Dispose();

            Path.Dispose();
        }
    }
}
