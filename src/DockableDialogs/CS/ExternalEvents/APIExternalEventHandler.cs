// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DockableDialogs.CS
{
    /// <summary>
    ///     A class to manage the safe execution of events in a modeless dialog.
    /// </summary>
    internal class ApiExternalEventHandler : IExternalEventHandler
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
