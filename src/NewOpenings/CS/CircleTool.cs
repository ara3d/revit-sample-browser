// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RevitMultiSample.NewOpenings.CS
{
    /// <summary>
    ///     Tool used to draw circle
    /// </summary>
    internal class CircleTool : Tool
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public CircleTool()
        {
            Type = ToolType.Circle;
        }

        /// <summary>
        ///     Draw circles contained in the tool
        /// </summary>
        /// <param name="graphic"></param>
        public override void Draw(Graphics graphic)
        {
            foreach (var line in Lines) DrawCircle(graphic, ForeGroundPen, line[0], line[1]);
        }

        /// <summary>
        ///     Mouse down event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
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

        /// <summary>
        ///     Mouse move event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
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

        /// <summary>
        ///     Mouse up event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnMouseUp(Graphics graphic, MouseEventArgs e)
        {
            base.OnMouseUp(graphic, e);

            if (2 == Points.Count)
            {
                var line = new List<Point>(Points);
                Lines.Add(line);
                Points.Clear();
            }
        }

        /// <summary>
        ///     Draw circle with center and one point on circle
        /// </summary>
        /// <param name="graphics">Graphics object, used  to draw geometry</param>
        /// <param name="pen">Pen used to set drawing color</param>
        /// <param name="pCenter">Circle center</param>
        /// <param name="pBound">One point on circle</param>
        private void DrawCircle(Graphics graphics, Pen pen, Point pCenter, Point pBound)
        {
            var radius = (int)Math.Sqrt((pBound.X - pCenter.X) * (pBound.X - pCenter.X)
                                        + (pBound.Y - pCenter.Y) * (pBound.Y - pCenter.Y));
            var radiusSize = new Size(radius, radius);
            var uperLeft = pCenter - radiusSize;
            graphics.DrawEllipse(pen, uperLeft.X, uperLeft.Y, 2 * radius, 2 * radius);
        }
    }
}
