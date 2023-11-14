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
   public class CreateAnalyticalMember : IExternalCommand
   {
      public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         try
         {
            var document = commandData.Application.ActiveUIDocument.Document;
            CreateMember(document);
            return Result.Succeeded;
         }
         catch (Exception ex)
         {
            message = ex.Message;
            return Result.Failed;
         }
      }

      /// <summary>
      /// Creates an Analytical Member
      /// </summary>
      /// <param name="document"></param>
      /// <returns></returns>
      public static AnalyticalMember CreateMember(Document document)
      {
         AnalyticalMember analyticalMember = null;
         //start Transaction
         using (var transaction = new Transaction(document, "Create Analytical Member"))
         {
            transaction.Start();

            analyticalMember = CreateAnalyticalMemberFromEndpoints(document, new XYZ(-5, 0, 0), new XYZ(0, 0, 0));

            transaction.Commit();
         }
         return analyticalMember;
      }

      /// <summary>
      /// Creates another Analytical Member convergent with the one above
      /// </summary>
      /// <param name="document"></param>
      /// <returns></returns>
      public static AnalyticalMember CreateConvergentMember(Document document)
      {
         AnalyticalMember analyticalMember = null;
         //start Transaction
         using (var transaction = new Transaction(document, "Create Convergent Analytical Member"))
         {
            transaction.Start();

            analyticalMember = CreateAnalyticalMemberFromEndpoints(document, new XYZ(0, 0, 0), new XYZ(-5, 5, 0));           

            transaction.Commit();

         }
         return analyticalMember;
      }

      /// <summary>
      /// Creates a new Analytical Member in the Document using the endpoints start and end.
      /// Optionally the Structural Role can be specified
      /// </summary>
      /// <param name="doc"></param>
      /// <param name="start"></param>
      /// <param name="end"></param>
      /// <returns></returns>
      private static AnalyticalMember CreateAnalyticalMemberFromEndpoints(Document doc, XYZ start, XYZ end)
      {
         AnalyticalMember analyticalMember = null;

         //create curve which will be assigned to the analytical member
         var line = Line.CreateBound(start, end);

         //create the AnalyticalMember
         analyticalMember = AnalyticalMember.Create(doc, line);

         analyticalMember.StructuralRole = AnalyticalStructuralRole.StructuralRoleBeam;
         analyticalMember.AnalyzeAs = AnalyzeAs.Lateral;

         return analyticalMember;
      }
   }
}

