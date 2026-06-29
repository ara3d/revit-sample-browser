// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ExtensibleStorageUtility.CS
{
    /// <summary>
    ///     Checks to see if any extensible storage in the document exists and displays elements
    ///     containing storage to the user.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class QueryStorage : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            var storageElements = StorageUtility.GetElementsWithAllSchemas(document);
            TaskDialog.Show("ExtensibleStorageUtility", storageElements);

            return Result.Succeeded;
        }
    }
}
