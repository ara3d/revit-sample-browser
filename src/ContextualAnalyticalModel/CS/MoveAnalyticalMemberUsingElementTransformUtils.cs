﻿using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ContextualAnalyticalModel
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class MoveAnalyticalMemberUsingElementTransformUtils : IExternalCommand
   {
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         // Expected results: the first Analytical Member has been moved and the connection with the second Analytical Member was kept
         try
         {
            // Get the document
            var document = commandData.Application.ActiveUIDocument.Document;

            // Create the first Analytical Member
            var analyticalMember = CreateAnalyticalMember.CreateMember(document);

            // Create the second Analytical Member that is convergent with the first one
            var convergentAnalyticalMember = CreateAnalyticalMember.CreateConvergentMember(document);


            // Move the first Analytical Member using ElementTransformUtils
            using (var transaction = new Transaction(document, "Move member with ElementTransformUtils"))
            {
               transaction.Start();

               ElementTransformUtils.MoveElement(document, analyticalMember.Id, new XYZ(15, 0, 0));

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