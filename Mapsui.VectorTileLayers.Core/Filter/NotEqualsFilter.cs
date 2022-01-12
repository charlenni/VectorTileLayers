using Mapsui.VectorTileLayers.Core.Interfaces;

namespace Mapsui.VectorTileLayers.Core.Filter
{
    public class NotEqualsFilter : Filter
    {
        public string Key { get; }
        public object Value { get; }

        public NotEqualsFilter(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public override bool Evaluate(IVectorElement feature)
        {
            return feature != null && !feature.Tags.ContainsKeyValue(Key, Value);
        }
    }
}
