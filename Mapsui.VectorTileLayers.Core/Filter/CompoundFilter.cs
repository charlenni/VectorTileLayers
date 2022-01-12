using System.Collections.Generic;
using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Filter
{
    public abstract class CompoundFilter : Filter
    {
        public List<IFilter> Filters { get; }

        public CompoundFilter()
        {
        }

        public CompoundFilter(List<IFilter> filters)
        {
            Filters = new List<IFilter>();

            if (filters == null)
                return;

            foreach (var filter in filters)
            {
                Filters.Add(filter);
            }
        }

        public abstract override bool Evaluate(IVectorElement feature);
    }
}
