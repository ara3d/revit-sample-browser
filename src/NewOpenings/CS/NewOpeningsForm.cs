// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RevitMultiSample.NewOpenings.CS
{
    /// <summary>
    ///     Main form used to display the profile of Wall or Floor and draw the opening profiles.
    /// </summary>
    public partial class NewOpeningsForm : Form
    {
        private readonly Matrix4 m_moveToCenterMatrix; //save the matrix use to move point to origin
        private readonly Profile m_profile; //save the profile date (ProfileFloor or ProfileWall)
        private Matrix4 m_scaleMatrix; //save the matrix use to scale
        private readonly Matrix4 m_to2DMatrix; //save the matrix use to transform 3D to 2D
        private Tool m_tool; //current using tool
        private readonly Queue<Tool> m_tools = new Queue<Tool>(); //all tool can use in pictureBox       

        /// <summary>
        ///     default constructor
        /// </summary>
        public NewOpeningsForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="profile">ProfileWall or ProfileFloor</param>
        public NewOpeningsForm(Profile profile)
            : this()
        {
            m_profile = profile;
            m_to2DMatrix = m_profile.To2DMatrix();
            m_moveToCenterMatrix = m_profile.ToCenterMatrix();
            InitTools();
        }

        /// <summary>
        ///     add tools, then use can draw by these tools in picture box
        /// </summary>
        private void InitTools()
        {
            //wall
            if (m_profile is ProfileWall)
            {
                m_tool = new RectTool();
                m_tools.Enqueue(m_tool);
                m_tools.Enqueue(new EmptyTool());
            }
            //floor
            else
            {
                m_tool = new LineTool();
                m_tools.Enqueue(m_tool);
                m_tools.Enqueue(new RectTool());
                m_tools.Enqueue(new CircleTool());
                m_tools.Enqueue(new ArcTool());
                m_tools.Enqueue(new EmptyTool());
            }
        }

        /// <summary>
        ///     use matrix to transform point
        /// </summary>
        /// <param name="pts">contain the points to be transform</param>
        private void TransFormPoints(Point[] pts)
        {
            var matrix = new Matrix(
                1, 0, 0, 1, openingPictureBox.Width / 2, openingPictureBox.Height / 2);
            matrix.Invert();
            matrix.TransformPoints(pts);
        }

        /// <summary>
        ///     get four points on circle by center and one point on circle
        /// </summary>
        /// <param name="points">contain the center and one point on circle</param>
        private List<Vector4> GenerateCircle4Point(List<Point> points)
        {
            var rotation = new Matrix();

            //get the circle center and bound point
            var center = points[0];
            var bound = points[1];
            rotation.RotateAt(90, center);
            var circle = new Point[4];
            circle[0] = points[1];
            for (var i = 1; i < 4; i++)
            {
                var ps = new Point[1] { bound };
                rotation.TransformPoints(ps);
                circle[i] = ps[0];
                bound = ps[0];
            }

            return TransForm2DTo3D(circle);
        }

        /// <summary>
        ///     Transform the point on Form to  3d world coordinate of Revit
        /// </summary>
        /// <param name="ps">contain the points to be transform</param>
        private List<Vector4> TransForm2DTo3D(Point[] ps)
        {
            var result = new List<Vector4>();
            TransFormPoints(ps);
            var transFormMatrix = Matrix4.Multiply(
                m_scaleMatrix.Inverse(), m_moveToCenterMatrix);
            transFormMatrix = Matrix4.Multiply(transFormMatrix, m_to2DMatrix);
            foreach (var point in ps)
            {
                var v = new Vector4(point.X, point.Y, 0);
                v = transFormMatrix.TransForm(v);
                result.Add(v);
            }

            return result;
        }

        /// <summary>
        ///     calculate the matrix use to scale
        /// </summary>
        /// <param name="size">pictureBox size</param>
        private Matrix4 ComputerScaleMatrix(Size size)
        {
            var boundPoints = m_profile.GetFaceBounds();
            var width = size.Width / (boundPoints[1].X - boundPoints[0].X);
            var hight = size.Height / (boundPoints[1].Y - boundPoints[0].Y);
            var factor = width <= hight ? width : hight;
            return new Matrix4(factor);
        }

        /// <summary>
        ///     Calculate the matrix use to transform 3D to 2D
        /// </summary>
        private Matrix4 Comuter3DTo2DMatrix()
        {
            var result = Matrix4.Multiply(
                m_to2DMatrix.Inverse(), m_moveToCenterMatrix.Inverse());
            result = Matrix4.Multiply(result, m_scaleMatrix);
            return result;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            foreach (var tool in m_tools)
            {
                var curcves = tool.GetLines();
                foreach (var curve in curcves)
                {
                    List<Vector4> ps3D;

                    switch (tool.ToolType)
                    {
                        case ToolType.Circle:
                            ps3D = GenerateCircle4Point(curve);
                            break;
                        case ToolType.Rectangle:
                        {
                            var ps = new Point[4]
                            {
                                curve[0], new Point(curve[0].X, curve[1].Y),
                                curve[1], new Point(curve[1].X, curve[0].Y)
                            };
                            ps3D = TransForm2DTo3D(ps);
                            break;
                        }
                        default:
                            ps3D = TransForm2DTo3D(curve.ToArray());
                            break;
                    }

                    m_profile.DrawOpening(ps3D, tool.ToolType);
                }
            }

            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openingPictureBox_Paint(object sender, PaintEventArgs e)
        {
            //Draw the pictures in the m_tools list
            foreach (var tool in m_tools) tool.Draw(e.Graphics);

            //draw the tips string
            e.Graphics.DrawString(m_tool.ToolType.ToString(),
                SystemFonts.DefaultFont, SystemBrushes.Highlight, 2, 5);

            //move the origin to the picture center
            var size = openingPictureBox.Size;
            e.Graphics.Transform = new Matrix(
                1, 0, 0, 1, size.Width / 2, size.Height / 2);

            //draw profile
            var scaleSize = new Size((int)(0.85 * size.Width), (int)(0.85 * size.Height));
            m_scaleMatrix = ComputerScaleMatrix(scaleSize);
            var trans = Comuter3DTo2DMatrix();
            m_profile.Draw2D(e.Graphics, Pens.Blue, trans);
        }

        /// <summary>
        ///     mouse event handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openingPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                case MouseButtons.Right:
                {
                    var g = openingPictureBox.CreateGraphics();

                    m_tool.OnMouseDown(g, e);
                    m_tool.OnRightMouseClick(g, e);
                    break;
                }
                case MouseButtons.Middle:
                {
                    m_tool.OnMidMouseDown(null, null);
                    m_tool = m_tools.Peek();
                    m_tools.Enqueue(m_tool);
                    m_tools.Dequeue();
                    var graphic = openingPictureBox.CreateGraphics();
                    graphic.DrawString(m_tool.ToolType.ToString(),
                        SystemFonts.DefaultFont, SystemBrushes.Highlight, 2, 5);
                    Refresh();
                    break;
                }
            }
        }

        /// <summary>
        ///     Mouse event handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openingPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            var g = openingPictureBox.CreateGraphics();
            m_tool.OnMouseUp(g, e);
        }

        /// <summary>
        ///     Mouse event handle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openingPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var graphics = openingPictureBox.CreateGraphics();
            m_tool.OnMouseMove(graphics, e);
            var paintArg = new PaintEventArgs(graphics, new Rectangle());
            openingPictureBox_Paint(null, paintArg);
        }
    }
}
