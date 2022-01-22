using System.Collections.Generic;

namespace Mapsui.VectorTileLayers.Core.Interfaces
{
    public interface ISymbolLayouter
    {
        void RefreshTree(IEnumerable<VectorTile> vectorTiles);
    }
}
