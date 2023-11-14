// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Drawing;

namespace Revit.SDK.Samples.CreateBeamSystem.CS
{
    /// <summary>
    ///     represent a geometry segment line
    /// </summary>
    public class Line2D
    {
        private PointF m_endPnt; // end point
        private float m_length; // length of the line
        private PointF m_normal; // normal of the line; start point to end point
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
        ///     get an interval point on the line of the segment
        /// </summary>
        /// <param name="rate">
        ///     rate of length from interval to StartPoint
        ///     to length from EndPoint to interval
        /// </param>
        /// <returns>interval point</returns>
        public PointF GetIntervalPoint(float rate)
        {
            var result = new PointF
            {
                X = m_startPnt.X + (m_endPnt.X - m_startPnt.X) * rate,
                Y = m_startPnt.Y + (m_endPnt.Y - m_startPnt.Y) * rate
            };
            return result;
        }

        /// <summary>
        ///     scale the segment according to the center of the segment
        /// </summary>
        /// <param name="rate">rate to scale</param>
        public void Scale(float rate)
        {
            var startPnt = GetIntervalPoint((1.0f - rate) / 2.0f);
            var endPnt = GetIntervalPoint((1.0f + rate) / 2.0f);
            m_startPnt = startPnt;
            m_endPnt = endPnt;
            CalculateLength();
        }

        /// <summary>
        ///     parallelly shift the line
        /// </summary>
        /// <param name="distance">distance</param>
        public void Shift(float distance)
        {
            var moveSize = new SizeF(-distance * m_normal.Y, distance * m_normal.X);
            m_startPnt = m_startPnt + moveSize;
            m_endPnt = m_endPnt + moveSize;
        }

        /// <summary>
        ///     creates an instance of the GeometryLine class that is identical to the current GeometryLine
        /// </summary>
        /// <returns>created GeometryLine</returns>
        public Line2D Clone()
        {
            var cloned = new Line2D(m_startPnt, m_endPnt);
            return cloned;
        }

        /// <summary>
        ///     find the number of intersection points for two segments
        /// </summary>
        /// <param name="line0">first line</param>
        /// <param name="line1">second line</param>
        /// <returns>number of intersection points; 0, 1, or 2</returns>
        public static int FindIntersection(Line2D line0, Line2D line1)
        {
            var intersectPnt = new PointF[2];
            return FindIntersection(line0, line1, ref intersectPnt);
        }

        /// <summary>
        ///     find the intersection points of two segments
        /// </summary>
        /// <param name="line0">first line</param>
        /// <param name="line1">second line</param>
        /// <param name="intersectPnt">0, 1 or 2 intersection points</param>
        /// <returns>number of intersection points; 0, 1, or 2</returns>
        public static int FindIntersection(Line2D line0, Line2D line1, ref PointF[] intersectPnt)
        {
            // segments p0 + s * d0 for s in [0, 0], p1 + t * d1 for t in [0, 1]
            var p0 = line0.StartPnt;
            var d0 = MathUtil.Multiply(line0.Length, line0.Normal);
            var p1 = line1.StartPnt;
            var d1 = MathUtil.Multiply(line1.Length, line1.Normal);

            var E = MathUtil.Subtract(p1, p0);
            var kross = d0.X * d1.Y - d0.Y * d1.X;
            var sqrKross = kross * kross;
            var sqrLen0 = d0.X * d0.X + d0.Y * d0.Y;
            var sqrLen1 = d1.X * d1.X + d1.Y * d1.Y;

            // lines of the segments are not parallel
            if (sqrKross > MathUtil.Float_Epsilon * sqrLen0 * sqrLen1)
            {
                var s = (E.X * d1.Y - E.Y * d1.X) / kross;
                if (s < 0 || s > 1)
                    // intersection of lines is not point on segment p0 + s * d0
                    return 0;

                var t = (E.X * d0.Y - E.Y * d0.X) / kross;
                if (t < 0 || t > 1)
                    // intersection of lines is not a point on segment p1 + t * d1
                    return 0;
                // intersection of lines is a point on each segment
                intersectPnt[0] = MathUtil.Add(p0, MathUtil.Multiply(s, d0));
                return 1;
            }

            // lines of the segments are paralled
            var sqrLenE = E.X * E.X + E.Y * E.Y;
            var kross2 = E.X * d0.Y - E.Y * d0.X;
            var sqrKross2 = kross2 * kross2;
            if (sqrKross2 > MathUtil.Float_Epsilon * sqrLen0 * sqrLenE)
                // lines of the segments are different
                return 0;

            // lines of the segments are the same. need to test for overlap of segments
            var s0 = MathUtil.Dot(d0, E) / sqrLen0;
            var s1 = s0 + MathUtil.Dot(d0, d1) / sqrLen0;
            var smin = MathUtil.GetMin(s0, s1);
            var smax = MathUtil.GetMax(s0, s1);
            var w = new float[2];

            var imax = MathUtil.FindIntersection(0.0f, 1.0f, smin, smax, ref w);
            for (var i = 0; i < imax; i++) intersectPnt[i] = MathUtil.Add(p0, MathUtil.Multiply(w[i], d0));

            return imax;
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
                (float)Math.Sqrt(Math.Pow(m_startPnt.X - m_endPnt.X, 2) + Math.Pow(m_startPnt.Y - m_endPnt.Y, 2));
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

        /// <summary>
        ///     calculate StartPoint by EndPoint, Length and Direction
        /// </summary>
        private void CalculateStartPoint()
        {
            m_startPnt.X = m_endPnt.X - m_length * m_normal.X;
            m_startPnt.Y = m_endPnt.Y - m_length * m_normal.Y;
        }
    }
}
