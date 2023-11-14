// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     Data class which stores the main information for exporting pdf format
    /// </summary>
    public class ExportPDFData : ExportDataWithViews
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportPDFData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            m_filter = "PDF Files |*.pdf";
            m_title = "Export PDF";

            Combine = true;
        }


        /// <summary>
        ///     Combine
        /// </summary>
        public bool Combine { get; set; }

        /// <summary>
        ///     Export PDF format
        /// </summary>
        /// <returns></returns>
        public override bool Export()
        {
            base.Export();

            // Parameter: The list of view/sheet id to export
            IList<ElementId> views = new List<ElementId>();
            if (m_currentViewOnly)
            {
                views.Add(m_activeDoc.ActiveView.Id);
            }
            else
            {
                var viewSet = m_selectViewsData.SelectedViews;
                foreach (View v in viewSet) views.Add(v.Id);
            }

            // Parameter: The exporting options, including paper size, orientation, file name or naming rule and etc.
            var options = new PDFExportOptions();
            options.FileName = m_exportFileName;
            options.Combine =
                Combine; // If not combined, PDFs will be exported with default naming rule "Type-ViewName"
            var exported = m_activeDoc.Export(m_exportFolder, views, options);

            return exported;
        }
    }
}
