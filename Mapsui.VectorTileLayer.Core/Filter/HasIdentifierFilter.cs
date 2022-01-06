using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Filter
{
    public class HasIdentifierFilter : Filter
    {
        public HasIdentifierFilter()
        {
        }

        public override bool Evaluate(IVectorElement feature)
        {
            return feature != null && !string.IsNullOrWhiteSpace(feature.Id);
        }
    }
}
