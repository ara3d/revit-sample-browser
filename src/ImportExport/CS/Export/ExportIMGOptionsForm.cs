// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;
using View = Autodesk.Revit.DB.View;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     Provide a dialog which provides the options of lower priority information for exporting image
    /// </summary>
    public partial class ExportIMGOptionsForm : Form
    {
        /// <summary>
        ///     Data class object of ExportDataWithViews
        /// </summary>
        private readonly ExportDataWithViews m_exportData;

        /// <summary>
        ///     Data class object of m_exportOptions
        /// </summary>
        private readonly ImageExportOptions m_exportOptions;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="exportData">Data class object</param>
        public ExportIMGOptionsForm(ExportDataWithViews exportData)
        {
            m_exportOptions = new ImageExportOptions();
            InitializeComponent();
            m_exportData = exportData;
            InitializeFormats();
        }

        private void InitializeFormats()
        {
            // Initialize
            currentWindowOption.Checked = true;
            selectRange.Enabled = false;
            fitToOption.Checked = true;
            zoomOption.Checked = false;
            verticalOption.Enabled = false;
            horizontalOption.Enabled = true;
            RIQCom.Enabled = false;
            pixelValue.Text = "512";
            zoomSize.Enabled = false;
            zoomSize.Value = 50;
            saveAs.Text = m_exportData.ExportFolder + "\\" + m_exportData.ActiveViewName;


            shadedCom.Items.Add("BMP");
            shadedCom.Items.Add("JPEG(lossless)");
            shadedCom.Items.Add("JPEG(medium)");
            shadedCom.Items.Add("JPEG(smallest)");
            shadedCom.Items.Add("PNG");
            shadedCom.Items.Add("TARGA");
            shadedCom.Items.Add("TIFF");
            shadedCom.SelectedIndex = 2;

            noShadedCom.Items.Add("BMP");
            noShadedCom.Items.Add("JPEG(lossless)");
            noShadedCom.Items.Add("JPEG(medium)");
            noShadedCom.Items.Add("JPEG(smallest");
            noShadedCom.Items.Add("PNG");
            noShadedCom.Items.Add("TARGA");
            noShadedCom.Items.Add("TIFF");
            noShadedCom.SelectedIndex = 2;


            RIQCom.Items.Add("72");
            RIQCom.Items.Add("150");
            RIQCom.Items.Add("300");
            RIQCom.Items.Add("600");
            RIQCom.SelectedIndex = 0;
            RIQCom.Enabled = false;
        }

        private void FitTo_CheckedChanged(object sender, EventArgs e)
        {
            verticalOption.Enabled = true;
            horizontalOption.Enabled = true;
            RIQCom.Enabled = false;
            zoomSize.Enabled = false;
            m_exportOptions.ZoomType = ZoomFitType.FitToPage;
            pixelValue.Enabled = true;
        }

        private void Zoom_CheckedChanged(object sender, EventArgs e)
        {
            verticalOption.Checked = false;
            horizontalOption.Checked = false;
            verticalOption.Enabled = false;
            horizontalOption.Enabled = false;
            RIQCom.Enabled = true;
            zoomSize.Enabled = true;
            m_exportOptions.ZoomType = ZoomFitType.Zoom;
            pixelValue.Enabled = false;
        }

        private void Cannel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Select_Click(object sender, EventArgs e)
        {
            using (var selectViewsForm = new SelectViewsForm(m_exportData.SelectViewsData))
            {
                m_exportData.SelectViewsData.SelectedViews.Clear();
                selectViewsForm.ShowDialog();
            }
        }

        private void CurrentWindow_CheckedChanged(object sender, EventArgs e)
        {
            m_exportOptions.ExportRange = ExportRange.CurrentView;
            selectRange.Enabled = false;
        }

        private void VisiblePortion_CheckedChanged(object sender, EventArgs e)
        {
            m_exportOptions.ExportRange = ExportRange.VisibleRegionOfCurrentView;
            selectRange.Enabled = false;
        }

        private void ViewsSheets_CheckedChanged(object sender, EventArgs e)
        {
            m_exportOptions.ExportRange = ExportRange.SetOfViews;
            selectRange.Enabled = true;
        }

        private void Change_Click(object sender, EventArgs e)
        {
            var fileName = string.Empty;
            var filterIndex = -1;
            var result = ShowSaveDialog(m_exportData, ref fileName, ref filterIndex);
            if (result != DialogResult.Cancel) saveAs.Text = fileName.Substring(0, fileName.LastIndexOf("."));
        }

        private DialogResult ShowSaveDialog(ExportData exportData, ref string returnFileName, ref int filterIndex)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = exportData.Title;
                saveDialog.InitialDirectory = exportData.ExportFolder;
                saveDialog.FileName = exportData.ActiveViewName;
                saveDialog.Filter = exportData.Filter;
                saveDialog.FilterIndex = 1;
                saveDialog.RestoreDirectory = true;

                var result = saveDialog.ShowDialog();
                if (result != DialogResult.Cancel)
                {
                    returnFileName = saveDialog.FileName;
                    filterIndex = saveDialog.FilterIndex;
                }

                return result;
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            m_exportOptions.FilePath = saveAs.Text;
            if (m_exportOptions.ZoomType == ZoomFitType.FitToPage)
            {
                m_exportOptions.PixelSize = int.Parse(pixelValue.Text);
            }
            else if (m_exportOptions.ZoomType == ZoomFitType.Zoom)
            {
                m_exportOptions.Zoom = (int)zoomSize.Value;
                m_exportOptions.ImageResolution = (ImageResolution)RIQCom.SelectedIndex;
            }

            m_exportOptions.ShadowViewsFileType = (ImageFileType)shadedCom.SelectedIndex;
            m_exportOptions.HLRandWFViewsFileType = (ImageFileType)noShadedCom.SelectedIndex;


            if (m_exportOptions.ExportRange == ExportRange.SetOfViews)
            {
                var views = m_exportData.SelectViewsData.SelectedViews;
                var viewIds = new List<ElementId>();
                foreach (View view in views) viewIds.Add(view.Id);
                m_exportOptions.SetViewsAndSheets(viewIds);
            }

            try
            {
                m_exportData.ActiveDocument.ExportImage(m_exportOptions);
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to export img" + ex;
                TaskDialog.Show("Error", errorMessage, TaskDialogCommonButtons.Ok);
            }

            Close();
        }

        private void Vertical_CheckedChanged(object sender, EventArgs e)
        {
            m_exportOptions.FitDirection = FitDirectionType.Vertical;
        }

        private void Horizontal_CheckedChanged(object sender, EventArgs e)
        {
            m_exportOptions.FitDirection = FitDirectionType.Horizontal;
        }
    }
}
