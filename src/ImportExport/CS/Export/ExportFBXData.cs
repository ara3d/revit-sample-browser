// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.ImportExport.CS
{
    /// <summary>
    ///     Data class which stores the information for exporting fbx format
    /// </summary>
    internal class ExportFbxData : ExportData
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportFbxData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Filter = "FBX Files |*.fbx";
            Title = "Export FBX";
        }

        /// <summary>
        ///     Export FBX format
        /// </summary>
        /// <returns></returns>
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
