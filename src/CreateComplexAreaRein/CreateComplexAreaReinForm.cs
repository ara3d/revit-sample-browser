// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.AreaReinParameters.CS;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CreateComplexAreaRein.CS
{
    public partial class CreateComplexAreaReinForm : Form
    {
        private readonly AreaReinData m_dataBuffer;

        public CreateComplexAreaReinForm(AreaReinData dataBuffer)
        {
            InitializeComponent();

            m_dataBuffer = dataBuffer;
        }

        private void CreateComplexAreaReinForm_Load(object sender, EventArgs e)
        {
            layoutRuleComboBox.DataSource = Enum.GetNames(typeof(LayoutRules));
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            m_dataBuffer.LayoutRule = (LayoutRules)Enum.Parse(typeof(LayoutRules),
                layoutRuleComboBox.SelectedItem.ToString());
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
