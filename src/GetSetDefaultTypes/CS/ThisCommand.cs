// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.GetSetDefaultTypes.CS
{
    [Transaction(TransactionMode.Manual)]
    internal class ThisCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (!DockablePane.PaneExists(DefaultFamilyTypes.PaneId) ||
                !DockablePane.PaneExists(DefaultElementTypes.PaneId))
                return Result.Failed;

            var uiApp = commandData.Application;
            if (uiApp == null)
                return Result.Failed;

            var pane = uiApp.GetDockablePane(DefaultFamilyTypes.PaneId);
            pane.Show();
            var elemTypePane = uiApp.GetDockablePane(DefaultElementTypes.PaneId);
            elemTypePane.Show();

            if (ThisApplication.DefaultFamilyTypesPane != null)
                ThisApplication.DefaultFamilyTypesPane.SetDocument(commandData.Application.ActiveUIDocument.Document);
            if (ThisApplication.DefaultElementTypesPane != null)
                ThisApplication.DefaultElementTypesPane.SetDocument(commandData.Application.ActiveUIDocument.Document);

            return Result.Succeeded;
        }
    }
}
