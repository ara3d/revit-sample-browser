// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.NewOpenings.CS
{
    public class CircleTool : Tool
    {
        public CircleTool()
        {
            Type = ToolType.Circle;
        }

        public override void Draw(Graphics graphic)
        {
            foreach (var line in Lines)
            {
                DrawCircle(graphic, ForeGroundPen, line[0], line[1]);
            }
        }

        public override void OnMouseDown(Graphics graphic, MouseEventArgs e)
        {
            base.OnMouseDown(graphic, e);
            if (MouseButtons.Left == e.Button)
            {
                PreMovePoint = e.Location;
                Points.Add(e.Location);

                if (2 == Points.Count) DrawCircle(graphic, ForeGroundPen, Points[0], Points[1]);
            }
        }

        public override void OnMouseMove(Graphics graphic, MouseEventArgs e)
        {
            base.OnMouseMove(graphic, e);

            if (1 == Points.Count)
            {
                DrawCircle(graphic, BackGroundPen, Points[0], PreMovePoint);
                PreMovePoint = e.Location;
                DrawCircle(graphic, ForeGroundPen, Points[0], e.Location);
            }
        }

        public override void OnMouseUp(Graphics graphic, MouseEventArgs e)
        {
            base.OnMouseUp(graphic, e);

            if (2 == Points.Count)
            {
                List<Point> line = new(Points);
                Lines.Add(line);
                Points.Clear();
            }
        }

        private void DrawCircle(Graphics graphics, Pen pen, Point pCenter, Point pBound)
        {
            var radius = (int)Math.Sqrt(((pBound.X - pCenter.X) * (pBound.X - pCenter.X))
                                        + ((pBound.Y - pCenter.Y) * (pBound.Y - pCenter.Y)));
            Size radiusSize = new(radius, radius);
            var uperLeft = pCenter - radiusSize;
            graphics.DrawEllipse(pen, uperLeft.X, uperLeft.Y, 2 * radius, 2 * radius);
        }
    }
}
