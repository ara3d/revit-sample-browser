// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.DockableDialogs.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ExternalCommandRegisterPage : IExternalCommand, IExternalCommandAvailability
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            ThisApplication.thisApp.GetDockableAPIUtility().Initialize(commandData.Application);
            ThisApplication.thisApp.CreateWindow();

            var dlg = new DockingSetupDialog();
            var dlgResult = dlg.ShowDialog();
            if (dlgResult == false)
                return Result.Succeeded;

            ThisApplication.thisApp.GetMainWindow().SetInitialDockingParameters(dlg.FloatLeft, dlg.FloatRight,
                dlg.FloatTop, dlg.FloatBottom, dlg.DockPosition, dlg.TargetGuid);
            try
            {
                ThisApplication.thisApp.RegisterDockableWindow(commandData.Application, dlg.MainPageGuid);
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
            if (applicationData.ActiveUIDocument == null)
                return true;
            return false;
        }
    }
}
