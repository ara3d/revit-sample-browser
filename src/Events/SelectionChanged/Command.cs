// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Events.SelectionChanged.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            if (SelectionChanged.InfoWindow == null)
            {
                SelectionChanged.InfoWindow = new InfoWindow();
                SelectionChanged.InfoWindow.Show();
            }
            else
            {
                SelectionChanged.InfoWindow.Focus();
            }

            return Result.Succeeded;
        }
    }
}
