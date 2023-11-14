// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Revit.SDK.Samples.NewOpenings.CS
{
    /// <summary>
    ///     Tool used to draw circle
    /// </summary>
    internal class CircleTool : ITool
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public CircleTool()
        {
            m_type = ToolType.Circle;
        }

        /// <summary>
        ///     Draw circles contained in the tool
        /// </summary>
        /// <param name="graphic"></param>
        public override void Draw(Graphics graphic)
        {
            foreach (var line in m_lines) DrawCircle(graphic, m_foreGroundPen, line[0], line[1]);
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
                m_preMovePoint = e.Location;
                m_points.Add(e.Location);

                if (2 == m_points.Count) DrawCircle(graphic, m_foreGroundPen, m_points[0], m_points[1]);
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

            if (1 == m_points.Count)
            {
                DrawCircle(graphic, m_backGroundPen, m_points[0], m_preMovePoint);
                m_preMovePoint = e.Location;
                DrawCircle(graphic, m_foreGroundPen, m_points[0], e.Location);
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

            if (2 == m_points.Count)
            {
                var line = new List<Point>(m_points);
                m_lines.Add(line);
                m_points.Clear();
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
