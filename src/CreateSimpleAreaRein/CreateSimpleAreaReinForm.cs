// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.CreateSimpleAreaRein.CS
{
    public partial class CreateSimpleAreaReinForm : Form
    {
        private readonly AreaReinData m_dataBuffer;

        public CreateSimpleAreaReinForm(AreaReinData dataBuffer)
        {
            InitializeComponent();

            m_dataBuffer = dataBuffer;
        }

        private void CreateSimpleAreaReinForm_Load(object sender, EventArgs e)
        {
            areaReinPropertyGrid.SelectedObject = m_dataBuffer;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
