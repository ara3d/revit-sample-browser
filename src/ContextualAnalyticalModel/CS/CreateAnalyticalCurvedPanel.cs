// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class CreateAnalyticalCurvedPanel : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var revitDoc = commandData.Application.ActiveUIDocument.Document;

                using (var transaction = new Transaction(revitDoc, "Create Analytical Curved Panel"))
                {
                    transaction.Start();

                    var arc = Arc.Create(new XYZ(10, 10, 0), new XYZ(0, 0, 0), new XYZ(15, 10, 0));

                    //create a curved AnalyticalPanel
                    var analyticalCrvPanel = AnalyticalPanel.Create(revitDoc, arc, new XYZ(0, 0, 1));

                    analyticalCrvPanel.StructuralRole = AnalyticalStructuralRole.StructuralRoleFloor;
                    analyticalCrvPanel.AnalyzeAs = AnalyzeAs.SlabOneWay;

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
