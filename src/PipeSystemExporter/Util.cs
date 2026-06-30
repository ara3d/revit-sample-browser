// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from PipeSystemExporter by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/PipeSystemExporter

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;

namespace Ara3D.RevitSampleBrowser.PipeSystemExporter.CS
{
    internal static class Util
    {
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

        public static List<XYZ> GetConnectorPoints(Element e)
        {
            var cons = GetConnectors(e);
            var n = cons.Size;
            var pts = new List<XYZ>(n);
            foreach (Connector con in cons)
                pts.Add(con.Origin);
            return pts;
        }

        public static string PluralSuffix(int n)
        {
            return 1 == n ? "" : "s";
        }

        public static string DotOrColon(int n)
        {
            return 0 < n ? ":" : ".";
        }

        public static string RealString(double a)
        {
            return a.ToString("0.####");
        }

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
