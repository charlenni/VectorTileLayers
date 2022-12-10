using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Styles
{
    public class RasterTileStyle : TileStyle
    {
        public RasterTileStyle(float minZoom, float maxZoom, IStyleLayer styleLayer) : base(minZoom, maxZoom)
        {
            StyleLayer = styleLayer;
        }

        public IStyleLayer StyleLayer { get; }
    }
}
