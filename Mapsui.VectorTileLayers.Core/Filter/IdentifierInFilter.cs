using System.Collections.Generic;
using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Filter
{
    public class IdentifierInFilter : Filter
    {
        public IList<string> Identifiers { get; }

        public IdentifierInFilter(IEnumerable<string> identifiers)
        {
            Identifiers = new List<string>();

            foreach (var identifier in identifiers)
                Identifiers.Add(identifier);
        }

        public override bool Evaluate(IVectorElement feature)
        {
            if (feature == null)
                return false;

            foreach (var identifier in Identifiers)
            {
                if (feature.Id == identifier)
                    return true;
            }

            return false;
        }
    }
}
