// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.SelectionChanged.CS
{
    /// <summary>
    ///     This class inherits IExternalCommand interface and is used to show the SelectionChanged event
    ///     monitor window
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            // track the selected events by showing the information in the information windows.
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
