// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Diagnostics;


namespace BuildingCoder
{
    public static class JtEdgeArrayExtensionMethods
    {
        public static List<XYZ> GetPolygon(
            this EdgeArray ea)
        {
            var n = ea.Size;

            List<XYZ> polygon = new(n);

            foreach (Edge e in ea)
            {
                var pts = e.Tessellate();

                n = polygon.Count;

                if (0 < n)
                {
                    Debug.Assert(pts[0]
                            .IsAlmostEqualTo(polygon[n - 1]),
                        "expected last edge end point to "
                        + "equal next edge start point");

                    polygon.RemoveAt(n - 1);
                }

                polygon.AddRange(pts);
            }

            n = polygon.Count;

            Debug.Assert(polygon[0]
                    .IsAlmostEqualTo(polygon[n - 1]),
                "expected first edge start point to "
                + "equal last edge end point");

            polygon.RemoveAt(n - 1);

            return polygon;
        }
    }
}
