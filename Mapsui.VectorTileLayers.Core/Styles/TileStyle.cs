using Mapsui.Styles;
using Mapsui.VectorTileLayers.Core.Extensions;

namespace Mapsui.VectorTileLayers.Core.Styles
{
    public abstract class TileStyle : IStyle
    {
        public TileStyle(float? minZoom, float? maxZoom)
        {
            Enabled = true;

            MinVisible = (maxZoom ?? 0).ToResolution();
            MaxVisible = (minZoom ?? 30).ToResolution();
        }

        public double MinVisible { get; set; }

        public double MaxVisible { get; set; }

        public bool Enabled { get; set; }

        public float Opacity { get; set; }
    }
}
