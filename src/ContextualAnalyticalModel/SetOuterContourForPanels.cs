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
    public class SetOuterContourForPanels : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                //create analytical panel
                var analyticalPanel = CreateAnalyticalPanel.CreateAmPanel(document);
                if (analyticalPanel != null)
                    using (Transaction transaction = new(document, "Edit Analytical Panel outer contour"))
                    {
                        transaction.Start();

                        CurveLoop profileloop = new();
                        profileloop.Append(Line.CreateBound(
                            new XYZ(0, 0, 0), new XYZ(5, 0, 0)));
                        profileloop.Append(Line.CreateBound(
                            new XYZ(5, 0, 0), new XYZ(5, 5, 0)));
                        profileloop.Append(Line.CreateBound(
                            new XYZ(5, 5, 0), new XYZ(-2, 5, 0)));
                        profileloop.Append(Line.CreateBound(
                            new XYZ(-2, 5, 0), new XYZ(0, 0, 0)));

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
