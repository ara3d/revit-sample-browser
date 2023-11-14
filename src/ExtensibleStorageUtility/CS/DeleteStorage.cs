﻿//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notify appears in all copies and
// that both that copyright notify and the limited warranty and
// restricted rights notify below appear in all supporting
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
using Autodesk.Revit.DB.ExtensibleStorage;

namespace Revit.SDK.Samples.ExtensibleStorageUtility.CS
{
   /// <summary>
   /// Deletes all extensible storage created by any application all active documents.
   /// This command will also report if there is no storage in the active document to delete.
   /// The document must be saved after the storage is deleted to commit the deletion.
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class DeleteStorage : IExternalCommand
   {
      public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
      {
         var document = commandData.Application.ActiveUIDocument.Document;

         if (!(StorageUtility.DoesAnyStorageExist(document)))
            message = "No storage in this document to delete.";
         else
         {
            var tErase = new Transaction(document, "Erase EStorage");
            tErase.Start();
            var schemas = Schema.ListSchemas();
            foreach (var schema in schemas)
            {
               //Note-this will delete storage of this schema in *all* open documents.
               document.EraseSchemaAndAllEntities(schema);
            }
            tErase.Commit();
            message = "All storage was deleted.";
         }

         TaskDialog.Show("ExtensibleStorageUtility", message);
         return Result.Succeeded;

         
      }
   }

}
