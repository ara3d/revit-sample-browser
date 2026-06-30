// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.Events.EventsMonitor.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
#if !(Debug || DEBUG)
            if (ExternalApplication.JnlProcessor.IsReplay)
            {
                ExternalApplication.ApplicationEvents =
 ExternalApplication.JnlProcessor.GetEventsListFromJournalData(commandData.JournalData);

            }
            else
            {
#endif

            ExternalApplication.SettingDialog.ShowDialog();
            if (DialogResult.OK == ExternalApplication.SettingDialog.DialogResult)
            {
                ExternalApplication.ApplicationEvents = ExternalApplication.SettingDialog.AppSelectionList;
                var journalData = commandData.JournalData;

#if !(Debug || DEBUG)
                    ExternalApplication.JnlProcessor.DumpEventListToJournalData(ExternalApplication.ApplicationEvents, ref journalData);
#endif
            }
#if !(Debug || DEBUG)
            }
#endif
            ExternalApplication.AppEventMgr.Update(ExternalApplication.ApplicationEvents);

            ExternalApplication.InfoWindows.Show();

            return Result.Succeeded;
        }
    }
}
