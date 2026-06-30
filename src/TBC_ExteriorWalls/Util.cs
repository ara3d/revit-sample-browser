using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static BoundingBoxXYZ GetBoundingBoxAroundAllWalls(
            Document doc,
            View view = null)
        {
            BoundingBoxXYZ bb = new();

            var walls
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(Wall));

            foreach (Wall wall in walls)
                bb.ExpandToContain(
                    wall.get_BoundingBox(view));
            return bb;
        }

        internal static List<ElementId>
            RetrieveWallsGeneratingRoomBoundaries(
                Document doc,
                Room room)
        {
            List<ElementId> ids = [];

            var boundaries
                = room.GetBoundarySegments(
                    new SpatialElementBoundaryOptions());

            foreach (var b in boundaries)
                foreach (var s in b)
                {
                    var neighbour = doc.GetElement(s.ElementId);

                    if (neighbour is Wall wall)
                        ids.Add(wall.Id);
                }

            return ids;
        }

        public static List<ElementId> GetOutermostWalls(
            Document doc,
            View view = null)
        {
            var offset = MmToFoot(1000);

            view ??= doc.ActiveView;

            var bb = GetBoundingBoxAroundAllWalls(doc, view);

            var voffset = offset * (XYZ.BasisX + XYZ.BasisY);
            bb.Min -= voffset;
            bb.Max += voffset;

            var bottom_corners = GetBottomCorners(bb, 0);

            CurveArray curves = new();
            for (var i = 0; i < 4; ++i)
            {
                var j = i < 3 ? i + 1 : 0;
                curves.Append(Line.CreateBound(
                    bottom_corners[i], bottom_corners[j]));
            }

            using TransactionGroup group = new(doc);
            Room newRoom;

            group.Start("Find Outermost Walls");

            using (Transaction transaction = new(doc))
            {
                transaction.Start("Create New Room Boundary Lines");

                var sketchPlane = SketchPlane.Create(
                    doc, view.GenLevel.Id);

                doc.Create.NewRoomBoundaryLines(
                    sketchPlane, curves, view);

                var d = MmToFoot(600);
                UV point = new(bb.Min.X + d, bb.Min.Y + d);

                newRoom = doc.Create.NewRoom(view.GenLevel, point);

                if (newRoom == null)
                {
                    TaskDialog.Show("xx", "创建房间失败。");
                    transaction.RollBack();
                    return null;
                }

                doc.Create.NewRoomTag(
                    new LinkElementId(newRoom.Id),
                    point, view.Id);

                transaction.Commit();
            }

            var ids = RetrieveWallsGeneratingRoomBoundaries(doc, newRoom);

            group.RollBack();

            return ids;
        }

        internal static List<ElementId> GetElementIdsFromString(string x)
        {
            return [.. x.Split('\n')
                .Select(s => new ElementId(Int64.Parse(s)))];
        }
    }
}
