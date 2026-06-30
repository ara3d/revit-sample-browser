// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Geometry;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.FindReferencesByDirection.FindColumns.CS
{
    /// <summary>
    /// Finds columns embedded in walls via ReferenceIntersector rays cast from each wall side.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        private readonly List<ElementId> m_allColumnsOnWalls = new List<ElementId>();
        private UIApplication m_app;
        private readonly Dictionary<ElementId, List<ElementId>> m_columnsOnWall =
            new Dictionary<ElementId, List<ElementId>>();
        private Document m_doc;
        private View3D m_view3D;

        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            m_app = revit.Application;
            m_doc = revit.Application.ActiveUIDocument.Document;
            m_view3D = ElementQuery.Get3DView(m_doc);

            var selection = revit.Application.ActiveUIDocument.Selection;
            var wallsToCheck = new List<Wall>();

            if (selection.GetElementIds().Count > 0)
            {
                foreach (var eId in selection.GetElementIds())
                {
                    var e = revit.Application.ActiveUIDocument.Document.GetElement(eId);
                    if (e is Wall wall) wallsToCheck.Add(wall);
                }

                if (wallsToCheck.Count <= 0)
                {
                    message = "No walls were found in the active document selection";
                    return Result.Cancelled;
                }
            }
            else
            {
                var collector = new FilteredElementCollector(m_doc);
                var iter = collector.OfClass(typeof(Wall)).GetElementIterator();
                while (iter.MoveNext()) wallsToCheck.Add((Wall)iter.Current);
            }

            CheckWallsForEmbeddedColumns(wallsToCheck);

            ICollection<ElementId> toSelected = new List<ElementId>();
            if (m_allColumnsOnWalls.Count > 0)
            {
                foreach (var id in m_allColumnsOnWalls)
                {
                    var familyInstanceId = id;
                    var familyInstance = m_doc.GetElement(familyInstanceId);
                    toSelected.Add(familyInstance.Id);
                }

                selection.SetElementIds(toSelected);
            }

            return Result.Succeeded;
        }

        private void CheckWallsForEmbeddedColumns(List<Wall> wallsToCheck)
        {
            foreach (var wall in wallsToCheck)
            {
                CheckWallForEmbeddedColumns(wall);
            }
        }

        private void CheckWallForEmbeddedColumns(Wall wall)
        {
            var locationCurve = wall.Location as LocationCurve;
            var wallCurve = locationCurve.Curve;
            if (wallCurve is Line line)
            {
                LogWallCurve(line);
                CheckLinearWallForEmbeddedColumns(wall, locationCurve, line);
            }
            else
            {
                CheckProfiledWallForEmbeddedColumns(wall, locationCurve, wallCurve);
            }
        }

        private void CheckLinearWallForEmbeddedColumns(Wall wall, LocationCurve locationCurve, Curve wallCurve)
        {
            var bottomHeight = ElementQuery.GetElevationForRay(m_doc, wall);

            FindColumnsOnEitherSideOfWall(wall, locationCurve, wallCurve, 0, bottomHeight, wallCurve.Length);
        }

        private void FindColumnsOnEitherSideOfWall(Wall wall, LocationCurve locationCurve, Curve wallCurve,
            double parameter, double elevation, double within)
        {
            var rayDirection = CurveGeometry.GetTangentAt(wallCurve, parameter);
            var wallLocation = wallCurve.Evaluate(parameter, true);

            var wallDelta = PlaneAndTransform.GetWallDeltaAt(wall, locationCurve, parameter);

            var rayStart = new XYZ(wallLocation.X + wallDelta.X, wallLocation.Y + wallDelta.Y, elevation);
            FindColumnsByDirection(rayStart, rayDirection, within, wall);

            rayStart = new XYZ(wallLocation.X - wallDelta.X, wallLocation.Y - wallDelta.Y, elevation);
            FindColumnsByDirection(rayStart, rayDirection, within, wall);
        }

        private void FindColumnsByDirection(XYZ rayStart, XYZ rayDirection, double within, Wall wall)
        {
            var referenceIntersector = new ReferenceIntersector(m_view3D);
            var intersectedReferences = referenceIntersector.Find(rayStart, rayDirection);
            ElementQuery.FindColumnsWithin(intersectedReferences, within, wall, m_allColumnsOnWalls, m_columnsOnWall);
        }

        private void CheckProfiledWallForEmbeddedColumns(Wall wall, LocationCurve locationCurve, Curve wallCurve)
        {
            var bottomHeight = ElementQuery.GetElevationForRay(m_doc, wall);

            var parameterIncrement = SampleBrowserUtils.WallIncrement / wallCurve.Length;

            // Search within 2 ft so a column boundary face is still inside the target range.
            double findColumnWithin = 2;

            for (double parameter = 0; parameter < 1.0; parameter += parameterIncrement)
                FindColumnsOnEitherSideOfWall(wall, locationCurve, wallCurve, parameter, bottomHeight,
                    findColumnWithin);
        }

        private void LogWallCurve(Line wallCurve)
        {
            Debug.WriteLine("Wall curve is line: ");
            Debug.WriteLine($"Start point: {XyzMath.XyzToString(wallCurve.GetEndPoint(0))}");
            Debug.WriteLine($"End point: {XyzMath.XyzToString(wallCurve.GetEndPoint(1))}");
        }
    }
}
