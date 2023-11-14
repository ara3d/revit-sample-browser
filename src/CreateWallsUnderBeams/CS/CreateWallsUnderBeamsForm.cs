// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.CreateWallsUnderBeams.CS
{
    public partial class CreateWallsUnderBeamsForm : Form
    {
        // Private members
        private readonly CreateWallsUnderBeams m_dataBuffer;

        /// <summary>
        ///     Constructor of CreateWallsUnderBeamsForm
        /// </summary>
        /// <param name="dataBuffer"> A reference of CreateWallsUnderBeams class </param>
        public CreateWallsUnderBeamsForm(CreateWallsUnderBeams dataBuffer)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            //Get a reference of CreateWallsUnderBeams
            m_dataBuffer = dataBuffer;
        }

        /// <summary>
        ///     Initialize the data on the form
        /// </summary>
        private void CreateWallsUnderBeamsForm_Load(object sender, EventArgs e)
        {
            wallTypeComboBox.DataSource = m_dataBuffer.WallTypes;
            wallTypeComboBox.DisplayMember = "Name";

            structualCheckBox.Checked = m_dataBuffer.IsSturctual;
        }

        /// <summary>
        ///     update the data to CreateWallsUnderBeams class
        /// </summary>
        private void OKButton_Click(object sender, EventArgs e)
        {
            m_dataBuffer.SelectedWallType = wallTypeComboBox.SelectedItem;

            m_dataBuffer.IsSturctual = structualCheckBox.Checked;
        }
    }
}
