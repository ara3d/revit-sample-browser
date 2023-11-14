// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.ImportExport.CS
{
    /// <summary>
    ///     Data class which stores the information for exporting gbxml format
    /// </summary>
    internal class ExportGbxmlData : ExportData
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportGbxmlData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Filter = "XML Documents |*.xml";
            Title = "Export GBXML";
        }

        /// <summary>
        ///     Export GBXML format
        /// </summary>
        /// <returns></returns>
        public override bool Export()
        {
            var transaction = new Transaction(ActiveDocument, "Export_To_GBXML");
            transaction.Start();
            base.Export();

            var options = new GBXMLExportOptions();
            var exported = ActiveDocument.Export(ExportFolder, ExportFileName, options);
            transaction.Commit();

            return exported;
        }
    }
}
