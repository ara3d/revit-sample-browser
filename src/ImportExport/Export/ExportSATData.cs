// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportSatData : ExportDataWithViews
    {
        private List<ACADVersion> m_enumFileVersion;

        private ACADVersion m_exportFileVersion;

        private List<string> m_fileVersion;

        public ExportSatData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Initialize();
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
            m_fileVersion.Add("AutoCAD 2013 DXF Files (*.sat)");
            m_enumFileVersion.Add(ACADVersion.R2013);
            m_fileVersion.Add("AutoCAD 2010 DXF Files (*.sat)");
            m_enumFileVersion.Add(ACADVersion.R2010);
            m_fileVersion.Add("AutoCAD 2007 DXF Files (*.sat)");
            m_enumFileVersion.Add(ACADVersion.R2007);

            var tmp = new StringBuilder();
            foreach (var version in m_fileVersion)
            {
                tmp.Append($"{version}|*.sat|");
            }

            Filter = tmp.ToString().TrimEnd('|');
            Title = "Export SAT";
        }

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
            foreach (View view in views)
            {
                viewIds.Add(view.Id);
            }

            //parameter : DXFExportOptions dxfExportOptions
            var satExportOptions = new SATExportOptions();

            //Export
            var exported = ActiveDocument.Export(ExportFolder, ExportFileName, viewIds, satExportOptions);

            return exported;
        }
    }
}
