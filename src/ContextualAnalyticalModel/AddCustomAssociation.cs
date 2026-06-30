// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class AddCustomAssociation : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var activeDoc = commandData.Application.ActiveUIDocument;
            var doc = activeDoc.Document;

            if (null == doc) return Result.Failed;

            using Transaction trans =
                   new(doc, "Ara3D.RevitSampleBrowser.AddRelationBetweenPhysicalAndAnalyticalElements");
            trans.Start();

            var analyticalElementIds = ElementQuery.GetSelectedObjects(activeDoc, "Please select analytical elements");
            var physicalElementIds = ElementQuery.GetSelectedObjects(activeDoc, "Please select physical elements");

            var analyticalToPhysicalmanager =
                AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(doc);
            if (analyticalToPhysicalmanager == null)
                return Result.Failed;

            //creates a new relation between physical and analytical selected elements
            analyticalToPhysicalmanager.AddAssociation(analyticalElementIds, physicalElementIds);

            trans.Commit();

            return Result.Succeeded;
        }
    }
}
