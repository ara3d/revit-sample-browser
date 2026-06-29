// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;
using Point = System.Drawing.Point;

namespace Ara3D.RevitSampleBrowser.NewPathReinforcement.CS
{
    /// <summary>
    ///     window form contains one picture box to show the
    ///     profile of wall (or slab), and four command buttons and a check box.
    ///     User can draw PathReinforcement on picture box.
    /// </summary>
    public partial class NewPathReinforcementForm : Form
    {
        private readonly Matrix4 m_moveToCenterMatrix; //save the matrix use to move point to origin
        private List<List<XYZ>> m_pointsPreview; //store all the points of the preview
        private bool m_previewMode; //indicate whether in the Preview Mode
        private readonly Profile m_profile; //save the profile date (ProfileFloor or ProfileWall)
        private Matrix4 m_scaleMatrix; //save the matrix use to scale
        private Size m_sizePictureBox; //size of picture box
        private readonly Matrix4 m_to2DMatrix; //save the matrix use to transform 3D to 2D
        private readonly LineTool m_tool; //current using tool
        private Matrix4 m_transMatrix; //save the final matrix

        /// <summary>
        ///     constructor
        /// </summary>
        public NewPathReinforcementForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="profile">ProfileWall or ProfileFloor</param>
        public NewPathReinforcementForm(Profile profile)
            : this()
        {
            m_profile = profile;
            m_to2DMatrix = m_profile.To2DMatrix;
            m_moveToCenterMatrix = m_profile.ToCenterMatrix();
            m_tool = new LineTool();
            KeyPreview = true; //respond Form events first
            m_pointsPreview = new List<List<XYZ>>();
            m_sizePictureBox = pictureBox.Size;
        }

        /// <summary>
        ///     store mouse location when mouse down
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (!m_previewMode)
            {
                var graphics = pictureBox.CreateGraphics();
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                m_tool.OnMouseDown(e);
                pictureBox.Refresh();
                if (m_tool.PointsNumber >= 2) previewButton.Enabled = true;
            }
        }

