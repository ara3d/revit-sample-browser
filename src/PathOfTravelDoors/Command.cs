// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from PathOfTravelDoors by Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/PathOfTravelDoors

using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    ///     Lists door marks crossed by selected path-of-travel elements.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
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

            var report = new StringBuilder();
            foreach (var path in paths)
            {
                var doors = PathOfTravelDoorFinder.FindDoorsInTraversalOrder(path, viewPlan, view3D);
                var marks = doors
                    .Select(GetDoorMark)
                    .Where(mark => !string.IsNullOrWhiteSpace(mark))
                    .ToList();

                report.AppendLine($"Path {path.Id.Value}: {marks.Count} door(s)");
                if (marks.Count == 0)
                {
                    report.AppendLine("  (none)");
                }
                else
                {
                    report.AppendLine($"  {string.Join(", ", marks)}");
                }
            }

            TaskDialog.Show("Path of Travel Doors", report.ToString().TrimEnd());
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

        internal static string GetDoorMark(FamilyInstance door)
        {
            return door.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)?.AsString()
                ?? door.Name;
        }
    }
}
