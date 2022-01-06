using System.Collections.Generic;
using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Filter
{
    public class AnyFilter : CompoundFilter
    {
        public AnyFilter(List<IFilter> filters) : base(filters)
        {
        }

        public override bool Evaluate(IVectorElement feature)
        {
            foreach (var filter in Filters)
            {
                if (filter.Evaluate(feature))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
