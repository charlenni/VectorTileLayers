using Mapsui.VectorTileLayers.Core.Primitives;

namespace Mapsui.VectorTileLayers.Core.Interfaces
{
    public interface IExpression
    {
        object Evaluate(EvaluationContext ctx);

        object PossibleOutputs();
    }
}
