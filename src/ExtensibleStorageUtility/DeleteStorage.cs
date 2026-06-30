// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.ExtensibleStorageUtility.CS
{
    /// <summary>
    /// Deletes all extensible storage in the active document. Save the document to persist the deletion.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DeleteStorage : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;

            if (!ExtensibleStorageHelper.DoesAnyStorageExist(document))
            {
                message = "No storage in this document to delete.";
            }
            else
            {
                Transaction tErase = new(document, "Erase EStorage");
                tErase.Start();
                var schemas = Schema.ListSchemas();
                foreach (var schema in schemas)
                {
                    // EraseSchemaAndAllEntities removes this schema from every open document, not just the active one.
                    document.EraseSchemaAndAllEntities(schema);
                }

                tErase.Commit();
                message = "All storage was deleted.";
            }

            TaskDialog.Show("ExtensibleStorageUtility", message);
            return Result.Succeeded;
        }
    }
}
