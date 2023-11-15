// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.DB;
using Form = System.Windows.Forms.Form;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS
{
    /// <summary>
    ///     Data class which stores information of lower priority for exporting DWF(x) format.
    /// </summary>
    public partial class ExportDwfOptionForm : Form
    {
        /// <summary>
        ///     ExportDWFData object
        /// </summary>
        private readonly ExportDwfData m_data;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="data">Data class object</param>
        public ExportDwfOptionForm(ExportDwfData data)
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
            checkBoxModelElements.Checked = m_data.ExportObjectData;
            checkBoxRoomsAndAreas.Checked = m_data.ExportAreas;
            radioButtonStandardFormat.Checked = true;
            radioButtonCompressedFormat.Checked = false;
            labelImageQuality.Enabled = false;
            comboBoxImageQuality.Enabled = false;
            comboBoxImageQuality.DataSource = m_data.ImageQualities;
            checkBoxMergeViews.Enabled = !m_data.CurrentViewOnly;
            checkBoxMergeViews.Checked = !m_data.CurrentViewOnly;
            checkBoxMergeViews.Text = "Combine selected views and sheets into a single file";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            m_data.ExportObjectData = checkBoxModelElements.Checked;
            m_data.ExportAreas = checkBoxRoomsAndAreas.Enabled ? checkBoxRoomsAndAreas.Checked : false;
            m_data.ExportMergeFiles = checkBoxMergeViews.Checked;
            if (radioButtonStandardFormat.Checked)
            {
                m_data.DwfImageFormat = DWFImageFormat.Lossless;
                m_data.DwfImageQuality = DWFImageQuality.Default;
            }
            else
            {
                m_data.DwfImageFormat = DWFImageFormat.Lossy;
                m_data.DwfImageQuality = m_data.ImageQualities[comboBoxImageQuality.SelectedIndex];
            }
        }

        private void checkBoxModelElements_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxRoomsAndAreas.Enabled = checkBoxModelElements.Checked ? true : false;
        }

        private void radioButtonCompressedFormat_CheckedChanged(object sender, EventArgs e)
        {
            labelImageQuality.Enabled = radioButtonCompressedFormat.Checked;
            comboBoxImageQuality.Enabled = radioButtonCompressedFormat.Checked;
        }
    }
}
