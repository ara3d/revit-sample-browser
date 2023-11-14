// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections;
using System.Drawing;

namespace Revit.SDK.Samples.SlabShapeEditing.CS
{
    /// <summary>
    ///     tool used to draw line
    /// </summary>
    internal class LineTool
    {
        private PointF m_movePoint; //record the coordinate of location where mouse just moved to. 

        /// <summary>
        ///     default constructor
        /// </summary>
        public LineTool()
        {
            Points = new ArrayList();
            m_movePoint = Point.Empty;
        }

        /// <summary>
        ///     Get all the points of this tool
        /// </summary>
        public ArrayList Points { get; set; }

        /// <summary>
        ///     Get coordinate of location where mouse just moved to.
        /// </summary>
        public PointF MovePoint
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
            for (var i = 0; i < Points.Count - 1; i += 2)
                graphics.DrawLine(pen, (PointF)Points[i], (PointF)Points[i + 1]);

            //draw the moving point
            if (!m_movePoint.IsEmpty)
                if (Points.Count >= 1)
                    graphics.DrawLine(pen, (PointF)Points[Points.Count - 1], m_movePoint);
        }

        /// <summary>
        ///     draw rectangle with specific graphics and pen
        /// </summary>
        /// <param name="graphics">Graphics object, used to draw geometry</param>
        /// <param name="pen">Pen which used to draw lines</param>
        public void DrawRectangle(Graphics graphics, Pen pen)
        {
            for (var i = 0; i < Points.Count - 1; i += 2)
            {
                var pointF = (PointF)Points[i];
                graphics.DrawRectangle(pen, pointF.X - 2, pointF.Y - 2, 4, 4);
            }
        }
    }
}
