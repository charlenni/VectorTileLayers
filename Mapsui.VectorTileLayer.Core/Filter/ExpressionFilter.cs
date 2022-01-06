using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Filter
{
    public class ExpressionFilter : Filter
    {
        public override bool Evaluate(IVectorElement feature)
        {
            return false;
        }
    }
}
