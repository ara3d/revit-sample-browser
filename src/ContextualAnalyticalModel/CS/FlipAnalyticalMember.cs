using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ContextualAnalyticalModel
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class FlipAnalyticalMember : IExternalCommand
   {
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         //Expected results: The Analytical Member is flipped, you can observe the IDs of the end nodes
         try
         {
            // Get the Document
            var document = commandData.Application.ActiveUIDocument.Document;

            // Create an Analytical Member
            var analyticalMember = CreateAnalyticalMember.CreateMember(document);

            // Start transaction
            using (var transaction = new Transaction(document, "Flip Analytical Member"))
            {
               transaction.Start();
               // Flip the Analytical Member
               analyticalMember.FlipCurve();
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
