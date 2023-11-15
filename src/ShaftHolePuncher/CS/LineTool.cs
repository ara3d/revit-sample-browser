// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS
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
                graphic.DrawLine(BackGroundPen, m_points[m_points.Count - 1], PreMovePoint);
                PreMovePoint = e.Location;
                graphic.DrawLine(ForeGroundPen, m_points[m_points.Count - 1], e.Location);
            }
        }

        /// <summary>
        ///     record the location point where mouse clicked
        /// </summary>
        /// <param name="e">mouse event args</param>
        public override void OnMouseDown(MouseEventArgs e)
        {
            switch (e.Button)
            {
                //when user click right button of mouse,
                //finish the curve if the number of points is more than 2
                case MouseButtons.Right when m_points.Count > 2:
                    m_finished = true;
                    break;
                case MouseButtons.Left when !m_finished 
                                            && GetDistance(PreDownPoint, e.Location) > 2:
                    PreDownPoint = e.Location;
                    m_points.Add(e.Location);
                    break;
            }
        }

        /// <summary>
        ///     draw lines recorded in the tool
        /// </summary>
        /// <param name="graphic">Graphics object, use to draw geometry</param>
        public override void Draw(Graphics graphic)
        {
            for (var i = 0; i < m_points.Count - 1; i++)
                graphic.DrawLine(ForeGroundPen, m_points[i], m_points[i + 1]);

            //if user finished draw (clicked the right button), then close the curve
            if (m_finished) graphic.DrawLine(ForeGroundPen, m_points[0], m_points[m_points.Count - 1]);
        }
    }
}
