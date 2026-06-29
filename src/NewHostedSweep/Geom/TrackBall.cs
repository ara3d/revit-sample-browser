// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Point = System.Drawing.Point;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Geom
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

        private void Track(Point currentPosition)
        {
            var currentPosition3D = SampleBrowserUtils.ProjectToTrackball(m_canvasWidth, m_canvasHeight, currentPosition);
            var axis = m_previousPosition3D.CrossProduct(currentPosition3D);
            if (axis.GetLength() == 0)
                return;

            m_rotation = Transform.CreateRotation(axis, -m_previousPosition3D.AngleTo(currentPosition3D));
            m_previousPosition3D = currentPosition3D;
        }

        private void Zoom(Point currentPosition) =>
            m_scale = Math.Exp((currentPosition.Y - m_previousPosition2D.Y) / 100);

        public void OnMouseDown(float width, float height, MouseEventArgs e)
        {
            m_rotation = Transform.Identity;
            m_scale = 1.0;
            m_canvasWidth = width;
            m_canvasHeight = height;
            m_previousPosition2D = e.Location;
            m_previousPosition3D = SampleBrowserUtils.ProjectToTrackball(m_canvasWidth, m_canvasHeight, m_previousPosition2D);
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            var currentPosition = e.Location;
            if (currentPosition == m_previousPosition2D)
                return;

            switch (e.Button)
            {
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
