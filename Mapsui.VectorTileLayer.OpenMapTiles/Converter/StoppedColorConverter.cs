using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using System;
using System.Collections.Generic;
using Mapsui.VectorTileLayer.OpenMapTiles.Expressions;
using Mapsui.VectorTileLayer.OpenMapTiles.Extensions;
using Mapsui.VectorTileLayer.OpenMapTiles.Json;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Converter
{
    public class StoppedColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonStoppedString) || objectType == typeof(string);
            //return typeof(StoppedDouble).IsAssignableFrom(objectType) || typeof(int).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object)
            {
                var stoppedColor = new StoppedColor { Stops = new List<KeyValuePair<float, SKColor>>() };

                if (token.SelectToken("base") != null)
                    stoppedColor.Base = token.SelectToken("base").ToObject<float>();
                else
                    stoppedColor.Base = 1f;

                foreach (var stop in token.SelectToken("stops"))
                {
                    var zoom = (float)stop.First.ToObject<float>();
                    var colorString = stop.Last.ToObject<string>();
                    stoppedColor.Stops.Add(new KeyValuePair<float, SKColor>(zoom, colorString.FromString()));
                }

                return stoppedColor;
            }

            return new StoppedColor() { SingleVal = token.Value<string>().FromString() };
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer,
            object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
