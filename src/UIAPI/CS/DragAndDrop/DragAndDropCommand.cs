// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.UIAPI.CS
{
    [Transaction(TransactionMode.Manual)]
    public class DragAndDropCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // show the form
            var form = FurnitureFamilyDragAndDropForm.GetTheForm(commandData.View.Document);
            form.Show();
            form.BringToFront();
            return Result.Succeeded;
        }
    }
}
