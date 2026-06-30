// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.GenerateFloor.CS
{
    public partial class GenerateFloorForm : Form
    {
        private readonly Data m_data;

        public GenerateFloorForm(Data data)
        {
            m_data = data;
            InitializeComponent();
        }

        private void previewPictureBox_Paint(object sender, PaintEventArgs e)
        {
            double maxLength = previewPictureBox.Width > previewPictureBox.Height
                ? previewPictureBox.Width
                : previewPictureBox.Height;
            var scale = (float)(maxLength / m_data.MaxLength * 0.8);
            e.Graphics.ScaleTransform(scale, scale);
            e.Graphics.DrawLines(new Pen(Color.Red, 1), m_data.Points);
        }

        private void GenerateFloorForm_Load(object sender, EventArgs e)
        {
            floorTypesComboBox.DataSource = m_data.FloorTypesName;
            m_data.ChooseFloorType(floorTypesComboBox.Text);
        }

        private void floorTypesComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            m_data.ChooseFloorType(floorTypesComboBox.Text);
        }

        private void structralCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            m_data.Structural = structuralCheckBox.Checked;
        }
    }
}
