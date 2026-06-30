// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportDwgData : ExportDataWithViews
    {
        private List<ACADVersion> m_enumFileVersion;

        private ACADVersion m_exportFileVersion;

        private ExportBaseOptionsData m_exportOptionsData;

        private List<string> m_fileVersion;

        public ExportDwgData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            m_exportOptionsData = new ExportBaseOptionsData();

            Initialize();
        }

        public ExportBaseOptionsData ExportOptionsData
        {
            get => m_exportOptionsData;
            set => m_exportOptionsData = value;
        }

        public ReadOnlyCollection<string> FileVersion => new ReadOnlyCollection<string>(m_fileVersion);

        public ReadOnlyCollection<ACADVersion> EnumFileVersion =>
            new ReadOnlyCollection<ACADVersion>(m_enumFileVersion);

        public ACADVersion ExportFileVersion
        {
            get => m_exportFileVersion;
            set => m_exportFileVersion = value;
        }

        private void Initialize()
        {
            //AutoCAD versions
            m_fileVersion = new List<string>();
            m_enumFileVersion = new List<ACADVersion>();
            m_fileVersion.Add("AutoCAD 2013 DWG Files (*.dwg)");
            m_enumFileVersion.Add(ACADVersion.R2013);
            m_fileVersion.Add("AutoCAD 2010 DWG Files (*.dwg)");
            m_enumFileVersion.Add(ACADVersion.R2010);
            m_fileVersion.Add("AutoCAD 2007 DWG Files (*.dwg)");
            m_enumFileVersion.Add(ACADVersion.R2007);

            var tmp = new StringBuilder();
            foreach (var version in m_fileVersion)
            {
                tmp.Append($"{version}|*.dwg|");
            }

            Filter = tmp.ToString().TrimEnd('|');
            Title = "Export DWG";
        }

        public override bool Export()
        {
            base.Export();

            //parameter :  views
            IList<ElementId> views = new List<ElementId>();
            if (CurrentViewOnly)
                views.Add(ActiveDocument.ActiveView.Id);
            else
                foreach (View view in SelectViewsData.SelectedViews)
                {
                    views.Add(view.Id);
                }

            // Default values
            m_exportFileVersion = ACADVersion.R2010;
            //parameter : DWGExportOptions dwgExportOptions
            var dwgExportOptions = new DWGExportOptions
            {
                ExportingAreas = m_exportOptionsData.ExportAreas,
                ExportOfSolids = m_exportOptionsData.ExportSolid,
                FileVersion = m_exportFileVersion,
                LayerMapping = m_exportOptionsData.ExportLayerMapping,
                LineScaling = m_exportOptionsData.ExportLineScaling,
                MergedViews = m_exportOptionsData.ExportMergeFiles,
                PropOverrides = m_exportOptionsData.ExportLayersAndProperties,
                SharedCoords = m_exportOptionsData.ExportCoorSystem,
                TargetUnit = m_exportOptionsData.ExportUnit
            };

            //Export
            var exported = ActiveDocument.Export(ExportFolder, ExportFileName, views, dwgExportOptions);

            return exported;
        }
    }
}
