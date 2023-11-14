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
   public class MoveAnalyticalMemberUsingSetCurve : IExternalCommand
   {
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         //Expected results: The first Analytical Member has been moved and the connection with the second Analytical Member was lost
         try
         {
            // Get the document
            var document = commandData.Application.ActiveUIDocument.Document;

            // Create the first Analytical Member
            var analyticalMember = CreateAnalyticalMember.CreateMember(document);

            // Create the second Analytical Member
            CreateAnalyticalMember.CreateConvergentMember(document);

            // Start transaction
            using (var transaction = new Transaction(document, "Offset member"))
            {
               transaction.Start();
              
               // Get the original curve and it's ends
               var originalCurve = analyticalMember.GetCurve();
               var originalCurveStart = originalCurve.GetEndPoint(0);
               var originalCurveEnd = originalCurve.GetEndPoint(1);

               // Create new start and end with offset value
               double offset = 15;
               var newLineStart = new XYZ(originalCurveStart.X + offset, 0, 0);
               var newLineEnd = new XYZ(originalCurveEnd.X + offset, 0, 0);

               // Create a new bounded line using the previous coordiantes
               var line = Line.CreateBound(newLineStart, newLineEnd);
             
               // Set the member's curve to the newly created line
               analyticalMember.SetCurve(line);    

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
