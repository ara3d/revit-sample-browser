// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    /// <summary>
    ///     Data class which stores the main information for exporting dxf format
    /// </summary>
    public class ExportDxfData : ExportDataWithViews
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
        ///     Data class ExportOptionsData
        /// </summary>
        private ExportBaseOptionsData m_exportOptionsData;

        /// <summary>
        ///     String list of AutoCAD versions
        /// </summary>
        private List<string> m_fileVersion;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="commandData">Revit command data</param>
        /// <param name="exportFormat">Format to export</param>
        public ExportDxfData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            m_exportOptionsData = new ExportBaseOptionsData();

            Initialize();
        }

        /// <summary>
        ///     Data class ExportOptionsData
        /// </summary>
        public ExportBaseOptionsData ExportOptionsData
        {
            get => m_exportOptionsData;
            set => m_exportOptionsData = value;
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
            m_fileVersion.Add("AutoCAD 2013 DXF Files (*.dxf)");
            m_enumFileVersion.Add(ACADVersion.R2013);
            m_fileVersion.Add("AutoCAD 2010 DXF Files (*.dxf)");
            m_enumFileVersion.Add(ACADVersion.R2010);
            m_fileVersion.Add("AutoCAD 2007 DXF Files (*.dxf)");
            m_enumFileVersion.Add(ACADVersion.R2007);

            var tmp = new StringBuilder();
            foreach (var version in m_fileVersion) tmp.Append(version + "|*.dxf|");
            Filter = tmp.ToString().TrimEnd('|');
            Title = "Export DXF";
        }

        /// <summary>
        ///     Collect the parameters and export
        /// </summary>
        /// <returns></returns>
        public override bool Export()
        {
            base.Export();

            //parameter :  views
            IList<ElementId> views = new List<ElementId>();
            if (CurrentViewOnly)
                views.Add(ActiveDocument.ActiveView.Id);
            else
                foreach (View view in SelectViewsData.SelectedViews)
                    views.Add(view.Id);
            // Default values
            m_exportFileVersion = ACADVersion.R2010;
            //parameter : DXFExportOptions dxfExportOptions
            var dxfExportOptions = new DXFExportOptions();
            dxfExportOptions.ExportingAreas = m_exportOptionsData.ExportAreas;
            dxfExportOptions.ExportOfSolids = m_exportOptionsData.ExportSolid;
            dxfExportOptions.FileVersion = m_exportFileVersion;
            dxfExportOptions.LayerMapping = m_exportOptionsData.ExportLayerMapping;
            dxfExportOptions.LineScaling = m_exportOptionsData.ExportLineScaling;
            //dxfExportOptions.MergedViews = m_exportOptionsData.ExportMergeFiles;
            dxfExportOptions.PropOverrides = m_exportOptionsData.ExportLayersAndProperties;
            dxfExportOptions.SharedCoords = m_exportOptionsData.ExportCoorSystem;
            dxfExportOptions.TargetUnit = m_exportOptionsData.ExportUnit;

            //Export
            var exported = ActiveDocument.Export(ExportFolder, ExportFileName, views, dxfExportOptions);

            return exported;
        }
    }
}
