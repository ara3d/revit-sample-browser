// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     Base data class which stores the common information for exporting view related format
    /// </summary>
    public class ExportDataWithViews : ExportData
    {
        /// <summary>
        ///     Whether to export current view only
        /// </summary>
        protected bool m_currentViewOnly;

        /// <summary>
        ///     Views to export
        /// </summary>
        private ViewSet m_exportViews;

        /// <summary>
        ///     Data class SelectViewsData
        /// </summary>
        protected SelectViewsData m_selectViewsData;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportDataWithViews(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            m_selectViewsData = new SelectViewsData(commandData);

            Initialize();
        }

        /// <summary>
        ///     Data class SelectViewsData
        /// </summary>
        public SelectViewsData SelectViewsData
        {
            get => m_selectViewsData;
            set => m_selectViewsData = value;
        }

        /// <summary>
        ///     Views to export
        /// </summary>
        public ViewSet ExportViews
        {
            get => m_exportViews;
            set => m_exportViews = value;
        }

        /// <summary>
        ///     Whether to export current view only
        /// </summary>
        public bool CurrentViewOnly
        {
            get => m_currentViewOnly;
            set => m_currentViewOnly = value;
        }

        /// <summary>
        ///     Initialize the variables
        /// </summary>
        private void Initialize()
        {
            //Views to export
            m_exportViews = new ViewSet();
        }
    }
}
