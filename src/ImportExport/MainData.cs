// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.IO;
using System.Windows.Forms;
using Ara3D.RevitSampleBrowser.ImportExport.CS.Export;
using Ara3D.RevitSampleBrowser.ImportExport.CS.Import;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS
{
    public enum ExportFormat
    {
        Dwg,

        Dxf,

        Sat,

        Dwf,

        DwFx,

        Gbxml,

        Fbx,

        Dgn,

        Image,

        Pdf
    }

    public enum ImportFormat
    {
        Dwg,

        Image,

        Gbxml,

        Inventor
    }

    public class MainData
    {
        public MainData(ExternalCommandData commandData)
        {
            CommandData = commandData;
            if (commandData.Application.ActiveUIDocument.Document.ActiveView.ViewType == ViewType.ThreeD)
                Is3DView = true;
            else
                Is3DView = false;
        }
        public ExternalCommandData CommandData { get; }

        public bool Is3DView { get; set; }

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
                var errorMessage = $"Failed to export {format} format{ex}";
                ;
                TaskDialog.Show("Error", errorMessage, TaskDialogCommonButtons.Ok);
            }

            return dialogResult;
        }

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
                    TaskDialog.Show("Export",
                        $"This project cannot be exported to {data.ExportFileName} in current settings.", TaskDialogCommonButtons.Ok);
            }

            return result;
        }

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
                var errorMessage = $"Failed to import {format} format";
                TaskDialog.Show("Error", errorMessage, TaskDialogCommonButtons.Ok);
            }

            return dialogResult;
        }

        private static DialogResult Import(ImportData data)
        {
            var returnFilename = string.Empty;
            var result = ShowOpenDialog(data, ref returnFilename);
            if (result != DialogResult.Cancel)
            {
                data.ImportFileFullName = returnFilename;
                if (!data.Import())
                    TaskDialog.Show("Import",
                        $"Cannot import {Path.GetFileName(data.ImportFileFullName)} in current settings.", TaskDialogCommonButtons.Ok);
            }

            return result;
        }

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
