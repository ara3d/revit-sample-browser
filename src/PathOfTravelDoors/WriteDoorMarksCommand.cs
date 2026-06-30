// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from PathOfTravelDoors by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/PathOfTravelDoors

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Ara3D.RevitSampleBrowser.Common.Documents;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using PathOfTravelElement = Autodesk.Revit.DB.Analysis.PathOfTravel;

namespace Ara3D.RevitSampleBrowser.PathOfTravelDoors.CS
{
    /// <summary>
    ///     Writes comma-separated door marks crossed by each path of travel
    ///     into the path element's Comments parameter.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class WriteDoorMarksCommand : IExternalCommand
    {
        class PathOfTravelSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element e) => e is PathOfTravelElement;

            public bool AllowReference(Reference r, XYZ p) => true;
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            if (doc.ActiveView is not ViewPlan viewPlan)
            {
                message = "Open a floor plan view that contains the path(s) of travel.";
                return Result.Failed;
            }

            var view3D = ElementQuery.Get3DView(doc)
                ?? new FilteredElementCollector(doc)
                    .OfClass(typeof(View3D))
                    .Cast<View3D>()
                    .FirstOrDefault(view => !view.IsTemplate);

            if (view3D == null)
            {
                message = "A non-template 3D view is required for door intersection testing.";
                return Result.Failed;
            }

            var paths = GetSelectedPaths(uidoc, ref message);
            if (paths == null)
            {
                return Result.Cancelled;
            }

            if (paths.Count == 0)
            {
                message = "Select at least one path of travel.";
                return Result.Failed;
            }

            var updated = 0;
            using (var tx = new Transaction(doc, "Write path of travel door marks"))
            {
                tx.Start();

                foreach (var path in paths)
                {
                    var marks = PathOfTravelDoorFinder
                        .FindDoorsInTraversalOrder(path, viewPlan, view3D)
                        .Select(Command.GetDoorMark)
                        .Where(mark => !string.IsNullOrWhiteSpace(mark))
                        .ToList();

                    var comments = path.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    if (comments == null || comments.IsReadOnly)
                    {
                        continue;
                    }

                    comments.Set(string.Join(", ", marks));
                    updated++;
                }

                tx.Commit();
            }

            TaskDialog.Show(
                "Path of Travel Doors",
                $"Updated Comments on {updated} path(s) of travel.");

            return Result.Succeeded;
        }

        static IList<PathOfTravelElement> GetSelectedPaths(UIDocument uidoc, ref string message)
        {
            var doc = uidoc.Document;
            var selectedIds = uidoc.Selection.GetElementIds();
            var paths = selectedIds
                .Select(id => doc.GetElement(id))
                .OfType<PathOfTravelElement>()
                .ToList();

            if (paths.Count > 0)
            {
                return paths;
            }

            try
            {
                var references = uidoc.Selection.PickObjects(
                    ObjectType.Element,
                    new PathOfTravelSelectionFilter(),
                    "Select path(s) of travel");

                return references
                    .Select(reference => doc.GetElement(reference.ElementId))
                    .OfType<PathOfTravelElement>()
                    .ToList();
            }
            catch (OperationCanceledException)
            {
                message = "Path of travel selection was cancelled.";
                return null;
            }
        }
    }
}
