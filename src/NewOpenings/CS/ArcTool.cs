// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Revit.SDK.Samples.NewOpenings.CS
{
    /// <summary>
    ///     Tool used to draw arc.
    /// </summary>
    internal class ArcTool : Tool
    {
        private bool m_isFinished;

        /// <summary>
        ///     Default constructor
        /// </summary>
        public ArcTool()
        {
            Type = ToolType.Arc;
        }

        /// <summary>
        ///     Draw Arcs
        /// </summary>
        /// <param name="graphic">Graphics object</param>
        public override void Draw(Graphics graphic)
        {
            foreach (var line in Lines)
            {
                var count = line.Count;
                if (count == 3)
                {
                    DrawArc(graphic, ForeGroundPen, line[0], line[1], line[3]);
                }
                else if (count > 3)
                {
                    DrawArc(graphic, ForeGroundPen, line[0], line[1], line[2]);
                    for (var i = 1; i < count - 3; i += 2)
                        DrawArc(graphic, ForeGroundPen, line[i], line[i + 2], line[i + 3]);
                }
            }
        }

        /// <summary>
        ///     Mouse down event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnMouseDown(Graphics graphic, MouseEventArgs e)
        {
            if (MouseButtons.Left == e.Button)
            {
                Points.Add(e.Location);
                PreMovePoint = e.Location;

                if (Points.Count >= 4 && Points.Count % 2 == 0)
                    graphic.DrawLine(BackGroundPen,
                        Points[Points.Count - 3], PreMovePoint);
                Draw(graphic);

                if (m_isFinished)
                {
                    m_isFinished = false;
                    var line = new List<Point>(Points);
                    Lines.Add(line);
                    Points.Clear();
                }
            }
        }

        /// <summary>
        ///     Mouse move event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnMouseMove(Graphics graphic, MouseEventArgs e)
        {
            if (2 == Points.Count)
            {
                DrawArc(graphic, BackGroundPen, Points[0], Points[1], PreMovePoint);
                PreMovePoint = e.Location;
                DrawArc(graphic, ForeGroundPen, Points[0], Points[1], e.Location);
            }
            else if (Points.Count > 2 && Points.Count % 2 == 0)
            {
                DrawArc(graphic, BackGroundPen, Points[Points.Count - 3],
                    Points[Points.Count - 1], PreMovePoint);
                PreMovePoint = e.Location;
                DrawArc(graphic, ForeGroundPen, Points[Points.Count - 3],
                    Points[Points.Count - 1], e.Location);
            }
            else if (!m_isFinished && Points.Count > 2 && Points.Count % 2 == 1)
            {
                graphic.DrawLine(BackGroundPen, Points[Points.Count - 2], PreMovePoint);
                PreMovePoint = e.Location;
                graphic.DrawLine(ForeGroundPen, Points[Points.Count - 2], e.Location);
            }
        }

        /// <summary>
        ///     Mouse right key click
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnRightMouseClick(Graphics graphic, MouseEventArgs e)
        {
            if (!m_isFinished && e.Button == MouseButtons.Right && Points.Count > 0)
            {
                m_isFinished = true;
                Points.Add(Points[0]);
                graphic.DrawLine(BackGroundPen, Points[Points.Count - 3], e.Location);
            }
        }

        /// <summary>
        ///     Mouse middle key down event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnMidMouseDown(Graphics graphic, MouseEventArgs e)
        {
            base.OnMidMouseDown(graphic, e);
            if (m_isFinished) m_isFinished = false;
        }

        /// <summary>
        ///     Calculate the arc center
        /// </summary>
        /// <param name="p1">Point on arc</param>
        /// <param name="p2">Point on arc</param>
        /// <param name="p3">Point on arc</param>
        /// <returns></returns>
        private PointF ComputeCenter(PointF p1, PointF p2, PointF p3)
        {
            var deta = 4 * (p2.X - p1.X) * (p3.Y - p1.Y) - 4 * (p2.Y - p1.Y) * (p3.X - p1.X);

            if (deta == 0) throw new Exception("Divided by Zero!");
            var constD1 = p2.X * p2.X + p2.Y * p2.Y - (p1.X * p1.X + p1.Y * p1.Y);
            var constD2 = p3.X * p3.X + p3.Y * p3.Y - (p1.X * p1.X + p1.Y * p1.Y);

            var centerX = (constD1 * 2 * (p3.Y - p1.Y) - constD2 * 2 * (p2.Y - p1.Y)) / deta;
            var centerY = (constD2 * 2 * (p2.X - p1.X) - constD1 * 2 * (p3.X - p1.X)) / deta;

            return new PointF(centerX, centerY);
        }

        /// <summary>
        ///     Draw arc
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="pen">Used to set drawing color</param>
        /// <param name="p1">Point on arc</param>
        /// <param name="p2">Point on arc</param>
        /// <param name="p3">Point on arc</param>
        private void DrawArc(Graphics graphic, Pen pen, PointF p1, PointF p2, PointF p3)
        {
            try
            {
                var pCenter = ComputeCenter(p1, p2, p3);

                //computer the arc rectangle
                var radius = (float)Math.Sqrt((p1.X - pCenter.X) * (p1.X - pCenter.X)
                                              + (p1.Y - pCenter.Y) * (p1.Y - pCenter.Y));
                var size = new SizeF(radius, radius);
                var upLeft = pCenter - size;
                var sizeRect = new SizeF(2 * radius, 2 * radius);
                var rectF = new RectangleF(upLeft, sizeRect);

                double startCos = (p1.X - pCenter.X) / radius;
                double startSin = (p1.Y - pCenter.Y) / radius;

                double endCos = (p2.X - pCenter.X) / radius;
                double endSin = (p2.Y - pCenter.Y) / radius;

                double midCos = (p3.X - pCenter.X) / radius;
                double midSin = (p3.Y - pCenter.Y) / radius;

                //computer the angle between [0, 360]
                var startAngle = GetAngle(startSin, startCos);
                var endAngle = GetAngle(endSin, endCos);
                var midAngle = GetAngle(midSin, midCos);

                //get the min angle and sweep angle
                var minAngle = Math.Min(startAngle, endAngle);
                var maxAngle = Math.Max(startAngle, endAngle);
                var sweepAngle = Math.Abs(endAngle - startAngle);
                if (midAngle < minAngle || midAngle > maxAngle)
                {
                    minAngle = maxAngle;
                    sweepAngle = 360 - sweepAngle;
                }

                graphic.DrawArc(pen, rectF, (float)minAngle, (float)sweepAngle);
            }
            //catch divided by zero exception
            catch (Exception)
            {
            }
        }

        /// <summary>
        ///     Get angle between [0,360]
        /// </summary>
        /// <param name="sin">Sin(Angle) value</param>
        /// <param name="cos">Cos(Angle) value</param>
        /// <returns></returns>
        private double GetAngle(double sin, double cos)
        {
            double result = 0;
            if (sin > 0)
                result = 180 / Math.PI * Math.Acos(cos);
            else if (cos < 0)
                result = 180 + 180 / Math.PI * Math.Acos(Math.Abs(cos));
            else if (cos > 0) result = 360 - 180 / Math.PI * Math.Acos(Math.Abs(cos));
            return result;
        }
    }
}
