using BruTile;
using Mapsui.Layers;
using System;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.Core
{
    /// <summary>
    /// Feature for vector tiles
    /// </summary>
    /// <remarks>
    /// This is a dummy feature, because Mapsui is Feature oriented for drawing.
    /// If there would be a possibility to render/draw the whole Layer only by style, 
    /// then this feature isn't needed.
    /// </remarks>
    public class BackgroundTileFeature : BaseFeature, IFeature
    {
        public BackgroundTileFeature(/*BackgroundTileFeature backgroundTileFeature*/) : base()
        {
            //Tiles = backgroundTileFeature.Tiles == null ? null : new List<TileInfo>(backgroundTileFeature.Tiles);
        }

        public IEnumerable<TileInfo> Tiles { get; set; }

        public int ZOrder { get; set; } = 0;

        public override MRect Extent => throw new NotImplementedException();

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        public override void CoordinateVisitor(Action<double, double, CoordinateSetter> visit)
        {
        }

        public void Dispose()
        {
        }
    }
}
