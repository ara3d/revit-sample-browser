// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Diagnostics;
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
            TaskDialog d = new(_caption)
            {
                MainInstruction = instruction,
                MainContent = content
            };
            d.Show();
        }

        public static void InfoMsg3(
            string instruction,
            IList<string> content)
        {
            Debug.WriteLine($"{instruction}\r\n{content}");
            TaskDialog d = new(_caption)
            {
                MainInstruction = instruction,
                MainContent = string.Join("\r\n", content)
            };
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

        public static XYZ GetFamilyInstanceLocation(
            FamilyInstance fi)
        {
            return ((LocationPoint)fi?.Location)?.Point;
        }

        #endregion
    }
}
