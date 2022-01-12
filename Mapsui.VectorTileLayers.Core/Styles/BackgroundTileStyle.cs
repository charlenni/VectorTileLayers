using Mapsui.Styles;
using Mapsui.VectorTileLayers.Core.Extensions;
using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Styles
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
