using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Filter
{
    public interface IFilter
    {
        bool Evaluate(IVectorElement feature);
    }
}
