#region Namespaces

using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        public static void TagWallsWithAdjacentRooms(Document doc)
        {
            var rooms
                = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .OfCategory(BuiltInCategory.OST_Rooms);

            var map_wall_to_rooms
                = new Dictionary<ElementId, List<string>>();

            var opts
                = new SpatialElementBoundaryOptions();

            foreach (Room room in rooms)
            {
                var loops
                    = room.GetBoundarySegments(opts);

                foreach (var loop in loops)
                foreach (var seg in loop)
                {
                    var idWall = seg.ElementId;

                    if (ElementId.InvalidElementId != idWall)
                    {
                        if (!map_wall_to_rooms.ContainsKey(idWall))
                            map_wall_to_rooms.Add(
                                idWall, new List<string>());

                        var room_name = room.Name;

                        if (!map_wall_to_rooms[idWall].Contains(room_name)) map_wall_to_rooms[idWall].Add(room_name);
                    }
                }
            }

            using var tx = new Transaction(doc);
            tx.Start("Add list of adjacent rooms to wall comments");

            var ids
                = map_wall_to_rooms.Keys;

            foreach (var id in ids)
            {
                var wall = doc.GetElement(id);

                var p = wall.get_Parameter(
                    BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);

                if (null != p)
                {
                    var s = string.Join(" / ",
                        map_wall_to_rooms[id]);

                    p.Set(s);
                }
            }

            tx.Commit();
        }

        public static void DetermineAdjacentElementLengthsAndWallAreas(
            Room room)
        {
            var doc = room.Document;

            var boundaries
                = room.GetBoundarySegments(
                    new SpatialElementBoundaryOptions());

            var n = 0;

            if (null != boundaries)
                n = boundaries.Count;

            Debug.Print(
                "{0} has {1} boundar{2}{3}",
                ElementDescription(room),
                n, PluralSuffixY(n),
                DotOrColon(n));

            if (0 < n)
            {
                int iBoundary = 0, iSegment;

                foreach (var b in boundaries)
                {
                    ++iBoundary;
                    iSegment = 0;
                    foreach (var s in b)
                    {
                        ++iSegment;

                        var neighbour = doc.GetElement(s.ElementId);

                        var curve = s.GetCurve();

                        var length = curve.Length;

                        Debug.Print(
                            "  Neighbour {0}:{1} {2} has {3}"
                            + " feet adjacent to room.",
                            iBoundary, iSegment,
                            ElementDescription(neighbour),
                            RealString(length));

                        if (neighbour is Wall wall)
                        {
                            var p = wall.get_Parameter(
                                BuiltInParameter.HOST_AREA_COMPUTED);

                            var area = p.AsDouble();

                            var lc
                                = wall.Location as LocationCurve;

                            var wallLength = lc.Curve.Length;

                            var bottomLevel = doc.GetElement(wall.LevelId) as Level;
                            var bottomElevation = bottomLevel.Elevation;
                            var topElevation = bottomElevation;

                            p = wall.get_Parameter(
                                BuiltInParameter.WALL_HEIGHT_TYPE);

                            if (null != p)
                            {
                                var id = p.AsElementId();
                                var topLevel = doc.GetElement(id) as Level;
                                topElevation = topLevel.Elevation;
                            }

                            var height = topElevation - bottomElevation;

                            Debug.Print(
                                "    This wall has a total length,"
                                + " height and area of {0} feet,"
                                + " {1} feet and {2} square feet.",
                                RealString(wallLength),
                                RealString(height),
                                RealString(area));
                        }
                    }
                }
            }
        }
    }
}
