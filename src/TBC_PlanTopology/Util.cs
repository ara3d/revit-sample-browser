#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_PlanTopology sample.</summary>
    internal static partial class Util
    {
        public static XYZ GetElementBbCenter(Element e)
        {
            var bb = e.get_BoundingBox(null);
            return Midpoint(bb.Min, bb.Max);
        }
        public static XYZ GetRoomCenter(Room room)
        {
            var locPt = (LocationPoint)room.Location;
            var pr = locPt.Point;

            var pbb = GetElementBbCenter(room);
            return new XYZ(pbb.X, pbb.Y, pr.Z);
        }
        public static void CreateRoom(
            Document doc,
            Level level)
        {
            var docCreation = doc.Create;

            XYZ pt1 = new(0, -5, 0);
            XYZ pt2 = new(0, 5, 0);
            XYZ pt3 = new(8, 5, 0);
            XYZ pt4 = new(8, -5, 0);

            var line1 = Line.CreateBound(pt1, pt2);
            var line2 = Line.CreateBound(pt2, pt3);
            var line3 = Line.CreateBound(pt3, pt4);
            var line4 = Line.CreateBound(pt4, pt1);

            CurveArray curveArr = new();

            curveArr.Append(line1);
            curveArr.Append(line2);
            curveArr.Append(line3);
            curveArr.Append(line4);

            docCreation.NewRoomBoundaryLines(
                doc.ActiveView.SketchPlane,
                curveArr, doc.ActiveView);

            UV tagPoint = new(4, 0);

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
