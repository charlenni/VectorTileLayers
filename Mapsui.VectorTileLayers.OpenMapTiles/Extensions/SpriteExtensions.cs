using Mapsui.Styles;
using Mapsui.VectorTileLayers.Core.Primitives;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Extensions
{
    public static class SpriteExtensions
    {
        static readonly Dictionary<long, SKImage> images = new Dictionary<long, SKImage>();

        public static SKImage ToSKImage(this Sprite sprite)
        {
            if (sprite.Data != null)
                return sprite.Data;

            /* var atlas = BitmapRegistry.Instance.Get(sprite.ImageSource) as SKImage;

            if (atlas == null)
                return SKImage.Create(SKImageInfo.Empty);

            var hash = sprite.GetHashCode();

            if (!images.ContainsKey(hash))
            {
                images[hash] = atlas.Subset(new SKRectI(sprite.BitmapRegion.X, sprite.BitmapRegion.Y, sprite.BitmapRegion.X + sprite.BitmapRegion.Width, sprite.BitmapRegion.Y + sprite.BitmapRegion.Height));
            }

            sprite.Data = images[hash]; */

            return sprite.Data;
        }
    }
}
