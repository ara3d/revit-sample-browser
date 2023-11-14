using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;

namespace ContextualAnalyticalModel
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// Break existing relation between physical and analytical elements using AnalyticalToPhysicalAssociationManager
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   class RemoveAssociation : IExternalCommand
   {
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         var activeDoc = commandData.Application.ActiveUIDocument;
         var doc = activeDoc.Document;
         using (var trans = new Transaction(doc, "ContextualAnalyticalModel.UpdateRelation"))
         {
            trans.Start();

            //select object for which we want to break the relation
            var eRef = activeDoc.Selection.PickObject(ObjectType.Element, "Please select the element for which you want to break relation");
            ElementId selectedElementId = null;
            if (eRef != null && eRef.ElementId != ElementId.InvalidElementId)
               selectedElementId = eRef.ElementId;

            // Gets the AnalyticalToPhysicalAssociationManager for this Revit document
             var analyticalToPhysicalmanager = AnalyticalToPhysicalAssociationManager.GetAnalyticalToPhysicalAssociationManager(doc);
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
