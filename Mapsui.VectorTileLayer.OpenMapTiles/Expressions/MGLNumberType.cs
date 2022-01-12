namespace Mapsui.VectorTileLayer.OpenMapTiles.Expressions
{
    public class MGLNumberType
    {
        long intValue;
        double floatValue;
        bool isFloat = false;

        public MGLNumberType(object v)
        {
            if (v is long integer)
            {
                intValue = integer;
                isFloat = false;
            }
            else
            {
                floatValue = (double)v;
                isFloat = true;
            }
        }

        public double Value 
        { 
            get
            {
                if (isFloat)
                    return floatValue;
                else
                    return intValue;
            }
        }

        public override string ToString()
        {
            return "number";
        }
    }
}
