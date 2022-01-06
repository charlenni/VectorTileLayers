using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Filter
{
    public abstract class Filter : IFilter
    {
        public abstract bool Evaluate(IVectorElement feature);
    }
}
