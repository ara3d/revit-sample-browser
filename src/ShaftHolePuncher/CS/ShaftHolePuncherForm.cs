// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ShaftHolePuncher.CS
{
    /// <summary>
    ///     window form contains one picture box to show the
    ///     profile of a wall or floor, and three command buttons.
    ///     User can draw curves of opening in picture box.
    /// </summary>
    public partial class ShaftHolePuncherForm : Form
    {
        private readonly Profile m_profile; //save the profile data
        private readonly Size m_sizePictureBox; //size of picture box
        private readonly Tool m_tool; //current using tool

        /// <summary>
        ///     constructor
        /// </summary>
        public ShaftHolePuncherForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="profile">ProfileWall, ProfileFloor or ProfileNull</param>
        public ShaftHolePuncherForm(Profile profile)
            : this()
        {
            m_profile = profile;
            m_sizePictureBox = pictureBox.Size;

            if (profile is ProfileWall)
                m_tool = new RectangleTool();
            else
                m_tool = new LineTool();

            if (profile is ProfileNull)
            {
                ScaleComboBox.Visible = true;
                ScaleComboBox.SelectedIndex = 0;
                scaleLabel.Visible = true;
            }
            else if (profile is ProfileBeam)
            {
                DirectionPanel.Visible = true;
                DirectionComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        ///     store mouse location when mouse down
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            var graphics = pictureBox.CreateGraphics();
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            m_tool.OnMouseDown(e);
            pictureBox.Refresh();
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
        ///     draw the curve of floor (or wall) and curves of Opening
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            //Draw the pictures in the m_tools
            m_tool.Draw(e.Graphics);

            //get transform matrix
            m_profile.ComputeScaleMatrix(m_sizePictureBox);
            var trans = m_profile.Compute3DTo2DMatrix();

            //draw profile
            m_profile.Draw2D(e.Graphics, Pens.Blue, trans);
        }

        /// <summary>
        ///     clear all the curves of the Opening
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void ButtonClean_Click(object sender, EventArgs e)
        {
            m_tool.Clear();
            m_tool.Finished = false;
            pictureBox.Refresh();
        }

        /// <summary>
        ///     create Shaft Opening in Revit
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void CreateButton_Click(object sender, EventArgs e)
        {
            var points = m_tool.Points;

            if (!m_tool.Finished)
            {
                TaskDialog.Show("Revit", "Please finish the curve of Opening first!");
                return;
            }

            var ps3D = m_profile.Transform2DTo3D(points.ToArray());

            try
            {
                m_profile.CreateOpening(ps3D);
                Close();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Revit", ex.Message);
                ButtonClean_Click(null, null);
            }
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
        ///     set the scale of profile when create Shaft Opening
        /// </summary>
        /// <param name="sender">object who sent this event</param>
        /// <param name="e">event args</param>
        private void ScaleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var profile = m_profile as ProfileNull;
            profile.Scale = (float)Convert.ToDouble(ScaleComboBox.Text);
            m_profile.ComputeScaleMatrix(m_sizePictureBox);
        }

        private void DirectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_tool.Clear();
            m_tool.Finished = false;
            var profile = m_profile as ProfileBeam;
            if (0 == DirectionComboBox.SelectedIndex)
                profile.ChangeTransformMatrix(true);
            else if (1 == DirectionComboBox.SelectedIndex) profile.ChangeTransformMatrix(false);
            pictureBox.Refresh();
        }
    }
}
