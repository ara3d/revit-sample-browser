// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Drawing;
using System.Windows.Forms;

namespace Revit.SDK.Samples.ShaftHolePuncher.CS
{
    /// <summary>
    ///     tool used to draw line
    /// </summary>
    public class LineTool : Tool
    {
        /// <summary>
        ///     draw a line from end point of tool to the location where mouse moved
        /// </summary>
        /// <param name="graphic">graphic object,used to draw geometry</param>
        /// <param name="e">mouse event args</param>
        public override void OnMouseMove(Graphics graphic,
            MouseEventArgs e)
        {
            if (m_points.Count != 0 && !m_finished)
            {
                graphic.DrawLine(m_backGroundPen, m_points[m_points.Count - 1], m_preMovePoint);
                m_preMovePoint = e.Location;
                graphic.DrawLine(m_foreGroundPen, m_points[m_points.Count - 1], e.Location);
            }
        }

        /// <summary>
        ///     record the location point where mouse clicked
        /// </summary>
        /// <param name="e">mouse event args</param>
        public override void OnMouseDown(MouseEventArgs e)
        {
            //when user click right button of mouse,
            //finish the curve if the number of points is more than 2
            if (MouseButtons.Right == e.Button && m_points.Count > 2) m_finished = true;

            if (MouseButtons.Left == e.Button && !m_finished
                                              && GetDistance(m_preDownPoint, e.Location) > 2)
            {
                m_preDownPoint = e.Location;
                m_points.Add(e.Location);
            }
        }

        /// <summary>
        ///     draw lines recorded in the tool
        /// </summary>
        /// <param name="graphic">Graphics object, use to draw geometry</param>
        public override void Draw(Graphics graphic)
        {
            for (var i = 0; i < m_points.Count - 1; i++)
                graphic.DrawLine(m_foreGroundPen, m_points[i], m_points[i + 1]);

            //if user finished draw (clicked the right button), then close the curve
            if (m_finished) graphic.DrawLine(m_foreGroundPen, m_points[0], m_points[m_points.Count - 1]);
        }
    }
}
