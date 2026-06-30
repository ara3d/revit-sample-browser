// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MoveAnalyticalPanelUsingElementTransformUtils : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                // Create Analytical Panel
                var analyticalPanel = CreateAnalyticalPanel.CreateAmPanel(document);

                // Create an Analytical Member connected with the Analytical Panel above
                CreateAnalyticalMember.CreateMember(document);

                // Move the Analytical Panel using ElementTransformUtils
                using Transaction transaction = new(document, "Move panel with ElementTransformUtils");
                transaction.Start();
                ElementTransformUtils.MoveElement(document, analyticalPanel.Id, new XYZ(5, 5, 0));
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
