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
namespace Revit.SDK.Samples.CreateSimpleAreaRein.CS
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Forms;

    using Autodesk.Revit;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.Structure;

    using GeoElement = Autodesk.Revit.DB.GeometryElement;
    using GeoSolid = Autodesk.Revit.DB.Solid;
    using Element = Autodesk.Revit.DB.Element;

    /// <summary>
    /// provide some common geometry judgement and calculate method
    /// </summary>
    class GeomUtil
    {
        const double PRECISION = 0.00001;    //precision when judge whether two doubles are equal

        /// <summary>
        /// get all faces that compose the geometry solid of given element
        /// </summary>
        /// <param name="elem">element to be calculated</param>
        /// <returns>all faces</returns>
        public static FaceArray GetFaces(Element elem)
        {
            var faces = new List<Face>();

            var geoOptions =
                Command.CommandData.Application.Application.Create.NewGeometryOptions();
            geoOptions.ComputeReferences = true;

            var geoElem = elem.get_Geometry(geoOptions);
            //GeometryObjectArray geoElems = geoElem.Objects;
            var Objects = geoElem.GetEnumerator();
            //foreach (object o in geoElems)
            while (Objects.MoveNext())
            {
                object o = Objects.Current;

                var geoSolid = o as GeoSolid;
                if (null == geoSolid)
                {
                    continue;
                }

                return geoSolid.Faces;
            }

            return null;
        }

        /// <summary>
        /// get all points proximate to the given face
        /// </summary>
        /// <param name="face">face to be calculated</param>
        /// <returns></returns>
        public static List<Autodesk.Revit.DB.XYZ> GetPoints(Face face)
        {
            var points = new List<Autodesk.Revit.DB.XYZ>();
            var XYZs = face.Triangulate().Vertices as List<Autodesk.Revit.DB.XYZ>;

            foreach (var point in XYZs)
            {
                points.Add(point);
            }

            return points;
        }

        /// <summary>
        /// judge whether the given face is horizontal
        /// </summary>
        /// <param name="face">face to be judged</param>
        /// <returns>is horizontal</returns>
        public static bool IsHorizontalFace(Face face)
        {
            var points = GetPoints(face);
            var z1 = points[0].Z;
            var z2 = points[1].Z;
            var z3 = points[2].Z;
            var z4 = points[3].Z;
            var flag = IsEqual(z1, z2);
            flag = flag && IsEqual(z2, z3);
            flag = flag && IsEqual(z3, z4);
            flag = flag && IsEqual(z4, z1);

            return flag;
        }

        /// <summary>
        /// judge whether a face and a line are parallel
        /// </summary>
        /// <param name="face"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsParallel(Face face, Line line)
        {
            var points = GetPoints(face);
            var vector1 = SubXYZ(points[0], points[1]);
            var vector2 = SubXYZ(points[1], points[2]);
            var refer = SubXYZ(line.GetEndPoint(0), line.GetEndPoint(1));

            var cross = CrossMatrix(vector1, vector2);
            var result = DotMatrix(cross, refer);

            if (result < PRECISION)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// judge whether given 4 lines can form a rectangular
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>is rectangular</returns>
        public static bool IsRectangular(IList<Curve> curves)
      {
         //make sure the CurveArray contains 4 line
         if (curves.Count != 4)
         {
            return false;
         }
         var lines = new Line[4];
         for (var i = 0; i < 4; i++)
         {
            lines[i] = curves[i] as Line;
            if (null == lines[i])
            {
               return false;
            }
         }

         //make sure the first line is vertical to 2 lines and parallel to another line
         var iniLine = lines[0];
         var verticalLines = new Line[2];
         Line paraLine = null;
         var index = 0;
         for (var i = 1; i < 4; i++)
         {
            if (IsVertical(lines[0], lines[i]))
            {
               verticalLines[index] = lines[i];
               index++;
            }
            else
            {
               paraLine = lines[i];
            }
         }
         if (index != 2)
         {
            return false;
         }
         var flag = IsVertical(paraLine, verticalLines[0]);
         return flag;
      }

        /// <summary>
        /// judge whether two lines are vertical
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        private static bool IsVertical(Line line1, Line line2)
        {
            var vector1 = SubXYZ(line1.GetEndPoint(0), line1.GetEndPoint(1));
            var vector2 = SubXYZ(line2.GetEndPoint(0), line2.GetEndPoint(1));

            var result = DotMatrix(vector1, vector2);

            if (Math.Abs(result) < PRECISION)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// subtraction of two Autodesk.Revit.DB.XYZ as Matrix
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static Autodesk.Revit.DB.XYZ SubXYZ(Autodesk.Revit.DB.XYZ p1, Autodesk.Revit.DB.XYZ p2)
        {
            var x = p1.X - p2.X;
            var y = p1.Y - p2.Y;
            var z = p1.Z - p2.Z;

            var result = new Autodesk.Revit.DB.XYZ(x, y, z);
            return result;
        }

        /// <summary>
        /// multiplication cross of two Autodesk.Revit.DB.XYZ as Matrix
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static Autodesk.Revit.DB.XYZ CrossMatrix(Autodesk.Revit.DB.XYZ p1, Autodesk.Revit.DB.XYZ p2)
        {
            var v1 = p1.X;
            var v2 = p1.Y;
            var v3 = p1.Z;

            var u1 = p2.X;
            var u2 = p2.Y;
            var u3 = p2.Z;

            var x = v3 * u2 - v2 * u3;
            var y = -v3 * u1 + v1 * u3;
            var z = v2 * u1 - v1 * u2;

            var point = new Autodesk.Revit.DB.XYZ(x, y, z);
            return point;
        }

        /// <summary>
        /// dot product of two Autodesk.Revit.DB.XYZ as Matrix
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static double DotMatrix(Autodesk.Revit.DB.XYZ p1, Autodesk.Revit.DB.XYZ p2)
        {
            var v1 = p1.X;
            var v2 = p1.Y;
            var v3 = p1.Z;

            var u1 = p2.X;
            var u2 = p2.Y;
            var u3 = p2.Z;

            var result = v1 * u1 + v2 * u2 + v3 * u3;

            return result;
        }

        /// <summary>
        /// judge whether the subtraction of two doubles is less than 
        /// the internal decided precision
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        private static bool IsEqual(double d1, double d2)
        {
            var diff = Math.Abs(d1 - d2);
            if (diff < PRECISION)
            {
                return true;
            }
            return false;
        }
    }
}
