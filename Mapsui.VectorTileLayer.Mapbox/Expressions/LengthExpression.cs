using Newtonsoft.Json.Linq;
using System;
using Mapsui.VectorTileLayer.Core.Primitives;
using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.MapboxGL.Expressions
{
    public class LengthExpression : Expression
    {
        public static IExpression Parse(JToken token, ExpressionParser parser)
        {
            if (token == null || !(token is JArray array))
                throw new ArgumentException("");

            var length = array.Count;

            if (length != 2)
            {
                parser.Error("Expected one argument, but found " + length.ToString() + " instead.");
                return null;
            }

            var input = parser.Parse(array[1], 1);
            
            if (input == null) 
                return null;

            if (!(input is MGLArrayType) && !(input is MGLStringType) && !(input is MGLValueType))
            {
                parser.Error("Expected argument of type string or array, but found " + input.GetType().ToString() + " instead.");
                return null;
            }

            return new LengthExpression(input);
        }

        public LengthExpression(IExpression input)
        {
            Input = input;
        }

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
