// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System.Drawing;
using System.Drawing.Drawing2D;

namespace Revit.SDK.Samples.Openings.CS
{
    /// <summary>
    ///     sketch line and any tag on it
    /// </summary>
    public class LineSketch : ObjectSketch
    {
        private readonly Line2D m_line = new Line2D(); // geometry line to draw

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="line"></param>
        public LineSketch(Line2D line)
        {
            m_line = line;
            m_boundingBox = line.BoundingBox;
            m_pen.Color = Color.Yellow;
            m_pen.Width = 1f;
        }

        /// <summary>
        ///     draw the line
        /// </summary>
        /// <param name="g">drawing object</param>
        /// <param name="translate">translation between drawn sketch and geometry object</param>
        public override void Draw(Graphics g, Matrix translate)
        {
            m_transform = translate;
            var path = new GraphicsPath();
            path.AddLine(m_line.StartPnt, m_line.EndPnt);
            path.Transform(translate);
            g.DrawPath(m_pen, path);
        }
    }
}
