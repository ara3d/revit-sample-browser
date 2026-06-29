// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.NewOpenings.CS
{
    /// <summary>
    ///     Tool used to draw line
    /// </summary>
    public class LineTool : Tool
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public LineTool()
        {
            Type = ToolType.Line;
        }

        /// <summary>
        ///     Mouse move event handle
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnMouseMove(Graphics graphic, MouseEventArgs e)
        {
            if (Points.Count != 0)
            {
                graphic.DrawLine(BackGroundPen, Points[Points.Count - 1], PreMovePoint);
                PreMovePoint = e.Location;
                graphic.DrawLine(ForeGroundPen, Points[Points.Count - 1], e.Location);
            }
        }

        /// <summary>
        ///     Mouse down event handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnMouseDown(Graphics graphic, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PreMovePoint = e.Location;
                Points.Add(e.Location);

                if (Points.Count >= 2)
                    graphic.DrawLine(ForeGroundPen, Points[Points.Count - 2],
                        Points[Points.Count - 1]);
            }
        }

        /// <summary>
        ///     Right mouse click handler
        /// </summary>
        /// <param name="graphic">Graphics object, used to drawing geometry</param>
        /// <param name="e">Mouse event argument</param>
        public override void OnRightMouseClick(Graphics graphic, MouseEventArgs e)
        {
            if (MouseButtons.Right == e.Button && Points.Count > 2)
            {
                var line = new List<Point>(Points);
                Lines.Add(line);

                graphic.DrawLine(ForeGroundPen, Points[Points.Count - 1], Points[0]);
                graphic.DrawLine(BackGroundPen, Points[Points.Count - 1], PreMovePoint);
                Points.Clear();
            }
        }

        /// <summary>
        ///     Draw lines
        /// </summary>
        /// <param name="graphic">Graphics object, used to draw geometry</param>
        public override void Draw(Graphics graphic)
        {
            foreach (var line in Lines)
            {
                for (var i = 0; i < line.Count - 1; i++) graphic.DrawLine(ForeGroundPen, line[i], line[i + 1]);
                //close the line
                graphic.DrawLine(ForeGroundPen, line[line.Count - 1], line[0]);
            }
        }
    }
}
