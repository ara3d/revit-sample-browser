// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS.LandingComponents
{
    /// <summary>
    ///     Utilities used in processing of landings.
    /// </summary>
    public class LandingComponentUtils
    {
        /// <summary>
        ///     Returns a new set of curves created as a projection of the input curves to the provided elevation.
        /// </summary>
        /// <remarks>Assumes, but does not validate, that the curves are in the XY plane.</remarks>
        /// <param name="curves">The curves.</param>
        /// <param name="elevation">The target elevation.</param>
        /// <returns>The projected curves.</returns>
        public static IList<Curve> ProjectCurvesToElevation(IList<Curve> curves, double elevation)
        {
            var ret = new List<Curve>();
            foreach (var curve in curves) ret.Add(ProjectCurveToElevation(curve, elevation));
            return ret;
        }

        /// <summary>
        ///     Returns a new curve created as a projection of the input curve to the provided elevation.
        /// </summary>
        /// <remarks>Assumes, but does not validate, that the curve is in the XY plane.</remarks>
        /// <param name="curve">The curve.</param>
        /// <param name="elevation">The target elevation.</param>
        /// <returns>The projected curve.</returns>
        public static Curve ProjectCurveToElevation(Curve curve, double elevation)
        {
            var offset1 = elevation - curve.GetEndPoint(0).Z;
            var projectedCurve = curve.CreateTransformed(Transform.CreateTranslation(new XYZ(0, 0, offset1)));
            return projectedCurve;
        }

        /// <summary>
        ///     Given two linear inputs, finds the combination of endpoints from each line which are farthest from each other, and
        ///     returns a line
        ///     connecting those points.
        /// </summary>
        /// <param name="line1">The first line.</param>
        /// <param name="line2">The second line.</param>
        /// <returns>The longest line connecting one endpoint from each line. </returns>
        public static Line FindLongestEndpointConnection(Line line1, Line line2)
        {
            var maxDistance = 0.0;
            XYZ endPoint1 = null;
            XYZ endPoint2 = null;

            for (var i = 0; i < 2; i++)
            for (var j = 0; j < 2; j++)
            {
                var distance = line1.GetEndPoint(j).DistanceTo(line2.GetEndPoint(i));
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    endPoint1 = line1.GetEndPoint(j);
                    endPoint2 = line2.GetEndPoint(i);
                }
            }

            return Line.CreateBound(endPoint1, endPoint2);
        }

        /// <summary>
        ///     Computes the parameter of the point projected onto the curve.
        /// </summary>
        /// <param name="curve">The curve.</param>
        /// <param name="point">The point.</param>
        /// <returns>The parameter of the projected point.</returns>
        public static double ComputeParameter(Curve curve, XYZ point)
        {
            var result = curve.Project(point);
            return result.Parameter;
        }
    }
}
