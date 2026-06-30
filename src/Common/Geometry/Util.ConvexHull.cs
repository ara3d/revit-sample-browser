// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Convex Hull

        public static List<XYZ> ConvexHull(List<XYZ> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            var startPoint = points.MinBy(p => p.X);
            List<XYZ> convexHullPoints = new();
            var walkingPoint = startPoint;
            var refVector = XYZ.BasisY.Negate();
            do
            {
                convexHullPoints.Add(walkingPoint);
                var wp = walkingPoint;
                var rv = refVector;
                walkingPoint = points.MinBy(p =>
                {
                    var angle = (p - wp).AngleOnPlaneTo(rv, XYZ.BasisZ);
                    if (angle < 1e-10)
                        angle = 2 * Math.PI;
                    return angle;
                });
                refVector = wp - walkingPoint;
            } while (walkingPoint != startPoint);

            convexHullPoints.Reverse();
            return convexHullPoints;
        }

        #endregion
    }
}
