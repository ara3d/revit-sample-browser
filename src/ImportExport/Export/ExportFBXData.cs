// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportFbxData : ExportData
    {
        public ExportFbxData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Filter = "FBX Files |*.fbx";
            Title = "Export FBX";
        }

        public override bool Export()
        {
            base.Export();

            //parameter : ViewSet views
            var views = new ViewSet();
            views.Insert(ActiveDocument.ActiveView);

            var options = new FBXExportOptions();
            var exported = ActiveDocument.Export(ExportFolder, ExportFileName, views, options);

            return exported;
        }
    }
}
