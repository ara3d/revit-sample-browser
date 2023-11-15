// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections;
using System.Drawing;

namespace Ara3D.RevitSampleBrowser.Truss.CS
{
    /// <summary>
    ///     tool used to draw line
    /// </summary>
    internal class LineTool
    {
        private Point m_movePoint; //record the coordinate of location where mouse just moved to. 

        /// <summary>
        ///     default constructor
        /// </summary>
        public LineTool()
        {
            Points = new ArrayList();
        }

        /// <summary>
        ///     Get all the points of this tool
        /// </summary>
        public ArrayList Points { get; set; }

        /// <summary>
        ///     Get coordinate of location where mouse just moved to.
        /// </summary>
        public Point MovePoint
        {
            get => m_movePoint;
            set => m_movePoint = value;
        }

        /// <summary>
        ///     draw the stored lines
        /// </summary>
        /// <param name="graphics">Graphics object, used to draw geometry</param>
        /// <param name="pen">Pen which used to draw lines</param>
        public void Draw2D(Graphics graphics, Pen pen)
        {
            for (var i = 0; i < Points.Count - 1; i++) graphics.DrawLine(pen, (Point)Points[i], (Point)Points[i + 1]);

            //draw the moving point
            if (!m_movePoint.IsEmpty)
                if (Points.Count >= 1)
                    graphics.DrawLine(pen, (Point)Points[Points.Count - 1], m_movePoint);
        }
    }
}
