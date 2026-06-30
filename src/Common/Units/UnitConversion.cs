// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Collections.Generic;
using ConversionValue = Ara3D.RevitSampleBrowser.BoundaryConditions.CS.ConversionValue;

namespace Ara3D.RevitSampleBrowser.Common.Units
{
    public static class UnitConversion
    {
        public static Dictionary<string, ConversionValue> UnitDictionary { get; }

        static UnitConversion()
        {
            UnitDictionary = [];
            var degree = (char)0xb0;
            var square = (char)0xB2;
            var cube = (char)0xB3;

            AddNewUnit(1, 175126.835246476, "kip/in", "PTSpringModulusConver");
            AddNewUnit(1, 47880.2589803358, $"kip-f/{degree}F", "PRSpringModulusConver");
            AddNewUnit(4, 14593.9029372064, $"kip/ft{square}", "LTSpringModulusConver");
            AddNewUnit(1, 47880.2589803358, $"kip-f/{degree}F/ft", "LRSpringModulusConver");
            AddNewUnit(1, 14593.9029372064, $"kip/ft{cube}", "ATSpringModulusConver");
        }

        private static void AddNewUnit(int precision, double ratio, string unitName, string key)
        {
            UnitDictionary.Add(key, new ConversionValue(precision, ratio, unitName));
        }

        public static double CovertFromApi(ForgeTypeId to, double value)
        {
            return value *= ImperialDutRatio(to);
        }

        public static double CovertToApi(double value, ForgeTypeId from)
        {
            return value /= ImperialDutRatio(from);
        }

        private static double ImperialDutRatio(ForgeTypeId unitTypeId)
        {
            if (unitTypeId == UnitTypeId.Feet) return 1;
            if (unitTypeId == UnitTypeId.FeetFractionalInches) return 1;
            if (unitTypeId == UnitTypeId.Inches) return 12;
            if (unitTypeId == UnitTypeId.FractionalInches) return 12;
            if (unitTypeId == UnitTypeId.Meters) return 0.3048;
            if (unitTypeId == UnitTypeId.Centimeters) return 30.48;
            return unitTypeId == UnitTypeId.Millimeters ? 304.8 : unitTypeId == UnitTypeId.MetersCentimeters ? 0.3048 : 1;
        }

        public static string GetUnitLabel(ForgeTypeId unitTypeId)
        {
            Dictionary<ForgeTypeId, string> unitLabels = new()
            {
                { UnitTypeId.Centimeters, "cm" },
                { UnitTypeId.Feet, "'" },
                { UnitTypeId.Inches, "\"" },
                { UnitTypeId.FeetFractionalInches, "'" },
                { UnitTypeId.FractionalInches, "\"" },
                { UnitTypeId.Meters, "m" },
                { UnitTypeId.MetersCentimeters, "m" },
                { UnitTypeId.Millimeters, "mm" }
            };
            return unitLabels.TryGetValue(unitTypeId, out var label) ? label : string.Empty;
        }
    }
}
