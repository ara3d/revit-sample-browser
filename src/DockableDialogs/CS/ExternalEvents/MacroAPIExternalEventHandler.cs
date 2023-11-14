// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.UI;
using RevitMultiSample.DockableDialogs.CS;

namespace RevitMultiSample.DockableDiagnostics.CS
{
    internal class MacroApiExternalEventHandler : IExternalEventHandler
    {
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
            return "MacroAPIExternalEventHandler";
        }
    }
}
