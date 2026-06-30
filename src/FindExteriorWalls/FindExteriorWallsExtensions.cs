// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from RevitFindExteriorWalls by Pekshev / Jeremy Tammik (MIT).
// https://github.com/jeremytammik/RevitFindExteriorWalls

using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.FindExteriorWalls.CS
{
    internal static class FindExteriorWallsExtensions
    {
        public static Line GetPerpendicularLine(this Curve curve, Wall wall, int leftRight)
        {
            var orientation = wall.Orientation;
            if (curve is Arc arc)
                orientation = (((arc.GetEndPoint(0) + arc.GetEndPoint(1)) / 2) - arc.GetCenterPoint()).Normalize();

            return Line.CreateBound(
                curve.GetCenterPoint(),
                leftRight == 0
                    ? curve.GetCenterPoint() - (orientation * 1000)
                    : curve.GetCenterPoint() + (orientation * 1000));
        }

        public static XYZ GetCenterPoint(this Curve curve)
        {
            return curve is Arc arc ? arc.Evaluate(0.5, true) : (curve.GetEndPoint(0) + curve.GetEndPoint(1)) / 2;
        }

        public static bool IntersectToByMovingZ(this Line line, Curve checkedCurve)
        {
            var z = line.GetCenterPoint().Z;
            checkedCurve = GetCurveWithChangedZ(checkedCurve, z);
            return checkedCurve != null && line.Intersect(checkedCurve) == SetComparisonResult.Overlap;
        }

        public static bool IntersectToByMovingZ(
            this Curve curve,
            Curve checkedCurve,
            out IntersectionResultArray intersectionResultArray)
        {
            intersectionResultArray = new IntersectionResultArray();
            var z = curve.GetCenterPoint().Z;
            checkedCurve = GetCurveWithChangedZ(checkedCurve, z);
            return checkedCurve != null && curve.Intersect(checkedCurve, out intersectionResultArray) == SetComparisonResult.Overlap;
        }

        static Curve GetCurveWithChangedZ(Curve curve, double z)
        {
            return curve is Line line
                ? Line.CreateBound(
                    new XYZ(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, z),
                    new XYZ(line.GetEndPoint(1).X, line.GetEndPoint(1).Y, z))
                : curve is Arc arc
                ? Arc.Create(
                    new XYZ(arc.GetEndPoint(0).X, arc.GetEndPoint(0).Y, z),
                    new XYZ(arc.GetEndPoint(1).X, arc.GetEndPoint(1).Y, z),
                    new XYZ(arc.GetCenterPoint().X, arc.GetCenterPoint().Y, z))
                : (Curve)null;
        }

        public static bool HasWallById(this List<Wall> listOfWalls, Wall checkedWall)
        {
            return listOfWalls.Any(w => w.Id == checkedWall.Id);
        }
    }
}
