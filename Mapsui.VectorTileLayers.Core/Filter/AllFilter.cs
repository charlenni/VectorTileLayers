using System.Collections.Generic;
using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Filter
{
    public class AllFilter : CompoundFilter
    {
        public AllFilter(List<IFilter> filters) : base(filters)
        {
        }

        public override bool Evaluate(IVectorElement feature)
        {
            foreach (var filter in Filters)
            {
                if (!filter.Evaluate(feature))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
