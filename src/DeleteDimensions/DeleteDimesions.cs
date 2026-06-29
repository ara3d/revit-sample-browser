// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DeleteDimensions.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var selectedIds = commandData.Application.ActiveUIDocument.Selection.GetElementIds();
            if (selectedIds.Count == 0)
            {
                message = "Please select dimensions";
                return Result.Failed;
            }

            var dimsToDelete = new ElementSet();
            foreach (var elementId in selectedIds)
            {
                if (doc.GetElement(elementId) is Dimension dimension && !dimension.Pinned)
                    dimsToDelete.Insert(dimension);
            }

            if (dimsToDelete.IsEmpty)
            {
                message = "There are no unpinned dimensions currently selected";
                return Result.Failed;
            }

            using (var transaction = new Transaction(doc, "External Tool"))
            {
                transaction.Start();
                foreach (Element e in dimsToDelete)
                    doc.Delete(e.Id);
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}
