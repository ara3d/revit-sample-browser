// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.DockableDialogs.CS.Application;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS.ExternalEvents
{
    /// <summary>
    ///     A class to manage the safe execution of events in a modeless dialog.
    /// </summary>
    public class ApiExternalEventHandler : IExternalEventHandler
    {
        /// <summary>
        ///     Called to execute an API command and update the UI after the command is finished.
        /// </summary>
        public void Execute(UIApplication app)
        {
            var data = ThisApplication.ThisApp.GetDockableApiUtility().ModelessCommand.Take();
            ThisApplication.ThisApp.GetDockableApiUtility().RunModelessCommand(data);
            ThisApplication.ThisApp.GetMainWindow()
                .UpdateUi(ThisApplication.ThisApp.GetDockableApiUtility().ModelessCommand.Take());
            ThisApplication.ThisApp.GetMainWindow().WakeUp();
        }

        public string GetName()
        {
            return "APIExternalEventHandler";
        }
    }
}
