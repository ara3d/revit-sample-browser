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

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PanelSchedule.CS
{
    /// <summary>
    /// Export Panel Schedule View form Revit to CSV or HTML file.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class PanelScheduleExport : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            // get all PanelScheduleView instances in the Revit document.
            var fec = new FilteredElementCollector(doc);
            var PanelScheduleViewsAreWanted = new ElementClassFilter(typeof(PanelScheduleView));
            fec.WherePasses(PanelScheduleViewsAreWanted);
            var psViews = fec.ToElements() as List<Element>;

            var noPanelScheduleInstance = true;

            foreach (var element in psViews)
            {
                var psView = element as PanelScheduleView;
                if (psView.IsPanelScheduleTemplate())
                {
                    // ignore the PanelScheduleView instance which is a template.
                    continue;
                }
                else
                {
                    noPanelScheduleInstance = false;
                }

                // choose what format export to, it can be CSV or HTML.
                var alternativeDlg = new TaskDialog("Choose Format to export");
                alternativeDlg.MainContent = "Click OK to export in .CSV format, Cancel to export in HTML format.";
                alternativeDlg.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
                alternativeDlg.AllowCancellation = true;
                var exportToCSV = alternativeDlg.Show();
                
                var translator = TaskDialogResult.Cancel == exportToCSV ? new HTMLTranslator(psView) : new CSVTranslator(psView) as Translator;
                var exported = translator.Export();

                // open the file if export successfully.
                if (!string.IsNullOrEmpty(exported))
                {
                    System.Diagnostics.Process.Start(exported);
                }
            }

            if (noPanelScheduleInstance)
            {
                var messageDlg = new TaskDialog("Warnning Message");
                messageDlg.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                messageDlg.MainContent = "No panel schedule view is in the current document.";
                messageDlg.Show();
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }

    }
}
