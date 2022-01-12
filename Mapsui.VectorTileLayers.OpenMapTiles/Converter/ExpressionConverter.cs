using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Mapsui.VectorTileLayers.OpenMapTiles.Expressions;

namespace Mapsui.VectorTileLayers.OpenMapTiles.Converter
{
    public class ExpressionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            //  objectType == typeof(JsonStoppedString) || objectType == typeof(string);
            return true;
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            switch (token.Type)
            {
                case JTokenType.Object:
                    // It is a object, so we assume, that it is a stopped type
                    if (objectType.GenericTypeArguments.ToString() == "string")
                        return CreateStoppedString(token);
                    break;
                case JTokenType.Array:
                    // We have an array, so we assume, that it is an expresion
                    break;
            }

            return null;
        }

        public StoppedString CreateStoppedString(JToken token)
        {
            var stoppedString = new StoppedString { Stops = new List<KeyValuePair<float, string>>() };

            stoppedString.Base = token.SelectToken("base").ToObject<float>();

            foreach (var stop in token.SelectToken("stops"))
            {
                var zoom = (float)stop.First.ToObject<float>();
                var text = stop.Last.ToObject<string>();
                stoppedString.Stops.Add(new KeyValuePair<float, string>(zoom, text));
            }

            return stoppedString;
        }

           // return new StoppedString() { SingleVal = token.Value<string>() };

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
