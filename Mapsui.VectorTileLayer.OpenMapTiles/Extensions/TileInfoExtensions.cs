using BruTile;
using System;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Extensions
{
    public static class TileInfoExtensions
    {
        public static TileInfo ToTMS(this TileInfo tileInfo)
        {
            var result = new TileInfo();
            var zoom = tileInfo.Index.Level;
            var newRow = (int)Math.Pow(2, zoom) - tileInfo.Index.Row - 1;

            result.Index = new TileIndex(tileInfo.Index.Col, newRow, tileInfo.Index.Level);

            return result;
        }

        public static TileInfo ToOSM(this TileInfo tileInfo)
        {
            var result = new TileInfo();
            var zoom = tileInfo.Index.Level;
            var newRow = (int)Math.Pow(2, zoom) - tileInfo.Index.Row - 1;

            result.Index = new TileIndex(tileInfo.Index.Col, newRow, tileInfo.Index.Level);

            return result;
        }

        public static TileInfo Copy(this TileInfo tileInfo)
        {
            var result = new TileInfo();

            result.Index = new TileIndex(tileInfo.Index.Col, tileInfo.Index.Row, tileInfo.Index.Level);

            return result;
        }
    }
}
