using System;

namespace Mapsui.VectorTileLayer.OpenMapTiles.Expressions
{
    internal class MGLArrayType : MGLType
    {
        public readonly MGLNumberType Length;
        public readonly Type ItemType;

        public MGLArrayType(Type itemType, MGLNumberType length)
        {
            ItemType = itemType;
            Length = length;
        }

        public override string ToString()
        {
            return (Length is MGLNumberType) ? $"array <{ItemType.ToString()},{Length}>" : (ItemType == typeof(MGLValueType)) ? "array" : $"array <{ItemType.ToString()}";
        }
    }
}
