// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.UI;
using Revit.SDK.Samples.DockableDialogs.CS;

namespace Revit.SDK.Samples.DockableDiagnostics.CS
{
    internal class MacroAPIExternalEventHandler : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            var data = ThisApplication.thisApp.GetDockableAPIUtility().ModelessCommand.Take();
            ThisApplication.thisApp.GetDockableAPIUtility().RunModelessCommand(data);
            ThisApplication.thisApp.GetMainWindow()
                .UpdateUI(ThisApplication.thisApp.GetDockableAPIUtility().ModelessCommand.Take());
            ThisApplication.thisApp.GetMainWindow().WakeUp();
        }

        public string GetName()
        {
            return "MacroAPIExternalEventHandler";
        }
    }
}
