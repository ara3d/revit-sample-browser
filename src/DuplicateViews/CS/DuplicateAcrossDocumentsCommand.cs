// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable. 

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DuplicateViews.CS
{
    /// <summary>
    ///     A command to copy schedules and drafting views across documents.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class DuplicateAcrossDocumentsCommand : IExternalCommand
    {
        /// <summary>
        ///     The command implementation.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var application = commandData.Application.Application;
            var doc = commandData.Application.ActiveUIDocument.Document;

            // Find target document - it must be the only other open document in session
            Document toDocument = null;
            var documents = application.Documents.Cast<Document>();
            if (documents.Count() != 2)
            {
                TaskDialog.Show("No target document",
                    "This tool can only be used if there are two documents (a source document and target document).");
                return Result.Cancelled;
            }

            foreach (var loadedDoc in documents)
            {
                if (loadedDoc.Title != doc.Title)
                {
                    toDocument = loadedDoc;
                    break;
                }
            }

            // Collect schedules and drafting views
            var collector = new FilteredElementCollector(doc);

            var viewTypes = new List<Type>
            {
                typeof(ViewSchedule),
                typeof(ViewDrafting)
            };
            var filter = new ElementMulticlassFilter(viewTypes);
            collector.WherePasses(filter);

            collector.WhereElementIsViewIndependent(); // skip view-specfic schedules (e.g. Revision Schedules);
            // These should not be copied as they are associated to another view that cannot be copied

            // Copy all schedules together so that any dependency elements are copied only once
            var schedules = collector.OfType<ViewSchedule>();
            DuplicateViewUtils.DuplicateSchedules(doc, schedules, toDocument);
            var numSchedules = schedules.Count();

            // Copy drafting views together
            var draftingViews = collector.OfType<ViewDrafting>();
            var numDraftingElements =
                DuplicateViewUtils.DuplicateDraftingViews(doc, draftingViews, toDocument);
            var numDrafting = draftingViews.Count();

            // Show results
            TaskDialog.Show("Statistics",
                string.Format("Copied: \n" +
                              "\t{0} schedules.\n" +
                              "\t{1} drafting views.\n" +
                              "\t{2} new drafting elements created.",
                    numSchedules, numDrafting, numDraftingElements));

            return Result.Succeeded;
        }
    }
}
