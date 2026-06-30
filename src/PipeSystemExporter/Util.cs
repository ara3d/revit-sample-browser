// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from PipeSystemExporter by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/PipeSystemExporter

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace Ara3D.RevitSampleBrowser.PipeSystemExporter.CS
{
    internal static class Util
    {
        /// <summary>
        ///     Get the connector set of a given element.
        /// </summary>
        public static ConnectorSet GetConnectors(Element e)
        {
            if (e == null) return null;

            if (e is FamilyInstance fi && fi.MEPModel != null)
                return fi.MEPModel.ConnectorManager.Connectors;

            if (e is MEPSystem mepSystem)
                return mepSystem.ConnectorManager.Connectors;

            if (e is MEPCurve mepCurve)
                return mepCurve.ConnectorManager.Connectors;

            return null;
        }

        /// <summary>
        ///     Get a list of connector origin points for a given element.
        /// </summary>
        public static List<XYZ> GetConnectorPoints(Element e)
        {
            var cons = GetConnectors(e);
            var n = cons.Size;
            var pts = new List<XYZ>(n);
            foreach (Connector con in cons)
                pts.Add(con.Origin);
            return pts;
        }

        /// <summary>
        ///     Return an English plural suffix for the given number of items.
        /// </summary>
        public static string PluralSuffix(int n)
        {
            return 1 == n ? "" : "s";
        }

        /// <summary>
        ///     Return a dot for zero or a colon for more than zero.
        /// </summary>
        public static string DotOrColon(int n)
        {
            return 0 < n ? ":" : ".";
        }

        /// <summary>
        ///     Return a string for a real number formatted to four decimal places max.
        /// </summary>
        public static string RealString(double a)
        {
            return a.ToString("0.####");
        }

        /// <summary>
        ///     Return a string for an XYZ point with coordinates formatted for display.
        /// </summary>
        public static string PointString(XYZ p, bool onlySpaceSeparator = false)
        {
            var formatString = onlySpaceSeparator
                ? "{0} {1} {2}"
                : "({0},{1},{2})";

            return string.Format(formatString,
                RealString(p.X),
                RealString(p.Y),
                RealString(p.Z));
        }
    }
}
