// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateCustomAreaLoad : IExternalCommand
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

                var start = activeDoc.Selection.PickPoint("start");
                var end = activeDoc.Selection.PickPoint("end");

                //create curveloop which will be assigned to the analytical panel
                var profileloop = new CurveLoop();
                profileloop.Append(Line.CreateBound(
                    start, new XYZ(end.X, start.Y, 0)));
                profileloop.Append(Line.CreateBound(
                    new XYZ(end.X, start.Y, 0), end));
                profileloop.Append(Line.CreateBound(
                    end, new XYZ(start.X, end.Y, 0)));
                profileloop.Append(Line.CreateBound(
                    new XYZ(start.X, end.Y, 0), start));

                var loops = new List<CurveLoop> { profileloop };

                using (var transaction = new Transaction(document, "Create custom AreaLoad"))
                {
                    transaction.Start();

                    if (AreaLoad.IsCurveLoopsInsideHostBoundaries(document, selectedElementId, loops))
                        AreaLoad.Create(document, selectedElementId, loops, new XYZ(1, 0, 0), null);

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
