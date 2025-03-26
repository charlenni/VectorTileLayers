using Mapsui.Styles;
using SkiaSharp;

namespace Mapsui.VectorTileLayers.Core.Primitives
{
    public class Sprite
    {
        Image _image;

        public Sprite(SKImage atlasImage, BitmapRegion br)
        {
            Data = atlasImage.Subset(new SKRectI(br.X, br.Y, br.X + br.Width, br.Y + br.Height));
        }

        /// <summary>
        /// Property for preconverted SKImage for drawing
        /// </summary>
        public SKImage Data { get; init; }
    }
}