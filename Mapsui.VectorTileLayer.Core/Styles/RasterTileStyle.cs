using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Styles
{
    public class RasterTileStyle : TileStyle
    {
        public RasterTileStyle(float minZoom, float maxZoom, IVectorStyleLayer vectorStyle) : base(minZoom, maxZoom)
        {
            StyleLayer = vectorStyle;
        }

        public IVectorStyleLayer StyleLayer { get; }
    }
}
