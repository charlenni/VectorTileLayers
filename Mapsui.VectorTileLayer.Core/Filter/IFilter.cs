using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Filter
{
    public interface IFilter
    {
        bool Evaluate(IVectorElement feature);
    }
}
