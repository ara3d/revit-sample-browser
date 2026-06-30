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
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class CmdCableTray2 : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                var doc = commandData.Application.ActiveUIDocument.Document;

                using Transaction transaction = new(doc, "Cable tray elbow fitting");
                transaction.Start();

                var idType = Util.FindCableTrayTypeId(doc, "Default");
                var idLevel = Util.FindLevelId(doc, "Level 1");

                XYZ start1 = new(10, 10, 10);
                XYZ end1 = new(20, 10, 10);
                var c1 = CableTray.Create(doc, idType, start1, end1, idLevel);

                XYZ start2 = new(30, 20, 10);
                XYZ end2 = new(30, 30, 10);
                var c2 = CableTray.Create(doc, idType, start2, end2, idLevel);

                Connector c1end = null;
                Connector c2start = null;

                foreach (Connector c in c1.ConnectorManager.Connectors)
                {
                    if (c.Origin.IsAlmostEqualTo(end1))
                        c1end = c;
                }

                foreach (Connector c in c2.ConnectorManager.Connectors)
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
