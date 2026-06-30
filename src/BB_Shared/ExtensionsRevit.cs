using Ara3D.Utils;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExternalService;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Arc = Autodesk.Revit.DB.Arc;

namespace Ara3D.Bowerbird.RevitSamples
{
    public static class ExtensionsRevit
    {
        public static ICollection<Element> GetElements(this Document doc)
        {
            return new FilteredElementCollector(doc)
                        .WhereElementIsNotElementType()
                        .ToElements();
        }

        public static ICollection<ElementId> GetElementIds(this Document doc)
        {
            return new FilteredElementCollector(doc)
                        .WhereElementIsNotElementType()
                        .ToElementIds();
        }

        public static ICollection<Element> GetTypeElements(this Document doc)
        {
            return new FilteredElementCollector(doc)
                        .WhereElementIsElementType()
                        .ToElements();
        }

        public static ICollection<ElementId> GetTypeElementIds(this Document doc)
        {
            return new FilteredElementCollector(doc)
                        .WhereElementIsElementType()
                        .ToElementIds();
        }

        public static IEnumerable<Phase> GetPhases(this Document doc)
        {
            return doc.CollectElements()
                        .OfClass(typeof(Phase))
                        .OfType<Phase>();
        }

        public static int GetPhaseSequenceNumber(this Phase p)
        {
            return p.get_Parameter(BuiltInParameter.PHASE_SEQUENCE_NUMBER).AsInteger();
        }

        public static Phase GetLastPhase(this Document doc)
        {
            return doc.GetPhases().OrderBy(GetPhaseSequenceNumber).LastOrDefault();
        }

        public static IEnumerable<Room> GetRooms(this Document doc)
        {
            return doc.CollectElements()
                        .OfClass(typeof(SpatialElement))
                        .OfType<Room>();
        }

        public static IDictionary<long, T> ToDictionary<T>(this IEnumerable<T> self) where T : Element
        {
            return self.ToDictionary(v => v.Id.Value, v => v);
        }

        public static IEnumerable<Level> GetLevels(this Document doc)
        {
            return doc.CollectElements()
                        .OfClass(typeof(Level))
                        .OfType<Level>();
        }

        public static Dictionary<long, List<FamilyInstance>> GroupByRoom(this IEnumerable<FamilyInstance> instances)
        {
            return instances.GroupBy(fi => fi.GetRoomId()).ToDictionary(g => g.Key, g => g.ToList());
        }

        public static FilteredElementCollector CollectElements(this Document doc)
        {
            return new FilteredElementCollector(doc);
        }

        public static IEnumerable<FamilyInstance> GetFamilyInstances(this Document doc)
        {
            return doc.CollectElements()
                        .OfClass(typeof(FamilyInstance))
                        .Cast<FamilyInstance>();
        }

        public static IEnumerable<FamilyInstance> GetFamilyInstances(this Document doc, BuiltInCategory cat)
        {
            return doc.CollectElements()
                        .OfCategory(cat)
                        .OfClass(typeof(FamilyInstance))
                        .Cast<FamilyInstance>();
        }

        public static IEnumerable<FamilyInstance> GetLights(this Document doc)
        {
            return doc.GetFamilyInstances(BuiltInCategory.OST_LightingFixtures);
        }

        public static IEnumerable<FamilyInstance> GetSockets(this Document doc)
        {
            return doc.GetFamilyInstances(BuiltInCategory.OST_ElectricalFixtures);
        }

        public static IEnumerable<FamilyInstance> GetDoors(this Document doc)
        {
            return doc.GetFamilyInstances(BuiltInCategory.OST_Doors);
        }

        public static IEnumerable<FamilyInstance> GetWindows(this Document doc)
        {
            return doc.GetFamilyInstances(BuiltInCategory.OST_Windows);
        }

        public static IEnumerable<FamilyInstance> GetWalls(this Document doc)
        {
            return doc.GetFamilyInstances(BuiltInCategory.OST_Walls);
        }

        public static IEnumerable<ViewSchedule> GetSchedules(this Document doc)
        {
            return doc.GetElements<ViewSchedule>();
        }

