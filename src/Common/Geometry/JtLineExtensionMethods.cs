// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;


namespace BuildingCoder
{
    public static class JtLineExtensionMethods
    {
        public static bool Contains(
            this Line line,
            XYZ p,
            double tolerance = Util._eps)
        {
            var a = line.GetEndPoint(0); // line start point
            var b = line.GetEndPoint(1); // line end point
            var f = a.DistanceTo(b); // distance between focal points
            var da = a.DistanceTo(p);
            var db = p.DistanceTo(b);
            // da + db is always greater or equal f
            return (da + db - f) * f < tolerance;
        }
    }
}
