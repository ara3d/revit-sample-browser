// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.CurtainWallGrid.CS
{
    /// <summary>
    ///     the entry point of the sample (to launch the sample dialog and allows further operations)
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        /// <summary>
        ///     The entrance of this example, implements the Execute method of IExternalCommand
        /// </summary>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var myDoc = new MyDocument(commandData);

            using (var gridForm = new GridForm(myDoc))
            {
                // The form is created successfully
                if (null != gridForm && false == gridForm.IsDisposed) gridForm.ShowDialog();
            }

            return Result.Succeeded;
        }
    }
}