        public static List<Dictionary<string, string>> GetScheduleData(this ViewSchedule schedule)
        {
            List<Dictionary<string, string>> scheduleData = [];

            // Access the table data
            var tableData = schedule.GetTableData();
            var bodySection = tableData.GetSectionData(SectionType.Body);

            var def = schedule.Definition;
            var numCols = bodySection.NumberOfColumns;
            var headers = Enumerable
                .Range(0, def.GetFieldCount())
                .Select(i => def.GetField(i))
                .Where(f => !f.IsHidden)
                .Select(f => f.ColumnHeading)
                .ToList();
            var numRows = bodySection.NumberOfRows;
            for (var row = 0; row < numRows; row++)
            {
                Dictionary<string, string> rowData = [];
                for (var col = 0; col < numCols; col++)
                {
                    var cellValue = schedule.GetCellText(SectionType.Body, row, col);
                    if (cellValue.IsNullOrEmpty())
                    {
                        // Retrieve numeric data
                        cellValue = bodySection?.GetCellCalculatedValue(row, col)?.ToString() ?? "";
                    }
                    var columnName = headers[col];
                    rowData[columnName] = cellValue;
                }
                scheduleData.Add(rowData);
            }

            return scheduleData;
        }

        public static int CountFamilyInstance(this Room room, BuiltInCategory cat)
        {
            return room.Document.GetFamilyInstances(cat).Count();
        }

        public static IEnumerable<Wall> GetBoundaryWalls(this Room room)
        {
            foreach (var segmentList in room.GetBoundarySegmentLists())
            {
                foreach (var segment in segmentList)
                {
                    var elementId = segment.ElementId;
                    var element = room.Document.GetElement(elementId);
                    if (element is Wall wall)
                        yield return wall;
                }
            }
        }

        public static bool IsCategoryType(this Element element, BuiltInCategory cat)
        {
            return element.Category.Id.Value == (int)cat
                       || (element is FamilyInstance fi && fi.Symbol.IsCategoryType(cat));
        }

        public static IEnumerable<Element> GetHostedElements(this HostObject self)
        {
            return self.Document.GetElements(self.FindInserts(true, false, true, true));
        }

        public static IEnumerable<Element> GetElements(this Document doc, IEnumerable<ElementId> ids)
        {
            return ids.Select(doc.GetElement);
        }

        /// <summary>
        /// Returns points on a curve.
        /// - If the curve is a Line: returns its two endpoints.
        /// - Otherwise: returns N points along the curve (normalized parameter).
        ///   For closed curves, avoids duplicating the start point.
        /// </summary>
        public static IEnumerable<XYZ> SamplePoints(this Curve curve, int n)
        {
            if (curve == null) throw new ArgumentNullException(nameof(curve));

            // Lines: just endpoints
            if (curve is Line)
            {
                yield return curve.GetEndPoint(0);
                yield return curve.GetEndPoint(1);
                yield break;
            }

            // Guard: at least 2 samples requested
            var count = Math.Max(2, n);

            // Closed curves: sample [0, 1) to avoid duplicating the start point
            if (curve.IsCyclic)
            {
                for (var i = 0; i < count; i++)
                {
                    var t = (double)i / count; // 0 <= t < 1
                    yield return curve.Evaluate(t, /*normalized*/ true);
                }
            }
            else
            {
                // Open curves: include both ends [0, 1]
                for (var i = 0; i < count; i++)
                {
                    var t = (double)i / (count - 1); // 0 ... 1 inclusive
                    yield return curve.Evaluate(t, /*normalized*/ true);
                }
            }
        }

        public static IEnumerable<XYZ> GetPoints(this BoundarySegment self, int curveSamples = 8)
        {
            return self.GetCurve().SamplePoints(curveSamples);
        }

        public static IEnumerable<IEnumerable<XYZ>> GetBoundaryLoops(this Room room, int curveSamples = 8)
        {
            return room.GetBoundarySegmentLists().Select(segList => segList.GetBoundaryLoop(curveSamples));
        }

