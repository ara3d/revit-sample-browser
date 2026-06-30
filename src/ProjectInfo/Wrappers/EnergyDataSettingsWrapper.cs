// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.ComponentModel;
using Ara3D.RevitSampleBrowser.ProjectInfo.CS.Converters;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Mechanical;

namespace Ara3D.RevitSampleBrowser.ProjectInfo.CS.Wrappers
{
    public class EnergyDataSettingsWrapper : IWrapper
    {
        /// <summary>
        ///     Revit Document
        /// </summary>
        private readonly Document m_document;

        private readonly EnergyDataSettings m_energyDataSettings;

        public EnergyDataSettingsWrapper(Document document)
        {
            m_document = document;
            m_energyDataSettings = EnergyDataSettings.GetFromDocument(document);
        }

        [Category("Common")]
        [DisplayName("Building Type")]
        [TypeConverter(typeof(BuildingTypeConverter))]
        public gbXMLBuildingType BuildingType
        {
            get => m_energyDataSettings.BuildingType;
            set => m_energyDataSettings.BuildingType = value;
        }

        [Category("Common")]
        [DisplayName("Ground Plane")]
        [TypeConverter(typeof(ElementIdConverter<Level>))]
        public ElementId GroundPlane
        {
            get => m_energyDataSettings.GroundPlane;
            set => m_energyDataSettings.GroundPlane = value;
        }

        [Category("Detailed Model")]
        [DisplayName("Building Service")]
        [TypeConverter(typeof(ServiceTypeConverter))]
        [RevitVersion(ProductType.MEP)]
        public gbXMLServiceType BuildingService
        {
            get => m_energyDataSettings.ServiceType;
            set => m_energyDataSettings.ServiceType = value;
        }

        [Category("Detailed Model")]
        [DisplayName("Building Construction")]
        [TypeConverter(typeof(WrapperConverter))]
        [RevitVersion(ProductType.MEP)]
        public MepBuildingConstructionWrapper BuildingConstruction
        {
            get
            {
                var eid = EnergyDataSettings.GetBuildingConstructionSetElementId(m_document);
                //MEPBuildingConstruction mEPBuildingConstruction = RevitStartInfo.GetElement(m_energyDataSettings.ConstructionSetElementId) as MEPBuildingConstruction;
                return RevitStartInfo.GetElement(eid) is MEPBuildingConstruction mEpBuildingConstruction
                    ? new MepBuildingConstructionWrapper(mEpBuildingConstruction)
                    : null;
            }
        }

        [Category("Detailed Model")]
        [DisplayName("Building Infiltration Class")]
        [TypeConverter(typeof(HvacLoadConstructionClassConverter))]
        [RevitVersion(ProductType.MEP)]
        public HVACLoadConstructionClass BuildingConstructionClass
        {
            get => m_energyDataSettings.BuildingConstructionClass;
            set => m_energyDataSettings.BuildingConstructionClass = value;
        }

        [Category("Detailed Model")]
        [DisplayName("Project Phase")]
        [TypeConverter(typeof(ElementIdConverter<Phase>))]
        public ElementId ProjectPhase
        {
            get => m_energyDataSettings.ProjectPhase;
            set => m_energyDataSettings.ProjectPhase = value;
        }

        [Category("Detailed Model")]
        [DisplayName("Sliver Space Tolerance")]
        public double SliverSpaceTolerance
        {
            get => m_energyDataSettings.SliverSpaceTolerance;
            set => m_energyDataSettings.SliverSpaceTolerance = value;
        }

        [Category("Detailed Model")]
        [DisplayName("Export Complexity")]
        [TypeConverter(typeof(ExportComplexityConverter))]
        public gbXMLExportComplexity ExportComplexity
        {
            get => m_energyDataSettings.ExportComplexity;
            set => m_energyDataSettings.ExportComplexity = value;
        }

        [Category("Detailed Model")]
        [DisplayName("Export Default Values")]
        [RevitVersion(ProductType.MEP)]
        public bool ExportDefaultValues
        {
            get => m_energyDataSettings.ExportDefaults;
            set => m_energyDataSettings.ExportDefaults = value;
        }

        [Category("Detailed Model")]
        [DisplayName("Report Type")]
        [TypeConverter(typeof(HvacLoadLoadsReportTypeConverter))]
        [RevitVersion(ProductType.MEP)]
        public HVACLoadLoadsReportType ProjectReportType
        {
            get => m_energyDataSettings.ProjectReportType;
            set => m_energyDataSettings.ProjectReportType = value;
        }

        [DisplayName("Project Location")]
        [TypeConverter(typeof(ProjectLocationConverter))]
        [RevitVersion(ProductType.MEP, ProductType.Architecture)]
        public ProjectLocation ProjectLocation
        {
            get => m_document.ActiveProjectLocation;
            set => m_document.ActiveProjectLocation = value;
        }

        [DisplayName("Site Location")]
        [TypeConverter(typeof(WrapperConverter))]
        [RevitVersion(ProductType.MEP, ProductType.Architecture)]
        public SiteLocationWrapper SiteLocation => new SiteLocationWrapper(m_document.SiteLocation);

        [Browsable(false)]
        public object Handle => m_energyDataSettings;

        [Browsable(false)]
        public string Name
        {
            get => "";
            set { }
        }
    }
}
