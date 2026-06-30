#region Namespaces

using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NewSprinkler sample.</summary>
    internal static partial class Util
    {
        internal static PlanarFace GetLargestHorizontalFace(
            Element e,
            bool computReferences = true,
            bool bottomFace = true)
        {
            Options opt = new()
            {
                ComputeReferences = computReferences
            };

            var geo = e.get_Geometry(opt);

            PlanarFace largest_face = null;

            foreach (var obj in geo)
            {
                var solid = obj as Solid;

                if (null != solid)
                    foreach (Face face in solid.Faces)
                    {
                        var pf = face as PlanarFace;

                        if (null != pf)
                        {
                            var normal = pf.FaceNormal.Normalize();

                            if (IsVertical(normal)
                                && (bottomFace ? 0.0 > normal.Z : 0.0 < normal.Z)
                                && (null == largest_face || largest_face.Area < pf.Area))
                            {
                                largest_face = pf;
                                break;
                            }
                        }
                    }
            }

            return largest_face;
        }
        internal static XYZ MedianPoint(MeshTriangle triangle)
        {
            var p = XYZ.Zero;
            p += triangle.get_Vertex(0);
            p += triangle.get_Vertex(1);
            p += triangle.get_Vertex(2);
            p *= 0.3333333333333333;
            return p;
        }
        internal static double TriangleArea(MeshTriangle triangle)
        {
            var a = triangle.get_Vertex(0);
            var b = triangle.get_Vertex(1);
            var c = triangle.get_Vertex(2);

            var l = Line.CreateBound(a, b);

            var h = l.Project(c).Distance;

            var area = 0.5 * l.Length * h;

            return area;
        }
        internal static XYZ PointOnFace(PlanarFace face)
        {
            var mesh = face.Triangulate();

            return 0 < mesh.NumTriangles
                ? MedianPoint(mesh.get_Triangle(0))
                : XYZ.Zero;
        }
        internal static XYZ PointOnFace2(PlanarFace face)
        {
            var mesh = face.Triangulate();
            double max_area = 0;
            var selected = 0;

            for (var i = 0; i < mesh.NumTriangles; i++)
            {
                var area = TriangleArea(
                    mesh.get_Triangle(i));

                if (max_area < area)
                {
                    max_area = area;
                    selected = i;
                }
            }

            return MedianPoint(mesh.get_Triangle(selected));
        }
    }
}
