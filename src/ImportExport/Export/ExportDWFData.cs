// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportDwfData : ExportDataWithViews
    {
        public ExportDwfData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Initialize();
        }

        public bool ExportObjectData { get; set; }

        public bool ExportMergeFiles { get; set; }

        public bool ExportAreas { get; set; }

        public DWFImageFormat DwfImageFormat { get; set; }

        public DWFImageQuality DwfImageQuality { get; set; }

        public List<DWFImageQuality> ImageQualities { get; set; }

        private void Initialize()
        {
            ExportObjectData = true;
            ExportAreas = false;
            ExportMergeFiles = true;
            ImageQualities =
            [
                DWFImageQuality.Low,
                DWFImageQuality.Medium,
                DWFImageQuality.High
            ];

            // Export DWF
            if (ExportFormat == ExportFormat.Dwf)
            {
                Filter = "DWF Files |*.dwf";
                Title = "Export DWF";
            }
            // Export DWFx
            else
            {
                Filter = "DWFx Files |*.dwfx";
                Title = "Export DWFx";
            }
        }

        public override bool Export()
        {
            Transaction transaction = new(ActiveDocument, "Export_To_DWF");
            transaction.Start();
            var exported = false;
            base.Export();

            //parameter : ViewSet views
            ViewSet views = new();
            if (CurrentViewOnly)
                views.Insert(ActiveDocument.ActiveView);
            else
                views = SelectViewsData.SelectedViews;

            // Export DWFx
            if (ExportFormat == ExportFormat.DwFx)
            {
                DWFXExportOptions options = new()
                {
                    ExportObjectData = ExportObjectData,
                    ExportingAreas = ExportAreas,
                    MergedViews = ExportMergeFiles,
                    ImageFormat = DwfImageFormat,
                    ImageQuality = DwfImageQuality
                };
                exported = ActiveDocument.Export(ExportFolder, ExportFileName, views, options);
            }
            // Export DWF
            else
            {
                DWFExportOptions options = new()
                {
                    ExportObjectData = ExportObjectData,
                    ExportingAreas = ExportAreas,
                    MergedViews = ExportMergeFiles,
                    ImageFormat = DwfImageFormat,
                    ImageQuality = DwfImageQuality
                };
                exported = ActiveDocument.Export(ExportFolder, ExportFileName, views, options);
            }

            transaction.Commit();

            return exported;
        }
    }
}
