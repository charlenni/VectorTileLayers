using Mapsui.Styles;
using Mapsui.VectorTileLayers.Core.Primitives;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    public class OMTSprite : Sprite
    {
        public OMTSprite(SKImage atlasImage, KeyValuePair<string, Json.JsonSprite> sprite) : base(atlasImage, new BitmapRegion(sprite.Value.X, sprite.Value.Y, sprite.Value.Width, sprite.Value.Height))
        {
            Name = sprite.Key;
            if (sprite.Value.Content != null && sprite.Value.Content.Count == 4)
                Content = new SKRect(sprite.Value.Content[0], sprite.Value.Content[1], sprite.Value.Content[2], sprite.Value.Content[3]);
            var strech = new SKRect(0, 0, 0, 0);
            if (sprite.Value.StrechX != null && sprite.Value.StrechX.Count == 2)
            {
                strech.Left = sprite.Value.StrechX[0];
                strech.Right = sprite.Value.StrechX[1];
            }
            if (sprite.Value.StrechY != null && sprite.Value.StrechY.Count == 2)
            {
                strech.Top = sprite.Value.StrechY[0];
                strech.Bottom = sprite.Value.StrechY[1];
            }
            Strech = strech;
            PixelRatio = sprite.Value.PixelRatio;
        }

        public string Name { get; }

        public SKRect Content { get; }

        public SKRect Strech { get; }

        public float PixelRatio { get; }
    }
}
