// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     Data class which stores the information for importing GBXML format
    /// </summary>
    internal class ImportGBXMLData : ImportData
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="importFormat">Format to import</param>
        public ImportGBXMLData(ExternalCommandData commandData, ImportFormat importFormat)
            : base(commandData, importFormat)
        {
            m_filter = "XML Documents (*.xml)|*.xml";
            m_title = "Import GBXML";
        }

        /// <summary>
        ///     Collect the parameters and export
        /// </summary>
        /// <returns></returns>
        public override bool Import()
        {
            //parameter: GBXMLImportOptions
            var options = new GBXMLImportOptions();

            //Import
            var t = new Transaction(m_activeDoc);
            t.SetName("Import GBXML");
            t.Start();
            var imported = m_activeDoc.Import(m_importFileFullName, options);
            t.Commit();

            return imported;
        }
    }
}
