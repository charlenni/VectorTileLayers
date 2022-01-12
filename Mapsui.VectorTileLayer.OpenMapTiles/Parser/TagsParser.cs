using System.Collections.Generic;
using Mapsui.VectorTileLayer.Core.Primitives;
using Mapsui.VectorTileLayer.OpenMapTiles.Pbf;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Parser
{
    public static class TagsParser
    {
        public static void Parse(VectorElement element, List<string> keys, List<Value> values, List<uint> tags)
        {
            for (var i = 0; i < tags.Count; i += 2)
            {
                var key = keys[(int)tags[i]];
                var val = values[(int)tags[i+1]];
                if (val.HasBoolValue)
                    element.Tags.Add(key, val.BoolValue);
                else if (val.HasDoubleValue)
                    element.Tags.Add(key, val.DoubleValue);
                else if (val.HasFloatValue)
                    element.Tags.Add(key, val.FloatValue);
                else if (val.HasIntValue)
                    element.Tags.Add(key, val.IntValue);
                else if (val.HasSIntValue)
                    element.Tags.Add(key, val.SintValue);
                else if (val.HasUIntValue)
                    element.Tags.Add(key, val.UintValue);
                else if (val.HasStringValue)
                    element.Tags.Add(key, val.StringValue);
                else
                    throw new System.ArgumentException($"Unknown value for tag key {key}");
            }
        }
    }
}