        /// <summary>
        ///     draw the line to where mouse moved
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox.Refresh();
            var graphics = pictureBox.CreateGraphics();
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            m_tool.OnMouseMove(graphics, e);
        }

        /// <summary>
        ///     draw the curve of slab (or wall) and path of PathReinforcement
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            if (m_previewMode)
            {
                //Draw the geometry of Path Reinforcement
                DrawPreview(e.Graphics);
                //move the gdi origin to the picture center
                e.Graphics.Transform = new
                    Matrix(1, 0, 0, 1, 0, 0);
                m_tool.Draw(e.Graphics);
            }
            else
            {
                //Draw the pictures in the m_tools list
                m_tool.Draw(e.Graphics);
            }

            //move the gdi origin to the picture center
            e.Graphics.Transform = new Matrix(
                1, 0, 0, 1, m_sizePictureBox.Width / 2, m_sizePictureBox.Height / 2);

            //get matrix
            var scaleSize = new Size((int)(0.85 * m_sizePictureBox.Width),
                (int)(0.85 * m_sizePictureBox.Height));
            m_scaleMatrix = ComputeScaleMatrix(scaleSize);
            m_transMatrix = Compute3DTo2DMatrix();

            //draw profile
            m_profile.Draw2D(e.Graphics, Pens.Blue, m_transMatrix);
        }

        /// <summary>
        ///     draw all the curves of PathReinforcement when click "Preview" Button
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void ButtonPreview_Click(object sender, EventArgs e)
        {
            m_previewMode = true;
            m_tool.Finished = true;
            //get points to create PathReinforcement
            var points = m_tool.GetPoints();
            if (points.Count <= 1)
            {
                TaskDialog.Show("Revit", "Please draw Path of Reinforcement before create!");
                return;
            }

            var ps3D = Transform2DTo3D(points.ToArray());

            //begin Transaction, so action here can be aborted.
            var transaction = new Transaction(m_profile.CommandData.Application.ActiveUIDocument.Document,
                "CreatePathReinforcement");
            transaction.Start();

            var pathRein = m_profile.CreatePathReinforcement(ps3D, flipCheckBox.Checked);
            m_pointsPreview = GetGeometry(pathRein);

            //Abort Transaction here, cancel what RevitAPI do after begin Transaction
            transaction.RollBack();

            pictureBox.Refresh();
        }

        /// <summary>
        ///     redraw curve of PathReinforcement when "flip" changed
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void FlipCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (m_previewMode) ButtonPreview_Click(null, null);
        }

        /// <summary>
        ///     clean all the points of the PathReinforcement when click "Clean" Button
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void ButtonClean_Click(object sender, EventArgs e)
        {
            m_tool.Clear();
            m_tool.Finished = false;
            m_previewMode = false;
            previewButton.Enabled = false;
            pictureBox.Refresh();
        }

        /// <summary>
        ///     stop drawing path when click "Escape" button
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void NewPathReinforcementForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            //finish the sketch when press "Escape"
            if (27 == e.KeyChar && m_tool.PointsNumber >= 2 && !m_tool.Finished)
            {
                m_tool.Finished = true;
                pictureBox.Refresh();
            }
            //Close form if sketch has finished when press "Escape"
            else if (27 == e.KeyChar && (m_tool.Finished || 0 == m_tool.PointsNumber))
            {
                Close();
            }
        }

        /// <summary>
        ///     create PathReinforcement in Revit
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void CreateButton_Click(object sender, EventArgs e)
        {
            var points = m_tool.GetPoints();

            if (0 == points.Count || 1 == points.Count)
            {
                TaskDialog.Show("Revit", "Please draw Path of Reinforcement before create!");
                return;
            }

            //begin Transaction, so action here can be aborted.
            var transaction = new Transaction(m_profile.CommandData.Application.ActiveUIDocument.Document,
                "CreatePathReinforcement");
            transaction.Start();

            var ps3D = Transform2DTo3D(points.ToArray());
            m_profile.CreatePathReinforcement(ps3D, flipCheckBox.Checked);

            transaction.Commit();

            Close();
        }

        /// <summary>
        ///     close the form
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     transform the point on Form to  3d world coordinate of revit
        /// </summary>
        /// <param name="ps">contain the points to be transformed</param>
        private List<Vector4> Transform2DTo3D(Point[] ps)
        {
            var result = new List<Vector4>();
            TransformPoints(ps);
            var transformMatrix = Matrix4.Multiply(
                m_scaleMatrix.Inverse(), m_moveToCenterMatrix);
            transformMatrix = Matrix4.Multiply(transformMatrix, m_to2DMatrix);
            foreach (var point in ps)
            {
                var v = new Vector4(point.X, point.Y, 0);
                v = transformMatrix.Transform(v);
                result.Add(v);
            }

            return result;
        }

        /// <summary>
        ///     calculate the matrix use to scale
        /// </summary>
        /// <param name="size">pictureBox size</param>
        private Matrix4 ComputeScaleMatrix(Size size)
        {
            var boundPoints = m_profile.GetFaceBounds();
            var width = size.Width / (boundPoints[1].X - boundPoints[0].X);
            var hight = size.Height / (boundPoints[1].Y - boundPoints[0].Y);
            var factor = width <= hight ? width : hight;
            return new Matrix4(factor);
        }

        /// <summary>
        ///     calculate the matrix used to transform 3D to 2D
        /// </summary>
        private Matrix4 Compute3DTo2DMatrix()
        {
            var result = Matrix4.Multiply(
                m_to2DMatrix.Inverse(), m_moveToCenterMatrix.Inverse());
            result = Matrix4.Multiply(result, m_scaleMatrix);
            return result;
        }

        /// <summary>
        ///     use matrix to transform point
        /// </summary>
        /// <param name="pts">contain the points to be transform</param>
        private void TransformPoints(Point[] pts)
        {
            var matrix = new Matrix(
                1, 0, 0, 1, pictureBox.Width / 2, pictureBox.Height / 2);
            matrix.Invert();
            matrix.TransformPoints(pts);
        }

        /// <summary>
        ///     Get geometry of the pathReinforcement
        /// </summary>
        /// <param name="pathRein">pathReinforcement created</param>
        private List<List<XYZ>> GetGeometry(Autodesk.Revit.DB.Structure.PathReinforcement pathRein)
        {
            var options = m_profile.CommandData.Application.Application.Create.NewGeometryOptions();
            options.DetailLevel = ViewDetailLevel.Medium;
            options.ComputeReferences = true;
            var geoElem = pathRein.get_Geometry(options);
            var curvesList = new List<Curve>();
            //GeometryObjectArray gObjects = geoElem.Objects;
            var objects = geoElem.GetEnumerator();
            //foreach (GeometryObject geo in gObjects)
            while (objects.MoveNext())
            {
                var geo = objects.Current;

                var curve = geo as Curve;
                curvesList.Add(curve);
            }

            var pointsPreview = new List<List<XYZ>>();
            foreach (var curve in curvesList)
            {
                pointsPreview.Add(curve.Tessellate() as List<XYZ>);
            }

            return pointsPreview;
        }

        /// <summary>
        ///     draw points in the List on the form
        /// </summary>
        /// <param name="graphics">form graphic</param>
        /// <param name="pen">pen used to draw line in pictureBox</param>
        /// <param name="matrix4">
        ///     Matrix used to transform 3d to 2d
        ///     and make picture in right scale
        /// </param>
        /// <param name="points">points need to be drawn</param>
        private void DrawPoints(Graphics graphics, Pen pen, Matrix4 matrix4, List<List<XYZ>> points)
        {
            foreach (var xyzArray in points)
            {
                for (var i = 0; i < xyzArray.Count - 1; i++)
                {
                    var point1 = xyzArray[i];
                    var point2 = xyzArray[i + 1];

                    var v1 = new Vector4(point1);
                    var v2 = new Vector4(point2);

                    v1 = matrix4.Transform(v1);
                    v2 = matrix4.Transform(v2);
                    graphics.DrawLine(pen, new Point((int)v1.X, (int)v1.Y),
                        new Point((int)v2.X, (int)v2.Y));
                }
            }
        }

        /// <summary>
        ///     draw preview of Path Reinforcement on the form
        /// </summary>
        private void DrawPreview(Graphics graphics)
        {
            if (null == m_pointsPreview) return;

            //move the gdi origin to the picture center
            graphics.Transform = new Matrix(
                1, 0, 0, 1, m_sizePictureBox.Width / 2, m_sizePictureBox.Height / 2);
            DrawPoints(graphics, Pens.Red, m_transMatrix, m_pointsPreview);
        }
    }
}
