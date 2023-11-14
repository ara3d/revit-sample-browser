// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Revit.SDK.Samples.NewOpenings.CS
{
    /// <summary>
    ///     Tool used to draw rectangle
    /// </summary>
    internal class RectTool : Tool
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public RectTool()
        {
            Type = ToolType.Rectangle;
        }

        /// <summary>
        ///     Mouse move event handler
        /// </summary>
        /// <param name="graphic">Graphics object,used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnMouseMove(Graphics graphic, MouseEventArgs e)
        {
            if (Points.Count == 1)
            {
                DrawRect(graphic, BackGroundPen, Points[0], PreMovePoint);
                PreMovePoint = e.Location;
                DrawRect(graphic, ForeGroundPen, Points[0], PreMovePoint);
            }
        }

        /// <summary>
        ///     Mouse down event handler
        /// </summary>
        /// <param name="graphic">Graphics object,used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnMouseDown(Graphics graphic, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PreMovePoint = e.Location;
                Points.Add(e.Location);

                if (Points.Count == 2) DrawRect(graphic, ForeGroundPen, Points[0], Points[1]);
            }

            ;
        }

        /// <summary>
        ///     Mouse up event handler
        /// </summary>
        /// <param name="graphic">Graphics object,used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnMouseUp(Graphics graphic, MouseEventArgs e)
        {
            if (Points.Count == 2)
            {
                var line = new List<Point>(Points);
                Lines.Add(line);
                Points.Clear();
            }
        }

        /// <summary>
        ///     Draw rectangles
        /// </summary>
        /// <param name="graphic">Graphics object,used to draw geometry </param>
        public override void Draw(Graphics graphic)
        {
            foreach (var line in Lines) DrawRect(graphic, ForeGroundPen, line[0], line[1]);
        }

        /// <summary>
        ///     Draw rectangle use the given two opposite point p1 and p2
        /// </summary>
        /// <param name="graphic">Graphics object,used to draw geometry</param>
        /// <param name="pen">Pen used to set color</param>
        /// <param name="p1">Rectangle one corner</param>
        /// <param name="p2">Opposite corner of p1</param>
        private void DrawRect(Graphics graphic, Pen pen, Point p1, Point p2)
        {
            var p = new Size(p2.X - p1.X, p2.Y - p1.Y);
            if (p.Width >= 0 && p.Height >= 0)
            {
                graphic.DrawRectangle(pen, p1.X, p1.Y, p.Width, p.Height);
            }
            //draw four lines
            else
            {
                var points = new Point[5]
                {
                    p1, new Point(p1.X, p2.Y),
                    p2, new Point(p2.X, p1.Y), p1
                };
                graphic.DrawLines(pen, points);
            }
        }
    }
}
