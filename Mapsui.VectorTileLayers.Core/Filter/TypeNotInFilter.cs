using System.Collections.Generic;
using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Filter
{
    public class TypeNotInFilter : Filter
    {
        public IList<GeometryType> Types { get; }

        public TypeNotInFilter(IEnumerable<GeometryType> types)
        {
            Types = new List<GeometryType>();

            foreach (var type in types)
                Types.Add(type);
        }

        public override bool Evaluate(IVectorElement feature)
        {
            if (feature == null)
                return true;

            foreach (var type in Types)
            {
                if (feature.Type.Equals(type))
                    return false;
            }

            return true;
        }
    }
}
