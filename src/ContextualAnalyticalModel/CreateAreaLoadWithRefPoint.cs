// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateAreaLoadWithRefPoint : IExternalCommand
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
                List<CurveLoop> loops = new()
                { SampleBrowserUtils.CreateRectangleLoop(start, end) };

                using Transaction transaction = new(document, "Create custom AreaLoad");
                transaction.Start();

                IList<int> refPointsIndexes = [0, 2, 2];
                IList<int> refPointsCurveEnds = [0, 1, 0];

                IList<XYZ> forceVector = [new XYZ(0, 0, -10000), new XYZ(0, 0, 0), new XYZ(0, 0, 0)];

                if (AreaLoad.IsCurveLoopsInsideHostBoundaries(document, selectedElementId, loops))
                    AreaLoad.Create(document, selectedElementId, loops, forceVector, refPointsIndexes,
                        refPointsCurveEnds, null);

                transaction.Commit();
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
