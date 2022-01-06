using Mapsui.VectorTileLayer.Core.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mapsui.VectorTileLayer.MapboxGL.Expressions
{
    public class ExpressionParser 
    {
        Dictionary<string, Func<JArray, ExpressionParser, IExpression>> expressionRegistry = new Dictionary<string, Func<JArray, ExpressionParser, IExpression>>
        {
            //new KeyValuePair<string, Func<JArray, ExpressionParser, IExpression>>("==", parseComparison),
            //new KeyValuePair<string, Func<string, Expression>>("!=", parseComparison),
            //new KeyValuePair<string, Func<string, Expression>>(">", parseComparison),
            //new KeyValuePair<string, Func<string, Expression>>("<", parseComparison),
            //new KeyValuePair<string, Func<string, Expression>>(">=", parseComparison),
            //new KeyValuePair<string, Func<string, Expression>>("<=", parseComparison),
            //new KeyValuePair<string, Func<string, Expression>>("all", All::parse),
            //new KeyValuePair<string, Func<string, Expression>>("any", Any::parse),
            //new KeyValuePair<string, Func<string, Expression>>("array", Assertion::parse),
            { "at", AtExpression.Parse },
            //new KeyValuePair<string, Func<string, Expression>>("in", In::parse),
            //new KeyValuePair<string, Func<string, Expression>>("boolean", Assertion::parse),
            //new KeyValuePair<string, Func<string, Expression>>("case", Case::parse),
            //new KeyValuePair<string, Func<string, Expression>>("coalesce", Coalesce::parse),
            //new KeyValuePair<string, Func<string, Expression>>("collator", CollatorExpression::parse),
            //new KeyValuePair<string, Func<string, Expression>>("format", FormatExpression::parse),
            //new KeyValuePair<string, Func<string, Expression>>("image", ImageExpression::parse),
            //new KeyValuePair<string, Func<string, Expression>>("interpolate", parseInterpolate),
            { "length", LengthExpression.Parse },
            { "let", LetExpression.Parse },
            //new KeyValuePair<string, Func<string, Expression>>("literal", Literal::parse),
            //new KeyValuePair<string, Func<string, Expression>>("match", parseMatch),
            //new KeyValuePair<string, Func<string, Expression>>("number", Assertion::parse),
            //new KeyValuePair<string, Func<string, Expression>>("number-format", NumberFormat::parse),
            //new KeyValuePair<string, Func<string, Expression>>("object", Assertion::parse),
            //new KeyValuePair<string, Func<string, Expression>>("step", Step::parse),
            //new KeyValuePair<string, Func<string, Expression>>("string", Assertion::parse),
            //new KeyValuePair<string, Func<string, Expression>>("to-boolean", Coercion::parse),
            //new KeyValuePair<string, Func<string, Expression>>("to-color", Coercion::parse),
            //new KeyValuePair<string, Func<string, Expression>>("to-number", Coercion::parse),
            //new KeyValuePair<string, Func<string, Expression>>("to-string", Coercion::parse),
            { "var", VarExpression.Parse },
            //new KeyValuePair<string, Func<string, Expression>>("within", Within::parse),
        };

        public string Key { get; }
        public Type Expected { get; }
        public Dictionary<string, string> Errors { get; }
        public Scope Scope { get; }

        public static IExpression ParseExpression(string str, Type expected)
        {
            var json = JsonConvert.DeserializeObject(str);

            if (json is JArray array)
            {
                var parser = new ExpressionParser("", new Dictionary<string, string>(), expected, new Scope(new Dictionary<string, IExpression>()));
                return parser.Parse(array, expected);
            }

            // Could be any literal, so check
            //else if (expression == null)
            //{
            //    result = new ConstantExpression<MGLNullType>(new MGLNullType());
            //}
            //else if (expression is string && expected == typeof(MGLColorType))
            //{
            //    result = new ConstantExpression<MGLColorType>(new MGLColorType((string)expression));
            //}
            //else if (expression is string)
            //{
            //    result = new ConstantExpression<MGLStringType>(new MGLStringType((string)expression));
            //}
            //else if (expression is long || expression is int || expression is double || expression is float)
            //{
            //    result = new ConstantExpression<MGLNumberType>(new MGLNumberType(expression));
            //}
            //else if (expression is bool)
            //{
            //    result = new ConstantExpression<bool>((bool)expression);
            //}


            return null;
        }

        public ExpressionParser(string key, Dictionary<string, string> errors, Type expected, Scope scope)
        {
            Key = key;
            Errors = errors;
            Expected = expected;
            Scope = scope;
        }

        public IExpression Parse(JToken token, Type expected)
        {
            IExpression result = null;

            if (token is JArray array)
            {
                var length = array.Count;

                if (length == 0)
                {
                    Errors.Add("Expected an array with at least one element. If you wanted a literal array, use [\"literal\", []].", Key);
                    return null;
                }

                if (array[0].Type != JTokenType.String)
                {
                    Errors.Add("Expression name must be a string, but found " + array[0].GetType() + " instead. If you wanted a literal array, use [\"literal\", [...]].", Key);
                    return null;
                }

                var op = array[0].ToString();

                if (expressionRegistry.ContainsKey(op))
                {
                    result = expressionRegistry[op](array, this);
                }
                else
                {
                    //result = parseCompoundExpression(op, array, this);
                }
            }

            if (result != null)
                return result;

            return null;
        }

        //internal IExpression ParseExpression(JToken token, TypeAnnotationOption typeAnnotationOption = TypeAnnotationOption.None)
        //{
        //    return Parse(token, typeAnnotationOption);
        //}

        /// <summary>
        /// Parse a child expression. For use by individual Expression.Parse() methods.
        /// </summary>
        /// <param name="array">Array containing the child</param>
        /// <param name="index">Index of child in array</param>
        /// <param name="expected">Expected type for this child</param>
        /// <param name="typeAnnotationOption">Type annotations</param>
        /// <returns>Expression for this child</returns>
        internal IExpression Parse(JToken token, int index, Type expected = null, TypeAnnotationOption typeAnnotationOption = TypeAnnotationOption.None) 
        {
            var parser = new ExpressionParser(Key + "[" + index.ToString() + "]", Errors, expected, Scope);
            return parser.Parse(token, expected);
        }

        internal IExpression Parse(JToken token, int index, Type expected, Dictionary<string, IExpression> bindings)
        {
            var parser = new ExpressionParser(Key +"[" + index.ToString() + "]", Errors, expected, new Scope(bindings, Scope));
            return parser.Parse(token, expected);
        }

        public void Error(string text)
        {
            Errors.Add(text, Key);
        }

        public void Error(string message, long child)
        {
            Errors.Add( message, Key + "[" + child.ToString() + "]");
        }

        public void Error(string message, long child, long grandchild)
        {
            Errors.Add( message, Key + "[" + child.ToString() + "][" + grandchild.ToString() + "]");
        }

        public void AppendErrors(ExpressionParser parser)
        {
            foreach (var entry in parser.Errors)
                Errors.Add(entry.Key, entry.Value);
            parser.ClearErrors();
        }

        public void ClearErrors()
        {
            Errors.Clear();
        }

        public string CombinedErrors()
        {
            StringBuilder combinedError = new StringBuilder();

            foreach (var error in Errors) 
            {
                combinedError.AppendLine((!string.IsNullOrEmpty(error.Value) ? error.Value + ": " : "") + error.Key);
            }

            return combinedError.ToString();
        }
    }
}
