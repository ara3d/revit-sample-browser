// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ImportExport.CS
{
    /// <summary>
    ///     Data class which stores the information for exporting fbx format
    /// </summary>
    internal class ExportFBXData : ExportData
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportFBXData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            m_filter = "FBX Files |*.fbx";
            m_title = "Export FBX";
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
            views.Insert(m_activeDoc.ActiveView);

            var options = new FBXExportOptions();
            var exported = m_activeDoc.Export(m_exportFolder, m_exportFileName, views, options);

            return exported;
        }
    }
}
