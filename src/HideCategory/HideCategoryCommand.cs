// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from HideCategory by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/HideCategory

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using PickObjectsCanceled = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace Ara3D.RevitSampleBrowser.HideCategory.CS
{
    /// <summary>
    /// Hides the category of a picked element in every view of the active project.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class HideCategoryCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null)
            {
                message = "Please run this command in an active project document.";
                return Result.Failed;
            }

            var doc = uidoc.Document;
            Element pickedElement;

            try
            {
                var reference = uidoc.Selection.PickObject(
                    ObjectType.Element,
                    "Please pick an element to define a category to hide in all views");
                pickedElement = doc.GetElement(reference.ElementId);
            }
            catch (PickObjectsCanceled)
            {
                return Result.Cancelled;
            }

            var category = pickedElement.Category;
            if (category == null)
            {
                message = "The picked element does not have a category.";
                return Result.Failed;
            }

            var categoryId = category.Id;
            List<string> viewnameOk = new();
            List<string> viewnameBad = new();
            List<string> viewnameBadX = new();

            using (Transaction tx = new(doc))
            {
                tx.Start($"Hide category '{category.Name}' in all views");

                var views = new FilteredElementCollector(doc)
                    .OfClass(typeof(View));

                foreach (View view in views)
                {
                    try
                    {
                        view.SetCategoryHidden(categoryId, true);
                        viewnameOk.Add(view.Name);
                    }
                    catch (Autodesk.Revit.Exceptions.ArgumentException)
                    {
                        viewnameBad.Add(view.Name);
                    }
                    catch (Exception ex)
                    {
                        viewnameBadX.Add($"{view.Name}({ex.GetType().Name})");
                    }
                }

                tx.Commit();
            }

            TaskDialog dialog = new("Hide Category")
            {
                MainInstruction =
                    $"Category '{category.Name}' hidden in {viewnameOk.Count} views; "
                    + $"{viewnameBad.Count} views skipped:",
                MainContent =
                    $"OK: {string.Join(", ", viewnameOk)}"
                    + $"\r\n\r\nSkipped: {string.Join(", ", viewnameBad)}"
                    + $"\r\n\r\nSkippedX: {string.Join(", ", viewnameBadX)}"
            };
            dialog.Show();

            return Result.Succeeded;
        }
    }
}
