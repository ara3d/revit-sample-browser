// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Drawing;
using System.Windows.Forms;

namespace RevitMultiSample.ShaftHolePuncher.CS
{
    /// <summary>
    ///     tool used to draw line
    /// </summary>
    public class RectangleTool : Tool
    {
        /// <summary>
        ///     draw a line from end point of tool to the location where mouse move
        /// </summary>
        /// <param name="graphic">graphic object,used to draw geometry</param>
        /// <param name="e">mouse event args</param>
        public override void OnMouseMove(Graphics graphic,
            MouseEventArgs e)
        {
            if (1 == m_points.Count)
            {
                DrawRect(graphic, BackGroundPen, m_points[0], PreMovePoint);
                PreMovePoint = e.Location;
                DrawRect(graphic, ForeGroundPen, m_points[0], PreMovePoint);
            }
        }

        /// <summary>
        ///     record the location point where mouse clicked
        /// </summary>
        /// <param name="e">mouse event args</param>
        public override void OnMouseDown(MouseEventArgs e)
        {
            if (MouseButtons.Left == e.Button && !m_finished
                                              && GetDistance(PreDownPoint, e.Location) > 2)
            {
                PreDownPoint = e.Location;
                m_points.Add(e.Location);
                if (2 == m_points.Count) m_finished = true;
            }
        }

        /// <summary>
        ///     draw a rectangle
        /// </summary>
        /// <param name="graphic">Graphics object, use to draw geometry</param>
        public override void Draw(Graphics graphic)
        {
            if (2 == m_points.Count) DrawRect(graphic, ForeGroundPen, m_points[0], m_points[1]);
        }

        /// <summary>
        ///     draw rectangle use the given two points p1 and p2
        /// </summary>
        /// <param name="graphic">Graphics object,used to draw geometry</param>
        /// <param name="pen">Pen used to set color</param>
        /// <param name="p1">rectangle one corner</param>
        /// <param name="p2">opposite corner of p1</param>
        private void DrawRect(Graphics graphic, Pen pen, Point p1, Point p2)
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
