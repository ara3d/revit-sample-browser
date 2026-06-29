// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DeleteObject.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var trans = new Transaction(doc, "Ara3D.RevitSampleBrowser.DeleteObject");
            trans.Start();

            var collection = new ElementSet();
            foreach (var elementId in commandData.Application.ActiveUIDocument.Selection.GetElementIds())
                collection.Insert(doc.GetElement(elementId));

            if (collection.IsEmpty)
            {
                message = "Please select object(s) before delete.";
                trans.RollBack();
                return Result.Cancelled;
            }

            try
            {
                foreach (Element component in collection)
                    doc.Delete(component.Id);
            }
            catch
            {
                foreach (Element c in collection)
                    elements.Insert(c);
                message = "object(s) can't be deleted.";
                trans.RollBack();
                return Result.Failed;
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }
}
