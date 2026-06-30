#region Namespaces

using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_PlanTopology sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return element bounding box centre point
        /// </summary>
        public static XYZ GetElementBbCenter(Element e)
        {
            var bb = e.get_BoundingBox(null);
            return Midpoint(bb.Min, bb.Max);
        }

        /// <summary>
        ///     Return room centre point for placing room tag
        /// </summary>
        public static XYZ GetRoomCenter(Room room)
        {
            var locPt = (LocationPoint) room.Location;
            var pr = locPt.Point;

            var pbb = GetElementBbCenter(room);
            return new XYZ(pbb.X, pbb.Y, pr.Z);
        }

        /// <summary>
        ///     Create a room on a given level.
        /// </summary>
        public static void CreateRoom(
            Document doc,
            Level level)
        {
            var docCreation = doc.Create;

            var pt1 = new XYZ(0, -5, 0);
            var pt2 = new XYZ(0, 5, 0);
            var pt3 = new XYZ(8, 5, 0);
            var pt4 = new XYZ(8, -5, 0);

            var line1 = Line.CreateBound(pt1, pt2);
            var line2 = Line.CreateBound(pt2, pt3);
            var line3 = Line.CreateBound(pt3, pt4);
            var line4 = Line.CreateBound(pt4, pt1);

            var curveArr = new CurveArray();

            curveArr.Append(line1);
            curveArr.Append(line2);
            curveArr.Append(line3);
            curveArr.Append(line4);

            docCreation.NewRoomBoundaryLines(
                doc.ActiveView.SketchPlane,
                curveArr, doc.ActiveView);

            var tagPoint = new UV(4, 0);

            var room = docCreation.NewRoom(
                level, tagPoint);

            if (null == room)
                throw new Exception(
                    "Create a new room failed.");
            room.Number = "42";
            room.Name = "Lobby";

            docCreation.NewRoomTag(
                new LinkElementId(room.Id), tagPoint,
                doc.ActiveView.Id);
        }
    }
}
