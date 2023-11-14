// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.ImportExport.CS
{
    /// <summary>
    ///     Data class which stores the main information for exporting dxf format
    /// </summary>
    public class ExportSatData : ExportDataWithViews
    {
        /// <summary>
        ///     List of Autodesk.Revit.DB.ACADVersion defined in Revit
        /// </summary>
        private List<ACADVersion> m_enumFileVersion;

        /// <summary>
        ///     File version option to export
        /// </summary>
        private ACADVersion m_exportFileVersion;

        /// <summary>
        ///     String list of AutoCAD versions
        /// </summary>
        private List<string> m_fileVersion;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportSatData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Initialize();
        }

        /// <summary>
        ///     String collection of AutoCAD versions
        /// </summary>
        public ReadOnlyCollection<string> FileVersion => new ReadOnlyCollection<string>(m_fileVersion);

        /// <summary>
        ///     Collection of Autodesk.Revit.DB.ACADVersion defined in Revit
        /// </summary>
        public ReadOnlyCollection<ACADVersion> EnumFileVersion =>
            new ReadOnlyCollection<ACADVersion>(m_enumFileVersion);

        /// <summary>
        ///     File version option to export
        /// </summary>
        public ACADVersion ExportFileVersion
        {
            get => m_exportFileVersion;
            set => m_exportFileVersion = value;
        }

        /// <summary>
        ///     Initialize the variables
        /// </summary>
        private void Initialize()
        {
            //AutoCAD versions
            m_fileVersion = new List<string>();
            m_enumFileVersion = new List<ACADVersion>();
            m_fileVersion.Add("AutoCAD 2013 DXF Files (*.sat)");
            m_enumFileVersion.Add(ACADVersion.R2013);
            m_fileVersion.Add("AutoCAD 2010 DXF Files (*.sat)");
            m_enumFileVersion.Add(ACADVersion.R2010);
            m_fileVersion.Add("AutoCAD 2007 DXF Files (*.sat)");
            m_enumFileVersion.Add(ACADVersion.R2007);

            var tmp = new StringBuilder();
            foreach (var version in m_fileVersion) tmp.Append(version + "|*.sat|");
            Filter = tmp.ToString().TrimEnd('|');
            Title = "Export SAT";
        }

        /// <summary>
        ///     Collect the parameters and export
        /// </summary>
        /// <returns></returns>
        public override bool Export()
        {
            base.Export();

            //parameter : ViewSet views
            var views = new ViewSet();
            if (CurrentViewOnly)
                views.Insert(ActiveDocument.ActiveView);
            else
                views = SelectViewsData.SelectedViews;

            ICollection<ElementId> viewIds = new List<ElementId>();
            foreach (View view in views) viewIds.Add(view.Id);

            //parameter : DXFExportOptions dxfExportOptions
            var satExportOptions = new SATExportOptions();

            //Export
            var exported = ActiveDocument.Export(ExportFolder, ExportFileName, viewIds, satExportOptions);

            return exported;
        }
    }
}
