// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportDgnData : ExportDataWithViews
    {
        private List<string> m_enumLayerMapping;

        private string m_exportFileVersion;

        private List<string> m_exportFileVersions;

        private string m_exportLayerMapping;

        private bool m_hideReferencePlane;

        private bool m_hideScopeBox;

        private bool m_hideUnreferenceViewTags;

        private List<string> m_layerMapping;

        public ExportDgnData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Initialize();
        }

        public string ExportLayerMapping
        {
            get => m_exportLayerMapping;
            set => m_exportLayerMapping = value;
        }

        public bool HideScopeBox
        {
            get => m_hideScopeBox;
            set => m_hideScopeBox = value;
        }

        public bool HideUnreferenceViewTags
        {
            get => m_hideUnreferenceViewTags;
            set => m_hideUnreferenceViewTags = value;
        }

        public bool HideReferencePlane
        {
            get => m_hideReferencePlane;
            set => m_hideReferencePlane = value;
        }

        public string ExportFileVersion
        {
            get => m_exportFileVersion;
            set => m_exportFileVersion = value;
        }

        public ReadOnlyCollection<string> LayerMapping => new ReadOnlyCollection<string>(m_layerMapping);

        public List<string> ExportFileVersions => m_exportFileVersions;

        public ReadOnlyCollection<string> EnumLayerMapping => new ReadOnlyCollection<string>(m_enumLayerMapping);

        private void Initialize()
        {
            //Layer Settings:
            m_layerMapping = new List<string>();
            m_enumLayerMapping = new List<string>();
            m_layerMapping.Add("AIA - American Institute of Architects standard");
            m_enumLayerMapping.Add("AIA");
            m_layerMapping.Add("ISO13567 - ISO standard 13567");
            m_enumLayerMapping.Add("ISO13567");
            m_layerMapping.Add("CP83 - Singapore standard 83");
            m_enumLayerMapping.Add("CP83");
            m_layerMapping.Add("BS1192 - British standard 1192");
            m_enumLayerMapping.Add("BS1192");

            //Export format:
            m_exportFileVersions = new List<string>
            {
                "MicroStation V8 Format",
                "MicroStation V7 Format"
            };

            Filter = "Microstation DGN Files |*.dgn";
            Title = "Export DGN";
        }

        public override bool Export()
        {
            base.Export();

            ICollection<ElementId> views = new List<ElementId>();
            if (CurrentViewOnly)
                views.Add(ActiveDocument.ActiveView.Id);
            else
                foreach (View view in SelectViewsData.SelectedViews)
                {
                    views.Add(view.Id);
                }

            //parameter : DWGExportOptions dwgExportOptions
            var dgnExportOptions = new DGNExportOptions
            {
                // default values
                FileVersion = DGNFileFormat.DGNVersion8
            };
            m_exportLayerMapping = m_enumLayerMapping[0];

            // set values from selected options
            dgnExportOptions.LayerMapping = m_exportLayerMapping;
            if (m_exportFileVersion == "MicroStation V7 Format")
                dgnExportOptions.FileVersion = DGNFileFormat.DGNVersion7;
            else
                dgnExportOptions.FileVersion = DGNFileFormat.DGNVersion8;
            dgnExportOptions.HideScopeBox = m_hideScopeBox;
            dgnExportOptions.HideUnreferenceViewTags = m_hideUnreferenceViewTags;
            dgnExportOptions.HideReferencePlane = m_hideReferencePlane;
            var mainModule = Process.GetCurrentProcess().MainModule;
            var revitFolder = Path.GetDirectoryName(mainModule.FileName);
            dgnExportOptions.SeedName = Path.Combine(revitFolder, @"ACADInterop\V8-Imperial-Seed3D.dgn");

            //Export
            var exported = ActiveDocument.Export(ExportFolder, ExportFileName, views, dgnExportOptions);
            return exported;
        }
    }
}
