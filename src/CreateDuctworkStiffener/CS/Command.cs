//
// (C) Copyright 2003-2021 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;


namespace Revit.SDK.Samples.CreateDuctworkStiffener.CS
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class Command : IExternalCommand
   {
      /// <summary>
      /// The current document of the application
      /// </summary>
      private static Document m_document;

      /// <summary>
      /// The ductwork to host stiffeners
      /// </summary>
      private FabricationPart m_ductwork;

      /// <summary>
      /// The type of the stiffeners
      /// </summary>
      private FamilySymbol m_stiffenerType;

      /// <summary>
      /// The distance from ductwork start point to stiffener position
      /// valid range: [0, m_ductwork.CenterlineLength]
      /// </summary>
      private double m_distanceToHostEnd;

            public Result Execute(ExternalCommandData commandData, ref string message,
            ElementSet elements)
      {
         m_document = commandData.Application.ActiveUIDocument.Document;
         if (m_document.IsFamilyDocument)
         {
            message = "Cannot create ductwork stiffener in family document";
            return Result.Failed;
         }

         //Get the ductwork in current project
         var ductCollector = new FilteredElementCollector(m_document);
         ductCollector.OfCategory(BuiltInCategory.OST_FabricationDuctwork).OfClass(typeof(FabricationPart));
         if (ductCollector.GetElementCount() == 0)
         {
            message = "The document does not contain fabrication ductwork";
            return Result.Failed;
         }
         m_ductwork = ductCollector.FirstElement() as FabricationPart;

         //Get the ductwork stiffener type
         var stiffenerTypeCollector = new FilteredElementCollector(m_document);
         stiffenerTypeCollector.OfCategory(BuiltInCategory.OST_FabricationDuctworkStiffeners).OfClass(typeof(FamilySymbol));
         if (stiffenerTypeCollector.GetElementCount() == 0)
         {
            message = "The document does not contain stiffener family symbol";
            return Result.Failed;
         }
         var allStiffenerTypes = stiffenerTypeCollector.ToElements();
         var stiffenerTypeName = "Duct Stiffener - External Rectangular Angle Iron: L Angle";
         foreach (var element in allStiffenerTypes)
         {
            var f = element as FamilySymbol;
            var name = f.Family.Name + ": " + f.Name;
            if (name == stiffenerTypeName)
            {
               m_stiffenerType = f;
               break;
            }
         }
         if (m_stiffenerType == null)
         {
            message = "The stiffener type cannot be found in this document";
            return Result.Failed;
         }

         //Place the stiffener at ductwork middle point
         m_distanceToHostEnd = 0.5 * m_ductwork.CenterlineLength;

         try
         {
            using (var transaction = new Transaction(m_document, "Sample_CreateDuctworkStiffener"))
            {
               transaction.Start();
               if (!m_stiffenerType.IsActive)
               {
                  m_stiffenerType.Activate();
                  m_document.Regenerate();
               }
               MEPSupportUtils.CreateDuctworkStiffener(m_document, m_stiffenerType.Id, m_ductwork.Id, m_distanceToHostEnd);
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

