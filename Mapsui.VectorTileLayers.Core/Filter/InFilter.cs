using System.Collections.Generic;
using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Filter
{
    public class InFilter : Filter
    {
        public string Key { get; }
        public IList<object> Values { get; }

        public InFilter(string key, IEnumerable<object> values)
        {
            Key = key;
            Values = new List<object>();

            foreach(var value in values)
                Values.Add(value);
        }

        public override bool Evaluate(IVectorElement feature)
        {
            if (feature == null || !feature.Tags.ContainsKey(Key))
                return false;

            var value = feature.Tags[Key];

            if (value == null)
                return false;

            foreach (var val in Values)
            {
                if (val.Equals(value))
                    return true;
            }

            return false;
        }
    }
}
