//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Revit.SDK.Samples.NewPathReinforcement.CS
{
    /// <summary>
    /// tool used to draw line
    /// </summary>
    public class LineTool
    {
        private List<Point> m_points = new List<Point>();  // Field used to store points of a line
        private Pen m_backGroundPen;  // background pen used to Erase the preview line
        private Pen m_foreGroundPen;  // foreground pen used to draw lines
        private Point m_preMovePoint;  // store the mouse position when mouse move in pictureBox

        
        /// <summary>
        /// Finished property to define whether curve was finished
        /// </summary>
        public bool Finished { get; set; }

        /// <summary>
        /// PointsNumber property to get the number of points stored
        /// </summary>
        public int PointsNumber => m_points.Count;

        /// <summary>
        /// default constructor
        /// </summary>
        public LineTool()
        {
            m_backGroundPen = new Pen(Color.White);
            m_backGroundPen.Width *= 2;
            m_foreGroundPen = new Pen(Color.Black);
            m_foreGroundPen.Width *= 2;
            Finished = false;
        }

        /// <summary>
        /// get all lines drawn in pictureBox
        /// </summary>
        public List<Point> GetPoints()
        {
            return m_points;
        }

        /// <summary>
        /// clear all the points in the tool
        /// </summary>
        public void Clear()
        {
            m_points.Clear();
        }

        /// <summary>
        /// draw a line from end point of tool to the location where mouse move
        /// </summary>
        /// <param name="graphic">graphic object, used to draw geometry</param>
        /// <param name="e">mouse event args</param>
        public void OnMouseMove(Graphics graphic, MouseEventArgs e)
        {
            if(m_points.Count != 0 && !Finished)
            {                
                graphic.DrawLine(m_backGroundPen, m_points[m_points.Count - 1], m_preMovePoint);
                m_preMovePoint = e.Location;
                graphic.DrawLine(m_foreGroundPen, m_points[m_points.Count - 1], e.Location);
            }
        }

        /// <summary>
        /// restore the location point where mouse click
        /// </summary>
        /// <param name="e">mouse event args</param>
        public void OnMouseDown(MouseEventArgs e)
        {
            //when user click right button of mouse, then erase last line
            if (MouseButtons.Right == e.Button && m_points.Count >= 2)
            {
                Finished = true;
            }

            if (MouseButtons.Left == e.Button && !Finished)
            {
                m_preMovePoint = e.Location;
                m_points.Add(e.Location);
            }
        }

        /// <summary>
        /// draw lines stored in the tool
        /// </summary>
        /// <param name="graphic">Graphics object, use to draw geometry</param>
        public void Draw(Graphics graphic)
        {
            for (var i = 0; i < m_points.Count - 1; i++)
            {
                graphic.DrawLine(m_foreGroundPen, m_points[i], m_points[i + 1]);
            }
        }
    }
}
