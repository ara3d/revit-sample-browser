// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand
    ///     Break existing relation between physical and analytical elements using AnalyticalToPhysicalAssociationManager
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RemoveAssociation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var activeDoc = commandData.Application.ActiveUIDocument;
            var doc = activeDoc.Document;
            using (var trans = new Transaction(doc, "ContextualAnalyticalModel.UpdateRelation"))
            {
                trans.Start();

                //select object for which we want to break the relation
                var eRef = activeDoc.Selection.PickObject(ObjectType.Element,
                    "Please select the element for which you want to break relation");
                ElementId selectedElementId = null;
                if (eRef != null && eRef.ElementId != ElementId.InvalidElementId)
                    selectedElementId = eRef.ElementId;

                // Gets the AnalyticalToPhysicalAssociationManager for this Revit document
                var analyticalToPhysicalmanager =
                    AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(doc);
                if (analyticalToPhysicalmanager == null)
                    return Result.Failed;

                //break relation
                analyticalToPhysicalmanager.RemoveAssociation(selectedElementId);

                trans.Commit();
            }

            return Result.Succeeded;
        }
    }
}
