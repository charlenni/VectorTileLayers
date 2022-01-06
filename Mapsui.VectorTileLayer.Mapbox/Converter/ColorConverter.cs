using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Mapsui.VectorTileLayer.MapboxGL.Extensions;

namespace Mapsui.VectorTileLayer.MapboxGL.Converter
{
    public class ColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            return token.Value<string>().FromString();
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
