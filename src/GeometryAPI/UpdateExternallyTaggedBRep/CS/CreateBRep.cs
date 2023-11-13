﻿//
// (C) Copyright 2003-2020 by Autodesk, Inc.
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

namespace Revit.SDK.Samples.UpdateExternallyTaggedBRep.CS
{
   /// <summary>
   /// This class implements method Execute as an external command for Revit.
   /// This external command creates the ExternallyTaggedBRep and adds it to the document.
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   public class CreateBRep : IExternalCommand
   {
      /// <summary>
      /// Creates the ExternallyTaggedBRep (named Podium), creates the new DirectShape for the open Document
      /// and adds the created Podium to this DirectShape. Also this external command checks the retrieving of
      /// the ExternallyTaggedBRep from the DirectShape by its ExternalGeometryId and the retrieving of the face and edge
      /// from the ExternallyTaggedBRep by their ExternalGeometryIds.
      /// </summary>
      /// <param name="commandData">An object that is passed to the external application
      /// which contains data related to the command,
      /// such as the application object and active view.</param>
      /// <param name="message">A message that can be set by the external application
      /// which will be displayed if a failure or cancellation is returned by
      /// the external command.</param>
      /// <param name="elements">A set of elements to which the external application
      /// can add elements that are to be highlighted in case of failure or cancellation.</param>
      /// <returns>Return the status of the external command.
      /// A result of Succeeded means that the API external method functioned as expected.
      /// Cancelled can be used to signify that the user canceled the external operation 
      /// at some point. Failure should be returned if the application is unable to proceed with
      /// the operation.</returns>
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         var dbDocument = commandData.Application.ActiveUIDocument.Document;

         try
         {
            if (Result.Succeeded != HelperMethods.executeCreateBRepCommand(dbDocument))
               return Result.Failed;
         }
         catch (Exception ex)
         {
            message = ex.Message;
            return Result.Failed;
         }

         return Result.Succeeded;
      }

      /// <summary>
      /// Created DirectShape by the CreateBRep external command.
      /// </summary>
      public static DirectShape CreatedDirectShape { set; get; } = null;
   }
}
