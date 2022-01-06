using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Mapsui.VectorTileLayer.Core.Primitives;
using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.MapboxGL.Expressions
{
    public class LetExpression : Expression
    {
        public static IExpression Parse(JToken token, ExpressionParser parser) 
        {
            if (token == null || !(token is JArray array))
                throw new ArgumentException("");

            var length = array.Count;

            if (length < 4)
            {
                parser.Error("Expected at least 3 arguments, but found " + (length - 1).ToString() + " instead.");
                return null;
            }

            Dictionary<string, IExpression> bindings = new Dictionary<string, IExpression>();

            for(var i = 1; i < length - 1; i += 2) 
            {
                var name = array[i].ToString();
                if (string.IsNullOrEmpty(name)) 
                {
                    parser.Error("Expected string, but found " + array[i].GetType().ToString() + " instead.", i);
                    return null;
                }

                bool isValidName = name.All(c => (c >= 48 && c <= 57 || c >= 65 && c <= 90 || c >= 97 && c <= 122 || c == '_'));

                if (!isValidName) 
                {
                    parser.Error("Variable names must contain only alphanumeric characters or '_'.", 1);
                    return null;
                }
        
                var bindingValue = parser.Parse(array[i + 1], null);
        
                if (bindingValue == null) 
                {
                    return null;
                }
        
                bindings.Add(name, bindingValue);
            }

            var result = parser.Parse(array[length - 1], length - 1, parser.Expected, bindings);
            
            if (result == null) 
            {
                return null;
            }

            return new LetExpression(bindings, result);
        }

        public LetExpression(Dictionary<string, IExpression> bindings, IExpression result)
        {
            Bindings = bindings;
            Result = result;
        }

        Dictionary<string, IExpression> Bindings;
        IExpression Result;

        public override object Evaluate(EvaluationContext ctx)
        {
            return Result.Evaluate(ctx);
        }

        public override object PossibleOutputs()
        {
            return Result.PossibleOutputs(); ;
        }
    }
}
