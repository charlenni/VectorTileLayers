using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Styles
{
    public class RasterTileStyle : TileStyle
    {
        public RasterTileStyle(float minZoom, float maxZoom, IVectorTileStyle vectorStyle) : base(minZoom, maxZoom)
        {
            StyleLayer = vectorStyle;
        }

        public IVectorTileStyle StyleLayer { get; }
    }
}
