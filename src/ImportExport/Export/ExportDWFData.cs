// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportDwfData : ExportDataWithViews
    {
        private DWFImageFormat m_dwfImageFormat;

        private DWFImageQuality m_dwfImageQuality;

        private bool m_exportAreas;

        private bool m_exportMergeFiles;

        private bool m_exportObjectData;

        private List<DWFImageQuality> m_imageQualities;

        public ExportDwfData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Initialize();
        }

        public bool ExportObjectData
        {
            get => m_exportObjectData;
            set => m_exportObjectData = value;
        }

        public bool ExportMergeFiles
        {
            get => m_exportMergeFiles;
            set => m_exportMergeFiles = value;
        }

        public bool ExportAreas
        {
            get => m_exportAreas;
            set => m_exportAreas = value;
        }

        public DWFImageFormat DwfImageFormat
        {
            get => m_dwfImageFormat;
            set => m_dwfImageFormat = value;
        }

        public DWFImageQuality DwfImageQuality
        {
            get => m_dwfImageQuality;
            set => m_dwfImageQuality = value;
        }

        public List<DWFImageQuality> ImageQualities
        {
            get => m_imageQualities;
            set => m_imageQualities = value;
        }

        private void Initialize()
        {
            m_exportObjectData = true;
            m_exportAreas = false;
            m_exportMergeFiles = true;
            m_imageQualities = new List<DWFImageQuality>
            {
                DWFImageQuality.Low,
                DWFImageQuality.Medium,
                DWFImageQuality.High
            };

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
            var transaction = new Transaction(ActiveDocument, "Export_To_DWF");
            transaction.Start();
            var exported = false;
            base.Export();

            //parameter : ViewSet views
            var views = new ViewSet();
            if (CurrentViewOnly)
                views.Insert(ActiveDocument.ActiveView);
            else
                views = SelectViewsData.SelectedViews;

            // Export DWFx
            if (ExportFormat == ExportFormat.DwFx)
            {
                var options = new DWFXExportOptions
                {
                    ExportObjectData = m_exportObjectData,
                    ExportingAreas = m_exportAreas,
                    MergedViews = m_exportMergeFiles,
                    ImageFormat = m_dwfImageFormat,
                    ImageQuality = m_dwfImageQuality
                };
                exported = ActiveDocument.Export(ExportFolder, ExportFileName, views, options);
            }
            // Export DWF
            else
            {
                var options = new DWFExportOptions
                {
                    ExportObjectData = m_exportObjectData,
                    ExportingAreas = m_exportAreas,
                    MergedViews = m_exportMergeFiles,
                    ImageFormat = m_dwfImageFormat,
                    ImageQuality = m_dwfImageQuality
                };
                exported = ActiveDocument.Export(ExportFolder, ExportFileName, views, options);
            }

            transaction.Commit();

            return exported;
        }
    }
}
