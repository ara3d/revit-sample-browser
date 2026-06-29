// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SplitStraight : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                // check user selection
                var uiDoc = commandData.Application.ActiveUIDocument;
                var doc = uiDoc.Document;

                var refObj =
                    uiDoc.Selection.PickObject(ObjectType.Element, "Pick a fabrication part straight to start.");

                if (!(doc.GetElement(refObj) is FabricationPart part) || part.IsAStraight() == false)
                {
                    message = "The selected element is not a fabrication part straight.";
                    return Result.Failed;
                }

                // get the 2 end connectors
                var connectors = new List<Connector>();
                foreach (Connector c in part.ConnectorManager.Connectors)
                {
                    if (c.ConnectorType == ConnectorType.End)
                        connectors.Add(c);
                }

                if (connectors.Count != 2)
                {
                    message = "There are not 2 end connectors on this straight.";
                    return Result.Failed;
                }

                var conn1 = connectors[0];
                var conn2 = connectors[1];

                var x = (conn1.Origin.X + conn2.Origin.X) / 2.0;
                var y = (conn1.Origin.Y + conn2.Origin.Y) / 2.0;
                var z = (conn1.Origin.Z + conn2.Origin.Z) / 2.0;

                var midpoint = new XYZ(x, y, z);

                if (part.CanSplitStraight(midpoint) == false)
                {
                    message = "straight cannot be split at its mid-point";
                    return Result.Failed;
                }

                using (var trans = new Transaction(doc, "split straight"))
                {
                    trans.Start();

                    part.SplitStraight(midpoint);

                    trans.Commit();
                }

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