        public static IEnumerable<XYZ> GetBoundaryLoop(this IEnumerable<BoundarySegment> self, int curveSamples = 8)
        {
            return self.SelectMany(segment => segment.GetPoints(curveSamples));
        }

        public static IEnumerable<IEnumerable<BoundarySegment>> GetBoundarySegmentLists(this Room room)
        {
            SpatialElementBoundaryOptions options = new();
            var boundaries = room.GetBoundarySegments(options);
            if (boundaries == null) yield break;
            foreach (var boundaryList in boundaries)
                yield return boundaryList;
        }

        public static long GetRoomId(this FamilyInstance self)
        {
            try
            {
                return self.Room?.Id.Value ?? -1;
            }
            catch
            {
                return -1;
            }
        }

        public static IEnumerable<FamilyInstance> BelongingToRoom(this IEnumerable<FamilyInstance> self, Room room)
        {
            return self.Where(fi => fi.GetRoomId() == room.Id.Value);
        }

        public static IEnumerable<FamilyInstance> GetLights(this Room room)
        {
            return room.Document.GetLights().BelongingToRoom(room);
        }

        public static IEnumerable<FamilyInstance> GetDoors(this Room room)
        {
            return room.Document.GetDoors().BelongingToRoom(room);
        }

        public static IEnumerable<FamilyInstance> GetWindows(this Room room)
        {
            return room.Document.GetWindows().BelongingToRoom(room);
        }

        public static IEnumerable<FamilyInstance> GetSockets(this Room room)
        {
            return room.Document.GetSockets().BelongingToRoom(room);
        }

        public static double SpaceArea(Document doc, Room room)
        {
            SpatialElementGeometryCalculator calculator = new(doc);

            // compute the room geometry
            var results = calculator.CalculateSpatialElementGeometry(room);

            // get the solid representing the room's geometry
            var roomSolid = results.GetGeometry();

            var result = 0.0;

            foreach (Face face in roomSolid.Faces)
            {
                // TODO: I am not convinced that this computation is correct. 
                var faceArea = face.Area;

                // get the sub-faces for the face of the room
                var subfaceList = results.GetBoundaryFaceInfo(face);
                if (subfaceList.Count > 0) // there are multiple sub-faces that define the face
                {
                    // get the area of each sub-face
                    // sub-faces exist in situations such as when a room-bounding wall has been
                    // horizontally split and the faces of each split wall combine to create the 
                    // entire face of the room
                    foreach (var subface in subfaceList)
                    {
                        var subfaceArea = subface.GetSubface().Area;
                        result += subfaceArea;
                    }
                }
                else
                {
                    result += faceArea;
                }

            }

            return result;
        }

        public static Solid GetGeometry(this SpatialElement space)
        {
            SpatialElementGeometryCalculator calculator = new(space.Document);
            var results = calculator.CalculateSpatialElementGeometry(space);
            return results.GetGeometry();
        }

        public static IEnumerable<SpatialElementBoundarySubface> GetSubfaces(this SpatialElement space)
        {
            SpatialElementGeometryCalculator calculator = new(space.Document);
            var results = calculator.CalculateSpatialElementGeometry(space);
            var roomSolid = results.GetGeometry();

            foreach (Face face in roomSolid.Faces)
            {
                // get the sub-faces for the face of the room
                var subfaceList = results.GetBoundaryFaceInfo(face);
                foreach (var subface in subfaceList)
                    yield return subface;

                // sub-faces exist in situations such as when a room-bounding wall has been
                // horizontally split and the faces of each split wall combine to create the 
                // entire face of the room
            }
        }

        /// <summary>
        /// Retrieves the Opening elements grouped by their Host.
        /// </summary>
        public static Dictionary<long, List<Opening>> GroupOpeningsByHost(this Document doc)
        {
            return new FilteredElementCollector(doc).OfClass(typeof(Opening)).Cast<Opening>()
                        .GroupBy(opening => opening.Host?.Id ?? ElementId.InvalidElementId)
                        .ToDictionary(g => g.Key.Value, g => g.ToList());
        }

