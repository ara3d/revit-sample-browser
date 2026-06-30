// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Windows.Forms;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public partial class ExportWithViewsForm : Form
    {
        // Data class object of ExportDataWithViews
        private readonly ExportDataWithViews m_exportData;

        public ExportWithViewsForm(ExportDataWithViews data)
        {
            m_exportData = data;
            InitializeComponent();
            InitializeControls();
        }

        private void InitializeControls()
        {
            textBoxSaveAs.Text = $"{m_exportData.ExportFolder}\\{m_exportData.ExportFileName}";

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
                        using ExportImgOptionsForm exportOptionsForm = new(m_exportData);
                        exportOptionsForm.ShowDialog();

                        break;
                    }
            }
        }

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
                        using ExportBaseOptionsForm exportOptionsForm = new(exportDwgData.ExportOptionsData,
                                   contain3DView, "DWG");
                        exportOptionsForm.ShowDialog();

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

                        using ExportBaseOptionsForm exportOptionsForm = new(exportDxfData.ExportOptionsData,
                                   contain3DView, "DXF");
                        exportOptionsForm.ShowDialog();

                        break;
                    }
                // Export dgn
                case ExportFormat.Dgn:
                    {
                        var exportDgnData = m_exportData as ExportDgnData;
                        using ExportDgnOptionsForm exportOptionsForm = new(exportDgnData);
                        exportOptionsForm.ShowDialog();

                        break;
                    }
                // Export PDF
                case ExportFormat.Pdf:
                    {
                        var exportPdfData = m_exportData as ExportPdfData;
                        using ExportPdfOptionsForm exportOptionsForm = new(exportPdfData);
                        exportOptionsForm.ShowDialog();

                        break;
                    }
                // Export DWF
                default:
                    {
                        var exportDwfData = m_exportData as ExportDwfData;
                        using ExportDwfOptionForm exportOptionsForm = new(exportDwfData);
                        exportOptionsForm.ShowDialog();

                        break;
                    }
            }
        }

        private void buttonSelectViews_Click(object sender, EventArgs e)
        {
            using SelectViewsForm selectViewsForm = new(m_exportData.SelectViewsData);
            m_exportData.SelectViewsData.SelectedViews.Clear();
            selectViewsForm.ShowDialog();
            radioButtonCurrentView.Checked = m_exportData.SelectViewsData.SelectedViews.Size == 0;
        }

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

        private void radioButtonSelectView_CheckedChanged(object sender, EventArgs e)
        {
            buttonSelectViews.Enabled = radioButtonSelectView.Checked;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (ValidateExportFolder())
            {
                m_exportData.CurrentViewOnly = !radioButtonSelectView.Checked;

                try
                {
                    var exported = m_exportData.Export();
                    if (!exported)
                        TaskDialog.Show("Export",
                            $"This project cannot be exported to {m_exportData.ExportFileName} in current settings.", TaskDialogCommonButtons.Ok);
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Export Failed", ex.ToString(), TaskDialogCommonButtons.Ok);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
        }

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
