// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.SharedCoordinateSystem.CS;
using Autodesk.Revit.DB;
using System;
using System.Linq;
using Document = Autodesk.Revit.DB.Document;
using FormatValueType = Ara3D.RevitSampleBrowser.Common.Infrastructure.ValueType;

namespace Ara3D.RevitSampleBrowser.Common.Units
{
    public static class ValueFormatting
    {
        public static string FormatNumber(Document doc, double number, ForgeTypeId specTypeId)
        {
            return UnitFormatUtils.Format(doc.GetUnits(), specTypeId, number, false);
        }

        public static string FormatParameterLine(Parameter param, Document document)
        {
            var name = param.Definition.Name;
            return param.StorageType switch
            {
                StorageType.Double => $"{name}\tdouble\t{param.AsDouble()}",
                StorageType.ElementId => ElementQuery.FormatElementIdParameter(name, param, document),
                StorageType.Integer => $"{name}\tint\t{param.AsInteger()}",
                StorageType.String => $"{name}\tstring\t{param.AsString()}",
                StorageType.None => $"{name}\tnone\t{param.AsValueString()}",
                _ => $"{name}\tunknown\t"
            };
        }

        public static double AngleStringToDouble(string value)
        {
            var n = value.Length - 1;
            if (!char.IsDigit(value[n]))
                value = value.Substring(0, n);
            return double.Parse(value) * DegreesToRadians;
        }

        public static string DoubleToAngleString(double value)
        {
            return Math.Round(value / DegreesToRadians, 3).ToString() + (char)0xb0;
        }

        public static double TimeZoneStringToDouble(string value)
        {
            var timeZoneDouble = value.Substring(4, value.IndexOf(')') - 4).Replace(':', '.').Trim();
            return string.IsNullOrEmpty(timeZoneDouble) ? 0d : double.Parse(timeZoneDouble);
        }

        public static string TimeZoneDoubleToString(double timeZone, string[] timeZones)
        {
            return timeZones.LastOrDefault(tmpTimeZone => TimeZoneStringToDouble(tmpTimeZone) == timeZone);
        }

        public static string FormatFractionalInches(double value, string formatter = "#0.000")
        {
            return $"{(value / ToFractionalInches).ToString(formatter)}\"";
        }

        public const double ToFractionalInches = 0.08333333;

        public const double DegreesToRadians = Math.PI / 180.0;

        public static CityInfoString ConvertFrom(CityInfo cityInfo)
        {
            return new CityInfoString(
                SampleBrowserUtils.DoubleToString(cityInfo.Latitude, FormatValueType.Angle),
                SampleBrowserUtils.DoubleToString(cityInfo.Longitude, FormatValueType.Angle));
        }

        public static CityInfo ConvertTo(CityInfoString cityInfoString)
        {
            SampleBrowserUtils.StringToDouble(cityInfoString.Latitude, FormatValueType.Angle, out var lat);
            SampleBrowserUtils.StringToDouble(cityInfoString.Longitude, FormatValueType.Angle, out var lon);
            return new CityInfo(lat, lon);
        }

    }
}