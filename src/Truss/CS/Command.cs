// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.Truss.CS
{
    /// <summary>
    ///     The entrance of this example, implements the Execute method of IExternalCommand
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var trussForm = new TrussForm(commandData);
            // The form is created successfully
            if (null != trussForm && false == trussForm.IsDisposed) trussForm.ShowDialog();

            return Result.Succeeded;
        }
    }
}
