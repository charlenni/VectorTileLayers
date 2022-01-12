using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Filter
{
    public class NullFilter : Filter
    {
        public override bool Evaluate(IVectorElement feature)
        {
            return true;
        }
    }
}
