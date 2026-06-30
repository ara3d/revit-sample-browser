// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from CableTraySample by Gavin_WS / Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/CableTraySample

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.CableTraySample.CS
{
    /// <summary>
    ///     Creates a multi-segment cable tray run and inserts an elbow fitting
    ///     between the first horizontal and vertical segments.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CmdCableTray : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;

                var idType = Util.FindCableTrayTypeId(doc, "Default");
                var idLevel = Util.FindLevelId(doc, "Level 1");

                using Transaction transaction = new(doc, "Cable tray elbow fitting");
                transaction.Start();

                XYZ start1 = new(-30.498257567, 38.420015690, 10.058014598);
                XYZ end1 = new(-20.435555001, 30.837225417, 10.058014598);
                var tray1 = CableTray.Create(doc, idType, start1, end1, idLevel);

                XYZ start2 = new(-20.435555001, 30.837225417, 10.058014598);
                XYZ end2 = new(-20.435555001, 30.837225417, 13.338854493);
                var tray2 = CableTray.Create(doc, idType, start2, end2, idLevel);

                XYZ start3 = new(-20.435555001, 30.837225417, 13.338854493);
                XYZ end3 = new(-11.525321413, 24.122882809, 13.338854493);
                CableTray.Create(doc, idType, start3, end3, idLevel);

                XYZ start4 = new(-11.525321413, 24.122882809, 13.338854493);
                XYZ end4 = new(-11.525321413, 24.122882809, 10.058014598);
                CableTray.Create(doc, idType, start4, end4, idLevel);

                XYZ start5 = new(-11.525321413, 24.122882809, 10.058014598);
                XYZ end5 = new(-2.001326892, 16.946038164, 10.058014598);
                CableTray.Create(doc, idType, start5, end5, idLevel);

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
