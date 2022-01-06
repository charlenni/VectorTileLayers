using System.Collections.Generic;
using Mapsui.VectorTileLayer.Core.Enums;
using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Filter
{
    public class TypeInFilter : Filter
    {
        public IList<GeometryType> Types { get; }

        public TypeInFilter(IEnumerable<GeometryType> types)
        {
            Types = new List<GeometryType>();

            foreach (var type in types)
                Types.Add(type);
        }

        public override bool Evaluate(IVectorElement feature)
        {
            if (feature == null)
                return false;

            foreach (var type in Types)
            {
                if (feature.Type.Equals(type))
                    return true;
            }

            return false;
        }
    }
}
