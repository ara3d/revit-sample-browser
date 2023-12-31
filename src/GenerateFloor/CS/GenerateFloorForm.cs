// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.GenerateFloor.CS
{
    /// <summary>
    ///     User interface.
    /// </summary>
    public partial class GenerateFloorForm : Form
    {
        /// <summary>
        ///     the data get/set with revit.
        /// </summary>
        private readonly Data m_data;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="data"></param>
        public GenerateFloorForm(Data data)
        {
            m_data = data;
            InitializeComponent();
        }

        /// <summary>
        ///     paint the floor's profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void previewPictureBox_Paint(object sender, PaintEventArgs e)
        {
            double maxLength = previewPictureBox.Width > previewPictureBox.Height
                ? previewPictureBox.Width
                : previewPictureBox.Height;
            var scale = (float)(maxLength / m_data.MaxLength * 0.8);
            e.Graphics.ScaleTransform(scale, scale);
            e.Graphics.DrawLines(new Pen(Color.Red, 1), m_data.Points);
        }

        /// <summary>
        ///     initialize the data binding with revit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateFloorForm_Load(object sender, EventArgs e)
        {
            floorTypesComboBox.DataSource = m_data.FloorTypesName;
            m_data.ChooseFloorType(floorTypesComboBox.Text);
        }

        /// <summary>
        ///     set the floor type to be create.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void floorTypesComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            m_data.ChooseFloorType(floorTypesComboBox.Text);
        }

        /// <summary>
        ///     set if the floor to be create is structural.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void structralCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_data.Structural = structuralCheckBox.Checked;
        }
    }
}
