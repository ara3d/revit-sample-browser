// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.Massing.MeasurePanelArea.CS
{
    /// <summary>
    ///     A class inherits IExternalCommand interface.
    ///     this class creates an instance of the UI window and pop it up.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class MeasurePanelArea : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            using (var form = new FrmPanelArea(commandData))
            {
                // The form is created successfully
                if (null != form && !form.IsDisposed) form.ShowDialog();
            }

            return Result.Succeeded;
        }
    }
}
