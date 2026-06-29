// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CreateWallinBeamProfile.CS
{
    public partial class CreateWallinBeamProfileForm : Form
    {
        // Private members
        private readonly CreateWallinBeamProfile m_dataBuffer;

        /// <summary>
        ///     Constructor of CreateWallinBeamProfileForm
        /// </summary>
        /// <param name="dataBuffer"> A reference of CreateWallinBeamProfile class </param>
        public CreateWallinBeamProfileForm(CreateWallinBeamProfile dataBuffer)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            //Get a reference of CreateWallAndFloor
            m_dataBuffer = dataBuffer;
        }

        /// <summary>
        ///     Initialize the data on the form
        /// </summary>
        private void CreateWallinBeamProfileForm_Load(object sender, EventArgs e)
        {
            wallTypeComboBox.DataSource = m_dataBuffer.WallTypes;
            wallTypeComboBox.DisplayMember = "Name";

            structualCheckBox.Checked = m_dataBuffer.IsSturctual;
        }

        /// <summary>
        ///     update the data to CreateWallinBeamProfile class
        /// </summary>
        private void okButton_Click(object sender, EventArgs e)
        {
            m_dataBuffer.SelectedWallType = wallTypeComboBox.SelectedItem;

            m_dataBuffer.IsSturctual = structualCheckBox.Checked;
        }
    }
}
