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

using System;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.AreaReinCurve.CS
{
    using GeoElement = GeometryElement;

    /// <summary>
    ///     provide some common geometry judgement and calculate method
    /// </summary>
    internal class GeomUtil
    {
        private const double PRECISION = 0.00001; //precision when judge whether two doubles are equal

        /// <summary>
        ///     judge whether given 4 lines can form a rectangular
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>is rectangular</returns>
        /// <summary>
        ///     judge whether given 4 lines can form a rectangular
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>is rectangular</returns>
        public static bool IsRectangular(CurveArray curves)
        {
            if (curves.Size != 4) return false;

            var lines = new Line[4];
            for (var i = 0; i < 4; i++)
            {
                lines[i] = curves.get_Item(i) as Line;
                if (null == lines[i]) return false;
            }

            var verticalLines = new Line[2];
            Line paraLine = null;
            var index = 0;
            for (var i = 1; i < 4; i++)
                if (IsVertical(lines[0], lines[i]))
                {
                    verticalLines[index] = lines[i];
                    index++;
                }
                else
                {
                    paraLine = lines[i];
                }

            if (index != 2) return false;
            var flag = IsVertical(paraLine, verticalLines[0]);
            return flag;
        }

        /// <summary>
        ///     judge whether two lines are vertical
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static bool IsVertical(Line line1, Line line2)
        {
            var vector1 = SubXYZ(line1.GetEndPoint(0), line1.GetEndPoint(1));
            var vector2 = SubXYZ(line2.GetEndPoint(0), line2.GetEndPoint(1));

            var result = DotMatrix(vector1, vector2);

            if (Math.Abs(result) < PRECISION) return true;
            return false;
        }

        /// <summary>
        ///     subtraction of two Autodesk.Revit.DB.XYZ as Matrix
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static XYZ SubXYZ(XYZ p1, XYZ p2)
        {
            var x = p1.X - p2.X;
            var y = p1.Y - p2.Y;
            var z = p1.Z - p2.Z;

            var result = new XYZ(x, y, z);
            return result;
        }

        /// <summary>
        ///     dot product of two Autodesk.Revit.DB.XYZ as Matrix
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static double DotMatrix(XYZ p1, XYZ p2)
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
        ///     judge whether the subtraction of two doubles is less than the internal decided precision
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        private static bool IsEqual(double d1, double d2)
        {
            var diff = Math.Abs(d1 - d2);
            if (diff < PRECISION) return true;
            return false;
        }
    }
}