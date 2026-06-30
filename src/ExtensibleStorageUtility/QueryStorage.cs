// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
namespace Ara3D.RevitSampleBrowser.ExtensibleStorageUtility.CS
{
    /// <summary>
    /// Lists elements in the active document that carry extensible storage.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class QueryStorage : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            var storageElements = ExtensibleStorageHelper.GetElementsWithAllSchemas(document);
            TaskDialog.Show("ExtensibleStorageUtility", storageElements);

            return Result.Succeeded;
        }
    }
}
