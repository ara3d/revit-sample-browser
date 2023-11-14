// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.PanelSchedule.CS
{
    /// <summary>
    ///     Export Panel Schedule View form Revit to CSV or HTML file.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
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
                    // ignore the PanelScheduleView instance which is a template.
                    continue;
                noPanelScheduleInstance = false;

                // choose what format export to, it can be CSV or HTML.
                var alternativeDlg = new TaskDialog("Choose Format to export");
                alternativeDlg.MainContent = "Click OK to export in .CSV format, Cancel to export in HTML format.";
                alternativeDlg.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
                alternativeDlg.AllowCancellation = true;
                var exportToCSV = alternativeDlg.Show();

                var translator = TaskDialogResult.Cancel == exportToCSV
                    ? new HTMLTranslator(psView)
                    : new CSVTranslator(psView) as Translator;
                var exported = translator.Export();

                // open the file if export successfully.
                if (!string.IsNullOrEmpty(exported)) Process.Start(exported);
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
