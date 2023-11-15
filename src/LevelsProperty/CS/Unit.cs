// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.LevelsProperty.CS
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
        public static double CovertFromApi(ForgeTypeId to, double value)
        {
            return UnitUtils.ConvertFromInternalUnits(value, to);
        }

        /// <summary>
        ///     Convert a value indicated by ForgeTypeId to the value used by RevitAPI
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="from">ForgeTypeId indicates the unit of the value to be converted</param>
        /// <returns>Target value</returns>
        public static double CovertToApi(double value, ForgeTypeId from)
        {
            return UnitUtils.ConvertToInternalUnits(value, from);
        }
    }
}
