// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportDgnData : ExportDataWithViews
    {
        private List<string> m_enumLayerMapping;
        private List<string> m_layerMapping;

        public ExportDgnData(ExternalCommandData commandData, ExportFormat exportFormat)
            : base(commandData, exportFormat)
        {
            Initialize();
        }

        public string ExportLayerMapping { get; set; }

        public bool HideScopeBox { get; set; }

        public bool HideUnreferenceViewTags { get; set; }

        public bool HideReferencePlane { get; set; }

        public string ExportFileVersion { get; set; }

        public ReadOnlyCollection<string> LayerMapping => new(m_layerMapping);

        public List<string> ExportFileVersions { get; private set; }

        public ReadOnlyCollection<string> EnumLayerMapping => new(m_enumLayerMapping);

        private void Initialize()
        {
            //Layer Settings:
            m_layerMapping = [];
            m_enumLayerMapping = [];
            m_layerMapping.Add("AIA - American Institute of Architects standard");
            m_enumLayerMapping.Add("AIA");
            m_layerMapping.Add("ISO13567 - ISO standard 13567");
            m_enumLayerMapping.Add("ISO13567");
            m_layerMapping.Add("CP83 - Singapore standard 83");
            m_enumLayerMapping.Add("CP83");
            m_layerMapping.Add("BS1192 - British standard 1192");
            m_enumLayerMapping.Add("BS1192");

            //Export format:
            ExportFileVersions =
            [
                "MicroStation V8 Format",
                "MicroStation V7 Format"
            ];

            Filter = "Microstation DGN Files |*.dgn";
            Title = "Export DGN";
        }

        public override bool Export()
        {
            base.Export();

            ICollection<ElementId> views = [];
            if (CurrentViewOnly)
                views.Add(ActiveDocument.ActiveView.Id);
            else
                foreach (View view in SelectViewsData.SelectedViews)
                {
                    views.Add(view.Id);
                }

            //parameter : DWGExportOptions dwgExportOptions
            DGNExportOptions dgnExportOptions = new()
            {
                // default values
                FileVersion = DGNFileFormat.DGNVersion8
            };
            ExportLayerMapping = m_enumLayerMapping[0];

            // set values from selected options
            dgnExportOptions.LayerMapping = ExportLayerMapping;
            dgnExportOptions.FileVersion = ExportFileVersion == "MicroStation V7 Format" ? DGNFileFormat.DGNVersion7 : DGNFileFormat.DGNVersion8;
            dgnExportOptions.HideScopeBox = HideScopeBox;
            dgnExportOptions.HideUnreferenceViewTags = HideUnreferenceViewTags;
            dgnExportOptions.HideReferencePlane = HideReferencePlane;
            var mainModule = Process.GetCurrentProcess().MainModule;
            var revitFolder = Path.GetDirectoryName(mainModule.FileName);
            dgnExportOptions.SeedName = Path.Combine(revitFolder, @"ACADInterop\V8-Imperial-Seed3D.dgn");

            //Export
            var exported = ActiveDocument.Export(ExportFolder, ExportFileName, views, dgnExportOptions);
            return exported;
        }
    }
}
