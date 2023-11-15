// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace ContextualAnalyticalModel
{
    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand
    ///     Adds new relation between physical and analytical elements using AnalyticalToPhysicalAssociationManager
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class AddAssociation : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var activeDoc = commandData.Application.ActiveUIDocument;
            var doc = activeDoc.Document;

            if (null == doc) return Result.Failed;

            using (var trans =
                   new Transaction(doc, "Ara3D.RevitSampleBrowser.AddRelationBetweenPhysicalAndAnalyticalElements"))
            {
                trans.Start();

                var analyticalElementId = Utilities.GetSelectedObject(activeDoc, "Please select analytical element");
                var physicalElementId = Utilities.GetSelectedObject(activeDoc, "Please select physical element");

                //gets the AnalyticalToPhysicalAssociationManager for the current document
                var analyticalToPhysicalmanager =
                    AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(doc);
                if (analyticalToPhysicalmanager == null)
                    return Result.Failed;

                //creates a new relation between physical and analytical selected elements
                analyticalToPhysicalmanager.AddAssociation(analyticalElementId, physicalElementId);

                trans.Commit();
            }

            return Result.Succeeded;
        }
    }
}
