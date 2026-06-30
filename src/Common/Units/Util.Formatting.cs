// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Formatting

        public static string PluralSuffix(int n)
        {
            return 1 == n ? "" : "s";
        }

        public static string PluralSuffix(long n)
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

        public static string RealString(double a, string format = "0.##")
        {
            return a.ToString(format);
        }

        public static string HashString(double a)
        {
            return a.ToString("0.#########");
        }

        public static string AngleString(double angle, bool addUnits = true)
        {
            var sunits = addUnits ? " degrees" : string.Empty;
            return $"{RealString(angle * 180 / Math.PI)}" + sunits;
        }

        public static string MmString(double length, bool addUnits = true)
        {
            var sunits = addUnits ? " mm" : string.Empty;
            return $"{Math.Round(FootToMm(length))}" + sunits;
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

        public static string HashString(XYZ p)
        {
            return $"({HashString(p.X)},{HashString(p.Y)},{HashString(p.Z)})";
        }

        public static string BoundingBoxString(
            BoundingBoxUV bb,
            bool onlySpaceSeparator = false)
        {
            var format_string = onlySpaceSeparator
                ? "{0} {1}"
                : "({0},{1})";

            return string.Format(format_string,
                PointString(bb.Min, onlySpaceSeparator),
                PointString(bb.Max, onlySpaceSeparator));
        }

        public static string BoundingBoxString(
            BoundingBoxXYZ bb,
            bool onlySpaceSeparator = false)
        {
            var format_string = onlySpaceSeparator
                ? "{0} {1}"
                : "({0},{1})";

            return string.Format(format_string,
                PointString(bb.Min, onlySpaceSeparator),
                PointString(bb.Max, onlySpaceSeparator));
        }

        public static string PlaneString(Plane p)
        {
            return $"plane origin {PointString(p.Origin)}, plane normal {PointString(p.Normal)}";
        }

        public static string TransformString(Transform t)
        {
            return $"({PointString(t.Origin)},{PointString(t.BasisX)},{PointString(t.BasisY)},{PointString(t.BasisZ)})";
        }

        public static string DoubleArrayString(
            IEnumerable<double> a,
            bool onlySpaceSeparator = false)
        {
            var separator = onlySpaceSeparator
                ? " "
                : ", ";

            return string.Join(separator,
                a.Select(
                    x => RealString(x)));
        }

        public static string PointArrayString(
            IEnumerable<UV> pts,
            bool onlySpaceSeparator = false)
        {
            var separator = onlySpaceSeparator
                ? " "
                : ", ";

            return string.Join(separator,
                pts.Select(p
                    => PointString(p, onlySpaceSeparator)));
        }

        public static string PointArrayString(
            IEnumerable<XYZ> pts,
            bool onlySpaceSeparator = false)
        {
            var separator = onlySpaceSeparator
                ? " "
                : ", ";

            return string.Join(separator,
                pts.Select(p
                    => PointString(p, onlySpaceSeparator)));
        }

        public static string CurveString(Curve c)
        {
            var s = c.GetType().Name.ToLower();

            var p = c.GetEndPoint(0);
            var q = c.GetEndPoint(1);

            s += $" {PointString(p)} --> {PointString(q)}";

            // To list intermediate points or draw an
            // approximation using straight line segments,
            // we can access the curve tesselation, cf.
            // CurveTessellateString:

            //foreach( XYZ r in lc.Curve.Tessellate() )
            //{
            //}

            // List arc data:

            var arc = c as Arc;

            if (null != arc)
                s += $" center {PointString(arc.Center)} radius {arc.Radius}";

            // Todo: add support for other curve types
            // besides line and arc.

            return s;
        }

        public static string CurveTessellateString(
            Curve curve)
        {
            return $"curve tessellation {PointArrayString(curve.Tessellate())}";
        }

        #region Using Obsolete pre-Forge Unit API Functionality Deprecated in Revit 2021

#if USE_PRE_FORGE_UNIT_FUNCTIONALITY
    public static string UnitSymbolTypeString(
      UnitSymbolType u )
    {
      string s = u.ToString();

      Debug.Assert( s.StartsWith( "UST_" ),
        "expected UnitSymbolType enumeration value "
        + "to begin with 'UST_'" );

      s = s.Substring( 4 )
        .Replace( "_SUP_", "^" )
        .ToLower();

      return s;
    }
#endif // USE_PRE_FORGE_UNIT_FUNCTIONALITY

        #endregion // Using Obsolete pre-Forge Unit API Functionality Deprecated in Revit 2021

        #endregion
    }
}
