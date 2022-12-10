using Mapsui.Styles;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.Mapbox.Extensions
{
    public static class SpriteExtensions
    {
        static readonly Dictionary<long, SKImage> images = new Dictionary<long, SKImage>();

        public static SKImage ToSKImage(this Sprite sprite)
        {
            if (sprite.Data != null)
                return (SKImage)sprite.Data;

            var atlas = BitmapRegistry.Instance.Get(sprite.Atlas) as SKImage;

            if (atlas == null)
                return SKImage.Create(SKImageInfo.Empty);

            var hash = sprite.GetHashCode();

            if (!images.ContainsKey(hash))
            {
                images[hash] = atlas.Subset(new SKRectI(sprite.X, sprite.Y, sprite.X + sprite.Width, sprite.Y + sprite.Height));
            }

            sprite.Data = images[hash];

            return (SKImage)sprite.Data;
        }
    }
}
