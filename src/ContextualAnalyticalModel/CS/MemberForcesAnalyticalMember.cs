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
   public class MemberForcesAnalyticalMember : IExternalCommand
   {
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         try
         {
            // Get the Document
            var document = commandData.Application.ActiveUIDocument.Document;

            // Create an Analytical Member
            var analyticalMember = CreateAnalyticalMember.CreateMember(document);

            // Start transaction
            using (var transaction = new Transaction(document, "Member Forces"))
            {
               transaction.Start();

               // Get member forces of analytical member
               var memberForces = analyticalMember.GetMemberForces();
               foreach(var mf in memberForces)
               {
                  Console.WriteLine("Position: " + mf.Start + "Force: " + mf.Force.ToString() + "Moment: " + mf.Moment.ToString());
               }

               // Change some values
               analyticalMember.SetMemberForces(true, new XYZ(10000, 5000, 0), new XYZ(0, 0, 0));
               analyticalMember.SetMemberForces(new MemberForces(false, new XYZ(5000, 5000, 5000), new XYZ(10000, 10000, 10000)));

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
