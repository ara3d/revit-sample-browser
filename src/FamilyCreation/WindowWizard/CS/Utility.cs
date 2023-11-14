// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.WindowWizard.CS
{
    /// <summary>
    ///     A common class for users to get some specified element
    /// </summary>
    internal class Utility
    {
        /// <summary>
        ///     This method is used to allow user to get reference plane by name,if there is no proper reference plane,will return
        ///     null
        /// </summary>
        /// <param name="name">the name property of reference plane</param>
        /// <param name="app">the application</param>
        /// <param name="doc">the document</param>
        /// <returns>the reference plane or null</returns>
        public static Autodesk.Revit.DB.ReferencePlane GetRefPlaneByName(string name, UIApplication app, Document doc)
        {
            Autodesk.Revit.DB.ReferencePlane r = null;
            var collector = new FilteredElementCollector(app.ActiveUIDocument.Document);
            collector.OfClass(typeof(Autodesk.Revit.DB.ReferencePlane));
            var eit = collector.GetElementIterator();
            eit.Reset();
            while (eit.MoveNext())
            {
                r = eit.Current as Autodesk.Revit.DB.ReferencePlane;
                if (r.Name.Equals(name)) break;
            }

            return r;
        }

        /// <summary>
        ///     This method allows user to get view by name
        /// </summary>
        /// <param name="name">the name property of view</param>
        /// <param name="app">the application</param>
        /// <param name="doc">the document</param>
        /// <returns>the view or null</returns>
        public static View GetViewByName(string name, UIApplication app, Document doc)
        {
            View v = null;
            var collector = new FilteredElementCollector(app.ActiveUIDocument.Document);
            collector.OfClass(typeof(View));
            var eit = collector.GetElementIterator();
            eit.Reset();
            while (eit.MoveNext())
            {
                v = eit.Current as View;
                if (v.Name.Equals(name)) break;
            }

            return v;
        }

        /// <summary>
        ///     This method is used to get elements by type filter
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <param name="app">the application</param>
        /// <param name="doc">the document</param>
        /// <returns>the list of elements</returns>
        public static List<T> GetElements<T>(UIApplication app, Document doc) where T : Element
        {
            var elements = new List<T>();

            var collector = new FilteredElementCollector(app.ActiveUIDocument.Document);
            collector.OfClass(typeof(T));
            var eit = collector.GetElementIterator();
            eit.Reset();
            while (eit.MoveNext())
            {
                if (eit.Current is T element) elements.Add(element);
            }

            return elements;
        }

        /// <summary>
        ///     This function is used to convert from metric to imperial
        /// </summary>
        /// <param name="value">the metric value</param>
        /// <returns>the result</returns>
        public static double MetricToImperial(double value)
        {
            return value / 304.8; //* 0.00328;
        }

        /// <summary>
        ///     This function is used to convert from imperial to metric
        /// </summary>
        /// <param name="value">the imperial value</param>
        /// <returns>the result</returns>
        public static double ImperialToMetric(double value)
        {
            return value * 304.8;
        }
    }
}
