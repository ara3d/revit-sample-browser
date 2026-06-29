// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
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

                var selectedElementId = ElementQuery.GetSelectedObject(activeDoc, "Please select the analytical element");

                var start = activeDoc.Selection.PickPoint("start");
                var end = activeDoc.Selection.PickPoint("end");
                var loops = new List<CurveLoop> { SampleBrowserUtils.CreateRectangleLoop(start, end) };

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
