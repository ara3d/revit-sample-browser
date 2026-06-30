// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportGbxmlData : ExportData
    {
        public ExportGbxmlData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Filter = "XML Documents |*.xml";
            Title = "Export GBXML";
        }

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
