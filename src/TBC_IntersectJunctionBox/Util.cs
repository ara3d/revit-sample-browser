using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_IntersectJunctionBox sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return all faces from first solid of the given element.
        /// </summary>
        internal static IEnumerable<Face> GetElementFaces(Element e)
        {
            var opt = new Options();
            var faces = e
                .get_Geometry(opt)
                .OfType<Solid>()
                .First()
                .Faces
                .OfType<Face>();
            var n = faces.Count();
            Debug.Print("{0} has {1} face{2}.",
                e.GetType().Name, n, PluralSuffix(n));
            return faces;
        }

        /// <summary>
        ///     Test face-face intersection between a floor and wall.
        /// </summary>
        internal static void TestFaceIntersect(Document doc)
        {
            var view = doc.ActiveView;

            var list = new FilteredElementCollector(doc, view.Id)
                .WhereElementIsNotElementType()
                .Where(e => e is Wall or Floor);

            var n = list.Count();

            Element floor = null;
            Element wall = null;

            if (2 == n)
            {
                floor = list.First() as Floor;
                if (null == floor)
                {
                    floor = list.Last() as Floor;
                    wall = list.First() as Wall;
                }
                else
                {
                    wall = list.Last() as Wall;
                }
            }

            if (null == floor || null == wall)
            {
                ErrorMsg("Please run this command in a "
                         + "document with just one floor and one wall "
                         + "with no mutual intersection");
            }
            else
            {
                var floorFaces = GetElementFaces(floor);
                var wallFaces = GetElementFaces(wall);
                n = 0;
                foreach (var f1 in floorFaces)
                foreach (var f2 in wallFaces)
                    if (f1.Intersect(f2)
                        == FaceIntersectionFaceResult.Intersecting)
                    {
                        ++n;

                        if (MessageBox.Show(
                                "Intersects", "Continue",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Exclamation)
                            == DialogResult.Cancel)
                            return;
                    }

                Debug.Print("{0} face-face intersection{1}.",
                    n, PluralSuffix(n));
            }
        }

        /// <summary>
        ///     Create parallel conduit offsets from a curve element.
        /// </summary>
        internal static void CreateConduitOffsets(
            Element conduit,
            double diameter,
            double distance)
        {
            var doc = conduit.Document;

            var obj = conduit.Location as LocationCurve;
            var start = obj.Curve.GetEndPoint(0);
            var end = obj.Curve.GetEndPoint(1);
            var vx = end - start;

            var radius = 0.5 * diameter;
            var offset_lr = radius;
            var offset_up = radius * Math.Sqrt(3);

            var vz = XYZ.BasisZ;
            var vy = vz.CrossProduct(vx);
            var start1 = start + offset_lr * vy;
            var start2 = start - offset_lr * vy;
            var start3 = start + offset_up * vz;
            var end1 = start1 + vx;
            var end2 = start2 + vx;
            var end3 = start3 + vx;

            var L = Math.Sqrt((start.X - end.X) * (start.X - end.X)
                              + (start.Y - end.Y) * (start.Y - end.Y));
            var x1startO = start.X + distance * (end.Y - start.Y) / L;
            var x1endO = end.X + distance * (end.Y - start.Y) / L;
            var y1startO = start.Y + distance * (start.X - end.X) / L;
            var y1end0 = end.Y + distance * (start.X - end.X) / L;
            var conduit1 = Conduit.Create(doc, conduit.GetTypeId(),
                new XYZ(x1startO, y1startO, start.Z),
                new XYZ(x1endO, y1end0, end.Z), conduit.LevelId);
            conduit1.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM)
                .Set(diameter);

            var x2startO = start.X - distance * (end.Y - start.Y) / L;
            var x2endO = end.X - distance * (end.Y - start.Y) / L;
            var y2startO = start.Y - distance * (start.X - end.X) / L;
            var y2end0 = end.Y - distance * (start.X - end.X) / L;

            var conduit2 = Conduit.Create(doc, conduit.GetTypeId(),
                new XYZ(x2startO, y2startO, start.Z),
                new XYZ(x2endO, y2end0, end.Z), conduit.LevelId);
            conduit2.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM)
                .Set(diameter);

            var p0 = new XYZ(
                start.X - (end.Y - start.Y) * (1 / obj.Curve.ApproximateLength),
                start.Y + (end.X - start.X) * (1 / obj.Curve.ApproximateLength),
                start.Z);
            var p1 = new XYZ(
                start.X + (end.Y - start.Y) * (1 / obj.Curve.ApproximateLength),
                start.Y - (end.X - start.X) * (1 / obj.Curve.ApproximateLength),
                start.Z);

            var copyCurve = obj.Curve.CreateOffset(
                -diameter * Math.Sqrt(3) / 2,
                Line.CreateBound(p0, p1).Direction);
            var conduit3 = Conduit.Create(doc, conduit.GetTypeId(),
                copyCurve.GetEndPoint(0), copyCurve.GetEndPoint(1),
                conduit.LevelId);
            conduit3.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM)
                .Set(diameter);
        }
    }

    /// <summary>
    ///     Find conduits intersecting a junction box by geometry.
    /// </summary>
    internal class JunctionBoxConduitFinder
    {
        public readonly List<Conduit> GetListOfConduits = new();

        public JunctionBoxConduitFinder(
            FamilyInstance jbox,
            UIDocument uiDoc)
        {
            var jboxPoint = (jbox.Location as LocationPoint).Point;

            var listOfCloserConduit
                = new FilteredElementCollector(uiDoc.Document)
                    .OfClass(typeof(Conduit))
                    .ToList()
                    .Where(x =>
                        ((x as Conduit).Location as LocationCurve).Curve
                        .GetEndPoint(0).DistanceTo(jboxPoint) < 30
                        || ((x as Conduit).Location as LocationCurve).Curve
                        .GetEndPoint(1).DistanceTo(jboxPoint) < 30)
                    .ToList();

            var opt = new Options
            {
                View = uiDoc.ActiveView
            };
            var geoEle = jbox.get_Geometry(opt);

            foreach (var geomObje1 in geoEle)
            {
                var geoInstance = (geomObje1 as GeometryInstance)
                    ?.GetInstanceGeometry();

                if (geoInstance != null)
                    foreach (var geomObje2 in geoInstance)
                    {
                        var geoSolid = geomObje2 as Solid;
                        if (geoSolid != null)
                            foreach (Face face in geoSolid.Faces)
                            foreach (var cond in listOfCloserConduit)
                            {
                                var con = cond as Conduit;
                                var conCurve = (con.Location as LocationCurve).Curve;
                                var set = face.Intersect(conCurve);
                                if (set.ToString() == "Overlap")
                                    GetListOfConduits.Add(con);
                            }
                    }
            }
        }

        public Conduit ConduitRun { get; set; }

        public FamilyInstance Jbox { get; set; }
    }
}
