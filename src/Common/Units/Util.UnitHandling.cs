// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Unit Handling

        private enum BaseUnit
        {
            BU_Length = 0, // length, feet (ft)
            BU_Angle, // angle, radian (rad)
            BU_Mass, // mass, kilogram (kg)
            BU_Time, // time, second (s)
            BU_Electric_Current, // electric current, ampere (A)
            BU_Temperature, // temperature, kelvin (K)
            BU_Luminous_Intensity, // luminous intensity, candela (cd)
            BU_Solid_Angle, // solid angle, steradian (sr)

            NumBaseUnits
        }

        private const double _inchToMm = 25.4;
        private const double _footToMm = 12 * _inchToMm;
        private const double _footToMeter = _footToMm * 0.001;
        private const double _sqfToSqm = _footToMeter * _footToMeter;
        private const double _cubicFootToCubicMeter = _footToMeter * _sqfToSqm;

        public static double FootToMm(double length)
        {
            return length * _footToMm;
        }

        public static int FootToMmInt(double length)
        {
            //return (int) ( _feet_to_mm * d + 0.5 );
            return (int) Math.Round(_footToMm * length,
                MidpointRounding.AwayFromZero);
        }

        public static double FootToMetre(double length)
        {
            return length * _footToMeter;
        }

        public static double MmToFoot(double length)
        {
            return length / _footToMm;
        }

        public static XYZ MmToFoot(XYZ v)
        {
            return v.Divide(_footToMm);
        }

        public static double CubicFootToCubicMeter(double volume)
        {
            return volume * _cubicFootToCubicMeter;
        }

        public static string[] DisplayUnitTypeAbbreviation
            =
            {
                "m", // DUT_METERS = 0,
                "cm", // DUT_CENTIMETERS = 1,
                "mm", // DUT_MILLIMETERS = 2,
                "ft", // DUT_DECIMAL_FEET = 3,
                "N/A", // DUT_FEET_FRACTIONAL_INCHES = 4,
                "N/A", // DUT_FRACTIONAL_INCHES = 5,
                "in", // DUT_DECIMAL_INCHES = 6,
                "ac", // DUT_ACRES = 7,
                "ha", // DUT_HECTARES = 8,
                "N/A", // DUT_METERS_CENTIMETERS = 9,
                "y^3", // DUT_CUBIC_YARDS = 10,
                "ft^2", // DUT_SQUARE_FEET = 11,
                "m^2", // DUT_SQUARE_METERS = 12,
                "ft^3", // DUT_CUBIC_FEET = 13,
                "m^3", // DUT_CUBIC_METERS = 14,
                "deg", // DUT_DECIMAL_DEGREES = 15,
                "N/A", // DUT_DEGREES_AND_MINUTES = 16,
                "N/A", // DUT_GENERAL = 17,
                "N/A", // DUT_FIXED = 18,
                "%", // DUT_PERCENTAGE = 19,
                "in^2", // DUT_SQUARE_INCHES = 20,
                "cm^2", // DUT_SQUARE_CENTIMETERS = 21,
                "mm^2", // DUT_SQUARE_MILLIMETERS = 22,
                "in^3", // DUT_CUBIC_INCHES = 23,
                "cm^3", // DUT_CUBIC_CENTIMETERS = 24,
                "mm^3", // DUT_CUBIC_MILLIMETERS = 25,
                "l" // DUT_LITERS = 26,
            };

        public static void ListForgeTypeIds()
        {
            //ForgeTypeId a = SpecTypeId.Acceleration;
            //Debug.Print( a.TypeId );

            var spityp = typeof(SpecTypeId);

            //foreach( MemberInfo mi in spityp.GetMembers() )
            //{
            //  Debug.Print( mi.Name );
            //}

            var ps = spityp.GetProperties(
                BindingFlags.Public | BindingFlags.Static);

            // Sort properties alphabetically by name 

            Array.Sort(ps,
                delegate(PropertyInfo p1, PropertyInfo p2) { return p1.Name.CompareTo(p2.Name); });

            Debug.Print("{0} properties:", ps.Length);

            foreach (var pi in ps)
                if (pi.PropertyType == typeof(ForgeTypeId))
                {
                    var obj = pi.GetValue(null, null);

                    var fti = obj as ForgeTypeId;

                    Debug.Print("{0}: {1}", pi.Name, fti.TypeId);
                }

            //IList<ForgeTypeId> specs = UnitUtils.GetAllSpecs(); // 2021
            var specs = UnitUtils.GetAllMeasurableSpecs(); // 2022

            Debug.Print("{0} specs:", specs.Count);

            var units_metric = new Units(UnitSystem.Metric);
            var units_imperial = new Units(UnitSystem.Imperial);

            foreach (var fti in specs)
                Debug.Print("{0}: {1}, {2}, {3}, {4}",
                    fti, fti.TypeId,
                    UnitUtils.GetTypeCatalogStringForSpec(fti),
                    LabelUtils.GetLabelForSpec(fti),
                    UnitFormatUtils.Format(units_metric, fti, 1, false));

            var units = UnitUtils.GetAllUnits();

            Debug.Print("{0} units:", units.Count);

            //string s;

            foreach (var fti in units)
                // this returns false for all units! why?
                //UnitUtils.IsMeasurableSpec( fti );

                // this throwsz
                // Autodesk.Revit.Exceptions.FunctionId
                // "specTypeId is not a measurable spec identifier. See UnitUtils.IsMeasurableSpec(ForgeTypeId).\r\nParameter name: specTypeId"}
                // s = UnitFormatUtils.Format( units_metric, fti, 1, false );

                Debug.Print("{0}: {1}, {2}, {3}",
                    fti, fti.TypeId,
                    UnitUtils.GetTypeCatalogStringForUnit(fti),
                    LabelUtils.GetLabelForUnit(fti));
        }

        #region Conditional compilation using ifdef to handle ForgeTypeId

        public static double InternalUnitToMillimetres(
            double a,
            bool roundup = false)
        {
            double mm;

#if (CONFIG_R2019 || CONFIG_R2020)
      mm = UnitUtils.ConvertFromInternalUnits(
        a, DisplayUnitType.DUT_MILLIMETERS );
#else
            mm = UnitUtils.ConvertFromInternalUnits(
                a, UnitTypeId.Millimeters);
#endif

            return roundup ? Math.Round(mm, 0) : mm;
        }

        #endregion // Conditional compilation using ifdef to handle ForgeTypeId

        #endregion
    }
}
