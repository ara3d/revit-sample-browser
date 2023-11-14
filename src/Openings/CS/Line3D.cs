// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


namespace Revit.SDK.Samples.Openings.CS
{
    /// <summary>
    ///     Line class use to store information about line(include startPoint and endPoint)
    ///     and get the value via (startPoint, endPoint)property
    /// </summary>
    public class Line3D
    {
        private Vector m_endPnt; //end point
        private double m_length; //length of line
        private Vector m_normal; //normal
        private Vector m_startPnt; //start point

        /// <summary>
        ///     The default constructor
        /// </summary>
        public Line3D()
        {
            m_startPnt = new Vector(0.0, 0.0, 0.0);
            m_endPnt = new Vector(1.0, 0.0, 0.0);
            m_length = 1.0;
            m_normal = new Vector(1.0, 0.0, 0.0);
        }

        /// <summary>
        ///     The default constructor
        /// </summary>
        /// <param name="startPnt">start point of line</param>
        /// <param name="endPnt">enn point of line</param>
        public Line3D(Vector startPnt, Vector endPnt)
        {
            m_startPnt = startPnt;
            m_endPnt = endPnt;
            CalculateDirection();
        }

        //property
        /// <summary>
        ///     Property to get and set length of line
        /// </summary>
        public double Length
        {
            get => m_length;
            set
            {
                if (m_length == value) return;
                m_length = value;
                CalculateEndPoint();
            }
        }

        /// <summary>
        ///     Property to get and set Start Point of line
        /// </summary>
        public Vector StartPoint
        {
            get => m_startPnt;
            set
            {
                if (m_startPnt == value) return;
                m_startPnt = value;
                CalculateDirection();
            }
        }

        /// <summary>
        ///     Property to get and set End Point of line
        /// </summary>
        public Vector EndPoint
        {
            get => m_endPnt;
            set
            {
                if (m_endPnt == value) return;
                m_endPnt = value;
                CalculateDirection();
            }
        }

        /// <summary>
        ///     Property to get and set Normal of line
        /// </summary>
        public Vector Normal
        {
            get => m_normal;
            set
            {
                if (m_normal == value) return;
                m_normal = value;
                CalculateEndPoint();
            }
        }

        /// <summary>
        ///     calculate length by StartPoint and EndPoint
        /// </summary>
        private void CalculateLength()
        {
            m_length = ~(m_startPnt - m_endPnt);
        }

        /// <summary>
        ///     calculate Direction by StartPoint and EndPoint
        /// </summary>
        private void CalculateDirection()
        {
            CalculateLength();
            m_normal = (m_endPnt - m_startPnt) / m_length;
        }

        /// <summary>
        ///     calculate EndPoint by StartPoint, Length and Direction
        /// </summary>
        private void CalculateEndPoint()
        {
            m_endPnt = m_startPnt + m_normal * m_length;
        }
    }
}
