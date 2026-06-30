// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.Common.Geometry
{
    public static class PlaneAndTransform
    {
        public const double WallEpsilon = 1.0 / 8.0 / 12.0;

        public static XYZ GetNormalToWallAt(Wall wall, LocationCurve curve, double parameter)
        {
            var wallCurve = curve.Curve;
            if (wallCurve is Line)
            {
                var wallDirection = CurveGeometry.GetTangentAt(wallCurve, 0);
                return new XYZ(wallDirection.Y, wallDirection.X, 0);
            }

            return wallCurve.ComputeDerivatives(parameter, true).BasisY.Normalize();
        }

        public static XYZ GetWallDeltaAt(Wall wall, LocationCurve locationCurve, double parameter)
        {
            var wallNormal = GetNormalToWallAt(wall, locationCurve, parameter);
            var halfWidth = (wall.Width / 2) + WallEpsilon;
            return new XYZ(wallNormal.X * halfWidth, wallNormal.Y * halfWidth, 0);
        }

    }
}