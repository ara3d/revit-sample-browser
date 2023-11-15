// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS
{
    /// <summary>
    ///     Export formats
    /// </summary>
    public enum ExportFormat
    {
        /// <summary>
        ///     DWG format
        /// </summary>
        Dwg,

        /// <summary>
        ///     DXF format
        /// </summary>
        Dxf,

        /// <summary>
        ///     SAT format
        /// </summary>
        Sat,

        /// <summary>
        ///     DWF format
        /// </summary>
        Dwf,

        /// <summary>
        ///     DWFx format
        /// </summary>
        DwFx,

        /// <summary>
        ///     GBXML format
        /// </summary>
        Gbxml,

        /// <summary>
        ///     FBX format
        /// </summary>
        Fbx,

        /// <summary>
        ///     DGN format
        /// </summary>
        Dgn,

        /// <summary>
        ///     IMG format
        /// </summary>
        Image,

        /// <summary>
        ///     PDF format
        /// </summary>
        Pdf
    }

    /// <summary>
    ///     Import formats
    /// </summary>
    public enum ImportFormat
    {
        /// <summary>
        ///     DWF format
        /// </summary>
        Dwg,

        /// <summary>
        ///     IMAGE format
        /// </summary>
        Image,

        /// <summary>
        ///     GBXML format
        /// </summary>
        Gbxml,

        /// <summary>
        ///     Inventor format
        /// </summary>
        Inventor
    }

    /// <summary>
    ///     Data class contains the external command data.
    /// </summary>
    public class MainData
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        public MainData(ExternalCommandData commandData)
        {
            CommandData = commandData;

            //Whether current active view is 3D view
            if (commandData.Application.ActiveUIDocument.Document.ActiveView.ViewType == ViewType.ThreeD)
                Is3DView = true;
            else
                Is3DView = false;
        }
        // Revit command data

        // Whether current view is a 3D view

        /// <summary>
        ///     Revit command data
        /// </summary>
        public ExternalCommandData CommandData { get; }

        /// <summary>
        ///     Whether current view is a 3D view
        /// </summary>
        public bool Is3DView { get; set; }

        /// <summary>
        ///     Get the format to export
        /// </summary>
        /// <param name="selectedFormat">Selected format in format selecting dialog</param>
        /// <returns>The format to export</returns>
        private static ExportFormat GetSelectedExportFormat(string selectedFormat)
        {
            var format = ExportFormat.Dwg;
            switch (selectedFormat)
            {
                case "DWG":
                    format = ExportFormat.Dwg;
                    break;
                case "DXF":
                    format = ExportFormat.Dxf;
                    break;
                case "SAT":
                    format = ExportFormat.Sat;
                    break;
                case "DWF":
                    format = ExportFormat.Dwf;
                    break;
                case "DWFx":
                    format = ExportFormat.DwFx;
                    break;
                case "GBXML":
                    format = ExportFormat.Gbxml;
                    break;
                case "FBX":
                    format = ExportFormat.Fbx;
                    break;
                case "DGN":
                    format = ExportFormat.Dgn;
                    break;
                case "IMAGE":
                    format = ExportFormat.Image;
                    break;
                case "PDF":
                    format = ExportFormat.Pdf;
                    break;
            }

            return format;
        }

        /// <summary>
        ///     Export according to selected format
        /// </summary>
        /// <param name="selectedFormat">Selected format</param>
        public DialogResult Export(string selectedFormat)
        {
            var format = GetSelectedExportFormat(selectedFormat);
            var dialogResult = DialogResult.OK;

            try
            {
                switch (format)
                {
                    case ExportFormat.Dwg:
                        var exportDwgData = new ExportDwgData(CommandData, format);
                        using (var exportForm = new ExportWithViewsForm(exportDwgData))
                        {
                            dialogResult = exportForm.ShowDialog();
                        }

                        break;
                    case ExportFormat.Dxf:
                        var exportDxfData = new ExportDxfData(CommandData, format);
                        using (var exportForm = new ExportWithViewsForm(exportDxfData))
                        {
                            dialogResult = exportForm.ShowDialog();
                        }

                        break;
                    case ExportFormat.Sat:
                        var exportSatData = new ExportSatData(CommandData, format);
                        using (var exportForm = new ExportWithViewsForm(exportSatData))
                        {
                            dialogResult = exportForm.ShowDialog();
                        }

                        break;
                    case ExportFormat.Dwf:
                    case ExportFormat.DwFx:
                        var exportDwfData = new ExportDwfData(CommandData, format);
                        using (var exportForm = new ExportWithViewsForm(exportDwfData))
                        {
                            dialogResult = exportForm.ShowDialog();
                        }

                        break;
                    case ExportFormat.Gbxml:
                        var exportGbxmlData = new ExportGbxmlData(CommandData, format);
                        dialogResult = Export(exportGbxmlData);
                        break;
                    case ExportFormat.Fbx:
                        var exportFbxData = new ExportFbxData(CommandData, format);
                        dialogResult = Export(exportFbxData);
                        break;
                    case ExportFormat.Dgn:
                        var exportDgnData = new ExportDgnData(CommandData, format);
                        using (var exportForm = new ExportWithViewsForm(exportDgnData))
                        {
                            dialogResult = exportForm.ShowDialog();
                        }

                        break;
                    case ExportFormat.Image:
                        var exportImGdata = new ExportImgData(CommandData, format);
                        using (new ExportWithViewsForm(exportImGdata))
                        {
                            dialogResult = DialogResult.OK;
                        }

                        break;
                    case ExportFormat.Pdf:
                        var exportPdfData = new ExportPdfData(CommandData, format);
                        using (var exportForm = new ExportWithViewsForm(exportPdfData))
                        {
                            dialogResult = exportForm.ShowDialog();
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to export " + format + " format" + ex;
                ;
                TaskDialog.Show("Error", errorMessage, TaskDialogCommonButtons.Ok);
            }

            return dialogResult;
        }

        /// <summary>
        ///     Export
        /// </summary>
        /// <param name="data"></param>
        private static DialogResult Export(ExportData data)
        {
            var returnFilename = string.Empty;
            var filterIndex = -1;

            var result = ShowSaveDialog(data, ref returnFilename, ref filterIndex);
            if (result != DialogResult.Cancel)
            {
                data.ExportFileName = Path.GetFileName(returnFilename);
                data.ExportFolder = Path.GetDirectoryName(returnFilename);
                if (!data.Export())
                    TaskDialog.Show("Export", "This project cannot be exported to " + data.ExportFileName +
                                              " in current settings.", TaskDialogCommonButtons.Ok);
            }

            return result;
        }

        /// <summary>
        ///     Show Save dialog
        /// </summary>
        /// <param name="exportData">Data to export</param>
        /// <param name="returnFileName">File name will be returned</param>
        /// <param name="filterIndex">Selected filter index will be returned</param>
        /// <returns></returns>
        public static DialogResult ShowSaveDialog(ExportData exportData, ref string returnFileName,
            ref int filterIndex)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = exportData.Title;
                saveDialog.InitialDirectory = exportData.ExportFolder;
                saveDialog.FileName = exportData.ExportFileName;
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

        /// <summary>
        ///     Get the format to import
        /// </summary>
        /// <param name="selectedFormat">Selected format in format selecting dialog</param>
        /// <returns>The format to import</returns>
        private static ImportFormat GetSelectedImportFormat(string selectedFormat)
        {
            var format = ImportFormat.Dwg;
            switch (selectedFormat)
            {
                case "DWG":
                    format = ImportFormat.Dwg;
                    break;
                case "IMAGE":
                    format = ImportFormat.Image;
                    break;
                case "GBXML":
                    format = ImportFormat.Gbxml;
                    break;
                case "Inventor":
                    format = ImportFormat.Inventor;
                    break;
            }

            return format;
        }

        /// <summary>
        ///     Export according to selected format
        /// </summary>
        /// <param name="selectedFormat">Selected format</param>
        /// <returns></returns>
        public DialogResult Import(string selectedFormat)
        {
            var dialogResult = DialogResult.OK;
            var format = GetSelectedImportFormat(selectedFormat);

            try
            {
                switch (format)
                {
                    case ImportFormat.Dwg:
                        var importDwgData = new ImportDwgData(CommandData, format);
                        using (var importForm = new ImportDwgForm(importDwgData))
                        {
                            dialogResult = importForm.ShowDialog();
                        }

                        break;
                    case ImportFormat.Image:
                        var importImageData = new ImportImageData(CommandData, format);
                        dialogResult = Import(importImageData);
                        break;
                    case ImportFormat.Gbxml:
                        var importGbxmlData = new ImportGbxmlData(CommandData, format);
                        dialogResult = Import(importGbxmlData);
                        break;
                }
            }
            catch (Exception)
            {
                var errorMessage = "Failed to import " + format + " format";
                TaskDialog.Show("Error", errorMessage, TaskDialogCommonButtons.Ok);
            }

            return dialogResult;
        }

        /// <summary>
        ///     Import
        /// </summary>
        /// <param name="data"></param>
        private static DialogResult Import(ImportData data)
        {
            var returnFilename = string.Empty;
            var result = ShowOpenDialog(data, ref returnFilename);
            if (result != DialogResult.Cancel)
            {
                data.ImportFileFullName = returnFilename;
                if (!data.Import())
                    TaskDialog.Show("Import", "Cannot import " + Path.GetFileName(data.ImportFileFullName) +
                                              " in current settings.", TaskDialogCommonButtons.Ok);
            }

            return result;
        }

        /// <summary>
        ///     Show Open File dialog
        /// </summary>
        /// <param name="importData">Data to import</param>
        /// <param name="returnFileName">File name will be returned</param>
        /// <returns>Dialog result</returns>
        public static DialogResult ShowOpenDialog(ImportData importData, ref string returnFileName)
        {
            using (var importDialog = new OpenFileDialog())
            {
                importDialog.Title = importData.Title;
                importDialog.InitialDirectory = importData.ImportFolder;
                importDialog.Filter = importData.Filter;
                importDialog.RestoreDirectory = true;

                var result = importDialog.ShowDialog();
                if (result != DialogResult.Cancel) returnFileName = importDialog.FileName;

                return result;
            }
        }
    }
}
