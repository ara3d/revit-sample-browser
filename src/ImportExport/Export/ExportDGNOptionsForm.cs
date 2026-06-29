// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    /// <summary>
    ///     Provide a dialog which provides the options for exporting dgn format
    /// </summary>
    public partial class ExportDgnOptionsForm : Form
    {
        /// <summary>
        ///     data class
        /// </summary>
        private readonly ExportDgnData m_data;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="data">Data class object</param>
        public ExportDgnOptionsForm(ExportDgnData data)
        {
            m_data = data;
            InitializeComponent();
            InitializeControl();
        }

        /// <summary>
        ///     Initialize values and status of controls
        /// </summary>
        private void InitializeControl()
        {
            comboBoxLayerSettings.DataSource = m_data.LayerMapping;
            comboBoxLayerSettings.SelectedIndex = 0;
            comboBoxExportFileFormat.DataSource = m_data.ExportFileVersions;
            comboBoxExportFileFormat.SelectedIndex = 0;
            checkBoxHideReferencePlanes.Checked = false;
            checkBoxHideScopeBoxes.Checked = false;
            checkBoxHideUnrefereceViewTag.Checked = false;
        }

        /// <summary>
        ///     OK button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2DSeedFile.Text))
            {
                TaskDialog.Show("Specify 2D seed file", "Please specify the 2D seed file.", TaskDialogCommonButtons.Ok);
                button2DSeedFile.Focus();
                return;
            }

            if (!File.Exists(textBox2DSeedFile.Text))
            {
                TaskDialog.Show("Specify 2D seed file", "The 2D seed file does not exist.", TaskDialogCommonButtons.Ok);
                button2DSeedFile.Focus();
                return;
            }

            if (string.IsNullOrEmpty(textBox3DSeedFile.Text))
            {
                TaskDialog.Show("Specify 3D seed file", "Please specify the 3D seed file.", TaskDialogCommonButtons.Ok);
                button3DSeedFile.Focus();
                return;
            }

            if (!File.Exists(textBox3DSeedFile.Text))
            {
                TaskDialog.Show("Specify 3D seed file", "The 3D seed file does not exist.", TaskDialogCommonButtons.Ok);
                button3DSeedFile.Focus();
                return;
            }

            m_data.ExportLayerMapping =
                m_data.EnumLayerMapping[comboBoxLayerSettings.SelectedIndex];
            m_data.ExportFileVersion = m_data.ExportFileVersions[comboBoxExportFileFormat.SelectedIndex];
            m_data.HideScopeBox = checkBoxHideScopeBoxes.Checked;
            m_data.HideReferencePlane = checkBoxHideReferencePlanes.Checked;
            m_data.HideUnreferenceViewTags = checkBoxHideUnrefereceViewTag.Checked;
            Close();
        }

        /// <summary>
        ///     button2DSeedFile click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2DSeedFile_Click(object sender, EventArgs e)
        {
            var fileName = string.Empty;
            var result = ShowOpenDialog(ref fileName);
            if (result != DialogResult.Cancel) textBox2DSeedFile.Text = fileName;
        }

        /// <summary>
        ///     Show open file dialog
        /// </summary>
        /// <param name="returnFileName"></param>
        /// <returns></returns>
        public static DialogResult ShowOpenDialog(ref string returnFileName)
        {
            var mainModule = Process.GetCurrentProcess().MainModule;
            var folderRevit = Path.GetDirectoryName(mainModule.FileName);
            var folderAcadInterop = Path.Combine(folderRevit, "ACADInterop");
            var initialDirectory = folderRevit;
            if (Directory.Exists(folderAcadInterop)) initialDirectory = folderAcadInterop;

            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Specify seed file";
                openFileDialog.InitialDirectory = initialDirectory;
                openFileDialog.Filter = "DGN Files |*.dgn";
                openFileDialog.RestoreDirectory = true;

                var result = openFileDialog.ShowDialog();
                if (result != DialogResult.Cancel) returnFileName = openFileDialog.FileName;

                return result;
            }
        }

        private void button3DSeedFile_Click(object sender, EventArgs e)
        {
            var fileName = string.Empty;
            var result = ShowOpenDialog(ref fileName);
            if (result != DialogResult.Cancel) textBox3DSeedFile.Text = fileName;
        }
    }
}
