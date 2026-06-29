// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.NetworkPressureLossReport.CS
{
    /// <summary>
    ///     Demonstrate how to find all networks available in the active document.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the application and document from external command data.
            var activeDoc = commandData.Application.ActiveUIDocument.Document;

            var dlg = new NetworkDialog(activeDoc);
            dlg.ShowDialog();

            return Result.Succeeded;
        }
    }
}
