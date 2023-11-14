// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     Provides static functions to convert unit
    /// </summary>
    internal static class Unit
    {
        /// <summary>
        ///     Convert the value get from RevitAPI to the value indicated by ForgeTypeId
        /// </summary>
        /// <param name="to">ForgeTypeId indicates unit of target value</param>
        /// <param name="value">value get from RevitAPI</param>
        /// <returns>Target value</returns>
        public static double CovertFromAPI(ForgeTypeId to, double value)
        {
            return value *= ImperialDutRatio(to);
        }

        /// <summary>
        ///     Convert a value indicated by ForgeTypeId to the value used by RevitAPI
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="from">ForgeTypeId indicates the unit of the value to be converted</param>
        /// <returns>Target value</returns>
        public static double CovertToAPI(double value, ForgeTypeId from)
        {
            return value /= ImperialDutRatio(from);
        }

        /// <summary>
        ///     Get ratio between value in RevitAPI and value to display indicated by ForgeTypeId
        /// </summary>
        /// <param name="unit">ForgeTypeId indicates display unit type</param>
        /// <returns>Ratio </returns>
        private static double ImperialDutRatio(ForgeTypeId unit)
        {
            if (unit == UnitTypeId.Feet) return 1;
            if (unit == UnitTypeId.FeetFractionalInches) return 1;
            if (unit == UnitTypeId.Inches) return 12;
            if (unit == UnitTypeId.FractionalInches) return 12;
            if (unit == UnitTypeId.Meters) return 0.3048;
            if (unit == UnitTypeId.Centimeters) return 30.48;
            if (unit == UnitTypeId.Millimeters) return 304.8;
            if (unit == UnitTypeId.MetersCentimeters) return 0.3048;
            return 1;
        }
    }
}
