// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Color = System.Drawing.Color;
using Form = System.Windows.Forms.Form;

namespace Revit.SDK.Samples.SlabShapeEditing.CS
{
    /// <summary>
    ///     window form contains one picture box to show the
    ///     profile of slab geometry. user can add vertex and crease.
    ///     User can edit slab shape via vertex and crease too.
    /// </summary>
    public partial class SlabShapeEditingForm : Form
    {
        private const string JustNumber = "Please input numbers in textbox!"; //error message
        private const string SelectFirst = "Please select a Vertex (or Crease) first!"; //error message
        private EditorState m_editorState; //state of user's operation
        private int m_clickedIndex; //index of crease and vertex which mouse clicked.

        private readonly ExternalCommandData m_commandData; //object which contains reference of Revit Application
        private readonly ArrayList m_createCreases; // new created creases
        private readonly ArrayList m_createdVertices; // new created vertices
        private readonly ArrayList m_graphicsPaths; //store all the GraphicsPath objects of crease and vertex.
        private readonly LineTool m_lineTool; //tool use to draw crease
        private PointF m_mouseRightDownLocation; //where mouse right button down
        private readonly LineTool m_pointTool; //tool use to draw vertex
        private readonly Pen m_profilePen; // pen use to draw slab's profile
        private SlabShapeCrease m_selectedCrease; //selected crease, mouse clicked on
        private SlabShapeVertex m_selectedVertex; //selected vertex, mouse clicked on
        private int m_selectIndex; //index of crease and vertex which mouse hovering on.
        private readonly Pen m_selectPen; // pen use to draw vertex and crease which been selected
        private readonly SlabProfile m_slabProfile; //store geometry info of selected slab
        private readonly SlabShapeEditor m_slabShapeEditor; //object use to edit slab shape
        private readonly Pen m_toolPen; //pen use to draw new created vertex and crease

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="commandData">selected floor (or slab)</param>
        /// <param name="commandData">contains reference of Revit Application</param>
        public SlabShapeEditingForm(Floor floor, ExternalCommandData commandData)
        {
            InitializeComponent();
            m_commandData = commandData;
            m_slabProfile = new SlabProfile(floor, commandData);
            m_slabShapeEditor = floor.GetSlabShapeEditor();
            m_lineTool = new LineTool();
            m_pointTool = new LineTool();
            m_editorState = EditorState.AddVertex;
            m_graphicsPaths = new ArrayList();
            m_createdVertices = new ArrayList();
            m_createCreases = new ArrayList();
            m_selectIndex = -1;
            m_clickedIndex = -1;
            m_toolPen = new Pen(Color.Blue, 2);
            m_selectPen = new Pen(Color.Red, 2);
            m_profilePen = new Pen(Color.Black, (float)0.5);
        }

        /// <summary>
        ///     represents the geometry info for slab
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void SlabShapePictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            m_slabProfile.Draw2D(e.Graphics, m_profilePen);

            if (EditorState.Rotate != m_editorState)
            {
                m_lineTool.Draw2D(e.Graphics, m_toolPen);
                m_pointTool.DrawRectangle(e.Graphics, m_toolPen);
                //draw selected beam (line) by red pen
                DrawSelectedLineRed(e.Graphics, m_selectPen);
            }
        }

        /// <summary>
        ///     Draw selected crease or vertex red
        /// </summary>
        /// <param name="graphics">Form graphics object,</param>
        /// <param name="pen">Pen which used to draw lines</param>
        private void DrawSelectedLineRed(Graphics graphics, Pen pen)
        {
            if (-1 != m_selectIndex)
            {
                var selectedPath = (GraphicsPath)m_graphicsPaths[m_selectIndex];
                var pointF0 = (PointF)selectedPath.PathPoints.GetValue(0);
                var pointF1 = (PointF)selectedPath.PathPoints.GetValue(1);
                if (m_selectIndex < m_createCreases.Count)
                    graphics.DrawLine(pen, pointF0, pointF1);
                else
                    graphics.DrawRectangle(pen, pointF0.X - 2, pointF0.Y - 2, 4, 4);
            }

            if (-1 != m_clickedIndex)
            {
                var clickedPath = (GraphicsPath)m_graphicsPaths[m_clickedIndex];
                var pointF0 = (PointF)clickedPath.PathPoints.GetValue(0);
                var pointF1 = (PointF)clickedPath.PathPoints.GetValue(1);
                if (m_clickedIndex < m_createCreases.Count)
                    graphics.DrawLine(pen, pointF0, pointF1);
                else
                    graphics.DrawRectangle(pen, pointF0.X - 2, pointF0.Y - 2, 4, 4);
            }
        }

