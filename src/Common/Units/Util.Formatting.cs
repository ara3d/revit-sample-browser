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
        #region Formatting

        /// <summary>
        ///     Return an English plural suffix for the given
        ///     number of items, i.e. 's' for zero or more
        ///     than one, and nothing for exactly one.
        /// </summary>
        public static string PluralSuffix(int n)
        {
            return 1 == n ? "" : "s";
        }

        public static string PluralSuffix(long n)
        {
            return 1 == n ? "" : "s";
        }

        /// <summary>
        ///     Return an English plural suffix 'ies' or
        ///     'y' for the given number of items.
        /// </summary>
        public static string PluralSuffixY(int n)
        {
            return 1 == n ? "y" : "ies";
        }

        /// <summary>
        ///     Return a dot (full stop) for zero
        ///     or a colon for more than zero.
        /// </summary>
        public static string DotOrColon(int n)
        {
            return 0 < n ? ":" : ".";
        }

        /// <summary>
        ///     Return a string for a real number
        ///     formatted to two decimal places.
        /// </summary>
        public static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        /// <summary>
        ///     Return a hash string for a real number
        ///     formatted to nine decimal places.
        /// </summary>
        public static string HashString(double a)
        {
            return a.ToString("0.#########");
        }

        /// <summary>
        ///     Return a string representation in degrees
        ///     for an angle given in radians.
        /// </summary>
        public static string AngleString(double angle, bool addUnits = true)
        {
            string sunits = addUnits ? " degrees" : string.Empty;
            return $"{RealString(angle * 180 / Math.PI)}" + sunits;
        }

        /// <summary>
        ///     Return a string for a length in millimetres
        ///     formatted as an integer value.
        /// </summary>
        public static string MmString(double length, bool addUnits = true)
        {
            string sunits = addUnits ? " mm" : string.Empty;
            return $"{Math.Round(FootToMm(length))}" + sunits;
        }

        /// <summary>
        ///     Return a string for a UV point
        ///     or vector with its coordinates
        ///     formatted to two decimal places.
        /// </summary>
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

        /// <summary>
        ///     Return a string for an XYZ point
        ///     or vector with its coordinates
        ///     formatted to two decimal places.
        /// </summary>
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

        /// <summary>
        ///     Return a hash string for an XYZ point
        ///     or vector with its coordinates
        ///     formatted to nine decimal places.
        /// </summary>
        public static string HashString(XYZ p)
        {
            return $"({HashString(p.X)},{HashString(p.Y)},{HashString(p.Z)})";
        }

        /// <summary>
        ///     Return a string for this bounding box
        ///     with its coordinates formatted to two
        ///     decimal places.
        /// </summary>
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

        /// <summary>
        ///     Return a string for this bounding box
        ///     with its coordinates formatted to two
        ///     decimal places.
        /// </summary>
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

        /// <summary>
        ///     Return a string for this plane
        ///     with its coordinates formatted to two
        ///     decimal places.
        /// </summary>
        public static string PlaneString(Plane p)
        {
            return $"plane origin {PointString(p.Origin)}, plane normal {PointString(p.Normal)}";
        }

        /// <summary>
        ///     Return a string for this transformation
        ///     with its coordinates formatted to two
        ///     decimal places.
        /// </summary>
        public static string TransformString(Transform t)
        {
            return $"({PointString(t.Origin)},{PointString(t.BasisX)},{PointString(t.BasisY)},{PointString(t.BasisZ)})";
        }

        /// <summary>
        ///     Return a string for a list of doubles
        ///     formatted to two decimal places.
        /// </summary>
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

        /// <summary>
        ///     Return a string for this point array
        ///     with its coordinates formatted to two
        ///     decimal places.
        /// </summary>
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

        /// <summary>
        ///     Return a string for this point array
        ///     with its coordinates formatted to two
        ///     decimal places.
        /// </summary>
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

        /// <summary>
        ///     Return a string representing the data of a
        ///     curve. Currently includes detailed data of
        ///     line and arc elements only.
        /// </summary>
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

        /// <summary>
        ///     Return a string for this curve with its
        ///     tessellated point coordinates formatted
        ///     to two decimal places.
        /// </summary>
        public static string CurveTessellateString(
            Curve curve)
        {
            return $"curve tessellation {PointArrayString(curve.Tessellate())}";
        }

        #region Using Obsolete pre-Forge Unit API Functionality Deprecated in Revit 2021

#if USE_PRE_FORGE_UNIT_FUNCTIONALITY
    /// <summary>
    /// Convert a UnitSymbolType enumeration value
    /// to a brief human readable abbreviation string.
    /// </summary>
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