        /// <summary>
        /// Retrieves the Door elements grouped by their Host.
        /// </summary>
        public static Dictionary<long, List<FamilyInstance>> GroupDoorsByHost(this Document doc)
        {
            return doc.GetDoors()
                        .GroupBy(door => door.Host?.Id ?? ElementId.InvalidElementId)
                        .ToDictionary(g => g.Key.Value, g => g.ToList());
        }

        public static IEnumerable<FamilyInstance> GetBoundaryDoors(this Room room,
            Dictionary<long, List<FamilyInstance>> doorsByHost)
        {
            return room.GetBoundaryWalls().SelectMany(bw => doorsByHost.TryGetValue(bw.Id.Value, out var value)
                        ? value
                        : Enumerable.Empty<FamilyInstance>());
        }

        public static IEnumerable<Opening> GetBoundaryOpenings(this Room room,
            Dictionary<long, List<Opening>> openingsByHost)
        {
            return room.GetBoundaryWalls().SelectMany(bw => openingsByHost.TryGetValue(bw.Id.Value, out var value)
                        ? value
                        : Enumerable.Empty<Opening>());
        }

        public static Autodesk.Revit.DB.XYZ[] GetBaseBox(this Element e)
        {
            var box = e.get_BoundingBox(null);
            if (box == null)
                return null;
            var z = box.Min.Z;
            var minX = box.Min.X;
            var minY = box.Min.Y;
            var maxX = box.Max.X;
            var maxY = box.Max.Y;
            return new[]
                { new Autodesk.Revit.DB.XYZ(minX, minY, z), new Autodesk.Revit.DB.XYZ(maxX, minY, z), new Autodesk.Revit.DB.XYZ(maxX, maxY, z), new Autodesk.Revit.DB.XYZ(minX, maxY, z) };
        }

        public static IList<Autodesk.Revit.DB.XYZ> GetBaseBox(this Opening opening)
        {
            if (opening.IsRectBoundary)
                return opening.BoundaryRect.ToArray();

            List<XYZ> list = [];
            foreach (var curve in opening.BoundaryCurves)
            {
                if (curve is Line line)
                {
                    list.Add(line.GetEndPoint(0));
                    list.Add(line.GetEndPoint(1));
                }
                else if (curve is Arc arc)
                {
                    list.Add(arc.GetEndPoint(0));
                    list.Add(arc.GetEndPoint(1));
                }
            }

            return list;
        }

        public static Autodesk.Revit.DB.XYZ Current3DCameraPosition(this UIDocument uidoc)
        {
            return uidoc.ActiveView is not View3D view ? XYZ.Zero : view.GetOrientation().EyePosition;
        }

        public static void Update3DCameraPosition(this UIDocument uidoc, Autodesk.Revit.DB.XYZ pos)
        {
            if (uidoc.ActiveView is not View3D view)
                return;

            // Start a transaction to modify the view
            using (Transaction t = new(uidoc.Document, "Move Camera"))
            {
                t.Start();

                // Get the current camera orientation
                var orientation = view.GetOrientation();

                // Create the new orientation with the updated eye position
                ViewOrientation3D newOrientation = new(
                    pos,
                    orientation.UpDirection,
                    orientation.ForwardDirection);

                // Apply the updated orientation back to the view
                view.SetOrientation(newOrientation);

                t.Commit();
            }

            // Refresh the active view to reflect changes
            uidoc.RefreshActiveView();
        }

        public static void RegisterDirectDrawServer(this IExternalServer self)
        {
            // Register this class as a server with the DirectContext3D service.
            var directContext3DService =
                ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
            directContext3DService.AddServer(self);

            if (directContext3DService is not MultiServerService msDirectContext3DService)
                throw new Exception("Expected a MultiServerService");

            // Get current list 
            var serverIds = msDirectContext3DService.GetActiveServerIds();
            serverIds.Add(self.GetServerId());

            // Add the new server to the list of active servers.
            msDirectContext3DService.SetActiveServers(serverIds);
        }

