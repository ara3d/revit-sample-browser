// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportDxfData : ExportDataWithViews
    {
        private List<ACADVersion> m_enumFileVersion;
        private List<string> m_fileVersion;

        public ExportDxfData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            ExportOptionsData = new ExportBaseOptionsData();

            Initialize();
        }

        public ExportBaseOptionsData ExportOptionsData { get; set; }

        public ReadOnlyCollection<string> FileVersion => new(m_fileVersion);

        public ReadOnlyCollection<ACADVersion> EnumFileVersion =>
            new(m_enumFileVersion);

        public ACADVersion ExportFileVersion { get; set; }

        private void Initialize()
        {
            //AutoCAD versions
            m_fileVersion = [];
            m_enumFileVersion = [];
            m_fileVersion.Add("AutoCAD 2013 DXF Files (*.dxf)");
            m_enumFileVersion.Add(ACADVersion.R2013);
            m_fileVersion.Add("AutoCAD 2010 DXF Files (*.dxf)");
            m_enumFileVersion.Add(ACADVersion.R2010);
            m_fileVersion.Add("AutoCAD 2007 DXF Files (*.dxf)");
            m_enumFileVersion.Add(ACADVersion.R2007);

            StringBuilder tmp = new();
            foreach (var version in m_fileVersion)
            {
                tmp.Append($"{version}|*.dxf|");
            }

            Filter = tmp.ToString().TrimEnd('|');
            Title = "Export DXF";
        }

        public override bool Export()
        {
            base.Export();

            //parameter :  views
            IList<ElementId> views = [];
            if (CurrentViewOnly)
                views.Add(ActiveDocument.ActiveView.Id);
            else
                foreach (View view in SelectViewsData.SelectedViews)
                {
                    views.Add(view.Id);
                }

            // Default values
            ExportFileVersion = ACADVersion.R2010;
            //parameter : DXFExportOptions dxfExportOptions
            DXFExportOptions dxfExportOptions = new()
            {
                ExportingAreas = ExportOptionsData.ExportAreas,
                ExportOfSolids = ExportOptionsData.ExportSolid,
                FileVersion = ExportFileVersion,
                LayerMapping = ExportOptionsData.ExportLayerMapping,
                LineScaling = ExportOptionsData.ExportLineScaling,
                //dxfExportOptions.MergedViews = m_exportOptionsData.ExportMergeFiles;
                PropOverrides = ExportOptionsData.ExportLayersAndProperties,
                SharedCoords = ExportOptionsData.ExportCoorSystem,
                TargetUnit = ExportOptionsData.ExportUnit
            };

            //Export
            var exported = ActiveDocument.Export(ExportFolder, ExportFileName, views, dxfExportOptions);

            return exported;
        }
    }
}
