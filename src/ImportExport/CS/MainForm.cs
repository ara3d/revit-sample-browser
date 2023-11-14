// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;

namespace RevitMultiSample.ImportExport.CS
{
    /// <summary>
    ///     Provide a dialog which lets user choose the operation(export or import)
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        ///     Data class
        /// </summary>
        private readonly MainData m_mainData;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="mainData"></param>
        public MainForm(MainData mainData)
        {
            m_mainData = mainData;
            InitializeComponent();
            radioButtonExport.Checked = true;
            comboBoxExport.Enabled = true;
            comboBoxImport.Enabled = false;
            //Append formats to be exported or imported
            InitializeFormats();
        }

        /// <summary>
        ///     Append formats to be exported or imported
        /// </summary>
        private void InitializeFormats()
        {
            // Append formats to be exported
            comboBoxExport.Items.Add("DWG");
            comboBoxExport.Items.Add("DXF");
            comboBoxExport.Items.Add("SAT");
            comboBoxExport.Items.Add("DWF");
            comboBoxExport.Items.Add("DWFx");
            comboBoxExport.Items.Add("GBXML");
            comboBoxExport.Items.Add("DGN");
            comboBoxExport.Items.Add("PDF");
            if (m_mainData.Is3DView) comboBoxExport.Items.Add("FBX");
            comboBoxExport.Items.Add("IMAGE");
            comboBoxExport.SelectedIndex = 0;

            // Append formats to be imported
            comboBoxImport.Items.Add("DWG");
            if (!m_mainData.Is3DView) comboBoxImport.Items.Add("IMAGE");

            if (m_mainData.CommandData.Application.Application.Product == ProductType.MEP)
                comboBoxImport.Items.Add("GBXML");
            comboBoxImport.Items.Add("Inventor");

            comboBoxImport.SelectedIndex = 0;
        }

        /// <summary>
        ///     Show the export/import dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            var selectedFormat = string.Empty;
            var result = DialogResult.OK;

            if (radioButtonExport.Checked)
            {
                selectedFormat = comboBoxExport.SelectedItem.ToString();
                result = m_mainData.Export(selectedFormat);
            }
            else
            {
                selectedFormat = comboBoxImport.SelectedItem.ToString();
                result = m_mainData.Import(selectedFormat);
            }

            DialogResult = result != DialogResult.Cancel ? DialogResult.OK : DialogResult.None;
        }

        private void radioButtonExport_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxImport.Enabled = !radioButtonExport.Checked;
            comboBoxExport.Enabled = radioButtonExport.Checked;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
