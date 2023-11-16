// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.ElementFilterSample.CS
{
    /// <summary>
    ///     Generic utility class used to extend enum parse related methods
    /// </summary>
    /// <typeparam name="TEnum">Enum type for this class.</typeparam>
    public static class EnumParseUtility<TEnum>
    {
        /// <summary>
        ///     Parse string to TEnum values
        /// </summary>
        /// <param name="strValue">Enum value in string.</param>
        /// <returns>TEnum value is parsed from input string.</returns>
        public static TEnum Parse(string strValue)
        {
            if (!typeof(TEnum).IsEnum) return default;
            return (TEnum)Enum.Parse(typeof(TEnum), strValue);
        }

        /// <summary>
        ///     Parse TEnum to string
        /// </summary>
        /// <param name="enumVal">TEnum value to be parsed.</param>
        /// <returns>String parsed from input TEnum.</returns>
        public static string Parse(TEnum enumVal)
        {
            return !typeof(TEnum).IsEnum ? string.Empty : Enum.GetName(typeof(TEnum), enumVal);
        }

        /// <summary>
        ///     Parse TEnum from integer value to string
        /// </summary>
        /// <param name="enumValInt">Integer value to be parsed.</param>
        /// <returns>String parsed from input TEnum(integer type)</returns>
        public static string Parse(int enumValInt)
        {
            return !typeof(TEnum).IsEnum ? string.Empty : Enum.GetName(typeof(TEnum), enumValInt);
        }
    }

    /// <summary>
    ///     Generic utility class used to deal with List compare related methods.
    /// </summary>
    /// <typeparam name="T">Object type in list.</typeparam>
    public static class ListCompareUtility<T>
    {
        /// <summary>
        ///     Check to see if two lists are equals
        ///     When comparing, two list will be checked on amounts and contents
        /// </summary>
        /// <param name="coll1">1st list.</param>
        /// <param name="coll2">2nd list.</param>
        /// <returns>True if two list are identical on data.</returns>
        public static bool Equals(ICollection<T> coll1, ICollection<T> coll2)
        {
            if (coll1.Count != coll2.Count) return false;
            foreach (var val1 in coll1)
                if (!coll2.Contains(val1))
                    return false;
            foreach (var val2 in coll2)
                if (!coll1.Contains(val2))
                    return false;
            return true;
        }
    }
}
