// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.NewOpenings.CS
{
    public enum ToolType
    {
        None,

        Line,

        Rectangle,

        Circle,

        Arc
    }

    public abstract class Tool
    {
        protected readonly Pen BackGroundPen;

        protected readonly Pen ForeGroundPen;

        protected readonly List<List<Point>> Lines = [];

        protected readonly List<Point> Points = [];

        protected Point PreMovePoint;

        protected ToolType Type;

        public Tool()
        {
            BackGroundPen = Pens.White;
            ForeGroundPen = Pens.Red;
        }

        public ToolType ToolType => Type;

        public List<List<Point>> GetLines()
        {
            return Lines;
        }

        public virtual void OnRightMouseClick(Graphics graphic, MouseEventArgs e)
        {
        }

        public virtual void OnMouseMove(Graphics graphic, MouseEventArgs e)
        {
        }

        public virtual void OnMouseDown(Graphics graphic, MouseEventArgs e)
        {
        }

        public virtual void OnMouseUp(Graphics graphic, MouseEventArgs e)
        {
        }

        public virtual void OnMidMouseDown(Graphics graphic, MouseEventArgs e)
        {
            Points.Clear();
        }

        public abstract void Draw(Graphics graphic);
    }
}
