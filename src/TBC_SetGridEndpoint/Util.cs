#region Namespaces

using System;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        // Off-axis grid alignment; see https://forums.autodesk.com/t5/revit-api-forum/grids-off-axis/m-p/7129065
        public static void AlignOffAxisGrid(
            Grid grid)
        {
            var doc = grid.Document;

            var direction = grid.Curve
                .GetEndPoint(1)
                .Subtract(grid.Curve.GetEndPoint(0))
                .Normalize();

            var distance2hor = direction.DotProduct(XYZ.BasisY);
            var distance2vert = direction.DotProduct(XYZ.BasisX);
            double angle = 0;

            var max_distance = 0.0001;

            if (Math.Abs(distance2hor) < max_distance)
            {
                var vector = direction.X < 0
                    ? direction.Negate()
                    : direction;

                angle = Math.Asin(-vector.Y);
            }

            if (Math.Abs(distance2vert) < max_distance)
            {
                var vector = direction.Y < 0
                    ? direction.Negate()
                    : direction;

                angle = Math.Asin(vector.X);
            }

            if (angle.CompareTo(0) != 0)
            {
                using var t = new Transaction(doc);
                t.Start("correctGrid");

                ElementTransformUtils.RotateElement(doc,
                    grid.Id,
                    Line.CreateBound(grid.Curve.GetEndPoint(0),
                        grid.Curve.GetEndPoint(0).Add(XYZ.BasisZ)),
                    angle);

                t.Commit();
            }
        }
    }
}
