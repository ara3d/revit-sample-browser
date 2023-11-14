// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.UI;

namespace RevitMultiSample.ImportExport.CS
{
    /// <summary>
    ///     Base data class which stores the common information for exporting view related format
    /// </summary>
    public class ExportDataWithViews : ExportData
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportDataWithViews(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            SelectViewsData = new SelectViewsData(commandData);
        }

        public SelectViewsData SelectViewsData { get; }

        /// <summary>
        ///     Whether to export current view only
        /// </summary>
        public bool CurrentViewOnly { get; set; }
    }
}
