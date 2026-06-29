// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Import
{
    /// <summary>
    ///     Data class which stores the information for importing GBXML format
    /// </summary>
    public class ImportGbxmlData : ImportData
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="importFormat">Format to import</param>
        public ImportGbxmlData(ExternalCommandData commandData, ImportFormat importFormat)
            : base(commandData, importFormat)
        {
            Filter = "XML Documents (*.xml)|*.xml";
            Title = "Import GBXML";
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
            var t = new Transaction(ActiveDoc);
            t.SetName("Import GBXML");
            t.Start();
            var imported = ActiveDoc.Import(ImportFileFullName, options);
            t.Commit();

            return imported;
        }
    }
}
