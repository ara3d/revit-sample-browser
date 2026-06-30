// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MoveAnalyticalNodeUsingElementTransformUtils : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var activeDoc = commandData.Application.ActiveUIDocument;
                var document = activeDoc.Document;

                // Create Analytical Panel
                CreateAnalyticalPanel.CreateAmPanel(document);

                CreateAnalyticalMember.CreateMember(document);

                // Select the node
                var eRef = activeDoc.Selection.PickObject(ObjectType.PointOnElement, "Select an Analytical Node");

                // Move the Analytical Panel using ElementTransformUtils
                using (var transaction = new Transaction(document, "Move panel with ElementTransformUtils"))
                {
                    transaction.Start();
                    ElementTransformUtils.MoveElement(document, eRef.ElementId, new XYZ(-5, -5, 0));
                    transaction.Commit();
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
