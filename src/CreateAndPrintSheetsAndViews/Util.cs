// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from CreateAndPrintSheetsAndViews by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CreateAndPrintSheetsAndViews

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.CreateAndPrintSheetsAndViews.CS
{
    class Util
    {
        #region Geometrical Comparison
        public const double _eps = 1.0e-9;

        public static double Eps => _eps;

        public static double MinLineLength => _eps;

        public static double TolPointOnPlane => _eps;

        public static bool IsZero(
            double a,
            double tolerance = _eps)
        {
            return tolerance > Math.Abs(a);
        }

        public static bool IsEqual(
            double a,
            double b,
            double tolerance = _eps)
        {
            return IsZero(b - a, tolerance);
        }

        public static int Compare(
            double a,
            double b,
            double tolerance = _eps)
        {
            return IsEqual(a, b, tolerance)
                ? 0
                : a < b
                    ? -1
                    : 1;
        }

        public static int Compare(
            XYZ p,
            XYZ q,
            double tolerance = _eps)
        {
            var d = Compare(p.X, q.X, tolerance);

            if (0 == d)
            {
                d = Compare(p.Y, q.Y, tolerance);

                if (0 == d) d = Compare(p.Z, q.Z, tolerance);
            }

            return d;
        }

        public static bool IsEqual(
            XYZ p,
            XYZ q,
            double tolerance = _eps)
        {
            return 0 == Compare(p, q, tolerance);
        }

        public static bool IsParallel(XYZ p, XYZ q)
        {
            return p.CrossProduct(q).IsZeroLength();
        }

        public static bool AreCollinear(XYZ p, XYZ q, XYZ r)
        {
            var v = q - p;
            var w = r - p;
            return IsParallel(v, w);
        }
        #endregion // Geometrical Comparison

        #region Unit Handling

        private const double _inchToMm = 25.4;
        private const double _footToMm = 12 * _inchToMm;
        private const double _footToMeter = _footToMm * 0.001;
        private const double _sqfToSqm = _footToMeter * _footToMeter;

        public static double FootToMm(double length)
        {
            return length * _footToMm;
        }

        public static UV FootToMm(UV v)
        {
            return v * _footToMm;
        }

        public static XYZ FootToMm(XYZ v)
        {
            return v * _footToMm;
        }

        public static int ToInt(double x)
        {
            return (int)Math.Round(x,
                MidpointRounding.AwayFromZero);
        }

        public static int FootToMmInt(double length)
        {
            return ToInt(_footToMm * length);
        }
        #endregion // Unit Handling

        #region Formatting

        public static string PluralSuffix(int n)
        {
            return 1 == n ? "" : "s";
        }

        public static string PluralSuffixY(int n)
        {
            return 1 == n ? "y" : "ies";
        }

        public static string DotOrColon(int n)
        {
            return 0 < n ? ":" : ".";
        }

        public static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        public static string AngleString(double angle)
        {
            return $"{RealString(angle * 180 / Math.PI)} degrees";
        }

        public static string MmString(double length)
        {
            return $"{Math.Round(FootToMm(length))} mm";
        }

        public static string PointString(
            UV p,
            bool onlySpaceSeparator = false)
        {
            var format_string = onlySpaceSeparator
                ? "{0} {1}"
                : "({0},{1})";

            return string.Format(format_string,
                RealString(p.U),
                RealString(p.V));
        }

        public static string PointString(
            XYZ p,
            bool onlySpaceSeparator = false)
        {
            var format_string = onlySpaceSeparator
                ? "{0} {1} {2}"
                : "({0},{1},{2})";

            return string.Format(format_string,
                RealString(p.X),
                RealString(p.Y),
                RealString(p.Z));
        }

        public static string PointStringMm(
            XYZ p,
            bool onlySpaceSeparator = false)
        {
            var format_string = onlySpaceSeparator
                ? "{0} {1} {2}"
                : "({0},{1},{2})";

            return string.Format(format_string,
                FootToMmInt(p.X),
                FootToMmInt(p.Y),
                FootToMmInt(p.Z));
        }

        public static string PointStringInt(
            XYZ p,
            bool onlySpaceSeparator = false)
        {
            var format_string = onlySpaceSeparator
                ? "{0} {1} {2}"
                : "({0},{1},{2})";

            return string.Format(format_string,
                ToInt(p.X), ToInt(p.Y), ToInt(p.Z));
        }

        public static string ElementDescription(Element e)
        {
            if (null == e) return "<null>";

            var fi = e as FamilyInstance;

            var typeName = e.GetType().Name;

            var categoryName = null == e.Category
                ? string.Empty
                : $"{e.Category.Name} ";

            var familyName = null == fi
                ? string.Empty
                : $"{fi.Symbol.Family.Name} ";

            var symbolName = null == fi
                             || e.Name.Equals(fi.Symbol.Name)
                ? string.Empty
                : $"{fi.Symbol.Name} ";

            return $"{typeName} {categoryName}{familyName}{symbolName}<{e.Id.Value} {e.Name}>";
        }

        private const string _caption = "CreateAndPrintSheetsAndViews";

        public static void InfoMsg2(
            string instruction,
            string content)
        {
            Debug.WriteLine($"{instruction}\r\n{content}");
            var d = new TaskDialog(_caption);
            d.MainInstruction = instruction;
            d.MainContent = content;
            d.Show();
        }

        public static void InfoMsg3(
            string instruction,
            IList<string> content)
        {
            string s = string.Join("\r\n", content);
            Debug.WriteLine($"{instruction}\r\n{s}");
            var d = new TaskDialog(_caption);
            d.MainInstruction = instruction;
            d.MainContent = s;
            d.Show();
        }

        public static bool AskYesNoQuestion(string question)
        {
            TaskDialog taskDialog = new TaskDialog("Please answer Yes or No");
            taskDialog.MainContent = question;
            TaskDialogCommonButtons buttons
                = TaskDialogCommonButtons.Yes
                  | TaskDialogCommonButtons.No;
            taskDialog.CommonButtons = buttons;
            TaskDialogResult result = taskDialog.Show();
            return result == TaskDialogResult.Yes;
        }
        #endregion // Formatting

        public static string GetProductCode(Element e)
        {
            const BuiltInParameter _bip_product_code
                = BuiltInParameter.FABRICATION_PRODUCT_CODE;
            Parameter p = e.get_Parameter(_bip_product_code);
            return (null != p)
                ? p.AsString()
                : null;
        }
    }
}
