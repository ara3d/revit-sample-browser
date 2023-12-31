// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS
{
    /// <summary>
    ///     Abstract class used as a base class of all drawing tool class
    /// </summary>
    public abstract class Tool
    {
        protected readonly Pen BackGroundPen; // background pen used to Erase the preview line
        protected bool m_finished; // indicate whether user have finished drawing
        protected readonly Pen ForeGroundPen; // foreground pen used to draw lines
        protected readonly List<Point> m_points = new List<Point>(); // Field used to store points of a line
        protected Point PreDownPoint; // store the mouse position when right mouse button clicked in pictureBox
        protected Point PreMovePoint; // store the mouse position when mouse move in pictureBox

        /// <summary>
        ///     default constructor
        /// </summary>
        protected Tool()
        {
            BackGroundPen = new Pen(Color.White);
            BackGroundPen.Width *= 2;
            ForeGroundPen = new Pen(Color.Black);
            ForeGroundPen.Width *= 2;
            m_finished = false;
        }

        /// <summary>
        ///     Finished property to define whether curve was finished
        /// </summary>
        public bool Finished
        {
            get => m_finished;
            set => m_finished = value;
        }

        /// <summary>
        ///     get all lines drawn in pictureBox
        /// </summary>
        public List<Point> Points => m_points;

        /// <summary>
        ///     calculate the distance between two points
        /// </summary>
        /// <param name="p1">first point</param>
        /// <param name="p2">second point</param>
        /// <returns>distance between two points</returns>
        protected double GetDistance(Point p1, Point p2)
        {
            var distance = Math.Sqrt(
                (p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
            return distance;
        }

        /// <summary>
        ///     clear all the points in the tool
        /// </summary>
        public void Clear()
        {
            m_points.Clear();
        }

        /// <summary>
        ///     draw a line from end point to the location where mouse moved
        /// </summary>
        /// <param name="graphic">Graphics object,used to draw geometry</param>
        /// <param name="e">mouse event args</param>
        public virtual void OnMouseMove(Graphics graphic,
            MouseEventArgs e)
        {
        }

        /// <summary>
        ///     record the location point where mouse clicked
        /// </summary>
        /// <param name="e">mouse event args</param>
        public virtual void OnMouseDown(MouseEventArgs e)
        {
        }

        /// <summary>
        ///     draw the stored lines
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        public virtual void Draw(Graphics graphic)
        {
        }
    }
}
