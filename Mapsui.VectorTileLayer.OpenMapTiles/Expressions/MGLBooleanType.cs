namespace Mapsui.VectorTileLayer.OpenMapTiles.Expressions
{
    public class MGLBooleanType
    {
        public MGLBooleanType(bool v)
        {
            Value = v;
        }

        public bool Value { get; }

        public override string ToString()
        {
            return "boolean";
        }
    }
}
