// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Diagnostics;


namespace BuildingCoder
{
    public static class JtPlaneExtensionMethods
    {
        public static double SignedDistanceTo(
            this Plane plane,
            XYZ p)
        {
            Debug.Assert(
                Util.IsEqual(plane.Normal.GetLength(), 1),
                "expected normalised plane normal");

            var v = p - plane.Origin;

            return plane.Normal.DotProduct(v);
        }

        public static XYZ ProjectOnto(
            this Plane plane,
            XYZ p)
        {
            var d = plane.SignedDistanceTo(p);

            //XYZ q = p + d * plane.Normal; // wrong according to Ruslan Hanza and Alexander Pekshev in their comments http://thebuildingcoder.typepad.com/blog/2014/09/planes-projections-and-picking-points.html#comment-3765750464
            var q = p - (d * plane.Normal);

            Debug.Assert(
                Util.IsZero(plane.SignedDistanceTo(q)),
                "expected point on plane to have zero distance to plane");

            return q;
        }

        public static UV ProjectInto(
            this Plane plane,
            XYZ p)
        {
            var q = plane.ProjectOnto(p);
            var o = plane.Origin;
            var d = q - o;
            var u = d.DotProduct(plane.XVec);
            var v = d.DotProduct(plane.YVec);
            return new UV(u, v);
        }
    }
}
