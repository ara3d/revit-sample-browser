// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from PipeSystemExporter by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/PipeSystemExporter

using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.PipeSystemExporter.CS
{
    internal static class Util
    {
        public static ConnectorSet GetConnectors(Element e)
        {
            if (e == null) return null;

            if (e is FamilyInstance fi && fi.MEPModel != null)
                return fi.MEPModel.ConnectorManager.Connectors;

            return e is MEPSystem mepSystem
                ? mepSystem.ConnectorManager.Connectors
                : e is MEPCurve mepCurve ? mepCurve.ConnectorManager.Connectors : null;
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
    }
}
