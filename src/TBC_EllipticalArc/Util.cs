using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static Curve CreateEllipse(Application app)
        {
            var center = XYZ.Zero;

            double radX = 30;
            double radY = 50;

            var xVec = XYZ.BasisX;
            var yVec = XYZ.BasisY;

            var param0 = 0.0;
            var param1 = 2 * Math.PI;

            var c = Ellipse.CreateCurve(center, radX, radY, xVec, yVec, param0, param1);

            var targetAngle = Math.PI / 3.0;

            var direction = new XYZ(
                Math.Cos(targetAngle),
                Math.Sin(targetAngle),
                0);

            var line = Line.CreateUnbound(center, direction);

            c.Intersect(line, out var results);

            foreach (IntersectionResult result in results)
            {
                var p = result.UVPoint.U;
                if (p < param1) param1 = p;
            }

            c.MakeBound(param0, param1);

            return c;
        }
    }
}
