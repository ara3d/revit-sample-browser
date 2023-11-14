// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;
using Application = Autodesk.Revit.ApplicationServices.Application;

namespace Revit.SDK.Samples.ProjectInfo.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // initialize global information
            RevitStartInfo.RevitApp = commandData.Application.Application;
            RevitStartInfo.RevitDoc = commandData.Application.ActiveUIDocument.Document;
            RevitStartInfo.RevitProduct = commandData.Application.Application.Product;

            var transaction = new Transaction(RevitStartInfo.RevitDoc, "ProjectInfo");
            try
            {
                // Start transaction
                transaction.Start();

                // get current project information
                var pi = commandData.Application.ActiveUIDocument.Document.ProjectInformation;

                // show main form
                using (var pif = new ProjectInfoForm(new ProjectInfoWrapper(pi)))
                {
                    pif.StartPosition = FormStartPosition.CenterParent;
                    if (pif.ShowDialog() == DialogResult.OK)
                    {
                        transaction.Commit();
                        return Result.Succeeded;
                    }

                    transaction.RollBack();
                    return Result.Cancelled;
                }
            }
            catch (Exception ex)
            {
                transaction.RollBack();
                message = ex.ToString();
                return Result.Failed;
            }
        }
    }

    /// <summary>
    ///     Preserves global information
    /// </summary>
    public static class RevitStartInfo
    {
        /// <summary>
        ///     Current Revit application
        /// </summary>
        public static Application RevitApp;

        /// <summary>
        ///     Active Revit document
        /// </summary>
        public static Document RevitDoc;

        /// <summary>
        ///     Current Revit Product
        /// </summary>
        public static ProductType RevitProduct;

        /// <summary>
        ///     Time Zone Array
        /// </summary>
        public static string[] TimeZones;

        /// <summary>
        ///     BuildingType and its display string map.
        /// </summary>
        public static Dictionary<object, string> BuildingTypeMap;

        /// <summary>
        ///     ServiceType and its display string map.
        /// </summary>
        public static Dictionary<object, string> ServiceTypeMap;

        /// <summary>
        ///     ExportComplexity and its display string map.
        /// </summary>
        public static Dictionary<object, string> ExportComplexityMap;

        /// <summary>
        ///     HVACLoadLoadsReportType and its display string map.
        /// </summary>
        public static Dictionary<object, string> HVACLoadLoadsReportTypeMap;

        /// <summary>
        ///     HVACLoadConstructionClass and its display string map.
        /// </summary>
        public static Dictionary<object, string> HVACLoadConstructionClassMap;

        /// <summary>
        ///     Initialize some static members
        /// </summary>
        static RevitStartInfo()
        {
            TimeZones = new[]
            {
                "(GMT-12:00) International Date Line West",
                "(GMT-11:00) Midway Island, Samoa",
                "(GMT-10:00) Hawaii",
                "(GMT-09:00) Alaska",
                "(GMT-08:00) Pacific Time (US/Canada)",
                "(GMT-08:00) Tijuana, Baja California",
                "(GMT-07:00) Arizona",
                "(GMT-07:00) Chihuahua, La Paz, Mazatlan - New",
                "(GMT-07:00) Chihuahua, La Paz, Mazatlan - Old",
                "(GMT-07:00) Mountain Time (US/Canada)",
                "(GMT-06:00) Central America",
                "(GMT-06:00) Central Time (US/Canada)",
                "(GMT-06:00) Guadalajara, Mexico City, Monterrey - New",
                "(GMT-06:00) Guadalajara, Mexico City, Monterrey - Old",
                "(GMT-06:00) Saskatchewan",
                "(GMT-05:00) Bogota, Lima, Quito, Rio Branco",
                "(GMT-05:00) Eastern Time (US/Canada)",
                "(GMT-05:00) Indiana (East)",
                "(GMT-04:00) Atlantic Time (Canada)",
                "(GMT-04:00) Caracas, La Paz",
                "(GMT-04:00) Santiago",
                "(GMT-03:30) Newfoundland",
                "(GMT-03:00) Brazilia",
                "(GMT-03:00) Buanos Aires, Georgetown",
                "(GMT-03:00) Greenland",
                "(GMT-03:00) Montevideo",
                "(GMT-02:00) Mid-Atlantic",
                "(GMT-01:00) Azores",
                "(GMT-01:00) Cape Verde Is.",
                "(GMT) Casablanca, Monrovia,Reykjavik",
                "(GMT) Greenwich Time: Dublin, Edinburgh, Lisbon, London",
                "(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna",
                "(GMT+01:00) Belgrade, Brastislava, Budapest, Ljubljana, Prague",
                "(GMT+01:00) Brussels, Copenhagen, Madrid, Paris",
                "(GMT+01:00) Sarajevo, Skopje, Sofija, Vilnus, Warsaw, Zagreb",
                "(GMT+01:00) West Central Africa",
                "(GMT+02:00) Amman",
                "(GMT+02:00) Athens, Bucharest, Istanbul",
                "(GMT+02:00) Beirut",
                "(GMT+02:00) Cairo",
                "(GMT+02:00) Harare, Pretoria",
                "(GMT+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius",
                "(GMT+02:00) Jerusalem",
                "(GMT+02:00) Minsk",
                "(GMT+02:00) Windhoek",
                "(GMT+03:00) Baghdad",
                "(GMT+03:00) Kuwait, Riyadh",
                "(GMT+03:00) Moscow, St. Petersburg, Volgograd",
                "(GMT+03:00) Nairobi",
                "(GMT+03:00) Tbilisi",
                "(GMT+03:00) Tehran",
                "(GMT+04:00) Abu Dhabi, Muscat",
                "(GMT+04:00) Baku",
                "(GMT+04:00) Yerevan",
                "(GMT+04:30) Kabul",
                "(GMT+05:00) Ekaterinburg",
                "(GMT+05:00) Islamabad, Karachi, Tashkent",
                "(GMT+05:30) Chennai, Kolkata, Mumbai, New Delhi",
                "(GMT+05:30) Sri Jayawardenepura",
                "(GMT+05:45) Kathmandu ",
                "(GMT+06:00) Almaty, Novosibirsk",
                "(GMT+06:00) Astana, Dhaka",
                "(GMT+06:30) Yangon (Rangoon)",
                "(GMT+07:00) Bangkok, Hanoi, Jakarta ",
                "(GMT+07:00) Krasnoyarsk ",
                "(GMT+08:00) Beijing, Chongqing, Hong Kong, Urumqi ",
                "(GMT+08:00) Irkutsk, Ulaan Bataar ",
                "(GMT+08:00) Kuala Lumpur, Singapore ",
                "(GMT+08:00) Perth",
                "(GMT+08:00) Taipei",
                "(GMT+09:00) Osaka, Sapporo, Tokyo",
                "(GMT+09:00) Seoul",
                "(GMT+09:00) Yakutsk",
                "(GMT+09:30) Adelaide",
                "(GMT+09:30) Darwin",
                "(GMT+10:00) Brisbane",
                "(GMT+10:00) Canberra, Melbourne, Sydney",
                "(GMT+10:00) Guam, Port Moresby",
                "(GMT+10:00) Hobart",
                "(GMT+10:00) Vladivostok",
                "(GMT+11:00) Magadan, Solomon Is., New Caledonia ",
                "(GMT+12:00) Aukland, Wellington ",
                "(GMT+12:00) Fiji, Kamchatka, Marshall Is.",
                "(GMT+13:00) Nubu'alofa"
            };

            BuildingTypeMap = new Dictionary<object, string>();
            BuildingTypeMap.Add(gbXMLBuildingType.AutomotiveFacility, "Automotive Facility");
            BuildingTypeMap.Add(gbXMLBuildingType.ConventionCenter, "Convention Center");
            BuildingTypeMap.Add(gbXMLBuildingType.Courthouse, "Courthouse");
            BuildingTypeMap.Add(gbXMLBuildingType.DiningBarLoungeOrLeisure, "Dining Bar Lounge or Leisure");
            BuildingTypeMap.Add(gbXMLBuildingType.DiningCafeteriaFastFood, "Dining Cafeteria Fast Food");
            BuildingTypeMap.Add(gbXMLBuildingType.DiningFamily, "Dining Family");
            BuildingTypeMap.Add(gbXMLBuildingType.Dormitory, "Dormitory");
            BuildingTypeMap.Add(gbXMLBuildingType.ExerciseCenter, "Exercise Center");
            BuildingTypeMap.Add(gbXMLBuildingType.FireStation, "Fire Station");
            BuildingTypeMap.Add(gbXMLBuildingType.Gymnasium, "Gymnasium");
            BuildingTypeMap.Add(gbXMLBuildingType.HospitalOrHealthcare, "Hospital or Healthcare");
            BuildingTypeMap.Add(gbXMLBuildingType.Hotel, "Hotel");
            BuildingTypeMap.Add(gbXMLBuildingType.Library, "Library");
            BuildingTypeMap.Add(gbXMLBuildingType.Manufacturing, "Manufacturing");
            BuildingTypeMap.Add(gbXMLBuildingType.Motel, "Motel");
            BuildingTypeMap.Add(gbXMLBuildingType.MotionPictureTheatre, "Motion Picture Theatre");
            BuildingTypeMap.Add(gbXMLBuildingType.MultiFamily, "Multi Family");
            BuildingTypeMap.Add(gbXMLBuildingType.Museum, "Museum");
            BuildingTypeMap.Add(gbXMLBuildingType.NoOfBuildingTypes, "None");
            BuildingTypeMap.Add(gbXMLBuildingType.Office, "Office");
            BuildingTypeMap.Add(gbXMLBuildingType.ParkingGarage, "Parking Garage");
            BuildingTypeMap.Add(gbXMLBuildingType.Penitentiary, "Penitentiary");
            BuildingTypeMap.Add(gbXMLBuildingType.PerformingArtsTheater, "Performing Arts Theater");
            BuildingTypeMap.Add(gbXMLBuildingType.PoliceStation, "Police Station");
            BuildingTypeMap.Add(gbXMLBuildingType.PostOffice, "Post Office");
            BuildingTypeMap.Add(gbXMLBuildingType.ReligiousBuilding, "Religious Building");
            BuildingTypeMap.Add(gbXMLBuildingType.Retail, "Retail");
            BuildingTypeMap.Add(gbXMLBuildingType.SchoolOrUniversity, "School or University");
            BuildingTypeMap.Add(gbXMLBuildingType.SingleFamily, "Single Family");
            BuildingTypeMap.Add(gbXMLBuildingType.SportsArena, "Sports Arena");
            BuildingTypeMap.Add(gbXMLBuildingType.TownHall, "Town Hall");
            BuildingTypeMap.Add(gbXMLBuildingType.Transportation, "Transportation");
            BuildingTypeMap.Add(gbXMLBuildingType.Warehouse, "Warehouse");
            BuildingTypeMap.Add(gbXMLBuildingType.Workshop, "Workshop");

            ServiceTypeMap = new Dictionary<object, string>();
            ServiceTypeMap.Add(gbXMLServiceType.ActiveChilledBeams, "Active Chilled Beams");
            ServiceTypeMap.Add(gbXMLServiceType.CentralHeatingConvectors, "Central Heating: Convectors");
            ServiceTypeMap.Add(gbXMLServiceType.CentralHeatingHotAir, "Central Heating: Hot Air");
            ServiceTypeMap.Add(gbXMLServiceType.CentralHeatingRadiantFloor, "Central Heating: Radiant Floor");
            ServiceTypeMap.Add(gbXMLServiceType.CentralHeatingRadiators, "Central Heating: Radiators");
            ServiceTypeMap.Add(gbXMLServiceType.ConstantVolumeDualDuct, "Constant Volume - Dual Duct");
            ServiceTypeMap.Add(gbXMLServiceType.ConstantVolumeFixedOA, "Constant Volume - Fixed OA");
            ServiceTypeMap.Add(gbXMLServiceType.ConstantVolumeTerminalReheat, "Constant Volume - Terminal Reheat");
            ServiceTypeMap.Add(gbXMLServiceType.ConstantVolumeVariableOA, "Constant Volume - Variable OA");
            ServiceTypeMap.Add(gbXMLServiceType.FanCoilSystem, "Fan Coil System");
            ServiceTypeMap.Add(gbXMLServiceType.ForcedConvectionHeaterFlue, "Forced Convection Heater - Flue");
            ServiceTypeMap.Add(gbXMLServiceType.ForcedConvectionHeaterNoFlue, "Forced Convection Heater - No Flue");
            ServiceTypeMap.Add(gbXMLServiceType.InductionSystem, "Induction System");
            ServiceTypeMap.Add(gbXMLServiceType.MultizoneHotDeckColdDeck, "Multi-zone - Hot Deck / Cold Deck");
            ServiceTypeMap.Add(gbXMLServiceType.NoServiceType, "None");
            ServiceTypeMap.Add(gbXMLServiceType.OtherRoomHeater, "Other Room Heater");
            ServiceTypeMap.Add(gbXMLServiceType.RadiantCooledCeilings, "Radiant Cooled Ceilings");
            ServiceTypeMap.Add(gbXMLServiceType.RadiantHeaterFlue, "Radiant Heater - Flue");
            ServiceTypeMap.Add(gbXMLServiceType.RadiantHeaterMultiburner, "Radiant Heater - Multi-burner");
            ServiceTypeMap.Add(gbXMLServiceType.RadiantHeaterNoFlue, "Radiant Heater - No Flue");
            ServiceTypeMap.Add(gbXMLServiceType.SplitSystemsWithMechanicalVentilation,
                "Split System(s) with Mechanical Ventilation");
            ServiceTypeMap.Add(gbXMLServiceType.SplitSystemsWithMechanicalVentilationWithCooling,
                "Split System(s) with Mechanical Ventilation with Cooling");
            ServiceTypeMap.Add(gbXMLServiceType.SplitSystemsWithNaturalVentilation,
                "Split System(s) with Natural Ventilation");
            ServiceTypeMap.Add(gbXMLServiceType.VariableRefrigerantFlow, "Variable Refrigerant Flow");
            ServiceTypeMap.Add(gbXMLServiceType.VAVDualDuct, "VAV - Dual Duct");
            ServiceTypeMap.Add(gbXMLServiceType.VAVIndoorPackagedCabinet, "VAV - Indoor Packaged Cabinet");
            ServiceTypeMap.Add(gbXMLServiceType.VAVSingleDuct, "VAV - Single Duct");
            ServiceTypeMap.Add(gbXMLServiceType.VAVTerminalReheat, "VAV - Terminal Reheat");
            ServiceTypeMap.Add(gbXMLServiceType.WaterLoopHeatPump, "Water Loop Heat Pump");

            ExportComplexityMap = new Dictionary<object, string>();
            ExportComplexityMap.Add(gbXMLExportComplexity.Complex, "Complex");
            ExportComplexityMap.Add(gbXMLExportComplexity.ComplexWithMullionsAndShadingSurfaces,
                "Complex With Mullions And Shading Surfaces");
            ExportComplexityMap.Add(gbXMLExportComplexity.ComplexWithShadingSurfaces, "Complex With Shading Surfaces");
            ExportComplexityMap.Add(gbXMLExportComplexity.Simple, "Simple");
            ExportComplexityMap.Add(gbXMLExportComplexity.SimpleWithShadingSurfaces, "Simple With Shading Surfaces");

            HVACLoadLoadsReportTypeMap = new Dictionary<object, string>();
            HVACLoadLoadsReportTypeMap.Add(HVACLoadLoadsReportType.DetailedReport, "Detailed");
            HVACLoadLoadsReportTypeMap.Add(HVACLoadLoadsReportType.NoReport, "No");
            HVACLoadLoadsReportTypeMap.Add(HVACLoadLoadsReportType.SimpleReport, "Simple");
            HVACLoadLoadsReportTypeMap.Add(HVACLoadLoadsReportType.StandardReport, "Standard");

            HVACLoadConstructionClassMap = new Dictionary<object, string>();
            HVACLoadConstructionClassMap.Add(HVACLoadConstructionClass.LooseConstruction, "Loose");
            HVACLoadConstructionClassMap.Add(HVACLoadConstructionClass.NoneConstruction, "None");
            HVACLoadConstructionClassMap.Add(HVACLoadConstructionClass.MediumConstruction, "Medium");
            HVACLoadConstructionClassMap.Add(HVACLoadConstructionClass.TightConstruction, "Tight");
        }

        public static Element GetElement(ElementId elementId)
        {
            return RevitDoc.GetElement(elementId);
        }

        public static Element GetElement(long elementId)
        {
            return GetElement(new ElementId(elementId));
        }
    }
}
