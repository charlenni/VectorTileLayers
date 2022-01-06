using System.Collections.Generic;
using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Filter
{
    public class IdentifierNotInFilter : Filter
    {
        public IList<string> Identifiers { get; }

        public IdentifierNotInFilter(IEnumerable<string> identifiers)
        {
            Identifiers = new List<string>();

            foreach (var identifier in identifiers)
                Identifiers.Add(identifier);
        }

        public override bool Evaluate(IVectorElement feature)
        {
            if (feature == null)
                return true;

            foreach (var identifier in Identifiers)
            {
                if (feature.Id == identifier)
                    return false;
            }

            return true;
        }
    }
}
