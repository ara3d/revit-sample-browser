// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt


using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.Loads.CS
{
    public partial class LoadsForm : Form
    {
        // Private members
        private readonly Loads m_dataBuffer; // A reference of Loads.

        /// <summary>
        ///     Constructor of LoadsForm
        /// </summary>
        /// <param name="dataBuffer"> A reference of Loads class </param>
        public LoadsForm(Loads dataBuffer)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            //Get a reference of LoadsForm
            m_dataBuffer = dataBuffer;
        }

        /// <summary>
        ///     Initialize the data on the form
        /// </summary>
        private void LoadsForm_Load(object sender, EventArgs e)
        {
            // Initialize the data of loadCaseTabPage 
            InitializeLoadCasePage();

            // Initialize the data of LoadCombinationsTabPage
            InitializeLoadCombinationPage();
        }

        /// <summary>
        ///     Respond the ok button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        ///     Respond the cancel button click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
