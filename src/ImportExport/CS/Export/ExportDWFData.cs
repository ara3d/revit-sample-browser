// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     Data class which stores the information for exporting 2D DWF format
    /// </summary>
    public class ExportDWFData : ExportDataWithViews
    {
        /// <summary>
        ///     Selected image format for DWF export
        /// </summary>
        private DWFImageFormat m_dwfImageFormat;

        /// <summary>
        ///     Selected image quality for DWF export
        /// </summary>
        private DWFImageQuality m_dwfImageQuality;

        /// <summary>
        ///     Whether export areas
        /// </summary>
        private bool m_exportAreas;

        /// <summary>
        ///     Whether to create separate files for each view/sheet
        /// </summary>
        private bool m_exportMergeFiles;

        /// <summary>
        ///     Whether export object data
        /// </summary>
        private bool m_exportObjectData;

        /// <summary>
        ///     List of image quality for DWF export
        /// </summary>
        private List<DWFImageQuality> m_ImageQualities;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportDWFData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Initialize();
        }

        /// <summary>
        ///     Whether export object data
        /// </summary>
        public bool ExportObjectData
        {
            get => m_exportObjectData;
            set => m_exportObjectData = value;
        }

        /// <summary>
        ///     Whether to create separate files for each view/sheet
        /// </summary>
        public bool ExportMergeFiles
        {
            get => m_exportMergeFiles;
            set => m_exportMergeFiles = value;
        }

        /// <summary>
        ///     Whether export areas
        /// </summary>
        public bool ExportAreas
        {
            get => m_exportAreas;
            set => m_exportAreas = value;
        }

        /// <summary>
        ///     Selected image format for DWF export
        /// </summary>
        public DWFImageFormat DwfImageFormat
        {
            get => m_dwfImageFormat;
            set => m_dwfImageFormat = value;
        }

        /// <summary>
        ///     Selected image quality for DWF export
        /// </summary>
        public DWFImageQuality DwfImageQuality
        {
            get => m_dwfImageQuality;
            set => m_dwfImageQuality = value;
        }

        /// <summary>
        ///     List of image quality for DWF export
        /// </summary>
        public List<DWFImageQuality> ImageQualities
        {
            get => m_ImageQualities;
            set => m_ImageQualities = value;
        }

        /// <summary>
        ///     Initialize the variables
        /// </summary>
        private void Initialize()
        {
            m_exportObjectData = true;
            m_exportAreas = false;
            m_exportMergeFiles = true;
            m_ImageQualities = new List<DWFImageQuality>
            {
                DWFImageQuality.Low,
                DWFImageQuality.Medium,
                DWFImageQuality.High
            };

            // Export DWF
            if (m_exportFormat == ExportFormat.DWF)
            {
                m_filter = "DWF Files |*.dwf";
                m_title = "Export DWF";
            }
            // Export DWFx
            else
            {
                m_filter = "DWFx Files |*.dwfx";
                m_title = "Export DWFx";
            }
        }

        /// <summary>
        ///     Collect the parameters and export
        /// </summary>
        /// <returns></returns>
        public override bool Export()
        {
            var transaction = new Transaction(m_activeDoc, "Export_To_DWF");
            transaction.Start();
            var exported = false;
            base.Export();

            //parameter : ViewSet views
            var views = new ViewSet();
            if (m_currentViewOnly)
                views.Insert(m_activeDoc.ActiveView);
            else
                views = m_selectViewsData.SelectedViews;

            // Export DWFx
            if (m_exportFormat == ExportFormat.DWFx)
            {
                var options = new DWFXExportOptions
                {
                    ExportObjectData = m_exportObjectData,
                    ExportingAreas = m_exportAreas,
                    MergedViews = m_exportMergeFiles,
                    ImageFormat = m_dwfImageFormat,
                    ImageQuality = m_dwfImageQuality
                };
                exported = m_activeDoc.Export(m_exportFolder, m_exportFileName, views, options);
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
                exported = m_activeDoc.Export(m_exportFolder, m_exportFileName, views, options);
            }

            transaction.Commit();

            return exported;
        }
    }
}
