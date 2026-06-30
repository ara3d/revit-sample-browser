// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
namespace Ara3D.RevitSampleBrowser.GridCreation.CS
{
    public partial class CreateWithSelectedCurvesForm : Form
    {
        private readonly CreateWithSelectedCurvesData m_data;

        public CreateWithSelectedCurvesForm(CreateWithSelectedCurvesData data)
        {
            m_data = data;

            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            comboBoxBubbleLocation.SelectedIndex = 1;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (ValidateValues())
                SetData();
            else
                DialogResult = DialogResult.None;
        }

        private bool ValidateValues()
        {
            return SampleBrowserUtils.ValidateLabel(textBoxFirstLabel, m_data.LabelsList);
        }

        private void SetData()
        {
            m_data.BubbleLocation = (BubbleLocation)comboBoxBubbleLocation.SelectedIndex;
            m_data.FirstLabel = textBoxFirstLabel.Text;
            m_data.DeleteSelectedElements = checkBoxDeleteElements.Checked;
        }
    }
}
