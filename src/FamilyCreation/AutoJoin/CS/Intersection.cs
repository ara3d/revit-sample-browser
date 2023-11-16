// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.FamilyCreation.AutoJoin.CS
{
    /// <summary>
    ///     Tell if two geometry objects are overlapping or not.
    /// </summary>
    internal class Intersection
    {
        /// <summary>
        ///     Tell if the geometry object A and B are overlapped.
        /// </summary>
        /// <param name="geometryA">geometry object A</param>
        /// <param name="geometryB">geometry object B</param>
        /// <returns>return true if A and B are overlapped, or else return false.</returns>
        public static bool IsOverlapped(GeometryObject geometryA, GeometryObject geometryB)
        {
            var facesOfA = new List<Face>();
            var curvesOfB = new List<Curve>();

            GetAllFaces(geometryA, facesOfA);
            GetAllCurves(geometryB, curvesOfB);

            foreach (var face in facesOfA)
            foreach (var curve in curvesOfB)
                if (face.Intersect(curve) == SetComparisonResult.Overlap)
                    return true;

            return false;
        }

        /// <summary>
        ///     Get all faces of the geometry object and insert them to the list
        /// </summary>
        /// <param name="geometry">the geometry object</param>
        /// <param name="faces">the face list</param>
        private static void GetAllFaces(GeometryObject geometry, List<Face> faces)
        {
            switch (geometry)
            {
                case GeometryElement element:
                    GetAllFaces(element, faces);
                    return;
                case Solid solid:
                    GetAllFaces(solid, faces);
                    break;
            }
        }

        private static void GetAllFaces(GeometryElement geoElement, List<Face> faces)
        {
            //foreach (GeometryObject geObject in geoElement.Objects)
            var objects = geoElement.GetEnumerator();
            while (objects.MoveNext())
            {
                var geObject = objects.Current;

                GetAllFaces(geObject, faces);
            }
        }

        private static void GetAllFaces(Solid solid, List<Face> faces)
        {
            foreach (Face face in solid.Faces) faces.Add(face);
        }

        private static void GetAllCurves(GeometryObject geometry, List<Curve> curves)
        {
            switch (geometry)
            {
                case GeometryElement element:
                    GetAllCurves(element, curves);
                    return;
                case Solid solid:
                    GetAllCurves(solid, curves);
                    break;
            }
        }

        private static void GetAllCurves(GeometryElement geoElement, List<Curve> curves)
        {
            //foreach (GeometryObject geObject in geoElement.Objects)
            var objects = geoElement.GetEnumerator();
            while (objects.MoveNext())
            {
                var geObject = objects.Current;

                GetAllCurves(geObject, curves);
            }
        }

        private static void GetAllCurves(Solid solid, List<Curve> curves)
        {
            foreach (Face face in solid.Faces) GetAllCurves(face, curves);
        }

        private static void GetAllCurves(Face face, List<Curve> curves)
        {
            foreach (EdgeArray loop in face.EdgeLoops)
            foreach (Edge edge in loop)
            {
                var points = edge.Tessellate() as List<XYZ>;
                for (var ii = 0; ii + 1 < points.Count; ii++)
                {
                    var line = Line.CreateBound(points[ii], points[ii + 1]);
                    curves.Add(line);
                }
            }
        }
    }
}
