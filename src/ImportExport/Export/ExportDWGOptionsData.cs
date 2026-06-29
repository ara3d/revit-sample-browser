// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.ImportExport.CS.Export
{
    /// <summary>
    ///     Data class which stores lower priority information for exporting dwg format
    /// </summary>
    public class ExportDwgOptionsData
    {
        /// <summary>
        ///     String list of Coordinate system basis
        /// </summary>
        private List<string> m_coorSystem;

        /// <summary>
        ///     List of values whether to use shared coordinate system
        /// </summary>
        private List<bool> m_enumCoorSystem;

        /// <summary>
        ///     String list of layer settings values defined in Revit
        /// </summary>
        private List<string> m_enumLayerMapping;

        /// <summary>
        ///     List of Autodesk.Revit.DB.PropOverrideMode
        /// </summary>
        private List<PropOverrideMode> m_enumLayersAndProperties;

        /// <summary>
        ///     List of Autodesk.Revit.DB.LineScaling defined in Revit
        /// </summary>
        private List<LineScaling> m_enumLineScaling;

        /// <summary>
        ///     List of Autodesk.Revit.DB.SolidGeometry defined in Revit
        /// </summary>
        private List<SolidGeometry> m_enumSolids;

        /// <summary>
        ///     List of Autodesk.Revit.DB.ExportUnit values defined in Revit
        /// </summary>
        private List<ExportUnit> m_enumUnits;

        /// <summary>
        ///     Coordinate system basis option to export
        /// </summary>
        private bool m_exportCoorSystem;

        /// <summary>
        ///     Layer setting option to export
        /// </summary>
        private string m_exportLayerMapping;

        /// <summary>
        ///     PropOverrideMode Option to export
        /// </summary>
        private PropOverrideMode m_exportLayersAndProperties;

        /// <summary>
        ///     Line scaling option to export
        /// </summary>
        private LineScaling m_exportLineScaling;

        /// <summary>
        ///     Whether to create separate files for each view/sheet
        /// </summary>
        private bool m_exportMergeFiles;

        /// <summary>
        ///     Solid geometry option to export
        /// </summary>
        private SolidGeometry m_exportSolid;

        /// <summary>
        ///     Export unit option to export
        /// </summary>
        private ExportUnit m_exportUnit;

        /// <summary>
        ///     String list of Layer Settings used in UI
        /// </summary>
        private List<string> m_layerMapping;

        /// <summary>
        ///     String list of Layers and properties used in UI
        /// </summary>
        private List<string> m_layersAndProperties;

        /// <summary>
        ///     String list of Linetype scaling used in UI
        /// </summary>
        private List<string> m_lineScaling;

        /// <summary>
        ///     String list of solid used in UI
        /// </summary>
        private List<string> m_solids;

        /// <summary>
        ///     String list of DWG unit
        /// </summary>
        private List<string> m_units;

        /// <summary>
        ///     Constructor
        /// </summary>
        public ExportDwgOptionsData()
        {
            Initialize();
        }

        //Export rooms and areas as polylines

        /// <summary>
        ///     String collection of Layers and properties used in UI
        /// </summary>
        public ReadOnlyCollection<string> LayersAndProperties => new ReadOnlyCollection<string>(m_layersAndProperties);

        /// <summary>
        ///     Collection of Autodesk.Revit.DB.PropOverrideMode
        /// </summary>
        public ReadOnlyCollection<PropOverrideMode> EnumLayersAndProperties =>
            new ReadOnlyCollection<PropOverrideMode>(m_enumLayersAndProperties);

        /// <summary>
        ///     PropOverrideMode Option to export
        /// </summary>
        public PropOverrideMode ExportLayersAndProperties
        {
            get => m_exportLayersAndProperties;
            set => m_exportLayersAndProperties = value;
        }

        /// <summary>
        ///     String collection of Layer Settings used in UI
        /// </summary>
        public ReadOnlyCollection<string> LayerMapping => new ReadOnlyCollection<string>(m_layerMapping);

        /// <summary>
        ///     String collection of layer settings values defined in Revit
        /// </summary>
        public ReadOnlyCollection<string> EnumLayerMapping => new ReadOnlyCollection<string>(m_enumLayerMapping);

        /// <summary>
        ///     Layer setting option to export
        /// </summary>
        public string ExportLayerMapping
        {
            get => m_exportLayerMapping;
            set => m_exportLayerMapping = value;
        }

        /// <summary>
        ///     String collection of Linetype scaling used in UI
        /// </summary>
        public ReadOnlyCollection<string> LineScaling => new ReadOnlyCollection<string>(m_lineScaling);

        /// <summary>
        ///     Collection of Autodesk.Revit.DB.LineScaling defined in Revit
        /// </summary>
        public ReadOnlyCollection<LineScaling> EnumLineScaling =>
            new ReadOnlyCollection<LineScaling>(m_enumLineScaling);

        /// <summary>
        ///     Line scaling option to export
        /// </summary>
        public LineScaling ExportLineScaling
        {
            get => m_exportLineScaling;
            set => m_exportLineScaling = value;
        }

        /// <summary>
        ///     String collection of Coordinate system basis
        /// </summary>
        public ReadOnlyCollection<string> CoorSystem => new ReadOnlyCollection<string>(m_coorSystem);

        /// <summary>
        ///     Collection of values whether to use shared coordinate system
        /// </summary>
        public ReadOnlyCollection<bool> EnumCoorSystem => new ReadOnlyCollection<bool>(m_enumCoorSystem);

        /// <summary>
        ///     Coordinate system basis option to export
        /// </summary>
        public bool ExportCoorSystem
        {
            get => m_exportCoorSystem;
            set => m_exportCoorSystem = value;
        }

        /// <summary>
        ///     String collection of DWG unit
        /// </summary>
        public ReadOnlyCollection<string> Units => new ReadOnlyCollection<string>(m_units);

        /// <summary>
        ///     Collection of Autodesk.Revit.DB.ExportUnit values defined in Revit
        /// </summary>
        public ReadOnlyCollection<ExportUnit> EnumUnits => new ReadOnlyCollection<ExportUnit>(m_enumUnits);

        /// <summary>
        ///     Export unit option to export
        /// </summary>
        public ExportUnit ExportUnit
        {
            get => m_exportUnit;
            set => m_exportUnit = value;
        }

        /// <summary>
        ///     String collection of solid used in UI
        /// </summary>
        public ReadOnlyCollection<string> Solids => new ReadOnlyCollection<string>(m_solids);

        /// <summary>
        ///     Collection of Autodesk.Revit.DB.SolidGeometry defined in Revit
        /// </summary>
        public ReadOnlyCollection<SolidGeometry> EnumSolids => new ReadOnlyCollection<SolidGeometry>(m_enumSolids);

        /// <summary>
        ///     Property of solid geometry option to export
        /// </summary>
        public SolidGeometry ExportSolid
        {
            get => m_exportSolid;
            set => m_exportSolid = value;
        }

        /// <summary>
        ///     Export rooms and areas as polylines
        /// </summary>
        public bool ExportAreas { get; set; }

        /// <summary>
        ///     Whether to create separate files for each view/sheet
        /// </summary>
        public bool ExportMergeFiles
        {
            get => m_exportMergeFiles;
            set => m_exportMergeFiles = value;
        }

        /// <summary>
        ///     Initialize values
        /// </summary>
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
