using Mapsui.Styles;
using SkiaSharp;
using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.OpenMapTiles
{
    public class OMTSprite : Sprite
    {
        public OMTSprite(KeyValuePair<string, Json.JsonSprite> sprite, int atlasId) : base(atlasId, sprite.Value.X, sprite.Value.Y, sprite.Value.Width, sprite.Value.Height, sprite.Value.PixelRatio)
        {
            Name = sprite.Key;
            if (sprite.Value.Content != null && sprite.Value.Content.Count == 4)
                Content = new SKRect(sprite.Value.Content[0], sprite.Value.Content[1], sprite.Value.Content[1], sprite.Value.Content[3]);
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
        }

        public string Name { get; }

        public SKRect Content { get; }

        public SKRect Strech { get; }
    }
}
