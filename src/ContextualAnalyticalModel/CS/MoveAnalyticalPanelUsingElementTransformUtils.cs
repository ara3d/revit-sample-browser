// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ContextualAnalyticalModel
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MoveAnalyticalPanelUsingElementTransformUtils : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Expected results: The Analytical Panel has been moved and the connection with the Analytical Member was kept
            try
            {
                // Get the Document
                var document = commandData.Application.ActiveUIDocument.Document;

                // Create Analytical Panel
                var analyticalPanel = CreateAnalyticalPanel.CreateAMPanel(document);

                // Create an Analytical Member connected with the Analytical Panel above
                CreateAnalyticalMember.CreateMember(document);

                // Move the Analytical Panel using ElementTransformUtils
                using (var transaction = new Transaction(document, "Move panel with ElementTransformUtils"))
                {
                    transaction.Start();
                    ElementTransformUtils.MoveElement(document, analyticalPanel.Id, new XYZ(5, 5, 0));
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
