// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Ara3D.RevitSampleBrowser.DockableDialogs.CS.Application;
using Ara3D.RevitSampleBrowser.DockableDialogs.CS.Utility;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS.TopLevelCommands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ExternalCommandRegisterPage : IExternalCommand, IExternalCommandAvailability
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            ThisApplication.ThisApp.GetDockableApiUtility().Initialize(commandData.Application);
            ThisApplication.ThisApp.CreateWindow();

            var dlg = new DockingSetupDialog.DockingSetupDialog();
            var dlgResult = dlg.ShowDialog();
            if (dlgResult == false)
                return Result.Succeeded;

            ThisApplication.ThisApp.GetMainWindow().SetInitialDockingParameters(dlg.FloatLeft, dlg.FloatRight,
                dlg.FloatTop, dlg.FloatBottom, dlg.DockPosition, dlg.TargetGuid);
            try
            {
                ThisApplication.ThisApp.RegisterDockableWindow(commandData.Application, dlg.MainPageGuid);
            }
            catch (Exception ex)
            {
                TaskDialog.Show(Globals.ApplicationName, ex.Message);
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     Onlys show the dialog when a document is open, as Dockable dialogs are only available
        ///     when a document is open.
        /// </summary>
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return applicationData.ActiveUIDocument == null;
        }
    }
}
