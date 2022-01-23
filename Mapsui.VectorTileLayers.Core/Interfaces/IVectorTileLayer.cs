using Mapsui.VectorTileLayers.Core.Primitives;
using RBush;

namespace Mapsui.VectorTileLayers.Core.Interfaces
{
    /// <summary>
    /// Interface for vector tile layers, that provide a tree for symbols
    /// </summary>
    public interface IVectorTileLayer
    {
        RBush<Symbol> Tree { get; }
    }
}
