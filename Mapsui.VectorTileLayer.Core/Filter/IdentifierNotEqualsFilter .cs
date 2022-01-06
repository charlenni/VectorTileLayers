using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Filter
{
    public class IdentifierNotEqualsFilter : Filter
    {
        public string Identifier { get; }

        public IdentifierNotEqualsFilter(string identifier)
        {
            Identifier = identifier;
        }

        public override bool Evaluate(IVectorElement feature)
        {
            return feature != null && feature.Id != Identifier;
        }
    }
}
