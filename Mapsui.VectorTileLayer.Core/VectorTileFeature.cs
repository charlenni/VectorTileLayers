using BruTile;
using Mapsui.Styles;
using System;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.Core
{
    public class VectorTileFeature : IFeature
    {
        public IEnumerable<TileInfo> Tiles { get; set; }

        public object this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<IStyle> Styles { get; }

        public IEnumerable<string> Fields => null;

        public MRect Extent { get; set; }

        public IDictionary<IStyle, object> RenderedGeometry => null;

        public void CoordinateVisitor(Action<double, double, CoordinateSetter> visit)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
