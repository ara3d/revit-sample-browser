// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public partial class ExportPdfOptionsForm : Form
    {
        private readonly ExportPdfData m_data;

        public ExportPdfOptionsForm(ExportPdfData data)
        {
            m_data = data;
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            checkBoxCombineViews.Checked = m_data.Combine;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            m_data.Combine = checkBoxCombineViews.Checked;
        }
    }
}
