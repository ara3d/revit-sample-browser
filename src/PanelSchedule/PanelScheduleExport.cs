// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ara3D.RevitSampleBrowser.PanelSchedule.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class PanelScheduleExport : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;

            FilteredElementCollector fec = new(doc);
            ElementClassFilter panelScheduleViewsAreWanted = new(typeof(PanelScheduleView));
            fec.WherePasses(panelScheduleViewsAreWanted);
            var psViews = fec.ToElements() as List<Element>;

            var noPanelScheduleInstance = true;

            foreach (var element in psViews)
            {
                var psView = element as PanelScheduleView;
                if (psView.IsPanelScheduleTemplate())
                    continue;
                noPanelScheduleInstance = false;

                TaskDialog alternativeDlg = new("Choose Format to export")
                {
                    MainContent = "Click OK to export in .CSV format, Cancel to export in HTML format.",
                    CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel,
                    AllowCancellation = true
                };
                var exportToCsv = alternativeDlg.Show();

                Translator translator = TaskDialogResult.Cancel == exportToCsv
                    ? new HtmlTranslator(psView)
                    : new CsvTranslator(psView);
                var exported = translator.Export();

                if (!string.IsNullOrEmpty(exported)) Process.Start(exported);
            }

            if (noPanelScheduleInstance)
            {
                TaskDialog messageDlg = new("Warnning Message")
                {
                    MainIcon = TaskDialogIcon.TaskDialogIconWarning,
                    MainContent = "No panel schedule view is in the current document."
                };
                messageDlg.Show();
                return Result.Cancelled;
            }

            return Result.Succeeded;
        }
    }
}
