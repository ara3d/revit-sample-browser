#region Header

//
// CmdGetDimensionPoints.cs - determine dimension segment start and end points
//
// Copyright (C) 2018-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//
// https://forums.autodesk.com/t5/revit-api-forum/how-to-retrieve-a-dimension-s-segment-geometry/m-p/7145688
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    public class CmdGetDimensionPoints : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            ISelectionFilter f
                = new JtElementsOfClassSelectionFilter<Dimension>();

            var elemRef = sel.PickObject(
                ObjectType.Element, f, "Pick a dimension");

            var dim = doc.GetElement(elemRef) as Dimension;

            var p = Util.GetDimensionStartPoint(dim);
            var pts = Util.GetDimensionPoints(dim, p);

            var n = pts.Count;

            Debug.Print("Dimension origin at {0} followed "
                        + "by {1} further point{2}{3} {4}",
                Util.PointString(p), n,
                Util.PluralSuffix(n), Util.DotOrColon(n),
                string.Join(", ", pts.Select(
                    q => Util.PointString(q))));

            List<double> d = new(n);
            var q0 = p;
            foreach (var q in pts)
            {
                d.Add(q.X - q0.X);
                q0 = q;
            }

            Debug.Print(
                $"Horizontal distances in metres: {string.Join(", ", d.Select(x => Util.RealString(Util.FootToMetre(x))))}");

            using Transaction tx = new(doc);
            tx.Start("Draw Point Markers");

            var sketchPlane = dim.View.SketchPlane;

            var size = 0.3;
            Util.DrawDimensionMarker(p, size, sketchPlane);
            pts.ForEach(q => Util.DrawDimensionMarker(q, size, sketchPlane));

            tx.Commit();

            return Result.Succeeded;
        }

        #region Obsolete initial attempts

        private List<XYZ> GetDimensionPointsObsoleteFirstAttempt(
            Dimension dim)
        {
            var dimLine = dim.Curve as Line;
            if (dimLine == null) return null;
            List<XYZ> pts = new();

            dimLine.MakeBound(0, 1);
            var pt1 = dimLine.GetEndPoint(0);
            var pt2 = dimLine.GetEndPoint(1);
            var direction = pt2.Subtract(pt1).Normalize();
            pts.Add(pt1);
            if (dim.Segments.Size == 0)
            {
                pt2 = pt1.Add(direction.Multiply((double)dim.Value));
                pts.Add(pt2);
            }
            else
            {
                var segmentPt0 = pt1;
                foreach (DimensionSegment seg in dim.Segments)
                {
                    var segmentPt1 = segmentPt0.Add(direction.Multiply((double)seg.Value));
                    Debug.Print("pt  {0},  value  {1}", segmentPt1, (double)seg.Value);
                    pts.Add(segmentPt1);
                    segmentPt0 = segmentPt1;
                }
            }

            return pts;
        }

        private XYZ GetDimensionStartPointFirstAttempt(
            Dimension dim)
        {
            var doc = dim.Document;

            var dimLine = dim.Curve as Line;
            if (dimLine == null) return null;
            dimLine.MakeBound(0, 1);

            XYZ dimStartPoint = null;
            var pt1 = dimLine.GetEndPoint(0);

            // dim.Origin throws "Cannot access this method
            // if this dimension has more than one segment."
            //Debug.Assert( Util.IsEqual( pt1, dim.Origin ),
            //  "expected equal points" );

            foreach (Reference ref1 in dim.References)
            {
                XYZ refPoint = null;
                var el = doc.GetElement(ref1.ElementId);
                var obj = el.GetGeometryObjectFromReference(
                    ref1);

                if (obj == null)
                {
                    switch (el)
                    {
                        // element is Grid or ReferencePlane or ??
                        case ReferencePlane refPl:
                            refPoint = refPl.GetPlane().Origin;
                            break;
                        case Grid grid:
                            refPoint = grid.Curve.GetEndPoint(0);
                            break;
                    }
                }
                else
                {
                    // reference to Line, Plane or Point?
                    var l = obj as Line;
                    if (l != null) refPoint = l.GetEndPoint(0);
                    var f = obj as PlanarFace;
                    if (f != null) refPoint = f.Origin;
                }

                if (refPoint != null)
                {
                    //View v = doc.ActiveView;
                    var v = dim.View;
                    var workPlane = v.SketchPlane.GetPlane();
                    var normal = workPlane.Normal.Normalize();

                    // Project the "globalpoint" of the reference onto the sketchplane

                    var refPtonPlane = refPoint.Subtract(
                        normal.Multiply(normal.DotProduct(
                            refPoint - workPlane.Origin)));

                    var lineNormal = normal.CrossProduct(
                        dimLine.Direction).Normalize();

                    // Project the result onto the dimensionLine

                    dimStartPoint = refPtonPlane.Subtract(
                        lineNormal.Multiply(lineNormal.DotProduct(
                            refPtonPlane - pt1)));
                }

                break;
            }

            return dimStartPoint;
        }

        #endregion // Obsolete initial attempts
    }
}