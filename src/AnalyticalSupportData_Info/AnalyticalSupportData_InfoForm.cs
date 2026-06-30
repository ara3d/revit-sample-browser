// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.AnalyticalSupportData_Info.CS
{
    public partial class AnalyticalSupportDataInfoForm : Form
    {
        private readonly Command m_dataBuffer;

        private AnalyticalSupportDataInfoForm()
        {
            InitializeComponent();
        }

        public AnalyticalSupportDataInfoForm(Command dataBuffer) : this()
        {
            m_dataBuffer = dataBuffer;
            elementInfoDataGridView.AutoGenerateColumns = false;
            elementInfoDataGridView.DataSource = m_dataBuffer.ElementInformation;

            id.DataPropertyName = "Id";
            typeName.DataPropertyName = "Element Type";
            support.DataPropertyName = "Support Type";
            remark.DataPropertyName = "Remark";
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
