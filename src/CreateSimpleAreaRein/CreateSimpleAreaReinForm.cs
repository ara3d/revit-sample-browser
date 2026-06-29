// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS
{
    /// <summary>
    ///     simple business process of UI
    /// </summary>
    public partial class CreateSimpleAreaReinForm : Form
    {
        private readonly AreaReinData m_dataBuffer;

        /// <summary>
        ///     constructor; initialize member data
        /// </summary>
        /// <param name="dataBuffer"></param>
        public CreateSimpleAreaReinForm(AreaReinData dataBuffer)
        {
            InitializeComponent();

            m_dataBuffer = dataBuffer;
        }

        /// <summary>
        ///     bind data to controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateSimpleAreaReinForm_Load(object sender, EventArgs e)
        {
            areaReinPropertyGrid.SelectedObject = m_dataBuffer;
        }

        /// <summary>
        ///     to create
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        ///     cancel the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
