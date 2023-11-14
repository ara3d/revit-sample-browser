// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace RevitMultiSample.NewHostedSweep.CS
{
    /// <summary>
    ///     This form contains a property grid control to modify the property of hosted sweep.
    /// </summary>
    public partial class HostedSweepModifyForm : Form
    {
        /// <summary>
        ///     Data for modification.
        /// </summary>
        private readonly ModificationData m_modificationData;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public HostedSweepModifyForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Customize constructor contains a parameter ModificationData.
        /// </summary>
        /// <param name="modificationData"></param>
        public HostedSweepModifyForm(ModificationData modificationData)
            : this()
        {
            m_modificationData = modificationData;
            Text = "Modify " + m_modificationData.CreatorName;
        }

        /// <summary>
        ///     OK button, exit this form with DialogResult.OK.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        ///     Load event, set the data source for property-grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HostedSweepModify_Load(object sender, EventArgs e)
        {
            propertyGrid.SelectedObject = m_modificationData;
            m_modificationData.ShowElement();
        }
    }
}
