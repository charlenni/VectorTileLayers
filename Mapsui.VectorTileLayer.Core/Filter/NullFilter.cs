using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Filter
{
    public class NullFilter : Filter
    {
        public override bool Evaluate(IVectorElement feature)
        {
            return true;
        }
    }
}
