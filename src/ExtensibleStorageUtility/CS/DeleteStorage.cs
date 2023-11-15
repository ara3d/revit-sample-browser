// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ExtensibleStorageUtility.CS
{
    /// <summary>
    ///     Deletes all extensible storage created by any application all active documents.
    ///     This command will also report if there is no storage in the active document to delete.
    ///     The document must be saved after the storage is deleted to commit the deletion.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class DeleteStorage : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;

            if (!StorageUtility.DoesAnyStorageExist(document))
            {
                message = "No storage in this document to delete.";
            }
            else
            {
                var tErase = new Transaction(document, "Erase EStorage");
                tErase.Start();
                var schemas = Schema.ListSchemas();
                foreach (var schema in schemas)
                    //Note-this will delete storage of this schema in *all* open documents.
                    document.EraseSchemaAndAllEntities(schema);
                tErase.Commit();
                message = "All storage was deleted.";
            }

            TaskDialog.Show("ExtensibleStorageUtility", message);
            return Result.Succeeded;
        }
    }
}
