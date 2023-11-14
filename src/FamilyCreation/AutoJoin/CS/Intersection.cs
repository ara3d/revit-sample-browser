//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System.Collections.Generic;
using Autodesk.Revit.DB;
using AppCreation = Autodesk.Revit.Creation.Application;

namespace Revit.SDK.Samples.AutoJoin.CS
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
            if (geometry is GeometryElement)
            {
                GetAllFaces(geometry as GeometryElement, faces);
                return;
            }

            if (geometry is Solid)
            {
                GetAllFaces(geometry as Solid, faces);
            }
        }

        private static void GetAllFaces(GeometryElement geoElement, List<Face> faces)
        {
            //foreach (GeometryObject geObject in geoElement.Objects)
            var Objects = geoElement.GetEnumerator();
            while (Objects.MoveNext())
            {
                var geObject = Objects.Current;

                GetAllFaces(geObject, faces);
            }
        }

        private static void GetAllFaces(Solid solid, List<Face> faces)
        {
            foreach (Face face in solid.Faces) faces.Add(face);
        }

        private static void GetAllCurves(GeometryObject geometry, List<Curve> curves)
        {
            if (geometry is GeometryElement)
            {
                GetAllCurves(geometry as GeometryElement, curves);
                return;
            }

            if (geometry is Solid)
            {
                GetAllCurves(geometry as Solid, curves);
            }
        }

        private static void GetAllCurves(GeometryElement geoElement, List<Curve> curves)
        {
            //foreach (GeometryObject geObject in geoElement.Objects)
            var Objects = geoElement.GetEnumerator();
            while (Objects.MoveNext())
            {
                var geObject = Objects.Current;

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