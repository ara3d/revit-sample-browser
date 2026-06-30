// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.ElementFilterSample.CS
{
    public static class EnumParseUtility<TEnum>
    {
        public static TEnum Parse(string strValue)
        {
            return !typeof(TEnum).IsEnum ? default : (TEnum)Enum.Parse(typeof(TEnum), strValue);
        }

        public static string Parse(TEnum enumVal)
        {
            return !typeof(TEnum).IsEnum ? string.Empty : Enum.GetName(typeof(TEnum), enumVal);
        }

        public static string Parse(int enumValInt)
        {
            return !typeof(TEnum).IsEnum ? string.Empty : Enum.GetName(typeof(TEnum), enumValInt);
        }
    }

    public static class ListCompareUtility<T>
    {
        public static bool Equals(ICollection<T> coll1, ICollection<T> coll2)
        {
            if (coll1.Count != coll2.Count) return false;
            foreach (var val1 in coll1)
            {
                if (!coll2.Contains(val1))
                    return false;
            }

            foreach (var val2 in coll2)
            {
                if (!coll1.Contains(val2))
                    return false;
            }

            return true;
        }
    }
}
