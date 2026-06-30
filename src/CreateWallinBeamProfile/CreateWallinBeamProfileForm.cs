// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CreateWallinBeamProfile.CS
{
    public partial class CreateWallinBeamProfileForm : Form
    {
        private readonly CreateWallinBeamProfile m_dataBuffer;

        public CreateWallinBeamProfileForm(CreateWallinBeamProfile dataBuffer)
        {
            InitializeComponent();

            //Get a reference of CreateWallAndFloor
            m_dataBuffer = dataBuffer;
        }

        private void CreateWallinBeamProfileForm_Load(object sender, EventArgs e)
        {
            wallTypeComboBox.DataSource = m_dataBuffer.WallTypes;
            wallTypeComboBox.DisplayMember = "Name";

            structualCheckBox.Checked = m_dataBuffer.IsSturctual;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            m_dataBuffer.SelectedWallType = wallTypeComboBox.SelectedItem;

            m_dataBuffer.IsSturctual = structualCheckBox.Checked;
        }
    }
}
