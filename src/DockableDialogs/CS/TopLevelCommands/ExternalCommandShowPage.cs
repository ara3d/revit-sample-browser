// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.DockableDialogs.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ExternalCommandShowPage : IExternalCommand, IExternalCommandAvailability
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                ThisApplication.ThisApp.GetDockableApiUtility().Initialize(commandData.Application);
                ThisApplication.ThisApp.SetWindowVisibility(commandData.Application, true);
            }
            catch (Exception)
            {
                TaskDialog.Show("Dockable Dialogs", "Dialog not registered.");
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     Onlys show the dialog when a document is open, as Dockable dialogs are only available
        ///     when a document is open.
        /// </summary>
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return applicationData.ActiveUIDocument != null;
        }
    }
}
