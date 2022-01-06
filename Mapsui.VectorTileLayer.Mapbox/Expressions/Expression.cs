using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Primitives;

namespace Mapsui.VectorTileLayer.MapboxGL.Expressions
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
