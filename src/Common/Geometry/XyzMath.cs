// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Ara3D.RevitSampleBrowser.Common.Documents;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.CreateBeamSystem.CS;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Document = Autodesk.Revit.DB.Document;
using RevitView = Autodesk.Revit.DB.View;

namespace Ara3D.RevitSampleBrowser.Common.Geometry
{
    public static class XyzMath
    {
        public const double Precision = 1e-5;

        public static double DotMatrix(XYZ p1, XYZ p2)
        {
            return (p1.X * p2.X) + (p1.Y * p2.Y) + (p1.Z * p2.Z);
        }

        public static XYZ CrossMatrix(XYZ p1, XYZ p2)
        {
            return new XYZ((p1.Y * p2.Z) - (p1.Z * p2.Y), (p1.Z * p2.X) - (p1.X * p2.Z), (p1.X * p2.Y) - (p1.Y * p2.X));
        }

        public static XYZ SubXyz(XYZ p1, XYZ p2)
        {
            return new XYZ(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        public static bool CompareXyz(XYZ pnt1, XYZ pnt2)
        {
            return IsEqual(pnt1.X, pnt2.X) &&
                    IsEqual(pnt1.Y, pnt2.Y) &&
                    IsEqual(pnt1.Z, pnt2.Z);
        }

        public static List<Line> SortLines(List<Line> originLines)
        {
            if (originLines.Count < 3) return null;

            List<Line> lines = [.. originLines];
            List<Line> result = [lines[0]];
            var intersectPnt = lines[0].GetEndPoint(1);
            lines[0] = null;

            for (var i = 0; i < lines.Count; i++)
                for (var j = 1; j < lines.Count; j++)
                {
                    if (lines[j] == null) continue;

                    if (CompareXyz(lines[j].GetEndPoint(0), intersectPnt))
                    {
                        result.Add(lines[j]);
                        intersectPnt = lines[j].GetEndPoint(1);
                        lines[j] = null;
                        break;
                    }

                    if (CompareXyz(lines[j].GetEndPoint(1), intersectPnt))
                    {
                        var inversedLine = Line.CreateBound(lines[j].GetEndPoint(1), lines[j].GetEndPoint(0));
                        result.Add(inversedLine);
                        intersectPnt = inversedLine.GetEndPoint(1);
                        lines[j] = null;
                        break;
                    }
                }

            if (result.Count != lines.Count) return null;
            if (!CompareXyz(intersectPnt, result[0].GetEndPoint(0))) return null;

            for (var i = 0; i < result.Count - 2; i++)
                for (var j = i + 2; j < result.Count; j++)
                {
                    if (i == 0 && j == result.Count - 1) continue;
                    if (Line2D.FindIntersection(ConvertTo2DLine(result[i]), ConvertTo2DLine(result[j])) > 0)
                        return null;
                }

            return result;
        }

        public static bool InSameHorizontalPlane(List<Line> lines)
        {
            var firstPnt = lines[0].GetEndPoint(0);
            return lines.TrueForAll(line =>
                IsEqual(line.GetEndPoint(0).Z, firstPnt.Z) &&
                IsEqual(line.GetEndPoint(1).Z, firstPnt.Z));
        }

        public static List<XYZ> GetPoints(Face face)
        {
            return face.Triangulate().Vertices as List<XYZ> ?? [];
        }

        public static double GetLength(XYZ pointA, XYZ pointB)
        {
            return pointA.DistanceTo(pointB);
        }

        public static double GetLength(Line line)
        {
            var sub = SubXyz(line.GetEndPoint(0), line.GetEndPoint(1));
            return Math.Sqrt((sub.X * sub.X) + (sub.Y * sub.Y) + (sub.Z * sub.Z));
        }

        public static double GetLength(XYZ vector)
        {
            return vector.GetLength();
        }

        public static bool IsEqual(XYZ p1, XYZ p2)
        {
            return CompareXyz(p1, p2);
        }

        public static Line GetScaledLine(Line inLine, double scale)
        {
            var startPoint = inLine.GetEndPoint(0);
            var endPoint = inLine.GetEndPoint(1);
            var offset = MultiXyz(SubXyz(endPoint, startPoint), (scale - 1) / 2);
            return Line.CreateBound(SubXyz(startPoint, offset), AddXyz(endPoint, offset));
        }

        public static bool IsEqual(double d1, double d2)
        {
            return Math.Abs(d1 - d2) < Precision;
        }

        public static XYZ AddXyz(XYZ p1, XYZ p2)
        {
            return new XYZ(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        private static XYZ MultiXyz(XYZ p1, double para)
        {
            return new XYZ(p1.X * para, p1.Y * para, p1.Z * para);
        }

        private class PointDto
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }

        private static Plane GetAppropriatePlane(RevitView view)
        {
            return Plane.CreateByNormalAndOrigin(view.ViewDirection, view.Origin);
        }

        public static void DrawLines(RevitView view, IList<XYZ> points, double tolerance)
        {
            var plane = GetAppropriatePlane(view);
            if (plane == null)
                return;

            for (var ii = 0; ii < points.Count; ii++)
            {
                plane.Project(points[ii], out var uvStart, out _);
                plane.Project(points[ii + 1], out var uvEnd, out _);

                var projectionStart = (uvStart.U * plane.XVec) + (uvStart.V * plane.YVec) + plane.Origin;
                var projectionEnd = (uvEnd.U * plane.XVec) + (uvEnd.V * plane.YVec) + plane.Origin;

                if (projectionStart.DistanceTo(projectionEnd) < tolerance)
                {
                    ii++;
                    continue;
                }

                _ = view.Document.Create.NewDetailCurve(view, Line.CreateBound(projectionStart, projectionEnd)) as DetailLine;
                ii++;
            }
        }

        public static Curve CalculateAlignedCurve(Curve curve, Curve baseCurve, XYZ baseDirect)
        {
            var direct = (curve as Line).Direction;
            return Math.Round(direct.X) == Math.Round(baseDirect.X) && Math.Round(direct.X) == 1
                ? Line.CreateBound(
                    new XYZ(baseCurve.GetEndPoint(0).X, curve.GetEndPoint(0).Y, curve.GetEndPoint(0).Z),
                    new XYZ(baseCurve.GetEndPoint(1).X, curve.GetEndPoint(1).Y, curve.GetEndPoint(1).Z))
                : Math.Round(direct.Y) == Math.Round(baseDirect.Y) && Math.Round(direct.Y) == 1
                        ? Line.CreateBound(
                            new XYZ(curve.GetEndPoint(0).X, baseCurve.GetEndPoint(0).Y, curve.GetEndPoint(0).Z),
                            new XYZ(curve.GetEndPoint(1).X, baseCurve.GetEndPoint(1).Y, curve.GetEndPoint(1).Z))
                        : Line.CreateBound(
                        new XYZ(curve.GetEndPoint(0).X, curve.GetEndPoint(0).Y, baseCurve.GetEndPoint(0).Z),
                        new XYZ(curve.GetEndPoint(1).X, curve.GetEndPoint(1).Y, baseCurve.GetEndPoint(1).Z));
        }

        public static string XyzToString(XYZ point)
        {
            return $"( {point.X}, {point.Y}, {point.Z})";
        }

        public static IList<Curve> CreateRectangularWallCurves(double length = 60, double width = 40)
        {
            return
            [
                Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, length, 0)),
                        Line.CreateBound(new XYZ(0, length, 0), new XYZ(0, length, width)),
                        Line.CreateBound(new XYZ(0, length, width), new XYZ(0, 0, width)),
                        Line.CreateBound(new XYZ(0, 0, width), new XYZ(0, 0, 0))
            ];
        }

        public static XYZ GetVector(XYZ startPoint, XYZ endPoint)
        {
            return new(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, endPoint.Z - startPoint.Z);
        }

        public static void DrawLine(Pen pen, Graphics graphic, CurveArray curveArray)
        {
            foreach (Curve curve in curveArray)
            {
                if (curve is Line line)
                {
                    graphic.DrawLine(pen,
                        new PointF((float)line.GetEndPoint(0).X, (float)line.GetEndPoint(0).Y),
                        new PointF((float)line.GetEndPoint(1).X, (float)line.GetEndPoint(1).Y));
                    continue;
                }

                var xyzArray = curve.Tessellate() as List<XYZ>;
                for (var i = 0; i < xyzArray.Count - 1; i++)
                {
                    graphic.DrawLine(pen,
                        new PointF((float)xyzArray[i].X, (float)xyzArray[i].Y),
                        new PointF((float)xyzArray[i + 1].X, (float)xyzArray[i + 1].Y));
                }
            }
        }

        public static UV[,] CreateMatrix(int xNumber, int yNumber, double distance)
        {
            var result = new UV[xNumber, yNumber];
            for (var i = 0; i < xNumber; i++)
                for (var j = 0; j < yNumber; j++)
                    result[i, j] = new UV(i * distance, j * distance);
            return result;
        }

        public static bool IsCurveInXyPlane(Curve curve)
        {
            if (Math.Abs(curve.GetEndPoint(1).Z - curve.GetEndPoint(0).Z) > 1e-05)
                return false;

            if (curve is Line || curve.IsCyclic)
                return true;

            var curveLoop = CurveLoop.Create(
                    [
                        curve,
                        Line.CreateBound(curve.GetEndPoint(1), curve.GetEndPoint(0))
                    ]);
            var normal = curveLoop.GetPlane().Normal.Normalize();
            return normal.IsAlmostEqualTo(XYZ.BasisZ) || normal.IsAlmostEqualTo(XYZ.BasisZ.Negate());
        }

        public static Curve CreateReversedCurve(Curve orig)
        {
            return !SampleBrowserUtils.SupportsLoopUtilities(orig)
                        ? throw new NotImplementedException($"CreateReversedCurve for type {orig.GetType().Name}")
                        : orig switch
                        {
                            Line _ => Line.CreateBound(orig.GetEndPoint(1), orig.GetEndPoint(0)),
                            Arc _ => Arc.Create(orig.GetEndPoint(1), orig.GetEndPoint(0), orig.Evaluate(0.5, true)),
                            _ => throw new Exception("CreateReversedCurve - Unreachable")
                        };
        }

        public static XYZ StringToXyz(string pointString)
        {
            var subString = pointString.TrimStart('(').TrimEnd(')');
            var coordinateString = subString.Split(',');
            if (coordinateString.Length != 3)
                throw new InvalidDataException("The point information in journal is incorrect");

            try
            {
                return new XYZ(
                    Convert.ToDouble(coordinateString[0]),
                    Convert.ToDouble(coordinateString[1]),
                    Convert.ToDouble(coordinateString[2]));
            }
            catch (Exception)
            {
                throw new InvalidDataException("The point information in journal is incorrect");
            }
        }

        public static ModelCurve MakeLine(UIApplication app, XYZ ptA, XYZ ptB)
        {
            return MakeLine(app, ptA, ptB, ptA.CrossProduct(ptB).GetLength() == 0 ? XYZ.BasisZ : ptA.CrossProduct(ptB));
        }

        public static ModelCurve MakeLine(UIApplication app, XYZ ptA, XYZ ptB, XYZ norm)
        {
            var doc = app.ActiveUIDocument.Document;
            var appCreate = app.Application.Create;
            var line = Line.CreateBound(ptA, ptB);
            var plane = Plane.CreateByNormalAndOrigin(norm, ptA);
            var sketch = SketchPlane.Create(doc, plane);
            return doc.Create.NewModelCurve(line, sketch);
        }

        private static Line2D ConvertTo2DLine(Line line)
        {
            var start = line.GetEndPoint(0);
            var end = line.GetEndPoint(1);
            return new Line2D(
                new PointF((float)start.X, (float)start.Y),
                new PointF((float)end.X, (float)end.Y));
        }

        public static SketchPlane GetSketchPlaneById(Document document, ElementId id)
        {
            return ElementQuery.GetElementById(document, id) is not SketchPlane workPlane
                        ? throw new System.Exception("Don't have the work plane you select.")
                        : workPlane;
        }

        public static bool IsSameDirection(XYZ firstVec, XYZ secondVec)
        {
            return IsEqual(DotMatrix(UnitVector(firstVec), UnitVector(secondVec)), 1);
        }

        public static bool IsOppositeDirection(XYZ firstVec, XYZ secondVec)
        {
            return IsEqual(DotMatrix(UnitVector(firstVec), UnitVector(secondVec)), -1);
        }

        public static XYZ UnitVector(XYZ vector)
        {
            var length = GetLength(vector);
            return new XYZ(vector.X / length, vector.Y / length, vector.Z / length);
        }

        public static XYZ MultiplyVector(XYZ vector, double rate)
        {
            return new XYZ(vector.X * rate, vector.Y * rate, vector.Z * rate);
        }

        public static XYZ TransformPoint(XYZ point, Transform transform)
        {
            var b0 = transform.get_Basis(0);
            var b1 = transform.get_Basis(1);
            var b2 = transform.get_Basis(2);
            var origin = transform.Origin;
            return new XYZ(
                (point.X * b0.X) + (point.Y * b1.X) + (point.Z * b2.X) + origin.X,
                (point.X * b0.Y) + (point.Y * b1.Y) + (point.Z * b2.Y) + origin.Y,
                (point.X * b0.Z) + (point.Y * b1.Z) + (point.Z * b2.Z) + origin.Z);
        }

        public static XYZ OffsetPoint(XYZ point, XYZ direction, double offset)
        {
            return AddXyz(point, MultiplyVector(UnitVector(direction), offset));
        }

        public const double DoubleTolerance = 1E-9;

        public const double RandomXScale = 5631;

        public const double RandomYScale = 4369;

        public static bool DoubleEquals(double x, double y)
        {
            return Math.Abs(x - y) < DoubleTolerance;
        }

        public static XYZ ProjectPointOnPlane(Plane plane, XYZ point)
        {
            plane.Project(point, out var uv, out _);
            return plane.Origin + (plane.XVec * uv.U) + (plane.YVec * uv.V);
        }

        public static bool IsPointInsideCurveLoop(XYZ point, CurveLoop curveloop)
        {
            var plane = Plane.CreateByThreePoints(curveloop.ElementAt(0).GetEndPoint(0),
                        curveloop.ElementAt(1).GetEndPoint(0),
                        curveloop.ElementAt(2).GetEndPoint(0));
            var projectedPoint = ProjectPointOnPlane(plane, point);
            var veryLongLine = Line.CreateBound(projectedPoint,
                        projectedPoint + (RandomXScale * plane.XVec) + (RandomYScale * plane.YVec));

            var intersectionCount = 0;
            foreach (var edge in curveloop)
            {
                if (veryLongLine.Intersect(edge, out var resultArray) == SetComparisonResult.Overlap)
                    intersectionCount += resultArray.Size;
            }

            return intersectionCount % 2 == 1;
        }

        public static XYZ MoveXyzToElevation(XYZ input, double elevation)
        {
            return input + (XYZ.BasisZ * (elevation - input.Z));
        }

        public static IList<XYZ> CalculateControlPoints2(Document rvtDoc, IList<ElementId> elements)
        {
            IList<Curve> curves = [];
            foreach (var e in elements)
            {
                var curve = rvtDoc.GetElement(e);
                curves.Add((curve.Location as LocationCurve).Curve);
            }

            IList<XYZ> controlPoints = [];
            switch (curves.Count)
            {
                case 2:
                    {
                        var curve1 = curves[0];
                        var curve2 = curves[1];
                        if (HasCommonEndPoint(curve1, curve2, out var commonPnt, out var index1, out var index2))
                        {
                            controlPoints.Add(curve1.GetEndPoint(1 - index1));
                            controlPoints.Add(commonPnt);
                            controlPoints.Add(curve2.GetEndPoint(1 - index2));
                        }

                        break;
                    }
                case 3:
                    {
                        var curve1 = curves[0];
                        var curve2 = curves[1];
                        var curve3 = curves[2];

                        XYZ start = null, commonPnt1 = null, commonPnt2 = null, end = null;
                        int index1 = -1, index2 = -1, index3 = -1, index4 = -1;

                        if (HasCommonEndPoint(curve1, curve2, out commonPnt1, out index1, out index2))
                        {
                            if (HasCommonEndPoint(curve1, curve3, out commonPnt2, out index3, out index4))
                            {
                                start = curve2.GetEndPoint(1 - index2);
                                end = curve3.GetEndPoint(1 - index4);
                            }
                            else if (HasCommonEndPoint(curve2, curve3, out commonPnt2, out index3, out index4))
                            {
                                start = curve1.GetEndPoint(1 - index1);
                                end = curve3.GetEndPoint(1 - index4);
                            }
                        }
                        else if (HasCommonEndPoint(curve1, curve3, out commonPnt1, out index1, out index2) &&
                                 HasCommonEndPoint(curve2, curve3, out commonPnt2, out index3, out index4))
                        {
                            start = curve1.GetEndPoint(1 - index1);
                            end = curve2.GetEndPoint(1 - index3);
                        }

                        if (start != null)
                        {
                            controlPoints.Add(start);
                            controlPoints.Add(commonPnt1);
                            controlPoints.Add(commonPnt2);
                            controlPoints.Add(end);
                        }

                        break;
                    }
            }

            return controlPoints;
        }

        private static bool HasCommonEndPoint(Curve crv1, Curve crv2, out XYZ common,
                    out int index1, out int index2)
        {
            var pnt1 = crv1.GetEndPoint(0);
            var pnt2 = crv1.GetEndPoint(1);
            var pnt3 = crv2.GetEndPoint(0);
            var pnt4 = crv2.GetEndPoint(1);

            if (pnt1.IsAlmostEqualTo(pnt3)) { index1 = 0; index2 = 0; common = pnt1; return true; }
            if (pnt1.IsAlmostEqualTo(pnt4)) { index1 = 0; index2 = 1; common = pnt1; return true; }
            if (pnt2.IsAlmostEqualTo(pnt3)) { index1 = 1; index2 = 0; common = pnt2; return true; }
            if (pnt2.IsAlmostEqualTo(pnt4)) { index1 = 1; index2 = 1; common = pnt2; return true; }

            common = null;
            index1 = index2 = -1;
            return false;
        }

    }
}