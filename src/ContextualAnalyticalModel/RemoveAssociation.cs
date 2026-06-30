// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RemoveAssociation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var activeDoc = commandData.Application.ActiveUIDocument;
            var doc = activeDoc.Document;
            using Transaction trans = new(doc, "ContextualAnalyticalModel.UpdateRelation");
            trans.Start();

            var selectedElementId = ElementQuery.GetSelectedObject(activeDoc,
                "Please select the element for which you want to break relation");

            var analyticalToPhysicalmanager =
                AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(doc);
            if (analyticalToPhysicalmanager == null)
                return Result.Failed;

            //break relation
            analyticalToPhysicalmanager.RemoveAssociation(selectedElementId);

            trans.Commit();

            return Result.Succeeded;
        }
    }
}
