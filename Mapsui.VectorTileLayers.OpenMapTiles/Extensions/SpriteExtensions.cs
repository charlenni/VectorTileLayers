using Mapsui.Styles;
using SkiaSharp;

namespace Mapsui.VectorTileLayer.Mapbox.Extensions
{
    public static class SpriteExtensions
    {
        public static SKImage ToSKImage(this Sprite sprite)
        {
            if (sprite.Data != null)
                return (SKImage)sprite.Data;

            var atlas = BitmapRegistry.Instance.Get(sprite.Atlas) as SKImage;

            if (atlas == null)
                return SKImage.Create(SKImageInfo.Empty);

            sprite.Data = atlas.Subset(new SKRectI(sprite.X, sprite.Y, sprite.X + sprite.Width, sprite.Y + sprite.Height));

            return (SKImage)sprite.Data;
        }
    }
}
