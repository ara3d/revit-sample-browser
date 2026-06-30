// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    public class ExportBaseOptionsData
    {
        private List<string> m_coorSystem;

        private List<bool> m_enumCoorSystem;

        private List<string> m_enumLayerMapping;

        private List<PropOverrideMode> m_enumLayersAndProperties;

        private List<LineScaling> m_enumLineScaling;

        private List<SolidGeometry> m_enumSolids;

        private List<ExportUnit> m_enumUnits;
        private List<string> m_layerMapping;

        private List<string> m_layersAndProperties;

        private List<string> m_lineScaling;

        private List<string> m_solids;

        private List<string> m_units;

        public ExportBaseOptionsData()
        {
            Initialize();
        }

        //Export rooms and areas as polylines

        public ReadOnlyCollection<string> LayersAndProperties => new(m_layersAndProperties);

        public ReadOnlyCollection<PropOverrideMode> EnumLayersAndProperties =>
            new(m_enumLayersAndProperties);

        public PropOverrideMode ExportLayersAndProperties { get; set; }

        public ReadOnlyCollection<string> LayerMapping => new(m_layerMapping);

        public ReadOnlyCollection<string> EnumLayerMapping => new(m_enumLayerMapping);

        public string ExportLayerMapping { get; set; }

        public ReadOnlyCollection<string> LineScaling => new(m_lineScaling);

        public ReadOnlyCollection<LineScaling> EnumLineScaling =>
            new(m_enumLineScaling);

        public LineScaling ExportLineScaling { get; set; }

        public ReadOnlyCollection<string> CoorSystem => new(m_coorSystem);

        public ReadOnlyCollection<bool> EnumCoorSystem => new(m_enumCoorSystem);

        public bool ExportCoorSystem { get; set; }

        public ReadOnlyCollection<string> Units => new(m_units);

        public ReadOnlyCollection<ExportUnit> EnumUnits => new(m_enumUnits);

        public ExportUnit ExportUnit { get; set; }

        public ReadOnlyCollection<string> Solids => new(m_solids);

        public ReadOnlyCollection<SolidGeometry> EnumSolids => new(m_enumSolids);

        public SolidGeometry ExportSolid { get; set; }

        public bool ExportAreas { get; set; }

        public bool ExportMergeFiles { get; set; }

        private void Initialize()
        {
            //Layers and properties:
            m_layersAndProperties = [];
            m_enumLayersAndProperties = [];
            m_layersAndProperties.Add("Category properties BYLAYER, overrides BYENTITY");
            m_enumLayersAndProperties.Add(PropOverrideMode.ByEntity);
            m_layersAndProperties.Add("All properties BYLAYER, no overrides");
            m_enumLayersAndProperties.Add(PropOverrideMode.ByLayer);
            m_layersAndProperties.Add("All properties BYLAYER, new Layers for overrides");
            m_enumLayersAndProperties.Add(PropOverrideMode.NewLayer);

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

            //Linetype scaling:
            m_lineScaling = [];
            m_enumLineScaling = [];
            m_lineScaling.Add("Scaled Linetype definitions");
            m_enumLineScaling.Add(Autodesk.Revit.DB.LineScaling.ViewScale);
            m_lineScaling.Add("ModelSpace (PSLTSCALE = 0)");
            m_enumLineScaling.Add(Autodesk.Revit.DB.LineScaling.ModelSpace);
            m_lineScaling.Add("Paperspace (PSLTSCALE = 1)");
            m_enumLineScaling.Add(Autodesk.Revit.DB.LineScaling.PaperSpace);

            //Coordinate system basis
            m_coorSystem = [];
            m_enumCoorSystem = [];
            m_coorSystem.Add("Project Internal");
            m_enumCoorSystem.Add(false);
            m_coorSystem.Add("Shared");
            m_enumCoorSystem.Add(true);

            //One DWG unit
            m_units = [];
            m_enumUnits = [];
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

            m_solids = [];
            m_enumSolids = [];
            m_solids.Add("Export as polymesh");
            m_enumSolids.Add(SolidGeometry.Polymesh);
            m_solids.Add("Export as ACIS solids");
            m_enumSolids.Add(SolidGeometry.ACIS);

            // Set default values
            ExportAreas = false;
            ExportSolid = SolidGeometry.Polymesh;
            ExportLayerMapping = "AIA";
            ExportLayersAndProperties = PropOverrideMode.ByEntity;
            ExportLineScaling = Autodesk.Revit.DB.LineScaling.PaperSpace;
            ExportMergeFiles = false;
            ExportCoorSystem = EnumCoorSystem[0];
            ExportUnit = ExportUnit.Inch;
        }
    }
}
