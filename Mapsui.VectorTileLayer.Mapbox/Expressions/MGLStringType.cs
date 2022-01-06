namespace Mapsui.VectorTileLayer.MapboxGL.Expressions
{
    internal class MGLStringType : MGLValueType
    {
        public MGLStringType(string v)
        {
            Value = v;
        }

        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }
    }
}
