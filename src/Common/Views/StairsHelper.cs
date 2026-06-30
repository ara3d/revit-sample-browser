// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.Common.Views
{
    public static class StairsHelper
    {
        public static Tuple<Level, Level, Level> FindTargetLevels(Document doc, string name1, string name2, string name3)
        {
            Level level1 = null;
            Level level2 = null;
            Level level3 = null;

            foreach (var level in new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>()
                         .Where(level => level.Name.Equals(name1) || level.Name.Equals(name2) || level.Name.Equals(name3)))
            {
                if (level.Name.Equals(name1))
                    level1 = level;
                else if (level.Name.Equals(name2))
                    level2 = level;
                else
                    level3 = level;
            }

            return Tuple.Create(level1, level2, level3);
        }

        public static IList<XYZ> CalculateControlPoints(Document rvtDoc, IList<ElementId> crvElements)
        {
            var maxOffset = 0.0;
            foreach (var t in crvElements)
            {
                var curve = rvtDoc.GetElement(t);
                if (curve.Location is not LocationCurve locationCrv || locationCrv.Curve is not Line)
                    throw new ArgumentException("The input elements are not Line base.");

                if (Math.Abs(locationCrv.Curve.GetEndPoint(0).Z
                             - locationCrv.Curve.GetEndPoint(1).Z) > 1.0e-9)
                    throw new AggregateException(
                        "The input curve elements are not in the same elevation plane.");
                if (curve is Wall wall) maxOffset = Math.Max(maxOffset, wall.Width * 0.5);
            }

            var controlPoints = XyzMath.CalculateControlPoints2(rvtDoc, crvElements);
            if (controlPoints.Count == 0)
                throw new ArgumentException("The input curve elements are not continues.");
            if (!CheckOrientation(controlPoints))
                throw new ArgumentException(
                    "The input curve elements should have the same orientation: CW or CCW.");
            if (maxOffset > 0.0) controlPoints = CalculateOffset(controlPoints, maxOffset);
            return controlPoints;
        }

        public static IList<uint> CalculateMaxStepsCount(IList<XYZ> controlPoints,
                    double runWidth, double treadDepth)
        {
            var innerPnts = CalculateOffset(controlPoints, runWidth);
            IList<uint> counts = [];
            for (var i = 1; i < innerPnts.Count; i++)
            {
                var dist = innerPnts[i].DistanceTo(innerPnts[i - 1]);
                counts.Add((uint)(dist / treadDepth));
            }

            return counts;
        }

        private static bool CheckOrientation(IList<XYZ> controlPoints)
        {
            XYZ previousDir = null;
            for (var i = 1; i < controlPoints.Count - 1; i++)
            {
                var dir1 = controlPoints[i] - controlPoints[i - 1];
                var dir2 = controlPoints[i + 1] - controlPoints[i];
                if (previousDir == null)
                    previousDir = dir1.CrossProduct(dir2).Normalize();
                else if (!previousDir.IsAlmostEqualTo(dir1.CrossProduct(dir2).Normalize())) return false;
            }

            return true;
        }

        private static IList<XYZ> CalculateOffset(IList<XYZ> controlPoints, double offset)
        {
            IList<XYZ> innerPnts = [];

            for (var i = 1; i < controlPoints.Count - 1; i++)
            {
                var dir1 = (controlPoints[i] - controlPoints[i - 1]).Normalize();
                var dir2 = (controlPoints[i + 1] - controlPoints[i]).Normalize();
                var bisectDir = (dir2 - dir1).Normalize();

                var stepInside1StDir = new XYZ(-dir1.Y, dir1.X, 0.0);
                if (stepInside1StDir.DotProduct(bisectDir) < 0.0)
                    stepInside1StDir = stepInside1StDir.Negate();

                if (i == 1) innerPnts.Add(controlPoints[i - 1] + (stepInside1StDir * offset));

                var stepInside2NdDir = new XYZ(-dir2.Y, dir2.X, 0.0);
                if (stepInside2NdDir.DotProduct(bisectDir) < 0.0)
                    stepInside2NdDir = stepInside2NdDir.Negate();

                var semiAngle = bisectDir.AngleTo(dir2);
                var slopDist = offset / Math.Sin(semiAngle);
                innerPnts.Add(controlPoints[i] + (bisectDir * slopDist));

                if (i == controlPoints.Count - 2)
                    innerPnts.Add(controlPoints[i + 1] + (stepInside2NdDir * offset));
            }

            return innerPnts;
        }

    }
}