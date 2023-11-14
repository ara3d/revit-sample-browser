// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     Data class which stores the information for exporting gbxml format
    /// </summary>
    internal class ExportGBXMLData : ExportData
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportGBXMLData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            m_filter = "XML Documents |*.xml";
            m_title = "Export GBXML";
        }

        /// <summary>
        ///     Export GBXML format
        /// </summary>
        /// <returns></returns>
        public override bool Export()
        {
            var transaction = new Transaction(m_activeDoc, "Export_To_GBXML");
            transaction.Start();
            base.Export();

            var options = new GBXMLExportOptions();
            var exported = m_activeDoc.Export(m_exportFolder, m_exportFileName, options);
            transaction.Commit();

            return exported;
        }
    }
}
