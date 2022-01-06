using Mapsui.VectorTileLayer.Core.Primitives;

namespace Mapsui.VectorTileLayer.Core.Interfaces
{
    public interface IExpression
    {
        object Evaluate(EvaluationContext ctx);

        object PossibleOutputs();
    }
}
