// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from CableTraySample by Gavin_WS / Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/CableTraySample

using System;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.CableTraySample.CS
{
    /// <summary>
    ///     Creates horizontal and vertical cable tray segments, rotates the vertical segment
    ///     to align connector coordinate systems, then inserts an elbow fitting.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CmdCableTray4 : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;

                using var transaction = new Transaction(doc, "Cable tray elbow fitting");
                transaction.Start();

                var idType = Util.FindCableTrayTypeId(doc, "Default");
                var idLevel = Util.FindLevelId(doc, "Level 1");

                var start1 = new XYZ(-30.498257567, 38.420015690, 10.058014598);
                var end1 = new XYZ(-20.435555001, 30.837225417, 10.058014598);
                var tray1 = CableTray.Create(doc, idType, start1, end1, idLevel);

                var start2 = new XYZ(-20.435555001, 30.837225417, 10.058014598);
                var end2 = new XYZ(-20.435555001, 30.837225417, 13.338854493);
                var tray2 = CableTray.Create(doc, idType, start2, end2, idLevel);

                Connector c1end = null;
                Connector c2start = null;

                foreach (Connector c in tray1.ConnectorManager.Connectors)
                {
                    if (c.Origin.IsAlmostEqualTo(end1))
                        c1end = c;
                }

                foreach (Connector c in tray2.ConnectorManager.Connectors)
                {
                    if (c.Origin.IsAlmostEqualTo(start2))
                        c2start = c;
                }

                if (c1end != null && c2start != null)
                {
                    var t1 = c1end.CoordinateSystem;
                    var t2 = c2start.CoordinateSystem;

#if DEBUG
                    var lc = tray1.Location as LocationCurve;
                    var line = lc.Curve as Line;
                    var direction1 = line.GetEndPoint(1) - line.GetEndPoint(0);

                    Debug.Assert(direction1.Normalize().IsAlmostEqualTo(t1.BasisZ),
                        "expected connector Z direction to point straight out of cable tray");

                    lc = tray2.Location as LocationCurve;
                    line = lc.Curve as Line;
                    var direction2 = line.GetEndPoint(1) - line.GetEndPoint(0);

                    Debug.Assert(direction2.Normalize().IsAlmostEqualTo(-t2.BasisZ),
                        "expected connector Z direction to point straight out of cable tray");

                    Debug.Assert(direction2.Normalize().IsAlmostEqualTo(XYZ.BasisZ),
                        "expected cable tray 2 to be pointing straight up");
#endif

                    var angle = t2.BasisY.AngleOnPlaneTo(t1.BasisZ, XYZ.BasisZ);
                    var axis = Line.CreateUnbound(start2, XYZ.BasisZ);
                    tray2.Location.Rotate(axis, angle);

                    c1end.ConnectTo(c2start);
                    doc.Create.NewElbowFitting(c1end, c2start);
                }

                transaction.Commit();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
