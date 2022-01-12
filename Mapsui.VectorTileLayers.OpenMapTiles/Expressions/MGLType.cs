namespace Mapsui.VectorTileLayers.OpenMapTiles.Expressions
{
    internal class MGLType
    {
        public static string CheckSubtype(MGLType expected, MGLType t)
        {
            //if (t is MGLErrorType)
            //{
            //    // Error is a subtype of every type
            //    return null;
            //}
            //else if (expected is MGLArrayType arrayExp)
            //{
            //    if (t is MGLArrayType arrayT &&
            //        ((arrayT.Length.Value == 0 && arrayT.ItemType == typeof(MGLValueType)) || CheckSubtype(arrayExp, arrayT)) == null &&
            //        (typeof expected.N !== 'number' || expected.N === t.N))
            //    {
            //        return null;
            //    }
            //}
            //else if (expected.GetType() == t.GetType())
            //{
            //    return null;
            //}
            //else if (expected is MGLValueType)
            //{
            //    foreach (var memberType in valueMemberTypes) 
            //    {
            //        if (!CheckSubtype(memberType, t))
            //        {
            //            return null;
            //        }
            //    }
            //}

            return $"Expected {expected.ToString()} but found { t.ToString()} instead.";
        }
    }
}
