// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Revit.SDK.Samples.NewOpenings.CS
{
    /// <summary>
    ///     Stand for the draw tool type
    /// </summary>
    public enum ToolType
    {
        /// <summary>
        ///     Draw nothing
        /// </summary>
        None,

        /// <summary>
        ///     Draw polygon
        /// </summary>
        Line,

        /// <summary>
        ///     Draw rectangle
        /// </summary>
        Rectangle,

        /// <summary>
        ///     Draw circle
        /// </summary>
        Circle,

        /// <summary>
        ///     Draw arc
        /// </summary>
        Arc
    }

    /// <summary>
    ///     Abstract class use as base class of all draw tool class
    /// </summary>
    public abstract class ITool
    {
        /// <summary>
        ///     Background pen used to erase the preview line
        /// </summary>
        protected readonly Pen m_backGroundPen;

        /// <summary>
        ///     Foreground pen used to draw lines
        /// </summary>
        protected readonly Pen m_foreGroundPen;

        /// <summary>
        ///     Field used to store lines
        /// </summary>
        protected readonly List<List<Point>> m_lines = new List<List<Point>>();

        /// <summary>
        ///     Field used to store points of a line
        /// </summary>
        protected readonly List<Point> m_points = new List<Point>();

        /// <summary>
        ///     Store the mouse position when mouse move in pictureBox
        /// </summary>
        protected Point m_preMovePoint;

        /// <summary>
        ///     ToolType is enum type indicate draw tools.
        /// </summary>
        protected ToolType m_type;

        /// <summary>
        ///     Default constructor
        /// </summary>
        public ITool()
        {
            m_backGroundPen = Pens.White;
            m_foreGroundPen = Pens.Red;
        }

        /// <summary>
        ///     Get the tool type
        /// </summary>
        public ToolType ToolType => m_type;

        /// <summary>
        ///     Get all lines drawn in pictureBox
        /// </summary>
        public List<List<Point>> GetLines()
        {
            return m_lines;
        }

        /// <summary>
        ///     Right mouse click event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public virtual void OnRightMouseClick(Graphics graphic, MouseEventArgs e)
        {
        }

        /// <summary>
        ///     Mouse move event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public virtual void OnMouseMove(Graphics graphic, MouseEventArgs e)
        {
        }

        /// <summary>
        ///     Mouse down event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public virtual void OnMouseDown(Graphics graphic, MouseEventArgs e)
        {
        }

        /// <summary>
        ///     Mouse up event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public virtual void OnMouseUp(Graphics graphic, MouseEventArgs e)
        {
        }

        /// <summary>
        ///     Mouse middle key down event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public virtual void OnMidMouseDown(Graphics graphic, MouseEventArgs e)
        {
            m_points.Clear();
        }

        /// <summary>
        ///     Draw geometries contained in the tool. which class derived from this class
        ///     must implement this abstract method
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        public abstract void Draw(Graphics graphic);
    }
}
