using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    internal static partial class Util
    {
        internal static void GetFaceNaos(
            Dictionary<XYZ, List<XYZ>> naos,
            Solid solid)
        {
            foreach (Face face in solid.Faces)
            {
                var planarFace = face as PlanarFace;
                if (null != planarFace)
                {
                    var normal = planarFace.FaceNormal;
                    var origin = planarFace.Origin;
                    var normals = new List<XYZ>(naos.Keys);
                    var i = normals.FindIndex(
                        delegate(XYZ v) { return IsParallel(v, normal); });

                    if (-1 == i)
                    {
                        Debug.Print(
                            "Face at {0} has new normal {1}",
                            PointString(origin),
                            PointString(normal));

                        naos.Add(normal, new List<XYZ>());
                        naos[normal].Add(origin);
                    }
                    else
                    {
                        Debug.Print(
                            "Face at {0} normal {1} matches {2}",
                            PointString(origin),
                            PointString(normal),
                            PointString(normals[i]));

                        naos[normals[i]].Add(origin);
                    }
                }
            }
        }

        internal static double GetMaxDistanceAlongNormal(
            List<XYZ> pts,
            XYZ normal)
        {
            int i, j;
            var n = pts.Count;
            double dmax = 0;

            for (i = 0; i < n - 1; ++i)
            for (j = i + 1; j < n; ++j)
            {
                var v = pts[i].Subtract(pts[j]);
                var d = v.DotProduct(normal);
                if (d > dmax) dmax = d;
            }

            return dmax;
        }

        internal static string GetWallDimensions(
            Dictionary<XYZ, List<XYZ>> naos)
        {
            string s, ret = string.Empty;

            foreach (var pair in naos)
            {
                var normal = pair.Key.Normalize();
                var pts = pair.Value;

                if (1 == pts.Count)
                {
                    s = string.Format(
                        "Only one wall face in "
                        + "direction {0} found.",
                        PointString(normal));
                }
                else
                {
                    var dmax = GetMaxDistanceAlongNormal(
                        pts, normal);

                    s = string.Format(
                        "Max wall dimension in "
                        + "direction {0} is {1} feet.",
                        PointString(normal),
                        RealString(dmax));
                }

                Debug.WriteLine(s);
                ret += $"\n{s}";
            }

            return ret;
        }

        internal static string ProcessWallDimensions(Wall wall)
        {
            var msg = $"Wall <{wall.Name} {wall.Id.Value}>:";

            Debug.WriteLine(msg);

            var o = wall.Document.Application.Create.NewGeometryOptions();
            var ge = wall.get_Geometry(o);

            IEnumerable<GeometryObject> objs = ge;

            var naos
                = new Dictionary<XYZ, List<XYZ>>();

            foreach (var obj in objs)
            {
                var solid = obj as Solid;
                if (null != solid) GetFaceNaos(naos, solid);
            }

            return $"{msg}{GetWallDimensions(naos)}\n";
        }
    }
}
