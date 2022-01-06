using BruTile;
using Mapsui.VectorTileLayer.Core.Extensions;
using System;

namespace Mapsui.VectorTileLayer.MapboxGL
{
    public class MGLBackgroundTileSource : ITileSource
    {
        public ITileSchema Schema { get; }

        public string Name => "Background";

        public Attribution Attribution => new Attribution();

        public MGLBackgroundTileSource()
        {
            var schema = new TileSchema();
            schema.Extent = new Extent(-20037508, -34662080, 20037508, 34662080);
            Schema = schema;

            for (var i = 0; i <= 30; i++)
                Schema.Resolutions.Add(i, new BruTile.Resolution(i, i.ToResolution(), tileWidth: Constants.TileSize, tileHeight: Constants.TileSize));
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            return null;
        }
    }
}
