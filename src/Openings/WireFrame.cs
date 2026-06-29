// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Ara3D.RevitSampleBrowser.Openings.CS
{
    /// <summary>
    ///     WireFrame class for generate the model lines and fit the picture box's size to display
    /// </summary>
    public class WireFrame : ObjectSketch
    {
        /// <summary>
        ///     ratio of margin to canvas width
        /// </summary>
        private const float Marginratio = 0.1f;

        //construct function
        /// <summary>
        ///     The default constructor
        /// </summary>
        /// <param name="line3Ds">a list contain all the line in WireFrame</param>
        public WireFrame(ReadOnlyCollection<Line3D> line3Ds)
        {
            Frame3DTo2D(line3Ds);
        }

        /// <summary>
        ///     draw the line contain in m_lines in 2d Preview
        /// </summary>
        /// <param name="previewWidth">Width of Preview</param>
        /// <param name="previewHeigh">Heigh of Preview</param>
        /// ///
        /// <param name="graphics">Graphics to draw</param>
        /// <returns></returns>
        public void Draw2D(float previewWidth, float previewHeigh, Graphics graphics)
        {
            graphics.Clear(Color.Black);
            CalculateTransform(previewWidth, previewHeigh);
            foreach (var sketch in Objects)
            {
                sketch.Draw(graphics, Transform);
            }
        }

        /// <summary>
        ///     draw override method
        /// </summary>
        /// <param name="g">graphics object</param>
        /// <param name="translate">matrix use to transform points or vectors</param>
        public override void Draw(Graphics g, Matrix translate)
        {
        }

        /// <summary>
        ///     calculate the transform between canvas and geometry objects
        /// </summary>
        private void CalculateTransform(float previewWidth, float previewHeigh)
        {
            var plgpts = CalculateCanvasRegion(previewWidth, previewHeigh);
            Transform = new Matrix(BoundingBox, plgpts);
        }

        /// <summary>
        ///     get the display region, adjust the proportion and location
        /// </summary>
        /// <returns>upper-left, upper-right, and lower-left corners of the rectangle </returns>
        private PointF[] CalculateCanvasRegion(float previewWidth, float previewHeigh)
        {
            // get the area without margin
            var realWidth = previewWidth * (1 - 2 * Marginratio);
            var realHeight = previewHeigh * (1 - 2 * Marginratio);
            var minX = previewWidth * Marginratio;
            var minY = previewHeigh * Marginratio;
            // ratio of width to height
            var originRate = BoundingBox.Width / BoundingBox.Height;
            var displayRate = realWidth / realHeight;

            if (originRate > displayRate)
            {
                // display area in canvas need move to center in height
                var goalHeight = realWidth / originRate;
                minY += (realHeight - goalHeight) / 2;
                realHeight = goalHeight;
            }
            else
            {
                // display area in canvas need move to center in width
                var goalWidth = realHeight * originRate;
                minX += (realWidth - goalWidth) / 2;
                realWidth = goalWidth;
            }

            var plgpts = new PointF[3];
            plgpts[0] = new PointF(minX, realHeight + minY); // upper-left point    
            plgpts[1] = new PointF(realWidth + minX, realHeight + minY); // upper-right point
            plgpts[2] = new PointF(minX, minY); // lower-left point

            return plgpts;
        }

        /// <summary>
        ///     transform 3d point to 2d (if all points in the same plane)
        /// </summary>
        private void Frame3DTo2D(ReadOnlyCollection<Line3D> line3Ds)
        {
            const double lengthEpsilon = 0.01;
            const double angleEpsilon = 0.1;
            // find 3 points to form 2 lines whose length is bigger than LengthEpsilon 
            // and angle between them should be bigger than AngleEpsilon 
            var line0 = line3Ds[0];
            var vector0 = new Vector();
            var vector1 = new Vector();
            // to find the first 2 points to form first line
            var index = 0;
            for (var i = 1; i < line3Ds.Count; i++)
            {
                vector0 = line3Ds[i].StartPoint - line0.StartPoint;
                if (vector0.GetLength() > lengthEpsilon)
                {
                    index = i;
                    break;
                }
            }

            if (index == 0) return;
            // to find the last points to form the second line
            for (var j = index + 1; j < line3Ds.Count; j++)
            {
                vector1 = line3Ds[j].StartPoint - line3Ds[index].StartPoint;
                var angle = Vector.GetAngleOf2Vectors(vector0, vector1, true);
                if (vector1.GetLength() > lengthEpsilon && angle > angleEpsilon) break;
            }

            // find the local coordinate system in which the profile of opening is horizontal
            var zAxis = (vector0 & vector1).GetNormal();
            var xAxis = zAxis & new Vector(0.0, 1.0, 0.0);
            var yAxis = zAxis & xAxis;
            var origin = new Vector(0.0, 0.0, 0.0);
            var ucs = new Ucs(origin, xAxis, yAxis);

            // transform all the 3D lines to UCS and create accordingly 2D lines
            var isFirst = true;
            foreach (var line in line3Ds)
            {
                var tmp = ucs.Gc2Lc(line);
                var startPnt = new PointF((float)tmp.StartPoint.X, (float)tmp.StartPoint.Y);
                var endPnt = new PointF((float)tmp.EndPoint.X, (float)tmp.EndPoint.Y);
                var line2D = new Line2D(startPnt, endPnt);
                var aLineSketch = new LineSketch(line2D);
                if (isFirst)
                {
                    BoundingBox = aLineSketch.BoundingBox;
                    isFirst = false;
                }
                else
                {
                    BoundingBox = RectangleF.Union(BoundingBox, aLineSketch.BoundingBox);
                }

                Objects.Add(aLineSketch);
            }
        }
    }
}
