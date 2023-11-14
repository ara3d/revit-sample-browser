// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     It contains a dialog which provides the options of common information for export
    /// </summary>
    public partial class ExportWithViewsForm : Form
    {
        // Data class object of ExportDataWithViews
        private readonly ExportDataWithViews m_exportData;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="data"></param>
        public ExportWithViewsForm(ExportDataWithViews data)
        {
            m_exportData = data;
            InitializeComponent();
            InitializeControls();
        }

        /// <summary>
        ///     Initialize values and status of controls
        /// </summary>
        private void InitializeControls()
        {
            textBoxSaveAs.Text = m_exportData.ExportFolder + "\\" + m_exportData.ExportFileName;

            radioButtonCurrentView.Checked = true;

            // Initialize the title
            Text = m_exportData.Title;
            switch (m_exportData.ExportFormat)
            {
                case ExportFormat.Sat:
                    buttonOptions.Visible = false;
                    break;
                case ExportFormat.Image:
                {
                    Hide();
                    using (var exportOptionsForm = new ExportImgOptionsForm(m_exportData))
                    {
                        exportOptionsForm.ShowDialog();
                    }

                    break;
                }
            }
        }

        /// <summary>
        ///     Provide the export option dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOptions_Click(object sender, EventArgs e)
        {
            switch (m_exportData.ExportFormat)
            {
                // Export dwg
                case ExportFormat.Dwg:
                {
                    var contain3DView = false;

                    if (radioButtonCurrentView.Checked)
                    {
                        if (m_exportData.Is3DView) contain3DView = true;
                    }
                    else
                    {
                        if (m_exportData.SelectViewsData.Contain3DView) contain3DView = true;
                    }

                    var exportDwgData = m_exportData as ExportDwgData;
                    using (var exportOptionsForm = new ExportBaseOptionsForm(exportDwgData.ExportOptionsData,
                               contain3DView, "DWG"))
                    {
                        exportOptionsForm.ShowDialog();
                    }

                    break;
                }
                //Export dxf
                case ExportFormat.Dxf:
                {
                    var contain3DView = false;

                    if (radioButtonCurrentView.Checked)
                    {
                        if (m_exportData.Is3DView) contain3DView = true;
                    }
                    else
                    {
                        if (m_exportData.SelectViewsData.Contain3DView) contain3DView = true;
                    }

                    var exportDxfData = m_exportData as ExportDxfData;

                    using (var exportOptionsForm = new ExportBaseOptionsForm(exportDxfData.ExportOptionsData,
                               contain3DView, "DXF"))
                    {
                        exportOptionsForm.ShowDialog();
                    }

                    break;
                }
                // Export dgn
                case ExportFormat.Dgn:
                {
                    var exportDgnData = m_exportData as ExportDgnData;
                    using (var exportOptionsForm = new ExportDgnOptionsForm(exportDgnData))
                    {
                        exportOptionsForm.ShowDialog();
                    }

                    break;
                }
                // Export PDF
                case ExportFormat.Pdf:
                {
                    var exportPdfData = m_exportData as ExportPdfData;
                    using (var exportOptionsForm = new ExportPdfOptionsForm(exportPdfData))
                    {
                        exportOptionsForm.ShowDialog();
                    }

                    break;
                }
                // Export DWF
                default:
                {
                    var exportDwfData = m_exportData as ExportDwfData;
                    using (var exportOptionsForm = new ExportDwfOptionForm(exportDwfData))
                    {
                        exportOptionsForm.ShowDialog();
                    }

                    break;
                }
            }
        }

        /// <summary>
        ///     Provide the views selecting dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSelectViews_Click(object sender, EventArgs e)
        {
            using (var selectViewsForm = new SelectViewsForm(m_exportData.SelectViewsData))
            {
                m_exportData.SelectViewsData.SelectedViews.Clear();
                selectViewsForm.ShowDialog();
                if (m_exportData.SelectViewsData.SelectedViews.Size == 0)
                    radioButtonCurrentView.Checked = true;
                else
                    radioButtonCurrentView.Checked = false;
            }
        }

        /// <summary>
        ///     Specify a file to export into
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBrowser_Click(object sender, EventArgs e)
        {
            var fileName = string.Empty;
            var filterIndex = -1;

            var result = MainData.ShowSaveDialog(m_exportData, ref fileName, ref filterIndex);
            if (result != DialogResult.Cancel)
            {
                textBoxSaveAs.Text = fileName;
                switch (m_exportData.ExportFormat)
                {
                    case ExportFormat.Dwg:
                    {
                        var exportDwgData = m_exportData as ExportDwgData;
                        exportDwgData.ExportFileVersion = exportDwgData.EnumFileVersion[filterIndex - 1];
                        break;
                    }
                    case ExportFormat.Dxf:
                    {
                        var exportDxfData = m_exportData as ExportDxfData;
                        exportDxfData.ExportFileVersion = exportDxfData.EnumFileVersion[filterIndex - 1];
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Change the status of buttonSelectViews according to the selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonSelectView_CheckedChanged(object sender, EventArgs e)
        {
            buttonSelectViews.Enabled = radioButtonSelectView.Checked;
        }

        /// <summary>
        ///     Transfer information back to ExportData class and execute EXPORT operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (ValidateExportFolder())
            {
                m_exportData.CurrentViewOnly = !radioButtonSelectView.Checked;

                try
                {
                    var exported = m_exportData.Export();
                    if (!exported)
                        TaskDialog.Show("Export", "This project cannot be exported to " + m_exportData.ExportFileName +
                                                  " in current settings.", TaskDialogCommonButtons.Ok);
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Export Failed", ex.ToString(), TaskDialogCommonButtons.Ok);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        ///     Check whether the folder specified is valid
        /// </summary>
        /// <returns></returns>
        private bool ValidateExportFolder()
        {
            var fileNameFull = textBoxSaveAs.Text;
            //If the textBoxSaveAs is empty
            if (string.IsNullOrEmpty(fileNameFull))
            {
                TaskDialog.Show("Information", "Please specify the folder and file name!", TaskDialogCommonButtons.Ok);
                textBoxSaveAs.Focus();
                return false;
            }

            //If has file name
            if (!fileNameFull.Contains("\\"))
            {
                TaskDialog.Show("Information", "Please specify file name!", TaskDialogCommonButtons.Ok);
                textBoxSaveAs.Focus();
                return false;
            }

            //Whether the folder exists
            var folder = Path.GetDirectoryName(fileNameFull);
            if (!Directory.Exists(folder))
            {
                TaskDialog.Show("Information", "The specified folder does not exist!", TaskDialogCommonButtons.Ok);
                textBoxSaveAs.Focus();
                return false;
            }

            m_exportData.ExportFileName = Path.GetFileNameWithoutExtension(fileNameFull);
            m_exportData.ExportFolder = folder;

            return true;
        }
    }
}
