// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     Data class which stores information of lower priority for exporting PDF format.
    /// </summary>
    public partial class ExportPdfOptionsForm : Form
    {
        /// <summary>
        ///     ExportPDFData object
        /// </summary>
        private readonly ExportPdfData m_data;

        /// <summary>
        ///     ExportPDFOptionsForm constructor
        /// </summary>
        public ExportPdfOptionsForm(ExportPdfData data)
        {
            m_data = data;
            InitializeComponent();
            Initialize();
        }

        /// <summary>
        ///     Initialize controls
        /// </summary>
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
