// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Drawing;

namespace Revit.SDK.Samples.Openings.CS
{
    /// <summary>
    ///     represent a geometry segment line
    /// </summary>
    public class Line2D
    {
        private PointF m_endPnt; // end point

        private float m_length; // length of the line

        // normal of the line; start point to end point
        private PointF m_normal;
        private PointF m_startPnt; // start point

        /// <summary>
        ///     constructor
        ///     default StartPoint = (0.0, 0.0), EndPoint = (1.0, 0.0)
        /// </summary>
        public Line2D()
        {
            m_startPnt.X = 0.0f;
            m_startPnt.Y = 0.0f;
            m_endPnt.X = 1.0f;
            m_endPnt.Y = 0.0f;
            CalculateDirection();
            CalculateBoundingBox();
        }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="startPnt">StartPoint</param>
        /// <param name="endPnt">EndPoint</param>
        public Line2D(PointF startPnt, PointF endPnt)
        {
            m_startPnt = startPnt;
            m_endPnt = endPnt;
            CalculateDirection();
            CalculateBoundingBox();
        }

        /// <summary>
        ///     rectangle box contains the line
        /// </summary>
        public RectangleF BoundingBox { get; private set; }

        /// <summary>
        ///     start point of the line; if it is set to new value,
        ///     EndPoint is changeless; Length, Normal and BoundingBox will updated
        /// </summary>
        public PointF StartPnt
        {
            get => m_startPnt;
            set
            {
                if (m_startPnt == value) return;
                m_startPnt = value;
                CalculateDirection();
                CalculateBoundingBox();
            }
        }

        /// <summary>
        ///     end point of the line; if it is set to new value,
        ///     StartPoint is changeless; Length, Normal and BoundingBox will updated
        /// </summary>
        public PointF EndPnt
        {
            get => m_endPnt;
            set
            {
                if (m_endPnt == value) return;
                m_endPnt = value;
                CalculateDirection();
                CalculateBoundingBox();
            }
        }

        /// <summary>
        ///     Length of the line; if it is set to new value,
        ///     StartPoint and Normal is changeless; EndPoint and BoundingBox will updated
        /// </summary>
        public float Length
        {
            get => m_length;
            set
            {
                if (m_length == value) return;
                m_length = value;
                CalculateEndPoint();
                CalculateBoundingBox();
            }
        }

        /// <summary>
        ///     Normal of the line; if it is set to new value,
        ///     StartPoint is changeless; EndPoint and BoundingBox will updated
        /// </summary>
        public PointF Normal
        {
            get => m_normal;
            set
            {
                if (m_normal == value) return;
                m_normal = value;
                CalculateEndPoint();
                CalculateBoundingBox();
            }
        }

        /// <summary>
        ///     calculate BoundingBox according to StartPoint and EndPoint
        /// </summary>
        private void CalculateBoundingBox()
        {
            var x1 = m_endPnt.X;
            var x2 = m_startPnt.X;
            var y1 = m_endPnt.Y;
            var y2 = m_startPnt.Y;

            var width = Math.Abs(x1 - x2);
            var height = Math.Abs(y1 - y2);

            if (x1 > x2) x1 = x2;
            if (y1 > y2) y1 = y2;
            BoundingBox = new RectangleF(x1, y1, width, height);
        }

        /// <summary>
        ///     calculate length by StartPoint and EndPoint
        /// </summary>
        private void CalculateLength()
        {
            m_length =
                (float)Math.Sqrt(Math.Pow(m_startPnt.X - m_endPnt.X, 2) +
                                 Math.Pow(m_startPnt.Y - m_endPnt.Y, 2));
        }

        /// <summary>
        ///     calculate Direction by StartPoint and EndPoint
        /// </summary>
        private void CalculateDirection()
        {
            CalculateLength();
            m_normal.X = (m_endPnt.X - m_startPnt.X) / m_length;
            m_normal.Y = (m_endPnt.Y - m_startPnt.Y) / m_length;
        }

        /// <summary>
        ///     calculate EndPoint by StartPoint, Length and Direction
        /// </summary>
        private void CalculateEndPoint()
        {
            m_endPnt.X = m_startPnt.X + m_length * m_normal.X;
            m_endPnt.Y = m_startPnt.Y + m_length * m_normal.Y;
        }
    }
}
