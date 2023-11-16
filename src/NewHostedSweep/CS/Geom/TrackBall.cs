// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Point = System.Drawing.Point;

namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS
{
    /// <summary>
    ///     This class is intent to convenience the geometry transformations.
    ///     It can produce rotation and scale transformations.
    /// </summary>
    public class TrackBall
    {
        /// <summary>
        ///     Canvas height.
        /// </summary>
        private float m_canvasHeight;

        /// <summary>
        ///     Canvas width.
        /// </summary>
        private float m_canvasWidth;

        /// <summary>
        ///     Previous position in 2D.
        /// </summary>
        private Point m_previousPosition2D;

        /// <summary>
        ///     Previous position in 3D.
        /// </summary>
        private XYZ m_previousPosition3D;

        /// <summary>
        ///     Current rotation transform.
        /// </summary>
        private Transform m_rotation = Transform.Identity;

        /// <summary>
        ///     Current scale transform.
        /// </summary>
        private double m_scale;

        /// <summary>
        ///     Current rotation transform.
        /// </summary>
        public Transform Rotation
        {
            get => m_rotation;
            set => m_rotation = value;
        }

        /// <summary>
        ///     Current scale transform.
        /// </summary>
        public double Scale
        {
            get => m_scale;
            set => m_scale = value;
        }

        /// <summary>
        ///     Project canvas 2D point to the track ball.
        /// </summary>
        /// <param name="width">Canvas width</param>
        /// <param name="height">Canvas height</param>
        /// <param name="point">2D point</param>
        /// <returns>Projected point in track ball</returns>
        private XYZ ProjectToTrackball(double width, double height, Point point)
        {
            var x = point.X / (width / 2); // Scale so bounds map to [0,0] - [2,2]
            var y = point.Y / (height / 2);

            x--; // Translate 0,0 to the center
            y = 1 - y; // Flip so +Y is up instead of down

            double z;

            var d = Math.Sqrt(x * x + y * y);
            if (d < 0.70710678118654752440)
            {
                /* Inside sphere */
                z = Math.Sqrt(1 - d * d);
            }
            else
            {
                /* On hyperbola */
                var t = 1 / 1.41421356237309504880;
                z = t * t / d;
            }

            return new XYZ(x, y, z);
        }

        /// <summary>
        ///     Yield the rotation transform according to current 2D point in canvas.
        /// </summary>
        /// <param name="currentPosition">2D point in canvas</param>
        private void Track(Point currentPosition)
        {
            var currentPosition3D = ProjectToTrackball(
                m_canvasWidth, m_canvasHeight, currentPosition);

            var axis = m_previousPosition3D.CrossProduct(currentPosition3D);
            if (axis.GetLength() == 0) return;

            var angle = m_previousPosition3D.AngleTo(currentPosition3D);
            m_rotation = Transform.CreateRotation(axis, -angle);
            m_previousPosition3D = currentPosition3D;
        }

        /// <summary>
        ///     Yield the scale transform according to current 2D point in canvas.
        /// </summary>
        /// <param name="currentPosition">2D point in canvas</param>
        private void Zoom(Point currentPosition)
        {
            double yDelta = currentPosition.Y - m_previousPosition2D.Y;

            var scale = Math.Exp(yDelta / 100); // e^(yDelta/100) is fairly arbitrary.

            m_scale = scale;
        }

        /// <summary>
        ///     Mouse down, initialize the transformation to identity.
        /// </summary>
        /// <param name="width">Canvas width</param>
        /// <param name="height">Canvas height</param>
        /// <param name="e"></param>
        public void OnMouseDown(float width, float height, MouseEventArgs e)
        {
            m_rotation = Transform.Identity;
            m_scale = 1.0;
            m_canvasWidth = width;
            m_canvasHeight = height;
            m_previousPosition2D = e.Location;
            m_previousPosition3D = ProjectToTrackball(m_canvasWidth,
                m_canvasHeight,
                m_previousPosition2D);
        }

        /// <summary>
        ///     Mouse move with left button press will yield the rotation transform,
        ///     with right button press will yield scale transform.
        /// </summary>
        /// <param name="e"></param>
        public void OnMouseMove(MouseEventArgs e)
        {
            var currentPosition = e.Location;

            // avoid any zero axis conditions
            if (currentPosition == m_previousPosition2D) return;

            switch (e.Button)
            {
                // Prefer tracking to zooming if both buttons are pressed.
                case MouseButtons.Left:
                    Track(currentPosition);
                    break;
                case MouseButtons.Right:
                    Zoom(currentPosition);
                    break;
            }

            m_previousPosition2D = currentPosition;
        }

        /// <summary>
        ///     Arrows key down will also yield the rotation transform.
        /// </summary>
        /// <param name="e"></param>
        public void OnKeyDown(KeyEventArgs e)
        {
            var axis = new XYZ(1.0, 0, 0);
            var angle = 0.1;
            switch (e.KeyCode)
            {
                case Keys.Down: break;
                case Keys.Up:
                    angle = -angle;
                    break;
                case Keys.Left:
                    axis = new XYZ(0, 1.0, 0);
                    angle = -angle;
                    break;
                case Keys.Right:
                    axis = new XYZ(0, 1.0, 0);
                    break;
            }

            m_rotation = Transform.CreateRotation(axis, angle);
        }
    }
}
