// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateCustomLineLoad : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;
                var activeDoc = commandData.Application.ActiveUIDocument;

                //select object for adding a line load
                var eRef = activeDoc.Selection.PickObject(ObjectType.Element, "Please select the analytical element");
                ElementId selectedElementId = null;
                if (eRef != null && eRef.ElementId != ElementId.InvalidElementId)
                    selectedElementId = eRef.ElementId;

                using (var transaction = new Transaction(document, "Create custom LineLoad"))
                {
                    transaction.Start();

                    var start = activeDoc.Selection.PickPoint("start");
                    var end = activeDoc.Selection.PickPoint("end");

                    var line = Line.CreateBound(start, end);

                    if (LineLoad.IsCurveInsideHostBoundaries(document, selectedElementId, line))
                        LineLoad.Create(document, selectedElementId, line, new XYZ(1, 0, 0), new XYZ(1, 0, 0), null);

                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
