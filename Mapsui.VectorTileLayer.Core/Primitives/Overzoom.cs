namespace Mapsui.VectorTileLayer.Core.Primitives
{
    /// <summary>
    /// Class for overzoom of values
    /// </summary>
    /// <remarks>
    /// Overzoom is used, when the zoom level is higher than zoom level of data
    /// which could be used. So we use only a part of the tile for rendering.
    /// </remarks>
    public class Overzoom
    {
        public static Overzoom None = new Overzoom(1, 0f, 0f);

        public Overzoom(int scale, float offsetX, float offsetY)
        {
            Scale = scale;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        public int Scale { get; }

        public float OffsetX { get; }

        public float OffsetY { get; }
    }
}
