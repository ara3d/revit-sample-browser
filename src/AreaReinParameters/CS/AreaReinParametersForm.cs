// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace RevitMultiSample.AreaReinParameters.CS
{
    public partial class AreaReinParametersForm : Form
    {
        private readonly IAreaReinData m_data;

        /// <summary>
        ///     initialize datasource
        /// </summary>
        /// <param name="data"></param>
        public AreaReinParametersForm(IAreaReinData data)
        {
            InitializeComponent();
            m_data = data;
        }

        /// <summary>
        ///     form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AreaReinParametersForm_Load(object sender, EventArgs e)
        {
            parameterGrid.SelectedObject = m_data;
        }

        /// <summary>
        ///     make changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        ///     cancel command
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
