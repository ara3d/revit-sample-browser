// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CreateCustomPointLoad : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;
                var activeDoc = commandData.Application.ActiveUIDocument;

                var selectedElementId = ElementQuery.GetSelectedObject(activeDoc, "Please select the analytical element");
                var location = activeDoc.Selection.PickPoint("Point Load location");

                using Transaction transaction = new(document, "Create custom PointLoad");
                transaction.Start();

                if (PointLoad.IsPointInsideHostBoundaries(document, selectedElementId, location))
                    PointLoad.Create(document, selectedElementId, location, new XYZ(1, 0, 0), new XYZ(1, 0, 0),
                        null);

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
