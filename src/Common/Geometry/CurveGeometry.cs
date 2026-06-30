// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.Common.Geometry
{
    public static class CurveGeometry
    {
        public static XYZ GetTangentAt(Curve curve, double parameter)
        {
            return curve.ComputeDerivatives(parameter, true).BasisX.Normalize();
        }

        public static IList<Curve> TransformCurves(IList<Curve> inputs, Transform trf)
        {
            return inputs.Select(input => TransformCurve(input, trf)).ToList();
        }

        public static Curve TransformCurve(Curve input, Transform trf)
        {
            return input.CreateTransformed(trf);
        }

        public static IList<Curve> ProjectCurvesToElevation(IList<Curve> curves, double elevation)
        {
            return curves.Select(curve => ProjectCurveToElevation(curve, elevation)).ToList();
        }

        public static Curve ProjectCurveToElevation(Curve curve, double elevation)
        {
            var offset = elevation - curve.GetEndPoint(0).Z;
            return curve.CreateTransformed(Transform.CreateTranslation(new XYZ(0, 0, offset)));
        }

        public static Line FindLongestEndpointConnection(Line line1, Line line2)
        {
            var maxDistance = 0.0;
            XYZ endPoint1 = null;
            XYZ endPoint2 = null;

            for (var i = 0; i < 2; i++)
                for (var j = 0; j < 2; j++)
                {
                    var distance = line1.GetEndPoint(j).DistanceTo(line2.GetEndPoint(i));
                    if (distance <= maxDistance)
                        continue;
                    maxDistance = distance;
                    endPoint1 = line1.GetEndPoint(j);
                    endPoint2 = line2.GetEndPoint(i);
                }

            return Line.CreateBound(endPoint1, endPoint2);
        }

        public static double ComputeParameter(Curve curve, XYZ point)
        {
            return curve.Project(point).Parameter;
        }
    }
}