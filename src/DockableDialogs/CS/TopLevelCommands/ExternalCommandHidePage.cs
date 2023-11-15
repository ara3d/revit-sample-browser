// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DockableDialogs.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ExternalCommandHidePage : IExternalCommand, IExternalCommandAvailability
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                ThisApplication.ThisApp.GetDockableApiUtility().Initialize(commandData.Application);
                ThisApplication.ThisApp.SetWindowVisibility(commandData.Application, false);
            }
            catch (Exception)
            {
                TaskDialog.Show("Dockable Dialogs", "Dialog not registered.");
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     Onlys show the dialog when a document is  not open, as Dockable dialogs should only be registered when
        ///     no documents are open.
        /// </summary>
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return applicationData.ActiveUIDocument != null;
        }
    }
}
