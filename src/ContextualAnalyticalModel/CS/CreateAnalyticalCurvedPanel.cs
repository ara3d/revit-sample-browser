﻿using System;
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
   class CreateAnalyticalCurvedPanel : IExternalCommand
   {
      public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         try
         {
            var revitDoc = commandData.Application.ActiveUIDocument.Document;
           
            using (var transaction = new Transaction(revitDoc, "Create Analytical Curved Panel"))
            {
               transaction.Start();

               var arc = Arc.Create(new XYZ(10, 10, 0), new XYZ(0, 0, 0), new XYZ(15, 10, 0));

               //create a curved AnalyticalPanel
               var analyticalCrvPanel = AnalyticalPanel.Create(revitDoc, arc, new XYZ(0, 0, 1));

               analyticalCrvPanel.StructuralRole = AnalyticalStructuralRole.StructuralRoleFloor;
               analyticalCrvPanel.AnalyzeAs = AnalyzeAs.SlabOneWay;

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
