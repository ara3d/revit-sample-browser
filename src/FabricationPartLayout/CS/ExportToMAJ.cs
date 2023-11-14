//
// (C) Copyright 2003-2019 by Autodesk, Inc.
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
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Fabrication;
using System.IO;
using System.Reflection;

namespace Revit.SDK.Samples.FabricationPartLayout.CS
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   public class ExportToMAJ : IExternalCommand
   {
      public virtual Result Execute(ExternalCommandData commandData
          , ref string message, ElementSet elements)
      {
         try
         {
            // check user selection
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var elementIds = new HashSet<ElementId>();
            uidoc.Selection.GetElementIds().ToList().ForEach( x => elementIds.Add(x) );

            var hasFabricationParts = false;
            foreach (var elementId in elementIds)
            {
               var part = doc.GetElement(elementId) as FabricationPart;
               if (part != null)
               {
                  hasFabricationParts = true;
                  break;
               }
            }

            if (hasFabricationParts == false)
            {
               message = "Select at least one fabrication part";
               return Result.Failed;
            }

            var callingFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var saveAsDlg = new FileSaveDialog("MAJ Files (*.maj)|*.maj");
            saveAsDlg.InitialFileName = callingFolder + "\\majExport";
            saveAsDlg.Title = "Export To MAJ";
            var result = saveAsDlg.Show();

            if (result == ItemSelectionDialogResult.Canceled)
               return Result.Cancelled;

            var filename = ModelPathUtils.ConvertModelPathToUserVisiblePath(saveAsDlg.GetSelectedModelPath());

            var exported = FabricationPart.SaveAsFabricationJob(doc, elementIds, filename, new FabricationSaveJobOptions(true));
            if (exported.Count > 0)
            {
               var td = new TaskDialog("Export to MAJ")
               {
                  MainIcon = TaskDialogIcon.TaskDialogIconInformation,
                  TitleAutoPrefix = false,
                  MainInstruction = string.Concat("Export to MAJ was successful - ", exported.Count.ToString(), " Parts written"),
                  MainContent = filename,
                  AllowCancellation = false,
                  CommonButtons = TaskDialogCommonButtons.Ok
               };

               td.Show();
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
