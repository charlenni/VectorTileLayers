using BruTile;
using Mapsui.VectorTileLayer.Core.Primitives;
using Mapsui.VectorTileLayer.Core.Utilities;
using System.IO;

namespace Mapsui.VectorTileLayer.Core.Interfaces
{
    public interface ITileDataParser
    {
        /// <summary>
        /// Parse data in stream for tile and give elements to sink
        /// </summary>
        /// <param name="tileInfo">Tile to parse</param>
        /// <param name="inputStream">Stream with data to parse</param>
        /// <param name="sink">Sink for elements</param>
        /// <param name="overzoom">Overzoom to use for this tile</param>
        /// <param name="clipper">TileClipper which reduces result to a given clipping rect</param>
        void Parse(TileInfo tileInfo, Stream inputStream, ITileDataSink sink, Overzoom overzoom, TileClipper clipper);
    }
}
