using System.Collections.Generic;
using Mapsui.VectorTileLayer.Core.Interfaces;

namespace Mapsui.VectorTileLayer.Core.Filter
{
    public class NotInFilter : Filter
    {
        public string Key { get; }
        public List<object> Values { get; }

        public NotInFilter(string key, IEnumerable<object> values)
        {
            Key = key;
            Values = new List<object>();

            foreach (var value in values)
                Values.Add(value);
        }

        public override bool Evaluate(IVectorElement feature)
        {
            if (feature == null || !feature.Tags.ContainsKey(Key))
                return true;

            var value = feature.Tags[Key];

            if (value == null)
                return true;

            foreach (var val in Values)
            {
                if (val.Equals(value))
                    return false;
            }

            return true;
        }
    }
}
