// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.DeleteObject.CS
{
    /// <summary>
    ///     Delete the elements that were selected
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var revit = commandData.Application;
            var trans = new Transaction(revit.ActiveUIDocument.Document, "Ara3D.RevitSampleBrowser.DeleteObject");
            trans.Start();
            var collection = new ElementSet();
            foreach (var elementId in revit.ActiveUIDocument.Selection.GetElementIds())
            {
                collection.Insert(revit.ActiveUIDocument.Document.GetElement(elementId));
            }

            // check user selection
            if (collection.Size < 1)
            {
                message = "Please select object(s) before delete.";
                trans.RollBack();
                return Result.Cancelled;
            }

            var error = true;
            try
            {
                error = true;

                // delete selection
                var e = collection.GetEnumerator();
                var moreValue = e.MoveNext();
                while (moreValue)
                {
                    var component = e.Current as Element;
                    revit.ActiveUIDocument.Document.Delete(component.Id);
                    moreValue = e.MoveNext();
                }

                error = false;
            }
            catch
            {
                // if revit threw an exception, try to catch it
                foreach (Element c in collection)
                {
                    elements.Insert(c);
                }

                message = "object(s) can't be deleted.";
                trans.RollBack();
                return Result.Failed;
            }
            finally
            {
                // if revit threw an exception, display error and return failed
                if (error) TaskDialog.Show("Error", "Delete failed.");
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }
}
