using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Filter
{
    public class IdentifierEqualsFilter : Filter
    {
        public string Identifier { get; }

        public IdentifierEqualsFilter(string identifier)
        {
            Identifier = identifier;
        }

        public override bool Evaluate(IVectorElement feature)
        {
            return feature != null && feature.Id == Identifier;
        }
    }
}
