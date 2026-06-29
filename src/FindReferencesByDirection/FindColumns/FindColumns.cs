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
    ///     Find all walls that have embedded columns in them, and the ids of those embedded columns.
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand
    {
        /// <summary>
        ///     ElementId list for columns which are on walls
        /// </summary>
        private readonly List<ElementId> m_allColumnsOnWalls = new List<ElementId>();

        /// <summary>
        ///     revit application
        /// </summary>
        private UIApplication m_app;

        /// <summary>
        ///     Dictionary of columns and walls
        /// </summary>
        private readonly Dictionary<ElementId, List<ElementId>> m_columnsOnWall =
            new Dictionary<ElementId, List<ElementId>>();

        /// <summary>
        ///     Revit active document
        /// </summary>
        private Document m_doc;

        /// <summary>
        ///     A 3d view
        /// </summary>
        private View3D m_view3D;

        /// <summary>
        ///     The top level command.
        /// </summary>
        /// <param name="revit">
        ///     An object that is passed to the external application
        ///     which contains data related to the command,
        ///     such as the application object and active view.
        /// </param>
        /// <param name="message">
        ///     A message that can be set by the external application
        ///     which will be displayed if a failure or cancellation is returned by
        ///     the external command.
        /// </param>
        /// <param name="elements">
        ///     A set of elements to which the external application
        ///     can add elements that are to be highlighted in case of failure or cancellation.
        /// </param>
        /// <returns>
        ///     Return the status of the external command.
        ///     A result of Succeeded means that the API external method functioned as expected.
        ///     Cancelled can be used to signify that the user cancelled the external operation
        ///     at some point. Failure should be returned if the application is unable to proceed with
        ///     the operation.
        /// </returns>
        public Result Execute(ExternalCommandData revit,
            ref string message,
            ElementSet elements)
        {
            // Initialization 
            m_app = revit.Application;
            m_doc = revit.Application.ActiveUIDocument.Document;

            // Find a 3D view to use for the ray tracing operation
            m_view3D = ElementQuery.Get3DView(m_doc);

            var selection = revit.Application.ActiveUIDocument.Selection;
            var wallsToCheck = new List<Wall>();

            // If wall(s) are selected, process them.
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
            // Find all walls in the document and process them.
            else
            {
                var collector = new FilteredElementCollector(m_doc);
                var iter = collector.OfClass(typeof(Wall)).GetElementIterator();
                while (iter.MoveNext()) wallsToCheck.Add((Wall)iter.Current);
            }

            // Execute the check for embedded columns
            CheckWallsForEmbeddedColumns(wallsToCheck);

            // Process the results, in this case set the active selection to contain all embedded columns
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

        /// <summary>
        ///     Check a list of walls for embedded columns.
        /// </summary>
        /// <param name="wallsToCheck">The list of walls to check.</param>
        private void CheckWallsForEmbeddedColumns(List<Wall> wallsToCheck)
        {
            foreach (var wall in wallsToCheck)
            {
                CheckWallForEmbeddedColumns(wall);
            }
        }

        /// <summary>
        ///     Checks a single wall for embedded columns.
        /// </summary>
        /// <param name="wall">The wall to check.</param>
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

        /// <summary>
        ///     Checks a single linear wall for embedded columns.
        /// </summary>
        /// <param name="wall">The wall to check.</param>
        /// <param name="locationCurve">The location curve extracted from this wall.</param>
        /// <param name="wallCurve">The profile of the wall.</param>
        private void CheckLinearWallForEmbeddedColumns(Wall wall, LocationCurve locationCurve, Curve wallCurve)
        {
            var bottomHeight = ElementQuery.GetElevationForRay(m_doc, wall);

            FindColumnsOnEitherSideOfWall(wall, locationCurve, wallCurve, 0, bottomHeight, wallCurve.Length);
        }

        /// <summary>
        ///     Finds columns on either side of the given wall.
        /// </summary>
        /// <param name="wall">The wall.</param>
        /// <param name="locationCurve">The location curve of the wall.</param>
        /// <param name="wallCurve">The profile of the wall.</param>
        /// <param name="parameter">The normalized parameter along the wall profile which is being evaluated.</param>
        /// <param name="elevation">The elevation at which the rays are cast.</param>
        /// <param name="within">The maximum distance away that columns may be found.</param>
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

        /// <summary>
        ///     Finds columns by projecting rays along a given direction.
        /// </summary>
        /// <param name="rayStart">The origin of the ray.</param>
        /// <param name="rayDirection">The direction of the ray.</param>
        /// <param name="within">The maximum distance away that columns may be found.</param>
        /// <param name="wall">The wall that this search is associated with.</param>
        private void FindColumnsByDirection(XYZ rayStart, XYZ rayDirection, double within, Wall wall)
        {
            var referenceIntersector = new ReferenceIntersector(m_view3D);
            var intersectedReferences = referenceIntersector.Find(rayStart, rayDirection);
            ElementQuery.FindColumnsWithin(intersectedReferences, within, wall, m_allColumnsOnWalls, m_columnsOnWall);
        }

        /// <summary>
        ///     Checks a single curved/profiled wall for embedded columns.
        /// </summary>
        /// <param name="wall">The wall to check.</param>
        /// <param name="locationCurve">The location curve extracted from this wall.</param>
        /// <param name="wallCurve">The profile of the wall.</param>
        private void CheckProfiledWallForEmbeddedColumns(Wall wall, LocationCurve locationCurve, Curve wallCurve)
        {
            var bottomHeight = ElementQuery.GetElevationForRay(m_doc, wall);

            // Figure out the increment for the normalized parameter based on how long the wall is.  
            var parameterIncrement = SampleBrowserUtils.WallIncrement / wallCurve.Length;

            // Find columns within 2' of the start of the ray.  Any smaller, and you run the risk of not finding a boundary
            // face of the column within the target range.
            double findColumnWithin = 2;

            // check for columns along every WallIncrement fraction of the wall
            for (double parameter = 0; parameter < 1.0; parameter += parameterIncrement)
                FindColumnsOnEitherSideOfWall(wall, locationCurve, wallCurve, parameter, bottomHeight,
                    findColumnWithin);
        }

        /// <summary>
        ///     Dump wall's curve(end points) to log
        /// </summary>
        private void LogWallCurve(Line wallCurve)
        {
            Debug.WriteLine("Wall curve is line: ");
            Debug.WriteLine($"Start point: {XyzMath.XyzToString(wallCurve.GetEndPoint(0))}");
            Debug.WriteLine($"End point: {XyzMath.XyzToString(wallCurve.GetEndPoint(1))}");
        }
    }
}
