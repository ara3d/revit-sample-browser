using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace ContextualAnalyticalModel
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class MoveAnalyticalNodeUsingElementTransformUtils : IExternalCommand
   {
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         //Expected results: The Analytical Node has been moved and the connection has been kept
         try
         {
            // Get the Document
            var activeDoc = commandData.Application.ActiveUIDocument;
            var document = activeDoc.Document;

            // Create Analytical Panel
            var analyticalPanel = CreateAnalyticalPanel.CreateAMPanel(document);

            // Create the connected Analytical Member
            var analyticalMember = CreateAnalyticalMember.CreateMember(document);

            // Select the node
            var eRef = activeDoc.Selection.PickObject(ObjectType.PointOnElement , "Select an Analytical Node");
          
            // Move the Analytical Panel using ElementTransformUtils
            using (var transaction = new Transaction(document, "Move panel with ElementTransformUtils"))
            {
               transaction.Start();
               ElementTransformUtils.MoveElement(document, eRef.ElementId, new XYZ(-5, -5, 0));
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
