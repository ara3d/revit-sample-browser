// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.AreaReinParameters.CS
{
    public partial class AreaReinParametersForm : Form
    {
        private readonly IAreaReinData m_data;

        public AreaReinParametersForm(IAreaReinData data)
        {
            InitializeComponent();
            m_data = data;
        }

        private void AreaReinParametersForm_Load(object sender, EventArgs e)
        {
            parameterGrid.SelectedObject = m_data;
        }

        private void OKButton_Click(object sender, EventArgs e)
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
