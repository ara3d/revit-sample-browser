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
        #region Consolidated misc sample helpers

        internal static void LogIdlingMessage(string msg)
        {
            var dt = DateTime.Now.ToString("u");
            Debug.Print($"{dt} {msg}");
        }

        internal static void PrintLibraryPathMap(
            IDictionary<string, string> map,
            string description)
        {
            Debug.Print("\n{0}:\n", description);

            foreach (var pair in map)
                Debug.Print("{0} -> {1}", pair.Key, pair.Value);
        }

        public static BitmapSource ConvertBitmapToBitmapSource(
            Bitmap bmp)
        {
            return Imaging
                .CreateBitmapSourceFromHBitmap(
                    bmp.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        ///     Create a new group of the specified elements
        ///     in the current active view at the given offset.
        /// </summary>
        internal static void CreateGroup(
            Document doc,
            ICollection<ElementId> ids,
            XYZ offset)
        {
            var group = doc.Create.NewGroup(ids);

            var location = group.Location
                as LocationPoint;

            var p = location.Point + offset;

            doc.Create.PlaceGroup(
                p, group.GroupType);

            group.UngroupMembers();
        }

        public static Dictionary<string, string> GetFilePaths(
            Autodesk.Revit.ApplicationServices.Application app,
            bool onlyImportedFiles)
        {
            var docs = app.Documents;
            var n = docs.Size;

            var dict
                = new Dictionary<string, string>(n);

            foreach (Document doc in docs)
                if (!onlyImportedFiles
                    || null == doc.ActiveView)
                {
                    var path = doc.PathName;
                    var i = path.LastIndexOf("\\") + 1;
                    var name = path.Substring(i);
                    dict.Add(name, path);
                }

            return dict;
        }

        internal static void SetModelCurvesColor(
            ModelCurveArray modelCurves,
            View view,
            Autodesk.Revit.DB.Color color)
        {
            foreach (var curve in modelCurves
                .Cast<ModelCurve>())
            {
                var overrides = view.GetElementOverrides(
                    curve.Id);

                overrides.SetProjectionLineColor(color);

                view.SetElementOverrides(curve.Id, overrides);
            }
        }

        /// <summary>
        ///     Return the first wall found that
        ///     uses the given wall type.
        /// </summary>
        public static Wall GetFirstWallUsingType(
            Document doc,
            WallType wallType)
        {
            var bip
                = BuiltInParameter.ELEM_TYPE_PARAM;

            var provider
                = new ParameterValueProvider(
                    new ElementId(bip));

            FilterNumericRuleEvaluator evaluator
                = new FilterNumericEquals();

            FilterRule rule = new FilterElementIdRule(
                provider, evaluator, wallType.Id);

            var filter
                = new ElementParameterFilter(rule);

            var collector
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(Wall))
                    .WherePasses(filter);

            return collector.FirstElement() as Wall;
        }

        #endregion
    }
}
