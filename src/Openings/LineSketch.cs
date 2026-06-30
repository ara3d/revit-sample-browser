// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Drawing;
using System.Drawing.Drawing2D;

namespace Ara3D.RevitSampleBrowser.Openings.CS
{
    public class LineSketch : ObjectSketch
    {
        private readonly Line2D m_line = new(); // geometry line to draw

        public LineSketch(Line2D line)
        {
            m_line = line;
            BoundingBox = line.BoundingBox;
            Pen.Color = Color.Yellow;
            Pen.Width = 1f;
        }

        public override void Draw(Graphics g, Matrix translate)
        {
            Transform = translate;
            GraphicsPath path = new();
            path.AddLine(m_line.StartPnt, m_line.EndPnt);
            path.Transform(translate);
            g.DrawPath(Pen, path);
        }
    }
}
