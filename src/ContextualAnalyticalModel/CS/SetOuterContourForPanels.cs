// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ContextualAnalyticalModel
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class SetOuterContourForPanels : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                //create analytical panel
                var analyticalPanel = CreateAnalyticalPanel.CreateAmPanel(document);
                if (analyticalPanel != null)
                    using (var transaction = new Transaction(document, "Edit Analytical Panel outer contour"))
                    {
                        transaction.Start();

                        //create a new curve loop
                        var profileloop = new CurveLoop();
                        profileloop.Append(Line.CreateBound(
                            new XYZ(0, 0, 0), new XYZ(5, 0, 0)));
                        profileloop.Append(Line.CreateBound(
                            new XYZ(5, 0, 0), new XYZ(5, 5, 0)));
                        profileloop.Append(Line.CreateBound(
                            new XYZ(5, 5, 0), new XYZ(-2, 5, 0)));
                        profileloop.Append(Line.CreateBound(
                            new XYZ(-2, 5, 0), new XYZ(0, 0, 0)));

                        //Sets the new contour for analytical panel
                        analyticalPanel.SetOuterContour(profileloop);

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
