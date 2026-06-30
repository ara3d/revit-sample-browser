// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.ApplicationServices;
using System;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS
{
    public partial class MainForm : Form
    {
        private readonly MainData m_mainData;

        public MainForm(MainData mainData)
        {
            m_mainData = mainData;
            InitializeComponent();
            radioButtonExport.Checked = true;
            comboBoxExport.Enabled = true;
            comboBoxImport.Enabled = false;
            InitializeFormats();
        }

        private void InitializeFormats()
        {
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
            comboBoxImport.Items.Add("DWG");
            if (!m_mainData.Is3DView) comboBoxImport.Items.Add("IMAGE");

            if (m_mainData.CommandData.Application.Application.Product == ProductType.MEP)
                comboBoxImport.Items.Add("GBXML");
            comboBoxImport.Items.Add("Inventor");

            comboBoxImport.SelectedIndex = 0;
        }

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
