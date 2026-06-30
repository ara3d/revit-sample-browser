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
        #region Display a message

        private const string _caption = "The Building Coder";

        public static void InfoMsg(string msg)
        {
            Debug.WriteLine(msg);
            WinForms.MessageBox.Show(msg,
                _caption,
                WinForms.MessageBoxButtons.OK,
                WinForms.MessageBoxIcon.Information);
        }

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
            Debug.WriteLine($"{instruction}\r\n{content}");
            var d = new TaskDialog(_caption);
            d.MainInstruction = instruction;
            d.MainContent = string.Join("\r\n",content);
            d.Show();
        }

        public static void ErrorMsg(string msg)
        {
            Debug.WriteLine(msg);
            WinForms.MessageBox.Show(msg,
                _caption,
                WinForms.MessageBoxButtons.OK,
                WinForms.MessageBoxIcon.Error);
        }

        /// <summary>
        ///     Return a string describing the given element:
        ///     .NET type name,
        ///     category name,
        ///     family and symbol name for a family instance,
        ///     element id and element name.
        /// </summary>
        public static string ElementDescription(
            Element e)
        {
            if (null == e) return "<null>";

            // For a wall, the element name equals the
            // wall type name, which is equivalent to the
            // family name ...

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

        /// <summary>
        ///     Return a location for the given element using
        ///     its LocationPoint Point property,
        ///     LocationCurve start point, whichever
        ///     is available.
        /// </summary>
        /// <param name="p">Return element location point</param>
        /// <param name="e">Revit Element</param>
        /// <returns>
        ///     True if a location point is available
        ///     for the given element, otherwise false.
        /// </returns>
        public static bool GetElementLocation(
            out XYZ p,
            Element e)
        {
            p = XYZ.Zero;
            var rc = false;
            var loc = e.Location;
            if (null != loc)
            {
                if (loc is LocationPoint lp)
                {
                    p = lp.Point;
                    rc = true;
                }
                else
                {
                    var lc = loc as LocationCurve;

                    Debug.Assert(null != lc,
                        "expected location to be either point or curve");

                    p = lc.Curve.GetEndPoint(0);
                    rc = true;
                }
            }

            // Todo: if thew Location property is null,
            // try using the BuondingBox. If that is null
            // or empty, try to retrieve geometry vertices

            return rc;
        }

        /// <summary>
        ///     Return the location point of a family instance or null.
        ///     This null coalesces the location so you won't get an
        ///     error if the FamilyInstance is an invalid object.
        /// </summary>
        public static XYZ GetFamilyInstanceLocation(
            FamilyInstance fi)
        {
            return ((LocationPoint) fi?.Location)?.Point;
        }

        #endregion
    }
}
