// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Ara3D.RevitSampleBrowser.CreateBeamSystem.CS
{
    public class LineSketch : ObjectSketch
    {
        private const float DirectionTagDistanceRatio = 0.02f;

        private const float DirectionTagLengthRatio = 0.1f;

        private readonly Line2D m_line = new(); // geometry line to draw

        public LineSketch(Line2D line)
        {
            m_line = line;
            m_boundingBox = line.BoundingBox;
            Pen.Color = Color.DarkGreen;
            Pen.Width = 1f;
        }

        public bool IsDirection { get; set; }

        public override void Draw(Graphics g, Matrix translate)
        {
            GraphicsPath path = new();
            path.AddLine(m_line.StartPnt, m_line.EndPnt);

            if (IsDirection) DrawDirectionTag(path);

            path.Transform(translate);
            g.DrawPath(Pen, path);
        }

        private void DrawDirectionTag(GraphicsPath path)
        {
            var leftLine = m_line.Clone();
            var rightLine = m_line.Clone();
            leftLine.Scale(DirectionTagLengthRatio);
            leftLine.Shift(DirectionTagDistanceRatio * m_line.Length);
            rightLine.Scale(DirectionTagLengthRatio);
            rightLine.Shift(-DirectionTagDistanceRatio * m_line.Length);
            GraphicsPath leftPath = new();
            leftPath.AddLine(leftLine.StartPnt, leftLine.EndPnt);
            GraphicsPath rightPath = new();
            rightPath.AddLine(rightLine.StartPnt, rightLine.EndPnt);
            path.AddPath(leftPath, false);
            path.AddPath(rightPath, false);
        }
    }
}
