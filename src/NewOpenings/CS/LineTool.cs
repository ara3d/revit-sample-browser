// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Revit.SDK.Samples.NewOpenings.CS
{
    /// <summary>
    ///     Tool used to draw line
    /// </summary>
    public class LineTool : ITool
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public LineTool()
        {
            m_type = ToolType.Line;
        }

        /// <summary>
        ///     Mouse move event handle
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnMouseMove(Graphics graphic, MouseEventArgs e)
        {
            if (m_points.Count != 0)
            {
                graphic.DrawLine(m_backGroundPen, m_points[m_points.Count - 1], m_preMovePoint);
                m_preMovePoint = e.Location;
                graphic.DrawLine(m_foreGroundPen, m_points[m_points.Count - 1], e.Location);
            }
        }

        /// <summary>
        ///     Mouse down event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnMouseDown(Graphics graphic, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_preMovePoint = e.Location;
                m_points.Add(e.Location);

                if (m_points.Count >= 2)
                    graphic.DrawLine(m_foreGroundPen, m_points[m_points.Count - 2],
                        m_points[m_points.Count - 1]);
            }
        }

        /// <summary>
        ///     Right mouse click handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to drawing geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnRightMouseClick(Graphics graphic, MouseEventArgs e)
        {
            if (MouseButtons.Right == e.Button && m_points.Count > 2)
            {
                var line = new List<Point>(m_points);
                m_lines.Add(line);

                graphic.DrawLine(m_foreGroundPen, m_points[m_points.Count - 1], m_points[0]);
                graphic.DrawLine(m_backGroundPen, m_points[m_points.Count - 1], m_preMovePoint);
                m_points.Clear();
            }
        }

        /// <summary>
        ///     Draw lines
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        public override void Draw(Graphics graphic)
        {
            foreach (var line in m_lines)
            {
                for (var i = 0; i < line.Count - 1; i++) graphic.DrawLine(m_foreGroundPen, line[i], line[i + 1]);
                //close the line
                graphic.DrawLine(m_foreGroundPen, line[line.Count - 1], line[0]);
            }
        }
    }
}
