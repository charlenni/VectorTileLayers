using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Styles
{
    public class BackgroundTileStyle : TileStyle
    {
        private IExpression _color;
        private IExpression _opacity;
        private IExpression _pattern;

        public BackgroundTileStyle(float? minZoom, float? maxZoom, IExpression color, IExpression opacity, IExpression pattern) : base(minZoom, maxZoom)
        {
            _color = color;
            _opacity = opacity;
            _pattern = pattern;
        }
    }
}
