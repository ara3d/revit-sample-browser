// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Revit.SDK.Samples.CreateBeamSystem.CS
{
    /// <summary>
    ///     sketch line and any tag on it
    /// </summary>
    public class LineSketch : ObjectSketch
    {
        /// <summary>
        ///     the rate of direction tag's distance to the line
        /// </summary>
        private const float DirectionTag_Distance_Ratio = 0.02f;

        /// <summary>
        ///     the rate of direction tag's length to the line
        /// </summary>
        private const float DirectionTag_Length_Ratio = 0.1f;

        private readonly Line2D m_line = new Line2D(); // geometry line to draw

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="line"></param>
        public LineSketch(Line2D line)
        {
            m_line = line;
            m_boundingBox = line.BoundingBox;
            m_pen.Color = Color.DarkGreen;
            m_pen.Width = 1f;
        }

        /// <summary>
        ///     whether has direction tag
        /// </summary>
        public bool IsDirection { get; set; }

        /// <summary>
        ///     draw the line
        /// </summary>
        /// <param name="g">drawing object</param>
        /// <param name="translate">translation between drawn sketch and geometry object</param>
        public override void Draw(Graphics g, Matrix translate)
        {
            var path = new GraphicsPath();
            path.AddLine(m_line.StartPnt, m_line.EndPnt);

            if (IsDirection) DrawDirectionTag(path);

            path.Transform(translate);
            g.DrawPath(m_pen, path);
        }

        /// <summary>
        ///     draw 2 shorter parallel lines on each side of the line
        /// </summary>
        /// <param name="path"></param>
        private void DrawDirectionTag(GraphicsPath path)
        {
            var leftLine = m_line.Clone();
            var rightLine = m_line.Clone();
            leftLine.Scale(DirectionTag_Length_Ratio);
            leftLine.Shift(DirectionTag_Distance_Ratio * m_line.Length);
            rightLine.Scale(DirectionTag_Length_Ratio);
            rightLine.Shift(-DirectionTag_Distance_Ratio * m_line.Length);
            var leftPath = new GraphicsPath();
            leftPath.AddLine(leftLine.StartPnt, leftLine.EndPnt);
            var rightPath = new GraphicsPath();
            rightPath.AddLine(rightLine.StartPnt, rightLine.EndPnt);
            path.AddPath(leftPath, false);
            path.AddPath(rightPath, false);
        }
    }
}
