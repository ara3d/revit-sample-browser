// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;

namespace Ara3D.RevitSampleBrowser.Openings.CS
{
    public class Ucs
    {
        private Vector m_origin = new(0.0, 0.0, 0.0);
        private Vector m_xAxis = new(1.0, 0.0, 0.0);
        private Vector m_yAxis = new(0.0, 1.0, 0.0);
        private Vector m_zAxis = new(0.0, 0.0, 1.0);

        public Ucs(Vector origin, Vector xAxis, Vector yAxis)
            : this(origin, xAxis, yAxis, true)
        {
        }

        public Ucs(Vector origin, Vector xAxis, Vector yAxis, bool flag)
        {
            var x2 = xAxis / ~xAxis;
            var y2 = yAxis / ~yAxis;
            var z2 = x2 & y2;
            if (~z2 < double.Epsilon) throw new InvalidOperationException();

            if (!flag) z2 = -z2;

            m_origin = origin;
            m_xAxis = x2;
            m_yAxis = y2;
            m_zAxis = z2;
        }

        public Vector Origin => m_origin;

        public Vector XAxis => m_xAxis;

        public Vector YAxis => m_yAxis;

        public Vector ZAxis => m_zAxis;

        public Vector Lc2Gc(Vector arg)
        {
            Vector result = new()
            {
                X = (arg.X * m_xAxis.X) + (arg.Y * m_yAxis.X) + (arg.Z * m_zAxis.X) + m_origin.X,
                Y = (arg.X * m_xAxis.Y) + (arg.Y * m_yAxis.Y) + (arg.Z * m_zAxis.Y) + m_origin.Y,
                Z = (arg.X * m_xAxis.Z) + (arg.Y * m_yAxis.Z) + (arg.Z * m_zAxis.Z) + m_origin.Z
            };
            return result;
        }

        public Line3D Gc2Lc(Line3D line)
        {
            var startPnt = Gc2Lc(line.StartPoint);
            var endPnt = Gc2Lc(line.EndPoint);
            return new Line3D(startPnt, endPnt);
        }

        public Vector Gc2Lc(Vector arg)
        {
            Vector result = new();
            arg -= m_origin;
            result.X = m_xAxis * arg;
            result.Y = m_yAxis * arg;
            result.Z = m_zAxis * arg;
            return result;
        }
    }
}
