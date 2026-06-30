#region Header

//
// CmdListAllRooms.cs - list properties from all rooms
//
// Copyright (C) 2011-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdListAllRooms : IExternalCommand
    {
        private const bool _exportCsv = true;
        private const string _csv_headers = "Room nr;Name;Center;"
                                            + "Lower left;Boundary;Convex hull;Bounding box;"
                                            + "Area in sq ft";
        private const string _format_string = _exportCsv
            ? "{0},{1},{2},{3},{4},{5},{6},{7}"
            : "Room nr. '{0}' named '{1}' at {2} with "
              + "lower left corner {3}, "
              + "boundary points ({4}), convex hull ({5}), "
              + "bounding box {6} and area {7} sqf has "
              + "{8} loop{9} and {10} segment{11} in first "
              + "loop.";
        private static readonly bool _exportInMillimetres = true;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var app = uiapp.Application;
            var doc = uidoc.Document;

            // OfClass(typeof(Room)) throws; filter SpatialElement and cast to Room.

            var collector
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(SpatialElement));

            var rooms = collector
                .Where(e => e is Room);

            if (_exportCsv) Debug.Print(_csv_headers);

            foreach (Room room in rooms) ListRoomData(room);
            return Result.Succeeded;
        }
        private void DistinguishRoomsDraft(
            Document doc,
            ref StringBuilder sb,
            ref int numErr,
            ref int numWarn)
        {
            FilteredElementCollector rooms
                = new(doc);

            rooms.WherePasses(new RoomFilter());

            foreach (Room r in rooms)
            {
                sb.AppendFormat("\r\n  Room {0}:'{1}': ",
                    r.Id, r.Name);

                if (r.Area > 0) // OK if having Area
                    sb.AppendFormat("OK (A={0}[ft3])", r.Area);
                else if (null == r.Location) // Unplaced if no Location
                    sb.AppendFormat("UnPlaced (Location is null)");
                else
                    sb.AppendFormat("NotEnclosed or Redundant "
                                    + "- how to distinguish?");
            }
        }
        private void ListRoomData(Room room)
        {
            SpatialElementBoundaryOptions opt
                = new();

            var nr = room.Number;
            var name = room.Name;
            var area = room.Area;

            var loc = room.Location;
            var p = loc is not LocationPoint lp ? XYZ.Zero : lp.Point;

            var bb = room.get_BoundingBox(null);

            var boundary
                = room.GetBoundarySegments(opt);

            var nLoops = boundary.Count;

            var nFirstLoopSegments = 0 < nLoops
                ? boundary[0].Count
                : 0;

            var boundary_bounding_box
                = Util.GetRoomBoundaryBoundingBox(boundary);

            var convex_hull
                = Util.GetConvexHullOfRoomBoundary(boundary);

            var boundary_pts = Util.GetRoomBoundaryPoints(
                boundary);

            string room_point_str,
                lower_left_str,
                boundary_pts_str,
                convex_hull_str,
                bounding_box_str;

            var llx = boundary_bounding_box.Min.X;

            if (double.MaxValue == llx)
            {
                lower_left_str = "undefined";
                Debug.Assert(0 == boundary_pts.Count,
                    "expected empty boundary for undefined lower left corner");
                Debug.Assert(0 == convex_hull.Count,
                    "expected empty convex hull for undefined lower left corner");
            }
            else
            {
                lower_left_str = _exportInMillimetres
                    ? new IntPoint3d(boundary_bounding_box.Min)
                        .ToString(_exportCsv)
                    : Util.PointString(
                        boundary_bounding_box.Min, _exportCsv);
            }

            if (_exportInMillimetres)
            {
                room_point_str = new IntPoint3d(p)
                    .ToString(_exportCsv);

                var separator = _exportCsv ? " " : ", ";

                boundary_pts_str = string.Join(separator,
                    boundary_pts.Select(q
                        => new IntPoint2d(q.X, q.Y)
                            .ToString(_exportCsv)));

                convex_hull_str = string.Join(separator,
                    convex_hull.Select(q
                        => new IntPoint2d(q.X, q.Y)
                            .ToString(_exportCsv)));

                bounding_box_str = null == bb
                    ? "null"
                    : $"{new IntPoint3d(bb.Min).ToString(_exportCsv)}{separator}{new IntPoint3d(bb.Max).ToString(_exportCsv)}";
            }
            else
            {
                room_point_str = Util.PointString(
                    p, _exportCsv);

                var boundary_pts_2d = boundary_pts
                    .Select(q => new UV(q.X, q.Y));

                var convex_hull_2d = convex_hull
                    .Select(q => new UV(q.X, q.Y));

                boundary_pts_str = Util.PointArrayString(
                    boundary_pts_2d, _exportCsv);

                convex_hull_str = Util.PointArrayString(
                    convex_hull_2d, _exportCsv);

                bounding_box_str = null == bb
                    ? "null"
                    : Util.BoundingBoxString(bb, _exportCsv);
            }

            Debug.Print(_format_string, nr, name, room_point_str, lower_left_str, boundary_pts_str, convex_hull_str, bounding_box_str, area, nLoops, Util.PluralSuffix(nLoops),
                nFirstLoopSegments, Util.PluralSuffix(nFirstLoopSegments));
        }
    }
}
