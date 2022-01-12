using Mapsui.VectorTileLayer.Core.Primitives;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Expressions
{
    public class ConstantExpression<T> : Expression
    {
        T value;

        public ConstantExpression(T v)
        {
            value = v;
        }

        public override object Evaluate(EvaluationContext feature)
        {
            return value;
        }

        public override object PossibleOutputs()
        {
            return (T)new object();
        }
    }
}
