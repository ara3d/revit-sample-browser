// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.DockableDialogs.CS.Application;
using Ara3D.RevitSampleBrowser.DockableDialogs.CS.ExternalEvents;

namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS.APIUtility
{
    public partial class ApiUtility
    {
        /// <summary>
        ///     A method to examine an incoming command and start the process of executing
        ///     safely in the Idle() or ExternalEvent() loop.
        /// </summary>
        public void RunModelessCommand(ModelessCommandData command)
        {
            switch (command.CommandType)
            {
                case ModelessCommandType.PrintMainPageStatistics:
                {
                    command.WindowSummaryData = GetPaneSummary(ThisApplication.ThisApp.MainPageDockablePaneId);
                    ModelessCommand.Make(command);
                    break;
                }

                case ModelessCommandType.PrintSelectedPageStatistics:
                {
                    command.WindowSummaryData = GetPaneSummary(command.SelectedPaneId);
                    ModelessCommand.Make(command);
                    break;
                }
            }
        }
    }
}
