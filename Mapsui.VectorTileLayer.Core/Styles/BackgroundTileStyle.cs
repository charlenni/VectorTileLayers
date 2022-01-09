using Mapsui.Styles;
using Mapsui.VectorTileLayer.Core.Extensions;
using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Styles
{
    public class BackgroundTileStyle : IStyle
    {
        public IVectorPaint Paint { get; }

        public double MinVisible { get => 24.ToResolution(); set { } }

        public double MaxVisible { get => 0.ToResolution(); set { } }

        public bool Enabled { get; set; } = true;

        float IStyle.Opacity { get; set; }

        public BackgroundTileStyle(IVectorPaint paint)
        {
            Paint = paint;
        }
    }
}
