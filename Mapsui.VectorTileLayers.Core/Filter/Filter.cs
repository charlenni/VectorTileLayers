using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Filter
{
    public abstract class Filter : IFilter
    {
        public abstract bool Evaluate(IVectorElement feature);
    }
}
