// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Color = System.Drawing.Color;
using Control = System.Windows.Forms.Control;

namespace Ara3D.RevitSampleBrowser.CreateBeamSystem.CS
{
    /// <summary>
    ///     Sketch the profile of beam system on canvas
    ///     Code here have nothing with Revit API
    /// </summary>
    public class BeamSystemSketch : ObjectSketch
    {
        /// <summary>
        ///     ratio of margin to canvas width
        /// </summary>
        private const float MarginRatio = 0.1f;

        /// <summary>
        ///     the control to draw beam system
        /// </summary>
        private readonly Control m_canvas;

        /// <summary>
        ///     defines a local geometric inverse transform
        /// </summary>
        private Matrix m_inverse;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="canvas">the control to draw beam system</param>
        public BeamSystemSketch(Control canvas)
        {
            m_canvas = canvas;
        }

        /// <summary>
        ///     draw the profile in the canvas
        /// </summary>
        /// <param name="profile">the profile of the beam system</param>
        public void DrawProfile(IList<Line> profile)
        {
            Initialize(profile);
            CalculateTransform();
            m_canvas.Paint += Paint;
            m_canvas.Refresh();
        }

        /// <summary>
        ///     draw beam system
        /// </summary>
        /// <param name="g">encapsulates a GDI+ drawing surface</param>
        /// <param name="translate">translation matrix to canvas coordinates</param>
        public override void Draw(Graphics g, Matrix translate)
        {
            foreach (LineSketch sketch in Objects)
            {
                sketch.Draw(g, Transform);
            }
        }

        /// <summary>
        ///     draw beam system on canvas control
        /// </summary>
        /// <param name="sender">canvas control</param>
        /// <param name="e">data for the Paint event</param>
        protected void Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            Draw(g, Transform);
        }

        /// <summary>
        ///     generate a Line2D instance using a Line's data
        /// </summary>
        /// <param name="line">where new Line2D get data</param>
        /// <returns>new Line2D</returns>
        private static Line2D GetLine2D(Line line)
        {
            var result = new Line2D
            {
                StartPnt = new PointF((float)line.GetEndPoint(0).X, (float)line.GetEndPoint(0).Y),
                EndPnt = new PointF((float)line.GetEndPoint(1).X, (float)line.GetEndPoint(1).Y)
            };
            return result;
        }

        /// <summary>
        ///     calculate the transform between canvas and geometry objects
        /// </summary>
        private void CalculateTransform()
        {
            var plgpts = CalculateCanvasRegion();
            Transform = new Matrix(BoundingBox, plgpts);
            m_inverse = Transform.Clone();

            if (m_inverse.IsInvertible) m_inverse.Invert();
        }

        /// <summary>
        ///     initialize geometry objects and bounding box
        /// </summary>
        /// <param name="profile">the profile of the beam system</param>
        private void Initialize(IList<Line> profile)
        {
            // deal with first line in profile
            Objects.Clear();
            var firstSketch = new LineSketch(GetLine2D(profile[0]));
            m_boundingBox = firstSketch.BoundingBox;
            firstSketch.IsDirection = true;
            Objects.Add(firstSketch);

            // all other lines
            for (var i = 1; i < profile.Count; i++)
            {
                var sketch = new LineSketch(GetLine2D(profile[i]));
                m_boundingBox = RectangleF.Union(BoundingBox, sketch.BoundingBox);
                Objects.Add(sketch);
            }
        }

        /// <summary>
        ///     get the display region, adjust the proportion and location
        /// </summary>
        /// <returns>upper-left, upper-right, and lower-left corners of the rectangle </returns>
        private PointF[] CalculateCanvasRegion()
        {
            // get the area without margin
            var realWidth = m_canvas.Width * (1 - 2 * MarginRatio);
            var realHeight = m_canvas.Height * (1 - 2 * MarginRatio);
            var minX = m_canvas.Width * MarginRatio;
            var minY = m_canvas.Height * MarginRatio;
            // ratio of width to height
            var originRate = m_boundingBox.Width / m_boundingBox.Height;
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
    }
}
