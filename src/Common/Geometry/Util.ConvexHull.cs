// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Convex Hull

        /// <summary>
        ///     Return the convex hull of a list of points
        ///     using the Jarvis march or Gift wrapping:
        ///     https://en.wikipedia.org/wiki/Gift_wrapping_algorithm
        ///     Written by Maxence.
        /// </summary>
        public static List<XYZ> ConvexHull(List<XYZ> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
            var startPoint = points.MinBy(p => p.X);
            var convexHullPoints = new List<XYZ>();
            var walkingPoint = startPoint;
            var refVector = XYZ.BasisY.Negate();
            do
            {
                convexHullPoints.Add(walkingPoint);
                var wp = walkingPoint;
                var rv = refVector;
                walkingPoint = points.MinBy(p =>
                {
                    var angle = (p - wp).AngleOnPlaneTo(rv, XYZ.BasisZ);
                    if (angle < 1e-10)
                        angle = 2 * Math.PI;
                    return angle;
                });
                refVector = wp - walkingPoint;
            } while (walkingPoint != startPoint);

            convexHullPoints.Reverse();
            return convexHullPoints;
        }

        #endregion
    }
}
