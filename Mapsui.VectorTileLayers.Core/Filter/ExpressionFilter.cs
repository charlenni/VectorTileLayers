using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Filter
{
    public class ExpressionFilter : Filter
    {
        public override bool Evaluate(IVectorElement feature)
        {
            return false;
        }
    }
}
