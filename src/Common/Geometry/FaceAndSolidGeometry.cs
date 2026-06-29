// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

using Document = Autodesk.Revit.DB.Document;
using RevitView = Autodesk.Revit.DB.View;

using Ara3D.RevitSampleBrowser.Common.Mep;
using Ara3D.RevitSampleBrowser.Common.Parameters;

namespace Ara3D.RevitSampleBrowser.Common.Geometry
{
    public static class FaceAndSolidGeometry
    {
        public const long SolidToBeCutElementId = 30481;
        public const long CuttingSolidElementId = 30809;

        public static bool TryGetDemoSolids(Document doc, out Element solidToBeCut, out Element cuttingSolid)
                {
                    solidToBeCut = doc.GetElement(new ElementId(SolidToBeCutElementId));
                    cuttingSolid = doc.GetElement(new ElementId(CuttingSolidElementId));
                    return solidToBeCut != null && cuttingSolid != null;
                }

        public static FaceArray GetSolidFaces(Element element, RevitView view)
                {
                    if (element == null) return null;
                    var options = new Options { ComputeReferences = true, View = view };
                    return element.get_Geometry(options)
                        .OfType<Solid>()
                        .Select(s => s.Faces)
                        .FirstOrDefault();
                }

        public static Face GetExteriorFace(FaceArray faces) =>
                    faces.Cast<Face>().OrderByDescending(AverageY).FirstOrDefault();

        public static Face GetInteriorFace(FaceArray faces) =>
                    faces.Cast<Face>().OrderBy(AverageY).FirstOrDefault();

        public static FaceArray GetFaces(Element elem)
                {
                    var geoOptions = elem.Document.Application.Create.NewGeometryOptions();
                    geoOptions.ComputeReferences = true;
                    foreach (var obj in elem.get_Geometry(geoOptions))
                        if (obj is Solid solid)
                            return solid.Faces;
                    return null;
                }

        public static bool IsHorizontalFace(Face face)
                {
                    var points = XyzMath.GetPoints(face);
                    if (points.Count < 4) return false;
                    return XyzMath.IsEqual(points[0].Z, points[1].Z) && XyzMath.IsEqual(points[1].Z, points[2].Z) &&
                           XyzMath.IsEqual(points[2].Z, points[3].Z) && XyzMath.IsEqual(points[3].Z, points[0].Z);
                }

        public static double GetDistance(PlanarFace face1, PlanarFace face2)
                {
                    var boxUv = face2.GetBoundingBox();
                    var centerPt = face2.Evaluate((boxUv.Max + boxUv.Min) * 0.5);
                    return face1.Project(centerPt).Distance;
                }

        public static double GetDistance(double value1, double value2) =>
                    System.Math.Abs(value1 - value2);

        public static bool AreFaceNormalsPaired(List<Vector4> normals)
                {
                    if (normals == null || normals.Count != 6)
                        return false;

                    var matchedList = new bool[6];
                    for (var i = 0; i < matchedList.Length; i++)
                    {
                        if (matchedList[i]) continue;
                        var vec4A = normals[i];
                        for (var j = 0; j < matchedList.Length; j++)
                        {
                            if (j == i || matchedList[j]) continue;
                            if (!ParameterAccess.IsLinesParallel(vec4A, normals[j])) continue;
                            matchedList[i] = true;
                            matchedList[j] = true;
                            break;
                        }
                    }

                    return matchedList.All(matchedItem => matchedItem);
                }

        public static bool IsLinesParallel(Edge edgeA, Edge edgeB)
                {
                    var pointsA = edgeA.Tessellate() as List<XYZ>;
                    var pointsB = edgeB.Tessellate() as List<XYZ>;
                    if (pointsA == null || pointsB == null || pointsA.Count < 2 || pointsB.Count < 2)
                        return false;
                    return ParameterAccess.IsLinesParallel(
                        new Vector4(pointsA[1] - pointsA[0]),
                        new Vector4(pointsB[1] - pointsB[0]));
                }

        private static double AverageY(Face face)
                {
                    var mesh = face.Triangulate();
                    return mesh.Vertices.Sum(v => v.Y) / mesh.Vertices.Count;
                }

    }
}
