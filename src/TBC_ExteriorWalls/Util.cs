using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static BoundingBoxXYZ GetBoundingBoxAroundAllWalls(
            Document doc,
            View view = null)
        {
            var bb = new BoundingBoxXYZ();

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
            var ids = new List<ElementId>();

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

            var curves = new CurveArray();
            for (var i = 0; i < 4; ++i)
            {
                var j = i < 3 ? i + 1 : 0;
                curves.Append(Line.CreateBound(
                    bottom_corners[i], bottom_corners[j]));
            }

            using var group = new TransactionGroup(doc);
            Room newRoom;

            group.Start("Find Outermost Walls");

            using (var transaction = new Transaction(doc))
            {
                transaction.Start("Create New Room Boundary Lines");

                var sketchPlane = SketchPlane.Create(
                    doc, view.GenLevel.Id);

                doc.Create.NewRoomBoundaryLines(
                    sketchPlane, curves, view);

                var d = MmToFoot(600);
                var point = new UV(bb.Min.X + d, bb.Min.Y + d);

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
            return new List<ElementId>(x.Split('\n')
                .Select(s => new ElementId(Int64.Parse(s))));
        }
    }
}
