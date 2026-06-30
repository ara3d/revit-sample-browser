// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CreateWallsUnderBeams.CS
{
    public partial class CreateWallsUnderBeamsForm : Form
    {
        private readonly CreateWallsUnderBeams m_dataBuffer;

        public CreateWallsUnderBeamsForm(CreateWallsUnderBeams dataBuffer)
        {
            InitializeComponent();

            //Get a reference of CreateWallsUnderBeams
            m_dataBuffer = dataBuffer;
        }

        private void CreateWallsUnderBeamsForm_Load(object sender, EventArgs e)
        {
            wallTypeComboBox.DataSource = m_dataBuffer.WallTypes;
            wallTypeComboBox.DisplayMember = "Name";

            structualCheckBox.Checked = m_dataBuffer.IsSturctual;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            m_dataBuffer.SelectedWallType = wallTypeComboBox.SelectedItem;

            m_dataBuffer.IsSturctual = structualCheckBox.Checked;
        }
    }
}
