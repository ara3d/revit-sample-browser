//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM 'AS IS' AND WITH ALL ITS FAULTS.
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Autodesk.Revit.DB;
using Color = System.Drawing.Color;

namespace Revit.SDK.Samples.FoundationSlab.CS
{
    /// <summary>
    ///     An internal class for drawing slabs' profiles.
    ///     This class is intended to contain only static methods.
    /// </summary>
    internal class Sketch
    {
        // A private constructor to prevent the compiler from generating a default constructor.
        private Sketch()
        {
        }

        /// <summary>
        ///     Draw profiles.
        /// </summary>
        /// <param name="graphic">The object of Graphics to draw profiles.</param>
        /// <param name="rclip">The rectangle area to draw profiles.</param>
        /// <param name="baseSlabList">A set of base floors' datas containing profiles.</param>
        public static void DrawProfile(Graphics graphic, RectangleF rclip, Collection<RegularSlab> baseSlabList)
        {
            var maxBBox = GetMaxBBox(baseSlabList); // Get the max bounding box's rectangle area.
            var matrix = GetTransformMatrix(rclip, maxBBox); // Get the transform matrix.
            if (null == matrix)
                return;

            graphic.Clear(Color.Black); // Clear the object of graphics.
            graphic.Transform = matrix; // Transform the object of graphics.
            graphic.SmoothingMode = SmoothingMode.HighQuality; // Smooth it.

            // Two pens for drawing profiles.
            var yellowPen = new Pen(Color.Yellow, (float)0.05); // For floors' profiles.
            var greenPen = new Pen(Color.Green, (float)0.2); // For octagonal profiles.

            // Draw profiles.
            foreach (var slab in baseSlabList)
            {
                if (null != slab.Profile) DrawLine(yellowPen, graphic, slab.Profile); // Draw floor's profiles.
                if (slab.Selected) DrawLine(greenPen, graphic, slab.OctagonalProfile); // Draw octagonal profiles.
            }

            // Dispose pen and matrix.
            yellowPen.Dispose();
            greenPen.Dispose();
            matrix.Dispose();
        }

        /// <summary>
        ///     Draw Lines.
        /// </summary>
        /// <param name="pen">The pen to draw lines.</param>
        /// <param name="graphic">The object of graphics to draw lines.</param>
        /// <param name="curveArray">A set contains lines.</param>
        private static void DrawLine(Pen pen, Graphics graphic, CurveArray curveArray)
        {
            foreach (Curve curve in curveArray)
            {
                var line = curve as Line; // Draw one line.
                if (null != line)
                {
                    var startPoint = new PointF((float)line.GetEndPoint(0).X, (float)line.GetEndPoint(0).Y);
                    var endPoint = new PointF((float)line.GetEndPoint(1).X, (float)line.GetEndPoint(1).Y);
                    graphic.DrawLine(pen, startPoint, endPoint);
                    continue;
                }

                var xyzArray = curve.Tessellate() as List<XYZ>; // Draw lines which form one arc.
                for (var i = 0; i < xyzArray.Count - 1; i++)
                {
                    var startPoint = new PointF((float)xyzArray[i].X, (float)xyzArray[i].Y);
                    var endPoint = new PointF((float)xyzArray[i + 1].X, (float)xyzArray[i + 1].Y);
                    graphic.DrawLine(pen, startPoint, endPoint);
                }
            }
        }

        /// <summary>
        ///     Get transform matrix.
        /// </summary>
        /// <param name="rclip">The rectangle area to draw profiles.</param>
        /// <param name="rBox">the rectangle area of the all the floors' max bounding box.</param>
        /// <returns>The transform matrix.</returns>
        private static Matrix GetTransformMatrix(RectangleF rclip, RectangleF rBox)
        {
            try
            {
                var rdraw = rclip;

                // Calculate the draw area according to the size of the sketch:
                // Adjust the shrink to change borders
                float shrink = (float)0.15, shrinked = (float)1.0 - 2 * shrink;
                if (rBox.Width * rclip.Height > rBox.Height * rclip.Width)
                    rdraw.Inflate(-rclip.Width * shrink,
                        (rclip.Width * shrinked * rBox.Height / rBox.Width - rclip.Height) / 2);
                else
                    rdraw.Inflate((rclip.Height * shrinked * rBox.Width / rBox.Height - rclip.Width) / 2,
                        -rclip.Height * shrink);

                // Mapping the point in sketch to point in draw area:
                var plgpts = new PointF[3];
                plgpts[0].X = rdraw.Left;
                plgpts[0].Y = rdraw.Bottom;
                plgpts[1].X = rdraw.Right;
                plgpts[1].Y = rdraw.Bottom;
                plgpts[2].X = rdraw.Left;
                plgpts[2].Y = rdraw.Top;

                // Get the transform matrix and return.
                return new Matrix(rBox, plgpts);
            }
            catch (ArithmeticException)
            {
                return null;
            }
            catch (OutOfMemoryException)
            {
                return null;
            }
        }

        /// <summary>
        ///     Get the max bounding box of all floors.
        /// </summary>
        /// <param name="baseSlabList">A set of base floors' datas containing bounding box.</param>
        /// <returns>The rectangle area of all the base floors' max bounding box.</returns>
        private static RectangleF GetMaxBBox(Collection<RegularSlab> baseSlabList)
        {
            var count = 1;
            var union = new RectangleF();
            foreach (var slab in baseSlabList)
            {
                var x = (float)slab.BBox.Min.X;
                var y = (float)slab.BBox.Min.Y;
                var width = (float)(slab.BBox.Max.X - slab.BBox.Min.X);
                var height = (float)(slab.BBox.Max.Y - slab.BBox.Min.Y);
                var slabBox = new RectangleF(x, y, width, height); // Rectangle area of each floor.
                if (1 == count)
                    union = slabBox;
                else
                    union = RectangleF.Union(union, slabBox); // The union of all the floors' rectangle areas.
                count++;
            }

            return union;
        }
    }
}