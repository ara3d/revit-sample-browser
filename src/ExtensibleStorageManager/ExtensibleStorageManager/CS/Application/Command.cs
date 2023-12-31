// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.ExtensibleStorageManager.ExtensibleStorageManager.CS.User;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ExtensibleStorageManager.ExtensibleStorageManager.CS.Application
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var dialog = new UiCommand(commandData.Application.ActiveUIDocument.Document,
                commandData.Application.ActiveAddInId.GetGUID().ToString());
            dialog.ShowDialog();
            return Result.Succeeded;
        }
    }
}
