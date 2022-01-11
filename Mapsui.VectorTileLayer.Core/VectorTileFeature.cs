using BruTile;
using BruTile.Cache;
using Mapsui.Styles;
using System;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.Core
{
    /// <summary>
    /// Feature for vector tiles
    /// </summary>
    /// <remarks>
    /// This is a dummy feature, because Mapsui is Feature oriented for drawing.
    /// If there would be a possibility to render/draw the whole Layer only by style, 
    /// then this feature isn't needed.
    /// </remarks>
    public class VectorTileFeature : IFeature
    {
        public IEnumerable<TileInfo> Tiles { get; set; }

        public MemoryCache<VectorTile> Cache { get; set; }

        public object this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<IStyle> Styles => null;

        public IEnumerable<string> Fields => null;

        public MRect Extent { get; set; }

        public IDictionary<IStyle, object> RenderedGeometry => null;

        public void CoordinateVisitor(Action<double, double, CoordinateSetter> visit)
        {
        }

        public void Dispose()
        {
        }
    }
}
