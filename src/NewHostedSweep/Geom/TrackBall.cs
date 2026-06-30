// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using System;
using System.Windows.Forms;
using Point = System.Drawing.Point;
namespace Ara3D.RevitSampleBrowser.NewHostedSweep.CS.Geom
{
    public class TrackBall
    {
        private float m_canvasHeight;

        private float m_canvasWidth;

        private Point m_previousPosition2D;

        private XYZ m_previousPosition3D;

        public Transform Rotation { get; set; } = Transform.Identity;

        public double Scale { get; set; }

        private void Track(Point currentPosition)
        {
            var currentPosition3D = SampleBrowserUtils.ProjectToTrackball(m_canvasWidth, m_canvasHeight, currentPosition);
            var axis = m_previousPosition3D.CrossProduct(currentPosition3D);
            if (axis.GetLength() == 0)
                return;

            Rotation = Transform.CreateRotation(axis, -m_previousPosition3D.AngleTo(currentPosition3D));
            m_previousPosition3D = currentPosition3D;
        }

        private void Zoom(Point currentPosition)
        {
            Scale = Math.Exp((currentPosition.Y - m_previousPosition2D.Y) / 100);
        }

        public void OnMouseDown(float width, float height, MouseEventArgs e)
        {
            Rotation = Transform.Identity;
            Scale = 1.0;
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

        public void OnKeyDown(KeyEventArgs e)
        {
            XYZ axis = new(1.0, 0, 0);
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

            Rotation = Transform.CreateRotation(axis, angle);
        }
    }
}
