using Mapsui.VectorTileLayers.Core.Enums;
using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Filter
{
    public class TypeEqualsFilter : Filter
    {
        public GeometryType Type { get; }

        public TypeEqualsFilter(GeometryType type)
        {
            Type = type;
        }

        public override bool Evaluate(IVectorElement feature)
        {
            return feature != null && feature.Type.Equals(Type);
        }
    }
}
