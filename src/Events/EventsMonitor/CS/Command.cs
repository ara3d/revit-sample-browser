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

using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.EventsMonitor.CS
{
    /// <summary>
    ///     This class inherits IExternalCommand interface and used to retrieve the events setting
    ///     dialog to help user changing his choices. None event is register in this class.
    ///     Because all trigger points in this sample come from UI, all events in application level
    ///     must be registered to ControlledApplication. If the trigger point is from API,
    ///     user can register it to Application or Document according to what level it is in.
    ///     But then, the syntax is the same in these three cases.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            // These #if directives within file are used to compile project in different purpose:
            // . Build project with Release mode for regression test,
            // . Build project with Debug mode for manual run
#if !(Debug || DEBUG)
            // playing journal.
            if (ExternalApplication.JnlProcessor.IsReplay)
            {
                ExternalApplication.ApplicationEvents =
 ExternalApplication.JnlProcessor.GetEventsListFromJournalData(journaldata);

            }

            // running the sample form UI.
            else
            {
#endif

            ExternalApplication.SettingDialog.ShowDialog();
            if (DialogResult.OK == ExternalApplication.SettingDialog.DialogResult)
            {
                // get what user select.
                ExternalApplication.ApplicationEvents = ExternalApplication.SettingDialog.AppSelectionList;

#if !(Debug || DEBUG)
                    // dump what user select to a file in order to autotesting.
                    ExternalApplication.JnlProcessor.DumpEventListToJournalData(ExternalApplication.ApplicationEvents, ref journaldata);
#endif
            }
#if !(Debug || DEBUG)
            }
#endif
            // update the events according to the selection.
            ExternalApplication.AppEventMgr.Update(ExternalApplication.ApplicationEvents);

            // track the selected events by showing the information in the information windows.
            ExternalApplication.InfoWindows.Show();


            return Result.Succeeded;
        }
    }
}