// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Control = System.Windows.Forms.Control;

namespace Ara3D.RevitSampleBrowser.CreateBeamSystem.CS
{
    public class BeamSystemSketch : ObjectSketch
    {
        private const float MarginRatio = 0.1f;

        private readonly Control m_canvas;

        private Matrix m_inverse;

        public BeamSystemSketch(Control canvas)
        {
            m_canvas = canvas;
        }

        public void DrawProfile(IList<Line> profile)
        {
            Initialize(profile);
            CalculateTransform();
            m_canvas.Paint += Paint;
            m_canvas.Refresh();
        }

        public override void Draw(Graphics g, Matrix translate)
        {
            foreach (LineSketch sketch in Objects)
            {
                sketch.Draw(g, Transform);
            }
        }

        protected void Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);
            Draw(g, Transform);
        }

        private static Line2D GetLine2D(Line line)
        {
            Line2D result = new()
            {
                StartPnt = new PointF((float)line.GetEndPoint(0).X, (float)line.GetEndPoint(0).Y),
                EndPnt = new PointF((float)line.GetEndPoint(1).X, (float)line.GetEndPoint(1).Y)
            };
            return result;
        }

        private void CalculateTransform()
        {
            var plgpts = CalculateCanvasRegion();
            Transform = new Matrix(BoundingBox, plgpts);
            m_inverse = Transform.Clone();

            if (m_inverse.IsInvertible) m_inverse.Invert();
        }

        private void Initialize(IList<Line> profile)
        {
            // deal with first line in profile
            Objects.Clear();
            LineSketch firstSketch = new(GetLine2D(profile[0]));
            m_boundingBox = firstSketch.BoundingBox;
            firstSketch.IsDirection = true;
            Objects.Add(firstSketch);

            // all other lines
            for (var i = 1; i < profile.Count; i++)
            {
                LineSketch sketch = new(GetLine2D(profile[i]));
                m_boundingBox = RectangleF.Union(BoundingBox, sketch.BoundingBox);
                Objects.Add(sketch);
            }
        }

        private PointF[] CalculateCanvasRegion()
        {
            var realWidth = m_canvas.Width * (1 - (2 * MarginRatio));
            var realHeight = m_canvas.Height * (1 - (2 * MarginRatio));
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
