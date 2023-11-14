using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace ContextualAnalyticalModel
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class ReleaseConditionsAnalyticalMember : IExternalCommand {

      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         try
         {
            // Get the Document
            var document = commandData.Application.ActiveUIDocument.Document;

            // Create an Analytical Member
            var analyticalMember = CreateAnalyticalMember.CreateMember(document);

            // Start transaction
            using (var transaction = new Transaction(document, "Release Conditions"))
            {
               transaction.Start();



               // Get release conditions of analytical member
               var releaseConditions = analyticalMember.GetReleaseConditions();
               foreach (var rc in releaseConditions)
               {
                  Console.WriteLine("Position: " + rc.Start +
                                    "Fx: " + rc.Fx.ToString() +
                                    "Fy: " + rc.Fy.ToString() +
                                    "Fz: " + rc.Fz.ToString() +
                                    "Mx: " + rc.Mx.ToString() +
                                    "My: " + rc.My.ToString() +
                                    "Mz: " + rc.Mz.ToString());
               }

               // Get release type at start
               analyticalMember.GetReleaseType(true);

               // Change release type
               analyticalMember.SetReleaseType(true, ReleaseType.UserDefined);

               try
               {
                  analyticalMember.SetReleaseConditions(new ReleaseConditions(true, false, true, false, true, false, true));
               }
               catch(InvalidOperationException ex)
               {
                  message = ex.Message;
                  return Result.Failed;
               }

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
