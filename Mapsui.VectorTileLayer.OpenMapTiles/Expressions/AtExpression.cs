using Mapsui.VectorTileLayer.Core.Interfaces;
using Mapsui.VectorTileLayer.Core.Primitives;
using Newtonsoft.Json.Linq;
using System;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Expressions
{
    public class AtExpression : Expression
    {
        public static IExpression Parse(JArray array, ExpressionParser parser)
        {
            if (array == null)
                throw new ArgumentException("");

            var length = array.Count;

            if (length != 3)
            {
                parser.Error($"Expected 2 arguments, but found {length - 1} instead.");

                return null;
            }

            var index = parser.Parse(array[1], 1, typeof(MGLNumberType));
            var inputType = parser.Expected != null ? parser.Expected : typeof(MGLValueType);
            var input = parser.Parse(array[2], 2, inputType);

            if (index == null || input == null) 
                return null;

            return new AtExpression(index, input);
        }

        public AtExpression(IExpression index, IExpression input)
        {
            Index = index;
            Input = input;
        }

        IExpression Index;
        IExpression Input;

        public override object Evaluate(EvaluationContext ctx)
        {
            throw new System.NotImplementedException();
        }

        public override object PossibleOutputs()
        {
            throw new System.NotImplementedException();
        }
    }
}
