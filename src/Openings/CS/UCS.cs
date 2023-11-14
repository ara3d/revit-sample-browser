// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;

namespace RevitMultiSample.Openings.CS
{
    /// <summary>
    ///     This class stand for user coordinate system
    /// </summary>
    public class Ucs
    {
        private Vector m_origin = new Vector(0.0, 0.0, 0.0);
        private Vector m_xAxis = new Vector(1.0, 0.0, 0.0);
        private Vector m_yAxis = new Vector(0.0, 1.0, 0.0);
        private Vector m_zAxis = new Vector(0.0, 0.0, 1.0);

        /// <summary>
        ///     The default constructor,
        /// </summary>
        public Ucs(Vector origin, Vector xAxis, Vector yAxis)
            : this(origin, xAxis, yAxis, true)
        {
        }

        /// <summary>
        ///     constructor,
        ///     get a user coordinate system
        /// </summary>
        /// <param name="origin">origin of user coordinate system</param>
        /// <param name="xAxis">xAxis of user coordinate system</param>
        /// <param name="yAxis">yAxis of user coordinate system</param>
        /// <param name="flag">select left handness or right handness</param>
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

        /// <summary>
        ///     Property to get origin of user coordinate system
        /// </summary>
        public Vector Origin => m_origin;

        /// <summary>
        ///     Property to get X Axis of user coordinate system
        /// </summary>
        public Vector XAxis => m_xAxis;

        /// <summary>
        ///     Property to get Y Axis of user coordinate system
        /// </summary>
        public Vector YAxis => m_yAxis;

        /// <summary>
        ///     Property to get Z Axis of user coordinate system
        /// </summary>
        public Vector ZAxis => m_zAxis;

        /// <summary>
        ///     Transform local coordinate to global coordinate
        /// </summary>
        /// <param name="arg">a vector which need to transform</param>
        public Vector Lc2Gc(Vector arg)
        {
            var result = new Vector
            {
                X = arg.X * m_xAxis.X + arg.Y * m_yAxis.X + arg.Z * m_zAxis.X + m_origin.X,
                Y = arg.X * m_xAxis.Y + arg.Y * m_yAxis.Y + arg.Z * m_zAxis.Y + m_origin.Y,
                Z = arg.X * m_xAxis.Z + arg.Y * m_yAxis.Z + arg.Z * m_zAxis.Z + m_origin.Z
            };
            return result;
        }

        /// <summary>
        ///     Transform global coordinate to local coordinate
        /// </summary>
        /// <param name="line">a line which need to transform</param>
        public Line3D Gc2Lc(Line3D line)
        {
            var startPnt = Gc2Lc(line.StartPoint);
            var endPnt = Gc2Lc(line.EndPoint);
            return new Line3D(startPnt, endPnt);
        }

        /// <summary>
        ///     Transform global coordinate to local coordinate
        /// </summary>
        /// <param name="arg">a vector which need to transform</param>
        public Vector Gc2Lc(Vector arg)
        {
            var result = new Vector();
            arg = arg - m_origin;
            result.X = m_xAxis * arg;
            result.Y = m_yAxis * arg;
            result.Z = m_zAxis * arg;
            return result;
        }
    }
}
