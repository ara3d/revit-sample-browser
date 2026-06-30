using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ListAllRooms sample.</summary>
    internal static partial class Util
    {
        public enum ListAllRoomsRoomState
        {
            Unknown,
            Placed,
            NotPlaced,
            NotEnclosed,
            Redundant
        }

        /// <summary>
        ///     Room boundary tolerance for considering
        ///     slightly differing boundary points equal
        /// </summary>
        private static readonly double _listAllRoomsTolerance
            = MmToFoot(1.2);

        /// <summary>
        ///     Distinguish 'Not Placed',  'Redundant'
        ///     and 'Not Enclosed' rooms.
        /// </summary>
        public static ListAllRoomsRoomState DistinguishRoom(Room room)
        {
            var res = ListAllRoomsRoomState.Unknown;

            if (room.Area > 0)
            {
                res = ListAllRoomsRoomState.Placed;
            }
            else if (null == room.Location)
            {
                res = ListAllRoomsRoomState.NotPlaced;
            }
            else
            {
                var opt
                    = new SpatialElementBoundaryOptions();

                var segs
                    = room.GetBoundarySegments(opt);

                res = null == segs || segs.Count == 0
                    ? ListAllRoomsRoomState.NotEnclosed
                    : ListAllRoomsRoomState.Redundant;
            }

            return res;
        }

        /// <summary>
        ///     Add new points to the list.
        ///     Skip the first new point if it equals the last
        ///     old existing one. Actually, we can test all points
        ///     and always ignore very close consecutive ones.
        /// </summary>
        public static void AddNewRoomBoundaryPoints(
            IList<XYZ> pts,
            IList<XYZ> newpts)
        {
            foreach (var p in newpts)
                if (0 == pts.Count
                    || !IsEqual(p, pts.Last(),
                        _listAllRoomsTolerance))
                    pts.Add(p);
        }

        /// <summary>
        ///     Return room boundary points retrieved
        ///     from the room boundary segments.
        /// </summary>
        public static List<XYZ> GetRoomBoundaryPoints(
            IList<IList<BoundarySegment>> boundary)
        {
            var pts = new List<XYZ>();

            var n = boundary.Count;

            if (1 > n)
            {
                Debug.Print("Boundary contains no loops");
            }
            else
            {
                if (1 < n)
                    Debug.Print(
                        "Boundary contains {0} loop{1}; "
                        + "skipping all but first.",
                        n, PluralSuffix(n));

                foreach (var loop in boundary)
                {
                    foreach (var seg in loop)
                    {
                        var c = seg.GetCurve();
                        AddNewRoomBoundaryPoints(pts, c.Tessellate());
                    }

                    var z = pts[0].Z;

                    foreach (var p in pts)
                        Debug.Assert(
                            IsEqual(p.Z, z, _listAllRoomsTolerance),
                            "expected horizontal room boundary");

                    break;
                }
            }

            return pts;
        }

        /// <summary>
        ///     Return bounding box calculated from the room
        ///     boundary segments.
        /// </summary>
        public static BoundingBoxXYZ GetRoomBoundaryBoundingBox(
            IList<IList<BoundarySegment>> boundary)
        {
            var bb = new BoundingBoxXYZ();
            bb.Clear();

            foreach (var loop in boundary)
            foreach (var seg in loop)
            {
                var c = seg.GetCurve();
                var pts = c.Tessellate();
                foreach (var p in pts) bb.ExpandToContain(p);
            }

            return bb;
        }

        /// <summary>
        ///     Return convex hull calculated from the room
        ///     boundary segments.
        /// </summary>
        public static List<XYZ> GetConvexHullOfRoomBoundary(
            IList<IList<BoundarySegment>> boundary)
        {
            var convex_hull = new List<XYZ>();

            if (0 < boundary.Count)
            {
                var pts = new List<XYZ>();

                foreach (var loop in boundary)
                foreach (var seg in loop)
                {
                    var c = seg.GetCurve();
                    pts.AddRange(c.Tessellate());
                }

                var n = pts.Count;

                pts = new List<XYZ>(
                    pts.Distinct(new XyzEqualityComparer(1.0e-4)));

                Debug.Print(
                    "{0} points from tessellated room boundaries, "
                    + "{1} points after cleaning up duplicates",
                    n, pts.Count);

                convex_hull = ConvexHull(pts);
            }

            return convex_hull;
        }
    }
}
