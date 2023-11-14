// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;

namespace Revit.SDK.Samples.BoundaryConditions.CS
{
    /// <summary>
    ///     a structure about the conversion rule between display value and inside value
    /// </summary>
    public struct ConversionValue
    {
        // members

        /// <summary>
        ///     constructor, using to initialize the members
        /// </summary>
        /// <param name="precision">the precision of the diaplay value</param>
        /// <param name="ratio">the unit of the display value</param>
        /// <param name="unitName">the radio of inside value to display valuea</param>
        public ConversionValue(int precision, double ratio, string unitName)
        {
            Precision = precision;
            Ratio = ratio;
            UnitName = unitName;
        }

        /// <summary>
        ///     get the precision of the diaplay value
        /// </summary>
        public int Precision { get; }

        /// <summary>
        ///     get the unit of the display value
        /// </summary>
        public string UnitName { get; }

        /// <summary>
        ///     get the radio of inside value to display value
        /// </summary>
        public double Ratio { get; }
    }

    /// <summary>
    ///     value conversion dictionary, in this class the data we gave only fit for Imperial unit
    ///     the relationship about dislay paramete values and inside parameter values
    /// </summary>
    public static class UnitConversion
    {
        /// <summary>
        ///     static constructor to initialize the value conversion dictionary which we will used
        ///     according to the difference unit
        /// </summary>
        static UnitConversion()
        {
            UnitDictionary = new Dictionary<string, ConversionValue>();

            var degree = (char)0xb0;
            var square = (char)0xB2;
            var cube = (char)0xB3;

            // about point BC's translation spring modulus
            AddNewUnit(1, 175126.835246476, "kip/in", "PTSpringModulusConver");

            // about point BC's rotation spring modulus
            AddNewUnit(1, 47880.2589803358, "kip-f/" + degree + "F", "PRSpringModulusConver");

            // about Line BC's translation spring modulus
            AddNewUnit(4, 14593.9029372064, "kip/ft" + square, "LTSpringModulusConver");

            // about Line BC's rotation spring modulus
            AddNewUnit(1, 47880.2589803358, "kip-f/" + degree + "F/ft", "LRSpringModulusConver");

            // about Area BC's translation spring modulus
            AddNewUnit(1, 14593.9029372064, "kip/ft" + cube, "ATSpringModulusConver");
        }
        // member: dictionary

        /// <summary>
        ///     get the value dictionary
        /// </summary>
        public static Dictionary<string, ConversionValue> UnitDictionary { get; }

        /// <summary>
        ///     add a new unit conversion item
        /// </summary>
        /// <param name="precision"> the precision of the display value</param>
        /// <param name="ratio"> the radio of inside value to display value about current unit</param>
        /// <param name="unitName"> the unit of the display value</param>
        /// <param name="key"></param>
        private static void AddNewUnit(int precision, double ratio, string unitName, string key)
        {
            // create a ConversionValue instance
            var conversionValue = new ConversionValue(precision, ratio, unitName);

            // add this item to ourself dictionary
            UnitDictionary.Add(key, conversionValue);
        }
    }
}
