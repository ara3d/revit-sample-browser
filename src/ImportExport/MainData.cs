// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.ImportExport.CS.Export;
using Ara3D.RevitSampleBrowser.ImportExport.CS.Import;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Windows.Forms;

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
            Is3DView = commandData.Application.ActiveUIDocument.Document.ActiveView.ViewType == ViewType.ThreeD;
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
                        ExportDwgData exportDwgData = new(CommandData, format);
                        using (ExportWithViewsForm exportForm = new(exportDwgData))
                        {
                            dialogResult = exportForm.ShowDialog();
                        }

                        break;
                    case ExportFormat.Dxf:
                        ExportDxfData exportDxfData = new(CommandData, format);
                        using (ExportWithViewsForm exportForm = new(exportDxfData))
                        {
                            dialogResult = exportForm.ShowDialog();
                        }

                        break;
                    case ExportFormat.Sat:
                        ExportSatData exportSatData = new(CommandData, format);
                        using (ExportWithViewsForm exportForm = new(exportSatData))
                        {
                            dialogResult = exportForm.ShowDialog();
                        }

                        break;
                    case ExportFormat.Dwf:
                    case ExportFormat.DwFx:
                        ExportDwfData exportDwfData = new(CommandData, format);
                        using (ExportWithViewsForm exportForm = new(exportDwfData))
                        {
                            dialogResult = exportForm.ShowDialog();
                        }

                        break;
                    case ExportFormat.Gbxml:
                        ExportGbxmlData exportGbxmlData = new(CommandData, format);
                        dialogResult = Export(exportGbxmlData);
                        break;
                    case ExportFormat.Fbx:
                        ExportFbxData exportFbxData = new(CommandData, format);
                        dialogResult = Export(exportFbxData);
                        break;
                    case ExportFormat.Dgn:
                        ExportDgnData exportDgnData = new(CommandData, format);
                        using (ExportWithViewsForm exportForm = new(exportDgnData))
                        {
                            dialogResult = exportForm.ShowDialog();
                        }

                        break;
                    case ExportFormat.Image:
                        ExportImgData exportImGdata = new(CommandData, format);
                        using (new ExportWithViewsForm(exportImGdata))
                        {
                            dialogResult = DialogResult.OK;
                        }

                        break;
                    case ExportFormat.Pdf:
                        ExportPdfData exportPdfData = new(CommandData, format);
                        using (ExportWithViewsForm exportForm = new(exportPdfData))
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
            using SaveFileDialog saveDialog = new();
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
                        ImportDwgData importDwgData = new(CommandData, format);
                        using (ImportDwgForm importForm = new(importDwgData))
                        {
                            dialogResult = importForm.ShowDialog();
                        }

                        break;
                    case ImportFormat.Image:
                        ImportImageData importImageData = new(CommandData, format);
                        dialogResult = Import(importImageData);
                        break;
                    case ImportFormat.Gbxml:
                        ImportGbxmlData importGbxmlData = new(CommandData, format);
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
            using OpenFileDialog importDialog = new();
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
