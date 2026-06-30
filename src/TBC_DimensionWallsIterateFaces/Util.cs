using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_DimensionWallsIterateFaces sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Create a new dimension element using the given
        ///     references and dimension line end points.
        /// </summary>
        internal static void CreateDimensionElement(
            View view,
            XYZ p1,
            Reference r1,
            XYZ p2,
            Reference r2)
        {
            var doc = view.Document;

            var ra = new ReferenceArray();

            ra.Append(r1);
            ra.Append(r2);

            var line = Line.CreateBound(p1, p2);

            using var t = new Transaction(doc);
            t.Start("Create New Dimension");

            doc.Create.NewDimension(
                view, line, ra);

            t.Commit();
        }

        /// <summary>
        ///     Return the closest planar face to a given point
        ///     on the element e with a given normal vector.
        /// </summary>
        internal static Face GetClosestFace(
            Element e,
            XYZ p,
            XYZ normal,
            Options opt)
        {
            Face face = null;
            var min_distance = double.MaxValue;
            var geo = e.get_Geometry(opt);

            foreach (var obj in geo)
            {
                var solid = obj as Solid;
                if (solid != null)
                {
                    var fa = solid.Faces;
                    foreach (Face f in fa)
                    {
                        var pf = f as PlanarFace;

                        Debug.Assert(null != pf,
                            "expected planar wall faces");

                        if (null != pf
                            && IsParallel(normal, pf.FaceNormal))
                        {
                            var v = p - pf.Origin;
                            var d = v.DotProduct(-pf.FaceNormal);
                            if (d < min_distance)
                            {
                                face = f;
                                min_distance = d;
                            }
                        }
                    }
                }
            }

            return face;
        }

        internal static void DuplicateDimension(
            Document doc,
            Dimension dimension)
        {
            var line = dimension.Curve as Line;

            if (null != line)
            {
                var view = dimension.View;

                var references = dimension.References;

                doc.Create.NewDimension(
                    view, line, references);
            }
        }

        internal static Dimension CreateLinearDimension(
            Document doc)
        {
            var pt1 = new XYZ(5, 5, 0);
            var pt2 = new XYZ(5, 10, 0);
            var line = Line.CreateBound(pt1, pt2);

            var plane = Plane.CreateByNormalAndOrigin(pt1.CrossProduct(pt2), pt2);

            var skplane = SketchPlane.Create(doc, plane);

            var modelcurve1 = doc.FamilyCreate
                .NewModelCurve(line, skplane);

            pt1 = new XYZ(10, 5, 0);
            pt2 = new XYZ(10, 10, 0);
            line = Line.CreateBound(pt1, pt2);
            plane = Plane.CreateByNormalAndOrigin(pt1.CrossProduct(pt2), pt2);

            skplane = SketchPlane.Create(doc, plane);

            var modelcurve2 = doc.FamilyCreate
                .NewModelCurve(line, skplane);

            var ra = new ReferenceArray();
            ra.Append(modelcurve1.GeometryCurve.Reference);
            ra.Append(modelcurve2.GeometryCurve.Reference);

            pt1 = new XYZ(5, 10, 0);
            pt2 = new XYZ(10, 10, 0);
            line = Line.CreateBound(pt1, pt2);
            var dim = doc.FamilyCreate
                .NewLinearDimension(doc.ActiveView, line, ra);

            var param = doc.FamilyManager
                .AddParameter("width",
                    GroupTypeId.Constraints,
                    SpecTypeId.Length,
                    false);

            dim.FamilyLabel = param;

            return dim;
        }

        internal static void CreateFilledRegionDimensions(
            FilledRegion filledRegion,
            XYZ dimensionDirection)
        {
            var document = filledRegion.Document;

            var view = (View) document.GetElement(
                filledRegion.OwnerViewId);

            var edgesDirection = dimensionDirection.CrossProduct(
                view.ViewDirection);

            var edges = FindFilledRegionEdges(filledRegion)
                .Where(x => IsFilledRegionEdgeDirectionSatisfied(x, edgesDirection))
                .ToList();

            if (edges.Count < 2)
                return;

            var shift = UnitUtils.ConvertToInternalUnits(
                            -10 * view.Scale, UnitTypeId.Millimeters)
                        * edgesDirection;

            var dimensionLine = Line.CreateUnbound(
                filledRegion.get_BoundingBox(view).Min
                + shift, dimensionDirection);

            var references = new ReferenceArray();

            foreach (var edge in edges)
                references.Append(edge.Reference);

            document.Create.NewDimension(view, dimensionLine,
                references);
        }

        internal static void CreateFilledRegionDimensions(
            FilledRegion filledRegion,
            XYZ dimensionDirection,
            string typeName)
        {
            var document = filledRegion.Document;

            var view = (View) document.GetElement(
                filledRegion.OwnerViewId);

            var edgesDirection = dimensionDirection.CrossProduct(
                view.ViewDirection);

            var edges = FindFilledRegionEdges(filledRegion)
                .Where(x => IsFilledRegionEdgeDirectionSatisfied(x, edgesDirection))
                .ToList();

            if (edges.Count < 2)
                return;

            var shift = UnitUtils.ConvertToInternalUnits(
                            5 * view.Scale, UnitTypeId.Millimeters)
                        * edgesDirection;

            var dimensionLine = Line.CreateUnbound(
                filledRegion.get_BoundingBox(view).Min + shift,
                dimensionDirection);

            var references = new ReferenceArray();

            foreach (var edge in edges)
                references.Append(edge.Reference);

            var dim = document.Create.NewDimension(
                view, dimensionLine, references);

            var dr_id = DimensionTypeId(
                document, typeName);

            if (dr_id != null) dim.ChangeTypeId(dr_id);
        }

        internal static bool IsFilledRegionEdgeDirectionSatisfied(
            Edge edge,
            XYZ edgeDirection)
        {
            var edgeCurve = edge.AsCurve() as Line;

            if (edgeCurve == null)
                return false;

            return edgeCurve.Direction.CrossProduct(
                edgeDirection).IsAlmostEqualTo(XYZ.Zero);
        }

        internal static IEnumerable<FilledRegion>
            FindFilledRegions(
                Document document,
                ElementId viewId)
        {
            var collector = new FilteredElementCollector(
                document, viewId);

            return collector
                .OfClass(typeof(FilledRegion))
                .Cast<FilledRegion>();
        }

        internal static IEnumerable<Edge>
            FindFilledRegionEdges(
                FilledRegion filledRegion)
        {
            var view = (View) filledRegion.Document.GetElement(
                filledRegion.OwnerViewId);

            var options = new Options
            {
                View = view,
                ComputeReferences = true
            };

            return filledRegion
                .get_Geometry(options)
                .OfType<Solid>()
                .SelectMany(x => x.Edges.Cast<Edge>());
        }

        internal static ElementId DimensionTypeId(
            Document doc,
            string typeName)
        {
            var mt_coll
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(DimensionType))
                    .WhereElementIsElementType();

            DimensionType dimType = null;

            foreach (var type in mt_coll)
                if (type is DimensionType dimensionType)
                    if (dimensionType.Name == typeName)
                    {
                        dimType = dimensionType;
                        break;
                    }

            return dimType?.Id;
        }
    }
}
