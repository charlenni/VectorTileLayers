using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Filter
{
    public class NotHasIdentifierFilter : Filter
    {
        public NotHasIdentifierFilter()
        {
        }

        public override bool Evaluate(IVectorElement feature)
        {
            return feature != null && string.IsNullOrWhiteSpace(feature.Id);
        }
    }
}