        // TODO: this throws an exception if the server was not registered. Should check.
        public static void UnregisterDirectDrawServer(this IExternalServer self)
        {
            // Register this class as a server with the DirectContext3D service.
            var directContext3DService =
                ExternalServiceRegistry.GetService(ExternalServices.BuiltInExternalServices.DirectContext3DService);
            directContext3DService.RemoveServer(self.GetServerId());

            if (directContext3DService is not MultiServerService msDirectContext3DService)
                throw new Exception("Expected a MultiServerService");

            // Get current list 
            var serverIds = msDirectContext3DService.GetActiveServerIds();
            serverIds.Remove(self.GetServerId());

            // Remove the new server from the list of active servers.
            msDirectContext3DService.SetActiveServers(serverIds);
        }

        public static IReadOnlyList<int> GetIndexData(this TriangulatedShellComponent self)
        {
            List<int> r = [];
            for (var i = 0; i < self.TriangleCount; i++)
            {
                var tri = self.GetTriangle(i);
                r.Add(tri.VertexIndex0);
                r.Add(tri.VertexIndex1);
                r.Add(tri.VertexIndex2);
            }
            return r;
        }

        public static IReadOnlyList<double> GetVertexData(this TriangulatedShellComponent self)
        {
            List<double> r = [];
            for (var i = 0; i < self.VertexCount; i++)
            {
                var v = self.GetVertex(i);
                r.Add(v.X);
                r.Add(v.Y);
                r.Add(v.Z);
            }
            return r;
        }

        public static Ara3D.Utils.FilePath WriteToFileAsObj(this TriangulatedShellComponent self, Ara3D.Utils.FilePath filePath)
        {
            return filePath.WriteObjFile(GetVertexData(self), GetIndexData(self));
        }

        public static IReadOnlyList<TriangulatedShellComponent> TriangulatedComponents(
            this TriangulatedSolidOrShell solid)
        {
            return Enumerable.Range(0, solid.ShellComponentCount).Select(solid.GetShellComponent).ToList();
        }

        public static TriangulatedSolidOrShell Tessellate(this Solid solid)
        {
            SolidOrShellTessellationControls controls = new()
            {
                // https://www.revitapidocs.com/2020.1/720f75c5-8a11-bfc6-d698-a200ffc28be9.htm
                /*
                controls.MinAngleInTriangle = 0.01; // Max value is (Math.PI * 3)
                controls.MinExternalAngleBetweenTriangles = 0.2; // 
                controls.LevelOfDetail = 0.5; // 0 to 1 
                controls.Accuracy = 0.1;
                */

                /*
                // https://github.com/Autodesk/revit-ifc/blob/master/Source/Revit.IFC.Export/Utility/ExporterUtil.cs
                controls.Accuracy = 0.6;
                controls.LevelOfDetail = 0.1;
                controls.MinAngleInTriangle = 0.13;
                    controls.MinExternalAngleBetweenTriangles = 1.2;
                */

                // https://github.com/Autodesk/revit-ifc/blob/master/Source/Revit.IFC.Export/Utility/ExporterUtil.cs
                Accuracy = 0.5,
                LevelOfDetail = 0.4,
                MinAngleInTriangle = 0.13,
                MinExternalAngleBetweenTriangles = 0.55
            };

            return SolidUtils.TessellateSolidOrShell(solid, controls);
        }

        public static IEnumerable<T> GetElements<T>(this Document doc)
            where T : Element
        {
            return new FilteredElementCollector(doc).OfClass(typeof(T)).Cast<T>();
        }

        public static IEnumerable<View3D> GetNonTemplateView3Ds(this Document doc)
        {
            return doc.GetElements<View3D>().Where(v => !v.IsTemplate);
        }

        public static View3D GetDefault3DView(this Document doc)
        {
            var views = doc.GetNonTemplateView3Ds().ToList();
            return views.FirstOrDefault(v => v.Name == "{3D}")
                   ?? views.FirstOrDefault();
        }

        public static Utils.FilePath CurrentFileName(this Document doc)
        {
            return doc.PathName;
        }
    }
}