// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.Common.Structural
{
    public static class RebarGeometry
    {
        public static bool IsVertical(Face face, Line line, Transform faceTrans, Transform lineTrans)
        {
            if (face.Triangulate().Vertices is not List<XYZ> points || points.Count < 3)
                return false;

            var first = points[0];
            var second = points[1];
            var third = points[2];
            var lineStart = line.GetEndPoint(0);
            var lineEnd = line.GetEndPoint(1);

            if (faceTrans != null)
            {
                first = XyzMath.TransformPoint(first, faceTrans);
                second = XyzMath.TransformPoint(second, faceTrans);
                third = XyzMath.TransformPoint(third, faceTrans);
            }

            if (lineTrans != null)
            {
                lineStart = XyzMath.TransformPoint(lineStart, lineTrans);
                lineEnd = XyzMath.TransformPoint(lineEnd, lineTrans);
            }

            var vector1 = XyzMath.SubXyz(first, second);
            var vector2 = XyzMath.SubXyz(first, third);
            var vector3 = XyzMath.SubXyz(lineStart, lineEnd);
            return XyzMath.IsEqual(XyzMath.DotMatrix(vector1, vector3), 0) &&
                   XyzMath.IsEqual(XyzMath.DotMatrix(vector2, vector3), 0);
        }

        public static bool IsVertical(Line line1, Line line2)
        {
            return Math.Abs(XyzMath.DotMatrix(
                XyzMath.SubXyz(line1.GetEndPoint(0), line1.GetEndPoint(1)),
                XyzMath.SubXyz(line2.GetEndPoint(0), line2.GetEndPoint(1)))) < XyzMath.Precision;
        }

        public static RebarHookOrientation GetHookOrient(XYZ curveVec, XYZ normal, XYZ hookVec)
        {
            var tempVec = normal;

            for (var i = 0; i < 4; i++)
            {
                tempVec = XyzMath.CrossMatrix(tempVec, curveVec);
                if (XyzMath.IsSameDirection(tempVec, hookVec))
                {
                    switch (i)
                    {
                        case 0: return RebarHookOrientation.Right;
                        case 2: return RebarHookOrientation.Left;
                    }
                }
            }

            throw new Exception("Can't find the hook orient according to hook direction.");
        }

        public static bool IsInRightDir(XYZ normal)
        {
            var eps = 1.0e-8;
            if (Math.Abs(normal.X) <= eps)
            {
                return normal.Y <= 0;
            }

            return normal.X is > 0 or >= 0;
        }
    }
}
