// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.Loads.CS
{
    public partial class LoadsForm : Form
    {
        // Private members
        private readonly Loads m_dataBuffer; // A reference of Loads.

        public LoadsForm(Loads dataBuffer)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            //Get a reference of LoadsForm
            m_dataBuffer = dataBuffer;
        }

        private void LoadsForm_Load(object sender, EventArgs e)
        {
            // Initialize the data of loadCaseTabPage 
            InitializeLoadCasePage();

            // Initialize the data of LoadCombinationsTabPage
            InitializeLoadCombinationPage();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
