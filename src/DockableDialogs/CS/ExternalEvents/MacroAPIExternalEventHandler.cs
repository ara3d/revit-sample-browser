// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.DockableDialogs.CS.Application;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS.ExternalEvents
{
    public class MacroApiExternalEventHandler : IExternalEventHandler
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
