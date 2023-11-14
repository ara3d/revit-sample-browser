// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt
using System.ComponentModel;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Mechanical;

namespace Revit.SDK.Samples.ProjectInfo.CS
{
    /// <summary>
    ///     Wrapper class for gbXMLParamElem
    /// </summary>
    public class EnergyDataSettingsWrapper : IWrapper
    {
        /// <summary>
        ///     Revit Document
        /// </summary>
        private readonly Document m_document;

        /// <summary>
        ///     gbXMLParamElem
        /// </summary>
        private readonly EnergyDataSettings m_energyDataSettings;

        /// <summary>
        ///     Initializes private variables.
        /// </summary>
        /// <param name="gbXMLParamElem">gbXMLParamElem</param>
        public EnergyDataSettingsWrapper(Document document)
        {
            m_document = document;
            m_energyDataSettings = EnergyDataSettings.GetFromDocument(document);
        }


        /// <summary>
        ///     Gets or sets Building Type
        /// </summary>
        [Category("Common")]
        [DisplayName("Building Type")]
        [TypeConverter(typeof(BuildingTypeConverter))]
        public gbXMLBuildingType BuildingType
        {
            get => m_energyDataSettings.BuildingType;
            set => m_energyDataSettings.BuildingType = value;
        }

        /// <summary>
        ///     Gets or sets Ground Plane
        /// </summary>
        [Category("Common")]
        [DisplayName("Ground Plane")]
        [TypeConverter(typeof(ElementIdConverter<Level>))]
        public ElementId GroundPlane
        {
            get => m_energyDataSettings.GroundPlane;
            set => m_energyDataSettings.GroundPlane = value;
        }

        /// <summary>
        ///     Gets or sets Building Service
        /// </summary>
        [Category("Detailed Model")]
        [DisplayName("Building Service")]
        [TypeConverter(typeof(ServiceTypeConverter))]
        [RevitVersion(ProductType.MEP)]
        public gbXMLServiceType BuildingService
        {
            get => m_energyDataSettings.ServiceType;
            set => m_energyDataSettings.ServiceType = value;
        }

        /// <summary>
        ///     Gets Building Construction
        /// </summary>
        [Category("Detailed Model")]
        [DisplayName("Building Construction")]
        [TypeConverter(typeof(WrapperConverter))]
        [RevitVersion(ProductType.MEP)]
        public MEPBuildingConstructionWrapper BuildingConstruction
        {
            get
            {
                var eid = EnergyDataSettings.GetBuildingConstructionSetElementId(m_document);
                //MEPBuildingConstruction mEPBuildingConstruction = RevitStartInfo.GetElement(m_energyDataSettings.ConstructionSetElementId) as MEPBuildingConstruction;
                return RevitStartInfo.GetElement(eid) is MEPBuildingConstruction mEPBuildingConstruction
                    ? new MEPBuildingConstructionWrapper(mEPBuildingConstruction)
                    : null;
            }
        }

        /// <summary>
        ///     Gets and Sets BuildingConstructionClass
        /// </summary>
        [Category("Detailed Model")]
        [DisplayName("Building Infiltration Class")]
        [TypeConverter(typeof(HVACLoadConstructionClassConverter))]
        [RevitVersion(ProductType.MEP)]
        public HVACLoadConstructionClass BuildingConstructionClass
        {
            get => m_energyDataSettings.BuildingConstructionClass;
            set => m_energyDataSettings.BuildingConstructionClass = value;
        }

        /// <summary>
        ///     Gets or sets Project Phase
        /// </summary>
        [Category("Detailed Model")]
        [DisplayName("Project Phase")]
        [TypeConverter(typeof(ElementIdConverter<Phase>))]
        public ElementId ProjectPhase
        {
            get => m_energyDataSettings.ProjectPhase;
            set => m_energyDataSettings.ProjectPhase = value;
        }

        /// <summary>
        ///     Gets or sets Sliver Space Tolerance
        /// </summary>
        [Category("Detailed Model")]
        [DisplayName("Sliver Space Tolerance")]
        public double SliverSpaceTolerance
        {
            get => m_energyDataSettings.SliverSpaceTolerance;
            set => m_energyDataSettings.SliverSpaceTolerance = value;
        }

        /// <summary>
        ///     Gets or sets Export Complexity
        /// </summary>
        [Category("Detailed Model")]
        [DisplayName("Export Complexity")]
        [TypeConverter(typeof(ExportComplexityConverter))]
        public gbXMLExportComplexity ExportComplexity
        {
            get => m_energyDataSettings.ExportComplexity;
            set => m_energyDataSettings.ExportComplexity = value;
        }

        /// <summary>
        ///     Gets or sets Export Default Values
        /// </summary>
        [Category("Detailed Model")]
        [DisplayName("Export Default Values")]
        [RevitVersion(ProductType.MEP)]
        public bool ExportDefaultValues
        {
            get => m_energyDataSettings.ExportDefaults;
            set => m_energyDataSettings.ExportDefaults = value;
        }

        /// <summary>
        ///     Gets and Sets ProjectReportType
        /// </summary>
        [Category("Detailed Model")]
        [DisplayName("Report Type")]
        [TypeConverter(typeof(HVACLoadLoadsReportTypeConverter))]
        [RevitVersion(ProductType.MEP)]
        public HVACLoadLoadsReportType ProjectReportType
        {
            get => m_energyDataSettings.ProjectReportType;
            set => m_energyDataSettings.ProjectReportType = value;
        }

        /// <summary>
        ///     Gets Project Location
        /// </summary>
        [DisplayName("Project Location")]
        [TypeConverter(typeof(ProjectLocationConverter))]
        [RevitVersion(ProductType.MEP, ProductType.Architecture)]
        public ProjectLocation ProjectLocation
        {
            get => m_document.ActiveProjectLocation;
            set => m_document.ActiveProjectLocation = value;
        }

        /// <summary>
        ///     Gets Site Location
        /// </summary>
        [DisplayName("Site Location")]
        [TypeConverter(typeof(WrapperConverter))]
        [RevitVersion(ProductType.MEP, ProductType.Architecture)]
        public SiteLocationWrapper SiteLocation => new SiteLocationWrapper(m_document.SiteLocation);


        /// <summary>
        ///     Gets the handle object.
        /// </summary>
        [Browsable(false)]
        public object Handle => m_energyDataSettings;

        /// <summary>
        ///     Gets the name of the handle.
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get => "";
            set { }
        }
    }
}
