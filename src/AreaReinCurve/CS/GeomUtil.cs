// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.AreaReinCurve.CS
{
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

            return Math.Abs(result) < PRECISION;
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
            return diff < PRECISION;
        }
    }
}
