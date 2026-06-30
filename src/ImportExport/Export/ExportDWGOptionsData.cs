// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportDwgOptionsData
    {
        private List<string> m_coorSystem;

        private List<bool> m_enumCoorSystem;

        private List<string> m_enumLayerMapping;

        private List<PropOverrideMode> m_enumLayersAndProperties;

        private List<LineScaling> m_enumLineScaling;

        private List<SolidGeometry> m_enumSolids;

        private List<ExportUnit> m_enumUnits;

        private bool m_exportCoorSystem;

        private string m_exportLayerMapping;

        private PropOverrideMode m_exportLayersAndProperties;

        private LineScaling m_exportLineScaling;

        private bool m_exportMergeFiles;

        private SolidGeometry m_exportSolid;

        private ExportUnit m_exportUnit;

        private List<string> m_layerMapping;

        private List<string> m_layersAndProperties;

        private List<string> m_lineScaling;

        private List<string> m_solids;

        private List<string> m_units;

        public ExportDwgOptionsData()
        {
            Initialize();
        }

        //Export rooms and areas as polylines

        public ReadOnlyCollection<string> LayersAndProperties => new ReadOnlyCollection<string>(m_layersAndProperties);

        public ReadOnlyCollection<PropOverrideMode> EnumLayersAndProperties =>
            new ReadOnlyCollection<PropOverrideMode>(m_enumLayersAndProperties);

        public PropOverrideMode ExportLayersAndProperties
        {
            get => m_exportLayersAndProperties;
            set => m_exportLayersAndProperties = value;
        }

        public ReadOnlyCollection<string> LayerMapping => new ReadOnlyCollection<string>(m_layerMapping);

        public ReadOnlyCollection<string> EnumLayerMapping => new ReadOnlyCollection<string>(m_enumLayerMapping);

        public string ExportLayerMapping
        {
            get => m_exportLayerMapping;
            set => m_exportLayerMapping = value;
        }

        public ReadOnlyCollection<string> LineScaling => new ReadOnlyCollection<string>(m_lineScaling);

        public ReadOnlyCollection<LineScaling> EnumLineScaling =>
            new ReadOnlyCollection<LineScaling>(m_enumLineScaling);

        public LineScaling ExportLineScaling
        {
            get => m_exportLineScaling;
            set => m_exportLineScaling = value;
        }

        public ReadOnlyCollection<string> CoorSystem => new ReadOnlyCollection<string>(m_coorSystem);

        public ReadOnlyCollection<bool> EnumCoorSystem => new ReadOnlyCollection<bool>(m_enumCoorSystem);

        public bool ExportCoorSystem
        {
            get => m_exportCoorSystem;
            set => m_exportCoorSystem = value;
        }

        public ReadOnlyCollection<string> Units => new ReadOnlyCollection<string>(m_units);

        public ReadOnlyCollection<ExportUnit> EnumUnits => new ReadOnlyCollection<ExportUnit>(m_enumUnits);

        public ExportUnit ExportUnit
        {
            get => m_exportUnit;
            set => m_exportUnit = value;
        }

        public ReadOnlyCollection<string> Solids => new ReadOnlyCollection<string>(m_solids);

        public ReadOnlyCollection<SolidGeometry> EnumSolids => new ReadOnlyCollection<SolidGeometry>(m_enumSolids);

        public SolidGeometry ExportSolid
        {
            get => m_exportSolid;
            set => m_exportSolid = value;
        }

        public bool ExportAreas { get; set; }

        public bool ExportMergeFiles
        {
            get => m_exportMergeFiles;
            set => m_exportMergeFiles = value;
        }

        private void Initialize()
        {
            //Layers and properties:
            m_layersAndProperties = new List<string>();
            m_enumLayersAndProperties = new List<PropOverrideMode>();
            m_layersAndProperties.Add("Category properties BYLAYER, overrides BYENTITY");
            m_enumLayersAndProperties.Add(PropOverrideMode.ByEntity);
            m_layersAndProperties.Add("All properties BYLAYER, no overrides");
            m_enumLayersAndProperties.Add(PropOverrideMode.ByLayer);
            m_layersAndProperties.Add("All properties BYLAYER, new Layers for overrides");
            m_enumLayersAndProperties.Add(PropOverrideMode.NewLayer);

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

            //Linetype scaling:
            m_lineScaling = new List<string>();
            m_enumLineScaling = new List<LineScaling>();
            m_lineScaling.Add("Scaled Linetype definitions");
            m_enumLineScaling.Add(Autodesk.Revit.DB.LineScaling.ViewScale);
            m_lineScaling.Add("ModelSpace (PSLTSCALE = 0)");
            m_enumLineScaling.Add(Autodesk.Revit.DB.LineScaling.ModelSpace);
            m_lineScaling.Add("Paperspace (PSLTSCALE = 1)");
            m_enumLineScaling.Add(Autodesk.Revit.DB.LineScaling.PaperSpace);

            //Coordinate system basis
            m_coorSystem = new List<string>();
            m_enumCoorSystem = new List<bool>();
            m_coorSystem.Add("Project Internal");
            m_enumCoorSystem.Add(false);
            m_coorSystem.Add("Shared");
            m_enumCoorSystem.Add(true);

            //One DWG unit
            m_units = new List<string>();
            m_enumUnits = new List<ExportUnit>();
            m_units.Add(ExportUnit.Foot.ToString().ToLower());
            m_enumUnits.Add(ExportUnit.Foot);
            m_units.Add(ExportUnit.Inch.ToString().ToLower());
            m_enumUnits.Add(ExportUnit.Inch);
            m_units.Add(ExportUnit.Meter.ToString().ToLower());
            m_enumUnits.Add(ExportUnit.Meter);
            m_units.Add(ExportUnit.Centimeter.ToString().ToLower());
            m_enumUnits.Add(ExportUnit.Centimeter);
            m_units.Add(ExportUnit.Millimeter.ToString().ToLower());
            m_enumUnits.Add(ExportUnit.Millimeter);

            m_solids = new List<string>();
            m_enumSolids = new List<SolidGeometry>();
            m_solids.Add("Export as polymesh");
            m_enumSolids.Add(SolidGeometry.Polymesh);
            m_solids.Add("Export as ACIS solids");
            m_enumSolids.Add(SolidGeometry.ACIS);
        }
    }
}
