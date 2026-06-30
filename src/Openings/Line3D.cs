// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.Openings.CS
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

        public Line3D()
        {
            m_startPnt = new Vector(0.0, 0.0, 0.0);
            m_endPnt = new Vector(1.0, 0.0, 0.0);
            m_length = 1.0;
            m_normal = new Vector(1.0, 0.0, 0.0);
        }

        public Line3D(Vector startPnt, Vector endPnt)
        {
            m_startPnt = startPnt;
            m_endPnt = endPnt;
            CalculateDirection();
        }

        //property
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

        private void CalculateLength()
        {
            m_length = ~(m_startPnt - m_endPnt);
        }

        private void CalculateDirection()
        {
            CalculateLength();
            m_normal = (m_endPnt - m_startPnt) / m_length;
        }

        private void CalculateEndPoint()
        {
            m_endPnt = m_startPnt + (m_normal * m_length);
        }
    }
}