        /// <summary>
        ///     rotate slab and get selected vertex or crease
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void SlabShapePictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            var pointF = new PointF(e.X, e.Y);
            if (EditorState.AddCrease == m_editorState && 1 == m_lineTool.Points.Count % 2)
                m_lineTool.MovePoint = pointF;
            else
                m_lineTool.MovePoint = PointF.Empty;

            if (MouseButtons.Right == e.Button)
            {
                double moveX = e.Location.X - m_mouseRightDownLocation.X;
                double moveY = m_mouseRightDownLocation.Y - e.Location.Y;
                m_slabProfile.RotateFloor(moveY / 500, moveX / 500);
                m_mouseRightDownLocation = e.Location;
            }
            else if (EditorState.Select == m_editorState)
            {
                for (var i = 0; i < m_graphicsPaths.Count; i++)
                {
                    var path = (GraphicsPath)m_graphicsPaths[i];
                    if (path.IsOutlineVisible(pointF, m_toolPen))
                    {
                        m_selectIndex = i;
                        break;
                    }

                    m_selectIndex = -1;
                }
            }

            SlabShapePictureBox.Refresh();
        }

        /// <summary>
        ///     get location where right button click down.
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void SlabShapePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Right == e.Button)
            {
                m_mouseRightDownLocation = e.Location;
                m_editorState = EditorState.Rotate;
                m_clickedIndex = m_selectIndex = -1;
            }
        }

        /// <summary>
        ///     add vertex and crease, select new created vertex and crease
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void SlabShapePictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            switch (m_editorState)
            {
                case EditorState.AddCrease when !m_slabProfile.CanCreateVertex(new PointF(e.X, e.Y)):
                    return;
                case EditorState.AddCrease:
                {
                    m_lineTool.Points.Add(new PointF(e.X, e.Y));
                    var lineSize = m_lineTool.Points.Count;
                    if (0 == m_lineTool.Points.Count % 2)
                        m_createCreases.Add(
                            m_slabProfile.AddCrease((PointF)m_lineTool.Points[lineSize - 2],
                                (PointF)m_lineTool.Points[lineSize - 1]));
                    CreateGraphicsPath(); //create graphic path for all the vertex and crease
                    break;
                }
                case EditorState.AddVertex:
                {
                    var vertex = m_slabProfile.AddVertex(new PointF(e.X, e.Y));
                    if (null == vertex) return;
                    m_pointTool.Points.Add(new PointF(e.X, e.Y));
                    //draw point as a short line, so add two points here
                    m_pointTool.Points.Add(new PointF(e.X + 2, e.Y + 2));
                    m_createdVertices.Add(vertex);
                    CreateGraphicsPath(); //create graphic path for all the vertex and crease
                    break;
                }
                case EditorState.Select when m_selectIndex >= 0:
                {
                    m_clickedIndex = m_selectIndex;
                    if (m_selectIndex <= m_createCreases.Count - 1)
                    {
                        m_selectedCrease = (SlabShapeCrease)m_createCreases[m_selectIndex];
                        m_selectedVertex = null;
                    }
                    else
                    {
                        //put all path (crease and vertex) in one arrayList, so reduce creases.count
                        var index = m_selectIndex - m_createCreases.Count;
                        m_selectedVertex = (SlabShapeVertex)m_createdVertices[index];
                        m_selectedCrease = null;
                    }

                    break;
                }
                case EditorState.Select:
                    m_selectedVertex = null;
                    m_selectedCrease = null;
                    m_clickedIndex = -1;
                    break;
            }

            SlabShapePictureBox.Refresh();
        }

        /// <summary>
        ///     get ready to add vertex
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void PointButton_Click(object sender, EventArgs e)
        {
            m_editorState = EditorState.AddVertex;
            m_slabProfile.ClearRotateMatrix();
            SlabShapePictureBox.Cursor = Cursors.Cross;
        }

        /// <summary>
        ///     get ready to add crease
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void LineButton_Click(object sender, EventArgs e)
        {
            m_editorState = EditorState.AddCrease;
            m_slabProfile.ClearRotateMatrix();
            SlabShapePictureBox.Cursor = Cursors.Cross;
        }

        /// <summary>
        ///     get ready to move vertex and crease
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void MoveButton_Click(object sender, EventArgs e)
        {
            m_editorState = EditorState.Select;
            m_slabProfile.ClearRotateMatrix();
            SlabShapePictureBox.Cursor = Cursors.Arrow;
        }

        /// <summary>
        ///     Move vertex and crease, then update profile of slab
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (-1 == m_clickedIndex)
            {
                TaskDialog.Show("Revit", SelectFirst);
                return;
            }

            double moveDistance = 0;
            try
            {
                moveDistance = Convert.ToDouble(DistanceTextBox.Text);
            }
            catch (Exception)
            {
                TaskDialog.Show("Revit", JustNumber);
                return;
            }

            var transaction = new Transaction(
                m_commandData.Application.ActiveUIDocument.Document, "Update");
            transaction.Start();
            if (null != m_selectedCrease)
                m_slabShapeEditor.ModifySubElement(m_selectedCrease, moveDistance);
            else if (null != m_selectedVertex) m_slabShapeEditor.ModifySubElement(m_selectedVertex, moveDistance);
            transaction.Commit();
            //re-calculate geometry info
            m_slabProfile.GetSlabProfileInfo();
            SlabShapePictureBox.Refresh();
        }

        /// <summary>
        ///     Reset slab shape
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void ResetButton_Click(object sender, EventArgs e)
        {
            m_slabProfile.ResetSlabShape();
            m_lineTool.Points.Clear();
            m_pointTool.Points.Clear();
        }

        /// <summary>
        ///     Create Graphics Path for each vertex and crease
        /// </summary>
        public void CreateGraphicsPath()
        {
            m_graphicsPaths.Clear();
            //create path for all the lines draw by user
            for (var i = 0; i < m_lineTool.Points.Count - 1; i += 2)
            {
                var path = new GraphicsPath();
                path.AddLine((PointF)m_lineTool.Points[i], (PointF)m_lineTool.Points[i + 1]);
                m_graphicsPaths.Add(path);
            }

            for (var i = 0; i < m_pointTool.Points.Count - 1; i += 2)
            {
                var path = new GraphicsPath();
                path.AddLine((PointF)m_pointTool.Points[i], (PointF)m_pointTool.Points[i + 1]);
                m_graphicsPaths.Add(path);
            }
        }

        /// <summary>
        ///     set tool tip for MoveButton
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void MoveButton_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(MoveButton, "Select Vertex or Crease");
        }

        /// <summary>
        ///     set tool tip for PointButton
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void PointButton_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(PointButton, "Add Vertex");
        }

        /// <summary>
        ///     set tool tip for LineButton
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void LineButton_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(LineButton, "Add Crease");
        }

        /// <summary>
        ///     change cursor
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void SlabShapePictureBox_MouseHover(object sender, EventArgs e)
        {
            switch (m_editorState)
            {
                case EditorState.AddVertex:
                    SlabShapePictureBox.Cursor = Cursors.Cross;
                    break;
                case EditorState.AddCrease:
                    SlabShapePictureBox.Cursor = Cursors.Cross;
                    break;
                case EditorState.Select:
                    SlabShapePictureBox.Cursor = Cursors.Arrow;
                    break;
                default:
                    SlabShapePictureBox.Cursor = Cursors.Default;
                    break;
            }
        }

        private enum EditorState
        {
            AddVertex,
            AddCrease,
            Select,
            Rotate,
            Null
        }
    }
}
