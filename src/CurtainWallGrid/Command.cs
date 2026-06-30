// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.CurtainWallGrid.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            MyDocument myDoc = new(commandData);

            using GridForm gridForm = new(myDoc);
            // The form is created successfully
            if (null != gridForm && false == gridForm.IsDisposed) gridForm.ShowDialog();

            return Result.Succeeded;
        }
    }
}
