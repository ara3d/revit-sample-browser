// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.UI;

namespace RevitMultiSample.FabricationPartLayout.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FabricationPartLayout : IExternalCommand
    {
        private Document Doc { get; set; }

        private IList<FabricationService> Services { get; set; }

        // kept in sync
        private IList<int> MaterialIds { get; set; }

        private IList<string> MaterialGroups { get; set; }

        private IList<string> MaterialNames { get; set; }

        // kept in sync
        private IList<int> SpecIds { get; set; }

        private IList<string> SpecGroups { get; set; }

        private IList<string> SpecNames { get; set; }

        // kept in sync
        private IList<int> InsSpecIds { get; set; }

        private IList<string> InsSpecGroups { get; set; }

        private IList<string> InsSpecNames { get; set; }

        // kept in sync
        private IList<int> ConnIds { get; set; }

        private IList<string> ConnGroups { get; set; }

        private IList<string> ConnNames { get; set; }

        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                Doc = commandData.Application.ActiveUIDocument.Document;

                var cl = new FilteredElementCollector(Doc);
                cl.OfClass(typeof(Level));
                var levels = cl.ToElements();
                Level levelOne = null;
                foreach (Level level in levels)
                    if (level != null && level.Name.Equals("Level 1"))
                    {
                        levelOne = level;
                        break;
                    }

                if (levelOne == null)
                    return Result.Failed;

                // locate the AHU in the model - should only be one instance in the model.
                var c2 = new FilteredElementCollector(Doc);
                c2.OfClass(typeof(FamilyInstance));
                var families = c2.ToElements();
                if (families.Count != 1)
                    return Result.Failed;

                if (!(families[0] is FamilyInstance famAhu))
                    return Result.Failed;

                // locate the proper connector - rectangular 40"x40" outlet
                Connector connAhu = null;
                var connsAhu = famAhu.MEPModel.ConnectorManager.UnusedConnectors;
                var lengthInFeet = 40.0 / 12.0;
                foreach (Connector conn in connsAhu)
                    // Revit units measured in feet, so dividing the width and height by 12
                    if (conn.Shape == ConnectorProfileType.Rectangular && conn.Width == lengthInFeet &&
                        conn.Height == lengthInFeet)
                        connAhu = conn;

                if (connAhu == null)
                    return Result.Failed;

                // get the current fabrication configuration
                var config = FabricationConfiguration.GetFabricationConfiguration(Doc);
                if (config == null)
                    return Result.Failed;

                // create materials look-up tables
                GetMaterials(config);

                // create specs look-up tables
                GetSpecs(config);

                // create insulation specs look-up tables
                GetInsulationSpecs(config);

                // create fabrication configuration look-up tables
                GetFabricationConnectors(config);

                // get all the loaded services
                Services = config.GetAllLoadedServices();
                if (Services.Count == 0)
                    return Result.Failed;

                var havcService = Services.FirstOrDefault(x => x.Name.Contains("HVAC"));
                var pipeService = Services.FirstOrDefault(x => x.Name.Contains("Plumbing"));

                var btTransition = LocateButton(havcService, 0, "Transition");
                var btSqBend = LocateButton(havcService, 0, "Square Bend");
                var btTap = LocateButton(havcService, 0, "Tap");
                var btRectStraight = LocateButton(havcService, 0, "Straight");
                var btRadBend = LocateButton(havcService, 0, "Radius Bend");
                var btFlatShoe = LocateButton(havcService, 1, "Flat Shoe");
                var btTube = LocateButton(havcService, 1, "Tube");
                var bt90Bend = LocateButton(havcService, 1, "Bend - 90");
                var bt45Bend = LocateButton(havcService, 1, "Bend - 45");
                var btRectTee = LocateButton(havcService, 0, "Tee");
                var btSqToRound = LocateButton(havcService, 0, "Square to Round");
                var btReducer = LocateButton(havcService, 1, "Reducer - C");
                var btCurvedBoot = LocateButton(havcService, 1, "Curved Boot");
                var btHangerBearer = LocateButton(havcService, 4, "Rectangular Bearer");
                var btHangerRound = LocateButton(havcService, 4, "Round Duct Hanger");

                var btValve = LocateButton(pipeService, 2, "Globe Valve");
                var btGroovedPipe = LocateButton(pipeService, 1, "Type L Hard Copper");
                var bt90Elbow = LocateButton(pipeService, 1, "No610 - 90 Elbow");

                using (var tr = new Transaction(Doc, "Create Layout"))
                {
                    tr.Start();

                    // connect a square bend to the ahu
                    var ptSqBend1 = FabricationPart.Create(Doc, btSqBend, 0, levelOne.Id);
                    var conn1SqBend1 = GetPrimaryConnector(ptSqBend1.ConnectorManager);
                    var conn2SqBend1 = GetSecondaryConnector(ptSqBend1.ConnectorManager);
                    SizeAlignCoupleConnect(conn1SqBend1, connAhu, 3.0 * Math.PI / 2.0);

                    // add a 15' straight to the square bend
                    var ptRectStraight1 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 15.0);
                    var conn1Straight1 = GetPrimaryConnector(ptRectStraight1.ConnectorManager);
                    var conn2Straight1 = GetSecondaryConnector(ptRectStraight1.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Straight1, conn2SqBend1, 0);

                    // add two Rectangular Bearer hangers at 5' to each end of the 15' straight
                    FabricationPart.CreateHanger(Doc, btHangerBearer, ptRectStraight1.Id, conn1Straight1, 5.0,
                        true);
                    FabricationPart.CreateHanger(Doc, btHangerBearer, ptRectStraight1.Id, conn2Straight1, 5.0,
                        true);

                    // connect a tap to the straight half way along
                    var ptTap1 = FabricationPart.Create(Doc, btTap, 0, levelOne.Id);
                    var conn1Tap1 = GetPrimaryConnector(ptTap1.ConnectorManager);
                    var conn2Tap1 = GetSecondaryConnector(ptTap1.ConnectorManager);
                    FabricationPart.PlaceAsTap(Doc, conn1Tap1, conn1Straight1, 7.5, 3.0 * Math.PI / 2.0, 0);

                    // connect a square to round to the tap, with an outlet of 10"
                    var ptSqToRound1 = FabricationPart.Create(Doc, btSqToRound, 0, levelOne.Id);
                    var conn1SqToRound1 = GetPrimaryConnector(ptSqToRound1.ConnectorManager);
                    var conn2SqToRound1 = GetSecondaryConnector(ptSqToRound1.ConnectorManager);
                    conn2SqToRound1.Radius = 5.0 / 12.0; // convert to feet
                    SizeAlignCoupleConnect(conn1SqToRound1, conn2Tap1, 0);

                    // connect a bend 90, based on the condition (converting 10" into feet)
                    var pt90Bend1 = FabricationPart.Create(Doc, bt90Bend, 10.0 / 12.0, 10.0 / 12.0, levelOne.Id);
                    var conn190Bend1 = GetPrimaryConnector(pt90Bend1.ConnectorManager);
                    var conn290Bend1 = GetSecondaryConnector(pt90Bend1.ConnectorManager);
                    SizeAlignCoupleConnect(conn190Bend1, conn2SqToRound1, 0);

                    var pt90Bend2 = FabricationPart.Create(Doc, bt90Bend, 0, levelOne.Id);
                    var conn190Bend2 = GetPrimaryConnector(pt90Bend2.ConnectorManager);
                    var conn290Bend2 = GetSecondaryConnector(pt90Bend2.ConnectorManager);
                    SizeAlignCoupleConnect(conn190Bend2, conn290Bend1, 0);

                    // now let's add a tube in
                    var ptTube1 = CreateStraightPart(btTube, 0, levelOne.Id, 5.0);
                    var conn1Tube1 = GetPrimaryConnector(ptTube1.ConnectorManager);
                    var conn2Tube1 = GetSecondaryConnector(ptTube1.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube1, conn290Bend2, 0);

                    // and now add a square to round, connecting by round end
                    // change the spec to undefined, change material, add insulation, change a connector
                    var ptSqToRound2 = FabricationPart.Create(Doc, btSqToRound, 0, levelOne.Id);
                    var conn1SqToRound2 = GetPrimaryConnector(ptSqToRound2.ConnectorManager);
                    var conn2SqToRound2 = GetSecondaryConnector(ptSqToRound2.ConnectorManager); // round end
                    SizeAlignCoupleConnect(conn2SqToRound2, conn2Tube1, 0);

                    // set the spec to none           
                    ptSqToRound2.Specification = 0; // none         

                    // now locate specific material, insulation spec and fabrication connector
                    //int specId = config.LocateSpecification("Ductwork", "+6 WG");
                    var materialId = config.LocateMaterial("Ductwork", "Mild Steel");
                    var insSpecId = config.LocateInsulationSpecification("Ductwork", "Acoustic Liner 1''");
                    var connId = config.LocateFabricationConnector("Duct - S&D", "S&D", ConnectorDomainType.Undefined,
                        ConnectorProfileType.Rectangular);

                    // now set the material, insulation spec and one of the connectors
                    if (materialId >= 0)
                        ptSqToRound2.Material = materialId;
                    if (insSpecId >= 0)
                        ptSqToRound2.InsulationSpecification = insSpecId;
                    if (connId >= 0)
                        conn1SqToRound2.GetFabricationConnectorInfo().BodyConnectorId = connId;

                    // connect a 2' 6" transition to the square bend
                    var ptTransition1 = FabricationPart.Create(Doc, btTransition, 0, levelOne.Id);
                    SetDimValue(ptTransition1, "Length", 2.5); // set length of transition to 2' 6"
                    var conn1Transition1 = GetPrimaryConnector(ptTransition1.ConnectorManager);
                    var conn2Transition1 = GetSecondaryConnector(ptTransition1.ConnectorManager);
                    conn2Transition1.Width = 2.0;
                    conn2Transition1.Height = 2.0;
                    SizeAlignCoupleConnect(conn1Transition1, conn2Straight1, 0);

                    // connect a rising square bend to the transition
                    var ptSqBend2 = FabricationPart.Create(Doc, btSqBend, 0, levelOne.Id);
                    var conn1SqBend2 = GetPrimaryConnector(ptSqBend2.ConnectorManager);
                    var conn2SqBend2 = GetSecondaryConnector(ptSqBend2.ConnectorManager);
                    SizeAlignCoupleConnect(conn1SqBend2, conn2Transition1, 0);

                    // connect a 4' 5" straight to the square bend
                    var ptRectStraight2 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 4.0 + 5.0 / 12.0);
                    var conn1Straight2 = GetPrimaryConnector(ptRectStraight2.ConnectorManager);
                    var conn2Straight2 = GetSecondaryConnector(ptRectStraight2.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Straight2, conn2SqBend2, 0);

                    // connect a square bend to the straight
                    var ptSqBend3 = FabricationPart.Create(Doc, btSqBend, 0, levelOne.Id);
                    var conn1SqBend3 = GetPrimaryConnector(ptSqBend3.ConnectorManager);
                    var conn2SqBend3 = GetSecondaryConnector(ptSqBend3.ConnectorManager);
                    SizeAlignCoupleConnect(conn1SqBend3, conn2Straight2, Math.PI);

                    // add a 5' straight 
                    var ptRectStraight3 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 5.0);
                    var conn1Straight3 = GetPrimaryConnector(ptRectStraight3.ConnectorManager);
                    var conn2Straight3 = GetSecondaryConnector(ptRectStraight3.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Straight3, conn2SqBend3, 0);

                    //add a Bearer hanger in middle of straight3 
                    FabricationPart.CreateHanger(Doc, btHangerBearer, ptRectStraight3.Id, conn1Straight3, 2.5,
                        true);

                    // add a 45 degree radius bend 
                    var ptRadBend1 = FabricationPart.Create(Doc, btRadBend, 0, levelOne.Id);
                    var conn1RadBend1 = GetPrimaryConnector(ptRadBend1.ConnectorManager);
                    var conn2RadBend1 = GetSecondaryConnector(ptRadBend1.ConnectorManager);
                    SizeAlignCoupleConnect(conn1RadBend1, conn2Straight3, 3.0 * Math.PI / 2.0);
                    SetDimValue(ptRadBend1, "Angle", Math.PI / 4.0);

                    // add a 1' 8" straight 
                    var ptRectStraight4 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 1.0 + 8.0 / 12.0);
                    var conn1Straight4 = GetPrimaryConnector(ptRectStraight4.ConnectorManager);
                    var conn2Straight4 = GetSecondaryConnector(ptRectStraight4.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Straight4, conn2RadBend1, 0);

                    // add a 45 degree radius bend 
                    var ptRadBend2 = FabricationPart.Create(Doc, btRadBend, 0, levelOne.Id);
                    var conn1RadBend2 = GetPrimaryConnector(ptRadBend2.ConnectorManager);
                    var conn2RadBend2 = GetSecondaryConnector(ptRadBend2.ConnectorManager);
                    SizeAlignCoupleConnect(conn1RadBend2, conn2Straight4, Math.PI);
                    SetDimValue(ptRadBend2, "Angle", Math.PI / 4.0);

                    // add a 5' straight 
                    var ptRectStraight5 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 5.0);
                    var conn1Straight5 = GetPrimaryConnector(ptRectStraight5.ConnectorManager);
                    var conn2Straight5 = GetSecondaryConnector(ptRectStraight5.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Straight5, conn2RadBend2, 0);

                    //add a Bearer hanger in middle of straight5 
                    FabricationPart.CreateHanger(Doc, btHangerBearer, ptRectStraight5.Id, conn1Straight5, 2.5,
                        true);

                    // add a 2' 6" straight 
                    var ptRectStraight6 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 2.5);
                    var conn1Straight6 = GetPrimaryConnector(ptRectStraight6.ConnectorManager);
                    var conn2Straight6 = GetSecondaryConnector(ptRectStraight6.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Straight6, conn2Straight5, 0);

                    // add an 8" tap to the last straight - half way along the straight - using parameter to set the product entry
                    // could also set the radius directly.
                    var ptFlatShoe1 = FabricationPart.Create(Doc, btFlatShoe, 0, levelOne.Id);
                    var prodEntryFlatShoe1 = ptFlatShoe1.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY);
                    prodEntryFlatShoe1.Set("8''");
                    var conn1FlatShoe1 = GetPrimaryConnector(ptFlatShoe1.ConnectorManager);
                    var conn2FlatShoe1 = GetSecondaryConnector(ptFlatShoe1.ConnectorManager);
                    FabricationPart.PlaceAsTap(Doc, conn1FlatShoe1, conn1Straight6, 1.25, Math.PI, 0);

                    // add a 16' 8 long tube
                    var lengthTube2 = 16.0 + 8.0 / 12.0;
                    var ptTube2 = CreateStraightPart(btTube, 0, levelOne.Id, lengthTube2);
                    var conn1Tube2 = GetPrimaryConnector(ptTube2.ConnectorManager);
                    var conn2Tube2 = GetSecondaryConnector(ptTube2.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube2, conn2FlatShoe1, 0);
                    //add 3 hangers for tube2 , with specified button condition
                    for (var i = 0; i < 3; i++)
                        FabricationPart.CreateHanger(Doc, btHangerRound, 1, ptTube2.Id, conn1Tube2,
                            (i + 1) * lengthTube2 / 4, true);

                    // add a 90 degree bend
                    var pt90Bend3 = FabricationPart.Create(Doc, bt90Bend, 0, levelOne.Id);
                    var conn190Bend3 = GetPrimaryConnector(pt90Bend3.ConnectorManager);
                    var conn290Bend3 = GetSecondaryConnector(pt90Bend3.ConnectorManager);
                    SizeAlignCoupleConnect(conn190Bend3, conn2Tube2, Math.PI);

                    // add a 10' long tube
                    var ptTube3 = CreateStraightPart(btTube, 0, levelOne.Id, 10.0);
                    var conn1Tube3 = GetPrimaryConnector(ptTube3.ConnectorManager);
                    var conn2Tube3 = GetSecondaryConnector(ptTube3.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube3, conn290Bend3, 0);

                    //add one hangers in middle of tube3, by default button condition 
                    FabricationPart.CreateHanger(Doc, btHangerRound, ptTube3.Id, conn1Tube3, 5.0, true);

                    // add a 45 degree bend 
                    var pt45Bend1 = FabricationPart.Create(Doc, bt45Bend, 0, levelOne.Id);
                    var conn145Bend1 = GetPrimaryConnector(pt45Bend1.ConnectorManager);
                    var conn245Bend1 = GetSecondaryConnector(pt45Bend1.ConnectorManager);
                    SizeAlignCoupleConnect(conn145Bend1, conn2Tube3, Math.PI);

                    // add a 2' long tube
                    var ptTube4 = CreateStraightPart(btTube, 0, levelOne.Id, 2.0);
                    var conn1Tube4 = GetPrimaryConnector(ptTube4.ConnectorManager);
                    var conn2Tube4 = GetSecondaryConnector(ptTube4.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube4, conn245Bend1, 0);

                    // add a 45 degree bend 
                    var pt45Bend2 = FabricationPart.Create(Doc, bt45Bend, 0, levelOne.Id);
                    var conn145Bend2 = GetPrimaryConnector(pt45Bend2.ConnectorManager);
                    var conn245Bend2 = GetSecondaryConnector(pt45Bend2.ConnectorManager);
                    SizeAlignCoupleConnect(conn145Bend2, conn2Tube4, Math.PI);

                    // add a 10' long tube
                    var ptTube5 = CreateStraightPart(btTube, 0, levelOne.Id, 10.0);
                    var conn1Tube5 = GetPrimaryConnector(ptTube5.ConnectorManager);
                    GetSecondaryConnector(ptTube5.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube5, conn245Bend2, 0);

                    //add one hangers in middle of tube5, by default button condition 
                    FabricationPart.CreateHanger(Doc, btHangerRound, ptTube5.Id, conn1Tube5, 5.0, true);

                    // now go back to the straight (pt_rectStraight5) with the tap and add a square bend
                    var ptSqBend4 = FabricationPart.Create(Doc, btSqBend, 0, levelOne.Id);
                    var conn1SqBend4 = GetPrimaryConnector(ptSqBend4.ConnectorManager);
                    var conn2SqBend4 = GetSecondaryConnector(ptSqBend4.ConnectorManager);
                    SizeAlignCoupleConnect(conn1SqBend4, conn2Straight6, Math.PI);

                    // add a 5' straight
                    var ptRectStraight7 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 5.0);
                    var conn1Straight7 = GetPrimaryConnector(ptRectStraight7.ConnectorManager);
                    var conn2Straight7 = GetSecondaryConnector(ptRectStraight7.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Straight7, conn2SqBend4, 0);

                    //add a Bearer hanger in middle of straight7, by default condition
                    FabricationPart.CreateHanger(Doc, btHangerBearer, ptRectStraight7.Id, conn1Straight7, 2.5,
                        true);

                    // add a modified tee
                    var ptRectTee1 = FabricationPart.Create(Doc, btRectTee, 0, levelOne.Id);

                    // Set the size prior to connecting the part. Template parts with more than 2 connectors will disable editing of sizes once one of the connectors is connected to something.
                    SetDimValue(ptRectTee1, "Right Width",
                        16.0 / 12.0); // set right width dimension to 16" (converted to feet)
                    SetDimValue(ptRectTee1, "Btm Width",
                        20.0 / 12.0); // set bottom width dimension to 20" (converted to feet)

                    var conn1RectTee1 = GetPrimaryConnector(ptRectTee1.ConnectorManager);
                    var conn2RectTee1 = GetSecondaryConnector(ptRectTee1.ConnectorManager);
                    var conn3RectTee1 = GetFirstNonPrimaryOrSecondaryConnector(ptRectTee1.ConnectorManager);
                    SizeAlignCoupleConnect(conn3RectTee1, conn2Straight7, Math.PI);

                    // add a square to round to the tee (conn2)
                    var ptSqToRound3 = FabricationPart.Create(Doc, btSqToRound, 0, levelOne.Id);
                    var conn1SqToRound3 = GetPrimaryConnector(ptSqToRound3.ConnectorManager);
                    var conn2SqToRound3 = GetSecondaryConnector(ptSqToRound3.ConnectorManager);
                    SizeAlignCoupleConnect(conn1SqToRound3, conn2RectTee1, 0);
                    SetDimValue(ptSqToRound3, "Length",
                        1.0 + 8.5 / 12.0); // set length dimension to 1' 8 1/2" (converted to feet)
                    SetDimValue(ptSqToRound3, "Diameter", 1.0); // set diameter dimension to 1'

                    // add a 22' 4" long tube
                    var lengthTube6 = 22.0 + 4.0 / 12.0;
                    var ptTube6 = CreateStraightPart(btTube, 0, levelOne.Id, lengthTube6);
                    var conn1Tube6 = GetPrimaryConnector(ptTube6.ConnectorManager);
                    var conn2Tube6 = GetSecondaryConnector(ptTube6.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube6, conn2SqToRound3, 0);

                    //add 3 hangers for tube6, by default condition
                    for (var i = 0; i < 3; i++)
                        FabricationPart.CreateHanger(Doc, btHangerRound, 1, ptTube6.Id, conn1Tube6,
                            (i + 1) * lengthTube6 / 4, true);

                    // add a reducer, reducing to 8"
                    var ptReducer1 = FabricationPart.Create(Doc, btReducer, 0, levelOne.Id);
                    var conn1Reducer1 = GetPrimaryConnector(ptReducer1.ConnectorManager);
                    var conn2Reducer1 = GetSecondaryConnector(ptReducer1.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Reducer1, conn2Tube6, 0);
                    var prodEntryReducer1 = ptReducer1.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY);
                    prodEntryReducer1.Set("12''x8''");

                    // add a 10' long tube
                    var ptTube7 = CreateStraightPart(btTube, 0, levelOne.Id, 10.0);
                    var conn1Tube7 = GetPrimaryConnector(ptTube7.ConnectorManager);
                    GetSecondaryConnector(ptTube7.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube7, conn2Reducer1, 0);

                    //add one hangers in middle of tube7, by default button condition 
                    FabricationPart.CreateHanger(Doc, btHangerRound, ptTube7.Id, conn1Tube7, 5.0, true);

                    // add a curved boot tap to the 22' 4" tube (pt_tube6) 1' 2" from the end, reducing to 8"
                    var ptCurvedBoot1 = FabricationPart.Create(Doc, btCurvedBoot, 0, levelOne.Id);
                    var prodEntryCurvedBoot1 =
                        ptCurvedBoot1.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY);
                    prodEntryCurvedBoot1.Set("12''x8''");
                    var conn1CurvedBoot1 = GetPrimaryConnector(ptCurvedBoot1.ConnectorManager);
                    var conn2CurvedBoot1 = GetSecondaryConnector(ptCurvedBoot1.ConnectorManager);
                    FabricationPart.PlaceAsTap(Doc, conn1CurvedBoot1, conn2Tube6, 1.0 + 2.0 / 12.0, Math.PI,
                        Math.PI);

                    // add a 16' 8" long tube to the curved boot
                    var lengthTube8 = 16.0 + 8.0 / 12.0;
                    var ptTube8 = CreateStraightPart(btTube, 0, levelOne.Id, lengthTube8);
                    var conn1Tube8 = GetPrimaryConnector(ptTube8.ConnectorManager);
                    GetSecondaryConnector(ptTube8.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube8, conn2CurvedBoot1, 0);
                    //add 3 hangers for tube8, with specified condition 
                    for (var i = 0; i < 3; i++)
                        FabricationPart.CreateHanger(Doc, btHangerRound, 0, ptTube8.Id, conn1Tube8,
                            (i + 1) * lengthTube8 / 4, true);

                    // going back to the modified tee
                    // add a square to round to the tee (conn2)
                    var ptSqToRound4 = FabricationPart.Create(Doc, btSqToRound, 0, levelOne.Id);
                    var conn1SqToRound4 = GetPrimaryConnector(ptSqToRound4.ConnectorManager);
                    var conn2SqToRound4 = GetSecondaryConnector(ptSqToRound4.ConnectorManager);
                    SizeAlignCoupleConnect(conn1SqToRound4, conn1RectTee1, 0);
                    SetDimValue(ptSqToRound4, "Length",
                        1.0 + 8.5 / 12.0); // set length dimension to 1' 8 1/2" (converted to feet)
                    SetDimValue(ptSqToRound4, "Diameter", 1.0); // set diameter dimension to 1'

                    // add a 10' long tube
                    var ptTube9 = CreateStraightPart(btTube, 0, levelOne.Id, 10.0);
                    var conn1Tube9 = GetPrimaryConnector(ptTube9.ConnectorManager);
                    var conn2Tube9 = GetSecondaryConnector(ptTube9.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube9, conn2SqToRound4, 0);

                    //add one hangers in middle of tube9, by default button condition 
                    FabricationPart.CreateHanger(Doc, btHangerRound, ptTube9.Id, conn1Tube9, 5.0, true);

                    // add a curved boot to the tube 3/4 way along, reducing to 10"
                    var ptCurvedBoot2 = FabricationPart.Create(Doc, btCurvedBoot, 0, levelOne.Id);
                    var prodEntryCurvedBoot2 =
                        ptCurvedBoot2.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY);
                    prodEntryCurvedBoot2.Set("12''x8''");
                    var conn1CurvedBoot2 = GetPrimaryConnector(ptCurvedBoot2.ConnectorManager);
                    var conn2CurvedBoot2 = GetSecondaryConnector(ptCurvedBoot2.ConnectorManager);
                    FabricationPart.PlaceAsTap(Doc, conn1CurvedBoot2, conn1Tube9, 7.5, Math.PI, 0);

                    // add 8' 1" long tube to the curved boot
                    var lengthTube10 = 8.0 + 1.0 / 12.0;
                    var ptTube10 = CreateStraightPart(btTube, 0, levelOne.Id, lengthTube10);
                    var conn1Tube10 = GetPrimaryConnector(ptTube10.ConnectorManager);
                    var conn2Tube10 = GetSecondaryConnector(ptTube10.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube10, conn2CurvedBoot2, 0);

                    //add one hangers in middle of tube10, by default button condition 
                    FabricationPart.CreateHanger(Doc, btHangerRound, ptTube10.Id, conn1Tube10, lengthTube10 / 2,
                        true);

                    // add a 45 degree bend 
                    var pt45Bend3 = FabricationPart.Create(Doc, bt45Bend, 0, levelOne.Id);
                    var conn145Bend3 = GetPrimaryConnector(pt45Bend3.ConnectorManager);
                    var conn245Bend3 = GetSecondaryConnector(pt45Bend3.ConnectorManager);
                    SizeAlignCoupleConnect(conn145Bend3, conn2Tube10, 0);

                    // add 20' long tube                
                    var ptTube11 = CreateStraightPart(btTube, 0, levelOne.Id, 20.0);
                    var conn1Tube11 = GetPrimaryConnector(ptTube11.ConnectorManager);
                    var conn2Tube11 = GetSecondaryConnector(ptTube11.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube11, conn245Bend3, 0);

                    // add a 45 degree bend 
                    var pt45Bend4 = FabricationPart.Create(Doc, bt45Bend, 0, levelOne.Id);
                    var conn145Bend4 = GetPrimaryConnector(pt45Bend4.ConnectorManager);
                    var conn245Bend4 = GetSecondaryConnector(pt45Bend4.ConnectorManager);
                    SizeAlignCoupleConnect(conn145Bend4, conn2Tube11, 0);

                    // add 1' 8" long tube 
                    var ptTube12 = CreateStraightPart(btTube, 0, levelOne.Id, 1.0 + 8.0 / 12.0);
                    var conn1Tube12 = GetPrimaryConnector(ptTube12.ConnectorManager);
                    GetSecondaryConnector(ptTube12.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube12, conn245Bend4, 0);

                    // add a reducer (to 10") on the tube with the curved boot (pt_tube12)
                    var ptReducer2 = FabricationPart.Create(Doc, btReducer, 0, levelOne.Id);
                    var conn1Reducer2 = GetPrimaryConnector(ptReducer2.ConnectorManager);
                    var conn2Reducer2 = GetSecondaryConnector(ptReducer2.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Reducer2, conn2Tube9, 0);
                    var prodEntryReducer2 = ptReducer2.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY);
                    prodEntryReducer2.Set("12''x10''");

                    // add a 10' long tube
                    var ptTube13 = CreateStraightPart(btTube, 0, levelOne.Id, 10.0);
                    var conn1Tube13 = GetPrimaryConnector(ptTube13.ConnectorManager);
                    var conn2Tube13 = GetSecondaryConnector(ptTube13.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube13, conn2Reducer2, 0);

                    //add one hangers for tube13, by default button condition 
                    FabricationPart.CreateHanger(Doc, btHangerRound, ptTube13.Id, conn1Tube13, 5.0, true);

                    // add a 90 bend, going 45 degrees down
                    var pt90Bend4 = FabricationPart.Create(Doc, bt90Bend, 0, levelOne.Id);
                    var conn190Bend4 = GetPrimaryConnector(pt90Bend4.ConnectorManager);
                    var conn290Bend4 = GetSecondaryConnector(pt90Bend4.ConnectorManager);
                    SizeAlignCoupleConnect(conn190Bend4, conn2Tube13, 3.0 * Math.PI / 4.0);

                    // add a 1' 2.5" long tube
                    var ptTube14 = CreateStraightPart(btTube, 0, levelOne.Id, 1.0 + 2.5 / 12.0);
                    var conn1Tube14 = GetPrimaryConnector(ptTube14.ConnectorManager);
                    var conn2Tube14 = GetSecondaryConnector(ptTube14.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube14, conn290Bend4, 0);

                    // add a 45 bend
                    var pt45Bend5 = FabricationPart.Create(Doc, bt45Bend, 0, levelOne.Id);
                    var conn145Bend5 = GetPrimaryConnector(pt45Bend5.ConnectorManager);
                    var conn245Bend5 = GetSecondaryConnector(pt45Bend5.ConnectorManager);
                    SizeAlignCoupleConnect(conn145Bend5, conn2Tube14, Math.PI / 2.0);

                    // add a 20' long tube
                    var ptTube15 = CreateStraightPart(btTube, 0, levelOne.Id, 20.0);
                    var conn1Tube15 = GetPrimaryConnector(ptTube15.ConnectorManager);
                    GetSecondaryConnector(ptTube15.ConnectorManager);
                    SizeAlignCoupleConnect(conn1Tube15, conn245Bend5, 0);
                    //add 4 hangers for tube15, by default button condition 
                    for (var i = 0; i < 4; i++)
                        FabricationPart.CreateHanger(Doc, btHangerRound, ptTube15.Id, conn1Tube15,
                            (i + 1) * (20.0 / 5), true);

                    // now let's place a 6" valve by its insertion point in free space
                    var ptValve1 = FabricationPart.Create(Doc, btValve, 1, levelOne.Id);
                    var prodEntryPtValve1 = ptValve1.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY);
                    prodEntryPtValve1.Set("6''");
                    FabricationPart.AlignPartByInsertionPoint(Doc, ptValve1.Id, new XYZ(16, -10, 0), 0, 0, 0,
                        FabricationPartJustification.Middle, null);
                    Doc.Regenerate();
                    var conn2Valve1 = GetSecondaryConnector(ptValve1.ConnectorManager);

                    // add 10' copper pipe to the valve
                    var ptPipe1 = CreateStraightPart(btGroovedPipe, 0, levelOne.Id, 10.0);
                    var conn1Pipe1 = GetPrimaryConnector(ptPipe1.ConnectorManager);
                    var conn2Pipe1 = GetSecondaryConnector(ptPipe1.ConnectorManager);
                    SizeAlignSlopeJustifyCoupleConnect(conn1Pipe1, conn2Valve1, 0, 0,
                        FabricationPartJustification.Middle);

                    // insert a valve into the middle of the copper pipe - it will size automatically
                    var pipe1Pos = (conn1Pipe1.Origin + conn2Pipe1.Origin) / 2.0;
                    var ptValve2 = FabricationPart.Create(Doc, btValve, 1, levelOne.Id);
                    FabricationPart.AlignPartByInsertionPointAndCutInToStraight(Doc, ptPipe1.Id, ptValve2.Id,
                        pipe1Pos, Math.PI / 2.0, 0, false);
                    Doc.Regenerate();

                    // add a 90 elbow and slope it
                    var pt90Elbow1 = FabricationPart.Create(Doc, bt90Elbow, 0, levelOne.Id);
                    var conn190Elbow1 = GetPrimaryConnector(pt90Elbow1.ConnectorManager);
                    var conn290Elbow1 = GetSecondaryConnector(pt90Elbow1.ConnectorManager);
                    SizeAlignSlopeJustifyCoupleConnect(conn190Elbow1, conn2Pipe1, Math.PI, 0.02,
                        FabricationPartJustification.Middle);

                    // add a copper pipe
                    var ptPipe2 = CreateStraightPart(btGroovedPipe, 0, levelOne.Id, 10.0);
                    var conn1Pipe2 = GetPrimaryConnector(ptPipe2.ConnectorManager);
                    GetSecondaryConnector(ptPipe2.ConnectorManager);
                    SizeAlignSlopeJustifyCoupleConnect(conn1Pipe2, conn290Elbow1, 0, 0,
                        FabricationPartJustification.Middle);

                    tr.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     Convenience method to get fabrication part's dimension value, specified by the dimension name.
        /// </summary>
        /// <param name="part">
        ///     The fabrication part to be queried.
        /// </param>
        /// <param name="dimName">
        ///     The name of the fabrication dimension.
        /// </param>
        /// <returns>
        ///     Returns the fabrication dimension value for the fabrication part, as specified by the dimension name.
        /// </returns>
        private double GetDimValue(FabricationPart part, string dimName)
        {
            double value = 0;
            if (part != null)
            {
                var dims = part.GetDimensions();
                foreach (var def in dims)
                    if (def.Name.Equals(dimName))
                    {
                        value = part.GetDimensionValue(def);
                        break;
                    }
            }

            return value;
        }

        /// <summary>
        ///     Convenience method to set fabrication part's dimension value, specified by the dimension name.
        /// </summary>
        /// <param name="part">
        ///     The fabrication part.
        /// </param>
        /// <param name="dimName">
        ///     The name of the fabrication dimension.
        /// </param>
        /// <param name="dimValue">
        ///     The value of the fabrication dimension to set to.
        /// </param>
        /// <returns>
        ///     Returns the fabrication dimension value for the fabrication part, as specified by the dimension name.
        /// </returns>
        private bool SetDimValue(FabricationPart part, string dimName, double dimValue)
        {
            var dims = part.GetDimensions();
            FabricationDimensionDefinition dim = null;
            foreach (var def in dims)
                if (def.Name.Equals(dimName))
                {
                    dim = def;
                    break;
                }

            if (dim == null)
                return false;

            part.SetDimensionValue(dim, dimValue);
            Doc.Regenerate();
            return true;
        }

        /// <summary>
        ///     Convenience method to automatically size, align, couple (if needed) and connect two fabrication part
        ///     by the specified connectors.
        /// </summary>
        /// <param name="conn_from">
        ///     The connector to align by of the fabrication part to move.
        /// </param>
        /// <param name="conn_to">
        ///     The connector to align to.
        /// </param>
        /// <param name="rotation">
        ///     Rotation around the direction of connection - angle between width vectors in radians.
        /// </param>
        /// <returns>
        ///     Returns the fabrication dimension value for the fabrication part, as specified by the dimension name.
        /// </returns>
        private void SizeAlignCoupleConnect(Connector connFrom, Connector connTo, double rotation)
        {
            if (connFrom.Shape == ConnectorProfileType.Rectangular || connFrom.Shape == ConnectorProfileType.Oval)
            {
                connFrom.Height = connTo.Height;
                connFrom.Width = connTo.Width;
            }
            else
            {
                connFrom.Radius = connTo.Radius;
            }

            Doc.Regenerate();

            FabricationPart.AlignPartByConnectors(Doc, connFrom, connTo, rotation);
            Doc.Regenerate();

            FabricationPart.ConnectAndCouple(Doc, connFrom, connTo);
            Doc.Regenerate();
        }

        /// <summary>
        ///     Convenience method to automatically size, align, slope, justification couple (if needed) and connect two
        ///     fabrication parts
        ///     by the specified connectors.
        /// </summary>
        /// <param name="conn_from">
        ///     The connector to align by of the fabrication part to move.
        /// </param>
        /// <param name="conn_to">
        ///     The connector to align to.
        /// </param>
        /// <param name="rotation">
        ///     Rotation around the direction of connection - angle between width vectors in radians.
        /// </param>
        /// <param name="slope">
        ///     The slope value to flex to match if possible in fractional units (eg.1/50). Positive values are up, negative are
        ///     down. Slopes can only be applied
        ///     to fittings, whilst straights will inherit the slope from the piece it is connecting to.
        /// </param>
        /// <param name="justification">
        ///     The justification to align eccentric parts.
        /// </param>
        /// <returns>
        ///     Returns the fabrication dimension value for the fabrication part, as specified by the dimension name.
        /// </returns>
        private void SizeAlignSlopeJustifyCoupleConnect(Connector connFrom, Connector connTo, double rotation,
            double slope, FabricationPartJustification justification)
        {
            if (connFrom.Shape == ConnectorProfileType.Rectangular || connFrom.Shape == ConnectorProfileType.Oval)
            {
                connFrom.Height = connTo.Height;
                connFrom.Width = connTo.Width;
            }
            else
            {
                connFrom.Radius = connTo.Radius;
            }

            Doc.Regenerate();

            FabricationPart.AlignPartByConnectorToConnector(Doc, connFrom, connTo, rotation, slope, justification);
            Doc.Regenerate();

            FabricationPart.ConnectAndCouple(Doc, connFrom, connTo);
            Doc.Regenerate();
        }

        /// <summary>
        ///     Convenience method to locate a fabrication service button specified by palette and name.
        /// </summary>
        /// <param name="service">
        ///     The fabrication service.
        /// </param>
        /// <param name="palette">
        ///     The fabrication service palette index.
        /// </param>
        /// <param name="name">
        ///     The fabrication service button name.
        /// </param>
        /// <returns>
        ///     Returns the fabrication service button as specified by the fabrication service, palette and name.
        /// </returns>
        private FabricationServiceButton LocateButton(FabricationService service, int palette, string name)
        {
            FabricationServiceButton button = null;
            if (service != null && palette >= 0 && palette < service.PaletteCount)
            {
                var buttonCount = service.GetButtonCount(palette);
                for (var i = 0; button == null && i < buttonCount; i++)
                {
                    var bt = service.GetButton(palette, i);
                    if (bt != null && bt.Name.Equals(name))
                        button = bt;
                }
            }

            return button;
        }

        /// <summary>
        ///     Convenience method to create a straight fabrication part.
        /// </summary>
        /// <param name="fsb">
        ///     The FabricationServiceButton used to create the fabrication part from.
        /// </param>
        /// <param name="condition">
        ///     The condition index of the fabrication service button.
        /// </param>
        /// <param name="levelId">
        ///     The element identifier belonging to the level on which to create this fabrication part.
        /// </param>
        /// <param name="length">
        ///     The length, in feet, of the fabrication part to be created.
        /// </param>
        /// <returns>
        ///     Returns a straight fabrication part, as specified by the fabrication service button, condition, level id and
        ///     length.
        /// </returns>
        private FabricationPart CreateStraightPart(FabricationServiceButton fsb, int condition, ElementId levelId,
            double length)
        {
            var straight = FabricationPart.Create(Doc, fsb, condition, levelId);

            var lengthOption = straight.LookupParameter("Length Option");
            lengthOption.Set("Value");

            var lengthParam = straight.LookupParameter("Length");
            lengthParam.Set(length);

            Doc.Regenerate();

            return straight;
        }

        /// <summary>
        ///     Convenience method to get the primary connector from the specified connector manager.
        /// </summary>
        /// <param name="cm">
        ///     The connector manager.
        /// </param>
        /// <returns>
        ///     Returns the primary connector from the connector manager.
        /// </returns>
        private Connector GetPrimaryConnector(ConnectorManager cm)
        {
            foreach (Connector cn in cm.Connectors)
            {
                var info = cn.GetMEPConnectorInfo();
                if (info.IsPrimary)
                    return cn;
            }

            return null;
        }

        /// <summary>
        ///     Convenience method to get the secondary connector from the specified connector manager.
        /// </summary>
        /// <param name="cm">
        ///     The connector manager.
        /// </param>
        /// <returns>
        ///     Returns the secondary connector from the connector manager.
        /// </returns>
        private Connector GetSecondaryConnector(ConnectorManager cm)
        {
            foreach (Connector cn in cm.Connectors)
            {
                var info = cn.GetMEPConnectorInfo();
                if (info.IsSecondary)
                    return cn;
            }

            return null;
        }

        /// <summary>
        ///     Convenience method to get the first non-primary and non-secondary connector from the specified connector manager.
        /// </summary>
        /// <param name="cm">
        ///     The connector manager.
        /// </param>
        /// <returns>
        ///     Returns the first non-primary and non-secondary connector from the connector manager.
        /// </returns>
        private Connector GetFirstNonPrimaryOrSecondaryConnector(ConnectorManager cm)
        {
            foreach (Connector cn in cm.Connectors)
            {
                var info = cn.GetMEPConnectorInfo();
                if (!info.IsPrimary && !info.IsSecondary)
                    return cn;
            }

            return null;
        }

        /// <summary>
        ///     Convenience method to get all fabrication material identifiers from the
        ///     specified fabrication configuration.
        /// </summary>
        /// <param name="config">
        ///     The fabrication configuration.
        /// </param>
        /// <returns>
        ///     Returns a list of all the fabrication material identifiers for this
        ///     fabrication configuration.
        /// </returns>
        private void GetMaterials(FabricationConfiguration config)
        {
            MaterialIds = config.GetAllMaterials(null);

            MaterialGroups = new List<string>();
            MaterialNames = new List<string>();

            foreach (var materialId in MaterialIds)
            {
                MaterialGroups.Add(config.GetMaterialGroup(materialId));
                MaterialNames.Add(config.GetMaterialName(materialId));
            }
        }

        /// <summary>
        ///     Convenience method to get all fabrication specification identifiers from the
        ///     specified fabrication configuration.
        /// </summary>
        /// <param name="config">
        ///     The fabrication configuration.
        /// </param>
        /// <returns>
        ///     Returns a list of all the fabrication specification identifiers for this
        ///     fabrication configuration.
        /// </returns>
        private void GetSpecs(FabricationConfiguration config)
        {
            SpecIds = config.GetAllSpecifications(null);

            SpecGroups = new List<string>();
            SpecNames = new List<string>();

            foreach (var specId in SpecIds)
            {
                SpecGroups.Add(config.GetSpecificationGroup(specId));
                SpecNames.Add(config.GetSpecificationName(specId));
            }
        }

        /// <summary>
        ///     Convenience method to get all fabrication insulation specification identifiers from the
        ///     specified fabrication configuration.
        /// </summary>
        /// <param name="config">
        ///     The fabrication configuration.
        /// </param>
        /// <returns>
        ///     Returns a list of all the fabrication insulation specification identifiers for this
        ///     fabrication configuration.
        /// </returns>
        private void GetInsulationSpecs(FabricationConfiguration config)
        {
            InsSpecIds = config.GetAllInsulationSpecifications(null);

            InsSpecGroups = new List<string>();
            InsSpecNames = new List<string>();

            foreach (var specId in InsSpecIds)
            {
                InsSpecGroups.Add(config.GetInsulationSpecificationGroup(specId));
                InsSpecNames.Add(config.GetInsulationSpecificationName(specId));
            }
        }

        /// <summary>
        ///     Convenience method to get all fabrication connector identifiers from the
        ///     specified fabrication configuration.
        /// </summary>
        /// <param name="config">
        ///     The fabrication configuration.
        /// </param>
        /// <returns>
        ///     Returns a list of all the fabrication connector identifiers for this
        ///     fabrication configuration.
        /// </returns>
        private void GetFabricationConnectors(FabricationConfiguration config)
        {
            ConnIds = config.GetAllFabricationConnectorDefinitions(ConnectorDomainType.Undefined,
                ConnectorProfileType.Invalid);

            ConnGroups = new List<string>();
            ConnNames = new List<string>();

            foreach (var connId in ConnIds)
            {
                ConnGroups.Add(config.GetFabricationConnectorGroup(connId));
                ConnNames.Add(config.GetFabricationConnectorName(connId));
            }
        }
    }
}
