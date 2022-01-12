using Mapsui.VectorTileLayers.Core.Interfaces;
using Mapsui.VectorTileLayers.Core.Primitives;

namespace Mapsui.VectorTileLayers.OpenMapTiles.Expressions
{
    public class Expression : IExpression
    {
        public virtual object Evaluate(EvaluationContext ctx)
        {
            return null;
        }

        public virtual object PossibleOutputs()
        {
            return null;
        }
    }
}
