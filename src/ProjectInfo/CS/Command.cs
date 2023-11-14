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
        public static readonly string[] TimeZones;

        /// <summary>
        ///     BuildingType and its display string map.
        /// </summary>
        public static readonly Dictionary<object, string> BuildingTypeMap;

        /// <summary>
        ///     ServiceType and its display string map.
        /// </summary>
        public static readonly Dictionary<object, string> ServiceTypeMap;

        /// <summary>
        ///     ExportComplexity and its display string map.
        /// </summary>
        public static readonly Dictionary<object, string> ExportComplexityMap;

        /// <summary>
        ///     HVACLoadLoadsReportType and its display string map.
        /// </summary>
        public static readonly Dictionary<object, string> HvacLoadLoadsReportTypeMap;

        /// <summary>
        ///     HVACLoadConstructionClass and its display string map.
        /// </summary>
        public static readonly Dictionary<object, string> HvacLoadConstructionClassMap;

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

            BuildingTypeMap = new Dictionary<object, string>
            {
                { gbXMLBuildingType.AutomotiveFacility, "Automotive Facility" },
                { gbXMLBuildingType.ConventionCenter, "Convention Center" },
                { gbXMLBuildingType.Courthouse, "Courthouse" },
                { gbXMLBuildingType.DiningBarLoungeOrLeisure, "Dining Bar Lounge or Leisure" },
                { gbXMLBuildingType.DiningCafeteriaFastFood, "Dining Cafeteria Fast Food" },
                { gbXMLBuildingType.DiningFamily, "Dining Family" },
                { gbXMLBuildingType.Dormitory, "Dormitory" },
                { gbXMLBuildingType.ExerciseCenter, "Exercise Center" },
                { gbXMLBuildingType.FireStation, "Fire Station" },
                { gbXMLBuildingType.Gymnasium, "Gymnasium" },
                { gbXMLBuildingType.HospitalOrHealthcare, "Hospital or Healthcare" },
                { gbXMLBuildingType.Hotel, "Hotel" },
                { gbXMLBuildingType.Library, "Library" },
                { gbXMLBuildingType.Manufacturing, "Manufacturing" },
                { gbXMLBuildingType.Motel, "Motel" },
                { gbXMLBuildingType.MotionPictureTheatre, "Motion Picture Theatre" },
                { gbXMLBuildingType.MultiFamily, "Multi Family" },
                { gbXMLBuildingType.Museum, "Museum" },
                { gbXMLBuildingType.NoOfBuildingTypes, "None" },
                { gbXMLBuildingType.Office, "Office" },
                { gbXMLBuildingType.ParkingGarage, "Parking Garage" },
                { gbXMLBuildingType.Penitentiary, "Penitentiary" },
                { gbXMLBuildingType.PerformingArtsTheater, "Performing Arts Theater" },
                { gbXMLBuildingType.PoliceStation, "Police Station" },
                { gbXMLBuildingType.PostOffice, "Post Office" },
                { gbXMLBuildingType.ReligiousBuilding, "Religious Building" },
                { gbXMLBuildingType.Retail, "Retail" },
                { gbXMLBuildingType.SchoolOrUniversity, "School or University" },
                { gbXMLBuildingType.SingleFamily, "Single Family" },
                { gbXMLBuildingType.SportsArena, "Sports Arena" },
                { gbXMLBuildingType.TownHall, "Town Hall" },
                { gbXMLBuildingType.Transportation, "Transportation" },
                { gbXMLBuildingType.Warehouse, "Warehouse" },
                { gbXMLBuildingType.Workshop, "Workshop" }
            };

            ServiceTypeMap = new Dictionary<object, string>
            {
                { gbXMLServiceType.ActiveChilledBeams, "Active Chilled Beams" },
                { gbXMLServiceType.CentralHeatingConvectors, "Central Heating: Convectors" },
                { gbXMLServiceType.CentralHeatingHotAir, "Central Heating: Hot Air" },
                { gbXMLServiceType.CentralHeatingRadiantFloor, "Central Heating: Radiant Floor" },
                { gbXMLServiceType.CentralHeatingRadiators, "Central Heating: Radiators" },
                { gbXMLServiceType.ConstantVolumeDualDuct, "Constant Volume - Dual Duct" },
                { gbXMLServiceType.ConstantVolumeFixedOA, "Constant Volume - Fixed OA" },
                { gbXMLServiceType.ConstantVolumeTerminalReheat, "Constant Volume - Terminal Reheat" },
                { gbXMLServiceType.ConstantVolumeVariableOA, "Constant Volume - Variable OA" },
                { gbXMLServiceType.FanCoilSystem, "Fan Coil System" },
                { gbXMLServiceType.ForcedConvectionHeaterFlue, "Forced Convection Heater - Flue" },
                { gbXMLServiceType.ForcedConvectionHeaterNoFlue, "Forced Convection Heater - No Flue" },
                { gbXMLServiceType.InductionSystem, "Induction System" },
                { gbXMLServiceType.MultizoneHotDeckColdDeck, "Multi-zone - Hot Deck / Cold Deck" },
                { gbXMLServiceType.NoServiceType, "None" },
                { gbXMLServiceType.OtherRoomHeater, "Other Room Heater" },
                { gbXMLServiceType.RadiantCooledCeilings, "Radiant Cooled Ceilings" },
                { gbXMLServiceType.RadiantHeaterFlue, "Radiant Heater - Flue" },
                { gbXMLServiceType.RadiantHeaterMultiburner, "Radiant Heater - Multi-burner" },
                { gbXMLServiceType.RadiantHeaterNoFlue, "Radiant Heater - No Flue" },
                { gbXMLServiceType.SplitSystemsWithMechanicalVentilation, "Split System(s) with Mechanical Ventilation" },
                { gbXMLServiceType.SplitSystemsWithMechanicalVentilationWithCooling, "Split System(s) with Mechanical Ventilation with Cooling" },
                { gbXMLServiceType.SplitSystemsWithNaturalVentilation, "Split System(s) with Natural Ventilation" },
                { gbXMLServiceType.VariableRefrigerantFlow, "Variable Refrigerant Flow" },
                { gbXMLServiceType.VAVDualDuct, "VAV - Dual Duct" },
                { gbXMLServiceType.VAVIndoorPackagedCabinet, "VAV - Indoor Packaged Cabinet" },
                { gbXMLServiceType.VAVSingleDuct, "VAV - Single Duct" },
                { gbXMLServiceType.VAVTerminalReheat, "VAV - Terminal Reheat" },
                { gbXMLServiceType.WaterLoopHeatPump, "Water Loop Heat Pump" }
            };

            ExportComplexityMap = new Dictionary<object, string>
            {
                { gbXMLExportComplexity.Complex, "Complex" },
                {
                    gbXMLExportComplexity.ComplexWithMullionsAndShadingSurfaces,
                    "Complex With Mullions And Shading Surfaces"
                },
                { gbXMLExportComplexity.ComplexWithShadingSurfaces, "Complex With Shading Surfaces" },
                { gbXMLExportComplexity.Simple, "Simple" },
                { gbXMLExportComplexity.SimpleWithShadingSurfaces, "Simple With Shading Surfaces" }
            };

            HvacLoadLoadsReportTypeMap = new Dictionary<object, string>
            {
                { HVACLoadLoadsReportType.DetailedReport, "Detailed" },
                { HVACLoadLoadsReportType.NoReport, "No" },
                { HVACLoadLoadsReportType.SimpleReport, "Simple" },
                { HVACLoadLoadsReportType.StandardReport, "Standard" }
            };

            HvacLoadConstructionClassMap = new Dictionary<object, string>
            {
                { HVACLoadConstructionClass.LooseConstruction, "Loose" },
                { HVACLoadConstructionClass.NoneConstruction, "None" },
                { HVACLoadConstructionClass.MediumConstruction, "Medium" },
                { HVACLoadConstructionClass.TightConstruction, "Tight" }
            };
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
