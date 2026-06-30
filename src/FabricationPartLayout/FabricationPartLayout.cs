// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Fabrication;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.FabricationPartLayout.CS
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

                FilteredElementCollector cl = new(Doc);
                cl.OfClass(typeof(Level));
                var levels = cl.ToElements();
                Level levelOne = null;
                foreach (Level level in levels)
                {
                    if (level != null && level.Name.Equals("Level 1"))
                    {
                        levelOne = level;
                        break;
                    }
                }

                if (levelOne == null)
                    return Result.Failed;

                // Sample model: exactly one AHU family instance.
                FilteredElementCollector c2 = new(Doc);
                c2.OfClass(typeof(FamilyInstance));
                var families = c2.ToElements();
                if (families.Count != 1)
                    return Result.Failed;

                if (families[0] is not FamilyInstance famAhu)
                    return Result.Failed;

                // 40"x40" rectangular outlet; connector dimensions are in feet.
                Connector connAhu = null;
                var connsAhu = famAhu.MEPModel.ConnectorManager.UnusedConnectors;
                var lengthInFeet = 40.0 / 12.0;
                foreach (Connector conn in connsAhu)
                {
                    if (conn.Shape == ConnectorProfileType.Rectangular && conn.Width == lengthInFeet &&
                        conn.Height == lengthInFeet)
                        connAhu = conn;
                }

                if (connAhu == null)
                    return Result.Failed;

                var config = FabricationConfiguration.GetFabricationConfiguration(Doc);
                if (config == null)
                    return Result.Failed;

                GetMaterials(config);

                GetSpecs(config);

                GetInsulationSpecs(config);

                GetFabricationConnectors(config);

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

                using Transaction tr = new(Doc, "Create Layout");
                tr.Start();

                var ptSqBend1 = FabricationPart.Create(Doc, btSqBend, 0, levelOne.Id);
                var conn1SqBend1 = GetPrimaryConnector(ptSqBend1.ConnectorManager);
                var conn2SqBend1 = GetSecondaryConnector(ptSqBend1.ConnectorManager);
                SizeAlignCoupleConnect(conn1SqBend1, connAhu, 3.0 * Math.PI / 2.0);

                var ptRectStraight1 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 15.0);
                var conn1Straight1 = GetPrimaryConnector(ptRectStraight1.ConnectorManager);
                var conn2Straight1 = GetSecondaryConnector(ptRectStraight1.ConnectorManager);
                SizeAlignCoupleConnect(conn1Straight1, conn2SqBend1, 0);

                FabricationPart.CreateHanger(Doc, btHangerBearer, ptRectStraight1.Id, conn1Straight1, 5.0,
                    true);
                FabricationPart.CreateHanger(Doc, btHangerBearer, ptRectStraight1.Id, conn2Straight1, 5.0,
                    true);

                var ptTap1 = FabricationPart.Create(Doc, btTap, 0, levelOne.Id);
                var conn1Tap1 = GetPrimaryConnector(ptTap1.ConnectorManager);
                var conn2Tap1 = GetSecondaryConnector(ptTap1.ConnectorManager);
                FabricationPart.PlaceAsTap(Doc, conn1Tap1, conn1Straight1, 7.5, 3.0 * Math.PI / 2.0, 0);

                var ptSqToRound1 = FabricationPart.Create(Doc, btSqToRound, 0, levelOne.Id);
                var conn1SqToRound1 = GetPrimaryConnector(ptSqToRound1.ConnectorManager);
                var conn2SqToRound1 = GetSecondaryConnector(ptSqToRound1.ConnectorManager);
                conn2SqToRound1.Radius = 5.0 / 12.0; // convert to feet
                SizeAlignCoupleConnect(conn1SqToRound1, conn2Tap1, 0);

                var pt90Bend1 = FabricationPart.Create(Doc, bt90Bend, 10.0 / 12.0, 10.0 / 12.0, levelOne.Id);
                var conn190Bend1 = GetPrimaryConnector(pt90Bend1.ConnectorManager);
                var conn290Bend1 = GetSecondaryConnector(pt90Bend1.ConnectorManager);
                SizeAlignCoupleConnect(conn190Bend1, conn2SqToRound1, 0);

                var pt90Bend2 = FabricationPart.Create(Doc, bt90Bend, 0, levelOne.Id);
                var conn190Bend2 = GetPrimaryConnector(pt90Bend2.ConnectorManager);
                var conn290Bend2 = GetSecondaryConnector(pt90Bend2.ConnectorManager);
                SizeAlignCoupleConnect(conn190Bend2, conn290Bend1, 0);

                var ptTube1 = CreateStraightPart(btTube, 0, levelOne.Id, 5.0);
                var conn1Tube1 = GetPrimaryConnector(ptTube1.ConnectorManager);
                var conn2Tube1 = GetSecondaryConnector(ptTube1.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube1, conn290Bend2, 0);

                var ptSqToRound2 = FabricationPart.Create(Doc, btSqToRound, 0, levelOne.Id);
                var conn1SqToRound2 = GetPrimaryConnector(ptSqToRound2.ConnectorManager);
                var conn2SqToRound2 = GetSecondaryConnector(ptSqToRound2.ConnectorManager);
                SizeAlignCoupleConnect(conn2SqToRound2, conn2Tube1, 0);

                ptSqToRound2.Specification = 0;

                var materialId = config.LocateMaterial("Ductwork", "Mild Steel");
                var insSpecId = config.LocateInsulationSpecification("Ductwork", "Acoustic Liner 1''");
                var connId = config.LocateFabricationConnector("Duct - S&D", "S&D", ConnectorDomainType.Undefined,
                    ConnectorProfileType.Rectangular);

                if (materialId >= 0)
                    ptSqToRound2.Material = materialId;
                if (insSpecId >= 0)
                    ptSqToRound2.InsulationSpecification = insSpecId;
                if (connId >= 0)
                    conn1SqToRound2.GetFabricationConnectorInfo().BodyConnectorId = connId;

                var ptTransition1 = FabricationPart.Create(Doc, btTransition, 0, levelOne.Id);
                SetDimValue(ptTransition1, "Length", 2.5);
                var conn1Transition1 = GetPrimaryConnector(ptTransition1.ConnectorManager);
                var conn2Transition1 = GetSecondaryConnector(ptTransition1.ConnectorManager);
                conn2Transition1.Width = 2.0;
                conn2Transition1.Height = 2.0;
                SizeAlignCoupleConnect(conn1Transition1, conn2Straight1, 0);

                var ptSqBend2 = FabricationPart.Create(Doc, btSqBend, 0, levelOne.Id);
                var conn1SqBend2 = GetPrimaryConnector(ptSqBend2.ConnectorManager);
                var conn2SqBend2 = GetSecondaryConnector(ptSqBend2.ConnectorManager);
                SizeAlignCoupleConnect(conn1SqBend2, conn2Transition1, 0);

                var ptRectStraight2 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 4.0 + (5.0 / 12.0));
                var conn1Straight2 = GetPrimaryConnector(ptRectStraight2.ConnectorManager);
                var conn2Straight2 = GetSecondaryConnector(ptRectStraight2.ConnectorManager);
                SizeAlignCoupleConnect(conn1Straight2, conn2SqBend2, 0);

                var ptSqBend3 = FabricationPart.Create(Doc, btSqBend, 0, levelOne.Id);
                var conn1SqBend3 = GetPrimaryConnector(ptSqBend3.ConnectorManager);
                var conn2SqBend3 = GetSecondaryConnector(ptSqBend3.ConnectorManager);
                SizeAlignCoupleConnect(conn1SqBend3, conn2Straight2, Math.PI);

                var ptRectStraight3 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 5.0);
                var conn1Straight3 = GetPrimaryConnector(ptRectStraight3.ConnectorManager);
                var conn2Straight3 = GetSecondaryConnector(ptRectStraight3.ConnectorManager);
                SizeAlignCoupleConnect(conn1Straight3, conn2SqBend3, 0);

                FabricationPart.CreateHanger(Doc, btHangerBearer, ptRectStraight3.Id, conn1Straight3, 2.5,
                    true);

                var ptRadBend1 = FabricationPart.Create(Doc, btRadBend, 0, levelOne.Id);
                var conn1RadBend1 = GetPrimaryConnector(ptRadBend1.ConnectorManager);
                var conn2RadBend1 = GetSecondaryConnector(ptRadBend1.ConnectorManager);
                SizeAlignCoupleConnect(conn1RadBend1, conn2Straight3, 3.0 * Math.PI / 2.0);
                SetDimValue(ptRadBend1, "Angle", Math.PI / 4.0);

                var ptRectStraight4 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 1.0 + (8.0 / 12.0));
                var conn1Straight4 = GetPrimaryConnector(ptRectStraight4.ConnectorManager);
                var conn2Straight4 = GetSecondaryConnector(ptRectStraight4.ConnectorManager);
                SizeAlignCoupleConnect(conn1Straight4, conn2RadBend1, 0);

                var ptRadBend2 = FabricationPart.Create(Doc, btRadBend, 0, levelOne.Id);
                var conn1RadBend2 = GetPrimaryConnector(ptRadBend2.ConnectorManager);
                var conn2RadBend2 = GetSecondaryConnector(ptRadBend2.ConnectorManager);
                SizeAlignCoupleConnect(conn1RadBend2, conn2Straight4, Math.PI);
                SetDimValue(ptRadBend2, "Angle", Math.PI / 4.0);

                var ptRectStraight5 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 5.0);
                var conn1Straight5 = GetPrimaryConnector(ptRectStraight5.ConnectorManager);
                var conn2Straight5 = GetSecondaryConnector(ptRectStraight5.ConnectorManager);
                SizeAlignCoupleConnect(conn1Straight5, conn2RadBend2, 0);

                FabricationPart.CreateHanger(Doc, btHangerBearer, ptRectStraight5.Id, conn1Straight5, 2.5,
                    true);

                var ptRectStraight6 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 2.5);
                var conn1Straight6 = GetPrimaryConnector(ptRectStraight6.ConnectorManager);
                var conn2Straight6 = GetSecondaryConnector(ptRectStraight6.ConnectorManager);
                SizeAlignCoupleConnect(conn1Straight6, conn2Straight5, 0);

                // Tap outlet size via FABRICATION_PRODUCT_ENTRY; radius can be set on the connector instead.
                var ptFlatShoe1 = FabricationPart.Create(Doc, btFlatShoe, 0, levelOne.Id);
                var prodEntryFlatShoe1 = ptFlatShoe1.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY);
                prodEntryFlatShoe1.Set("8''");
                var conn1FlatShoe1 = GetPrimaryConnector(ptFlatShoe1.ConnectorManager);
                var conn2FlatShoe1 = GetSecondaryConnector(ptFlatShoe1.ConnectorManager);
                FabricationPart.PlaceAsTap(Doc, conn1FlatShoe1, conn1Straight6, 1.25, Math.PI, 0);

                var lengthTube2 = 16.0 + (8.0 / 12.0);
                var ptTube2 = CreateStraightPart(btTube, 0, levelOne.Id, lengthTube2);
                var conn1Tube2 = GetPrimaryConnector(ptTube2.ConnectorManager);
                var conn2Tube2 = GetSecondaryConnector(ptTube2.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube2, conn2FlatShoe1, 0);
                for (var i = 0; i < 3; i++)
                    FabricationPart.CreateHanger(Doc, btHangerRound, 1, ptTube2.Id, conn1Tube2,
                        (i + 1) * lengthTube2 / 4, true);

                var pt90Bend3 = FabricationPart.Create(Doc, bt90Bend, 0, levelOne.Id);
                var conn190Bend3 = GetPrimaryConnector(pt90Bend3.ConnectorManager);
                var conn290Bend3 = GetSecondaryConnector(pt90Bend3.ConnectorManager);
                SizeAlignCoupleConnect(conn190Bend3, conn2Tube2, Math.PI);

                var ptTube3 = CreateStraightPart(btTube, 0, levelOne.Id, 10.0);
                var conn1Tube3 = GetPrimaryConnector(ptTube3.ConnectorManager);
                var conn2Tube3 = GetSecondaryConnector(ptTube3.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube3, conn290Bend3, 0);

                FabricationPart.CreateHanger(Doc, btHangerRound, ptTube3.Id, conn1Tube3, 5.0, true);

                var pt45Bend1 = FabricationPart.Create(Doc, bt45Bend, 0, levelOne.Id);
                var conn145Bend1 = GetPrimaryConnector(pt45Bend1.ConnectorManager);
                var conn245Bend1 = GetSecondaryConnector(pt45Bend1.ConnectorManager);
                SizeAlignCoupleConnect(conn145Bend1, conn2Tube3, Math.PI);

                var ptTube4 = CreateStraightPart(btTube, 0, levelOne.Id, 2.0);
                var conn1Tube4 = GetPrimaryConnector(ptTube4.ConnectorManager);
                var conn2Tube4 = GetSecondaryConnector(ptTube4.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube4, conn245Bend1, 0);

                var pt45Bend2 = FabricationPart.Create(Doc, bt45Bend, 0, levelOne.Id);
                var conn145Bend2 = GetPrimaryConnector(pt45Bend2.ConnectorManager);
                var conn245Bend2 = GetSecondaryConnector(pt45Bend2.ConnectorManager);
                SizeAlignCoupleConnect(conn145Bend2, conn2Tube4, Math.PI);

                var ptTube5 = CreateStraightPart(btTube, 0, levelOne.Id, 10.0);
                var conn1Tube5 = GetPrimaryConnector(ptTube5.ConnectorManager);
                GetSecondaryConnector(ptTube5.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube5, conn245Bend2, 0);

                FabricationPart.CreateHanger(Doc, btHangerRound, ptTube5.Id, conn1Tube5, 5.0, true);

                var ptSqBend4 = FabricationPart.Create(Doc, btSqBend, 0, levelOne.Id);
                var conn1SqBend4 = GetPrimaryConnector(ptSqBend4.ConnectorManager);
                var conn2SqBend4 = GetSecondaryConnector(ptSqBend4.ConnectorManager);
                SizeAlignCoupleConnect(conn1SqBend4, conn2Straight6, Math.PI);

                var ptRectStraight7 = CreateStraightPart(btRectStraight, 0, levelOne.Id, 5.0);
                var conn1Straight7 = GetPrimaryConnector(ptRectStraight7.ConnectorManager);
                var conn2Straight7 = GetSecondaryConnector(ptRectStraight7.ConnectorManager);
                SizeAlignCoupleConnect(conn1Straight7, conn2SqBend4, 0);

                FabricationPart.CreateHanger(Doc, btHangerBearer, ptRectStraight7.Id, conn1Straight7, 2.5,
                    true);

                var ptRectTee1 = FabricationPart.Create(Doc, btRectTee, 0, levelOne.Id);

                // Set the size prior to connecting the part. Template parts with more than 2 connectors will disable editing of sizes once one of the connectors is connected to something.
                SetDimValue(ptRectTee1, "Right Width",
                    16.0 / 12.0);
                SetDimValue(ptRectTee1, "Btm Width",
                    20.0 / 12.0);

                var conn1RectTee1 = GetPrimaryConnector(ptRectTee1.ConnectorManager);
                var conn2RectTee1 = GetSecondaryConnector(ptRectTee1.ConnectorManager);
                var conn3RectTee1 = GetFirstNonPrimaryOrSecondaryConnector(ptRectTee1.ConnectorManager);
                SizeAlignCoupleConnect(conn3RectTee1, conn2Straight7, Math.PI);

                var ptSqToRound3 = FabricationPart.Create(Doc, btSqToRound, 0, levelOne.Id);
                var conn1SqToRound3 = GetPrimaryConnector(ptSqToRound3.ConnectorManager);
                var conn2SqToRound3 = GetSecondaryConnector(ptSqToRound3.ConnectorManager);
                SizeAlignCoupleConnect(conn1SqToRound3, conn2RectTee1, 0);
                SetDimValue(ptSqToRound3, "Length",
                    1.0 + (8.5 / 12.0));
                SetDimValue(ptSqToRound3, "Diameter", 1.0);

                var lengthTube6 = 22.0 + (4.0 / 12.0);
                var ptTube6 = CreateStraightPart(btTube, 0, levelOne.Id, lengthTube6);
                var conn1Tube6 = GetPrimaryConnector(ptTube6.ConnectorManager);
                var conn2Tube6 = GetSecondaryConnector(ptTube6.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube6, conn2SqToRound3, 0);

                for (var i = 0; i < 3; i++)
                    FabricationPart.CreateHanger(Doc, btHangerRound, 1, ptTube6.Id, conn1Tube6,
                        (i + 1) * lengthTube6 / 4, true);

                var ptReducer1 = FabricationPart.Create(Doc, btReducer, 0, levelOne.Id);
                var conn1Reducer1 = GetPrimaryConnector(ptReducer1.ConnectorManager);
                var conn2Reducer1 = GetSecondaryConnector(ptReducer1.ConnectorManager);
                SizeAlignCoupleConnect(conn1Reducer1, conn2Tube6, 0);
                var prodEntryReducer1 = ptReducer1.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY);
                prodEntryReducer1.Set("12''x8''");

                var ptTube7 = CreateStraightPart(btTube, 0, levelOne.Id, 10.0);
                var conn1Tube7 = GetPrimaryConnector(ptTube7.ConnectorManager);
                GetSecondaryConnector(ptTube7.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube7, conn2Reducer1, 0);

                FabricationPart.CreateHanger(Doc, btHangerRound, ptTube7.Id, conn1Tube7, 5.0, true);

                var ptCurvedBoot1 = FabricationPart.Create(Doc, btCurvedBoot, 0, levelOne.Id);
                var prodEntryCurvedBoot1 =
                    ptCurvedBoot1.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY);
                prodEntryCurvedBoot1.Set("12''x8''");
                var conn1CurvedBoot1 = GetPrimaryConnector(ptCurvedBoot1.ConnectorManager);
                var conn2CurvedBoot1 = GetSecondaryConnector(ptCurvedBoot1.ConnectorManager);
                FabricationPart.PlaceAsTap(Doc, conn1CurvedBoot1, conn2Tube6, 1.0 + (2.0 / 12.0), Math.PI,
                    Math.PI);

                var lengthTube8 = 16.0 + (8.0 / 12.0);
                var ptTube8 = CreateStraightPart(btTube, 0, levelOne.Id, lengthTube8);
                var conn1Tube8 = GetPrimaryConnector(ptTube8.ConnectorManager);
                GetSecondaryConnector(ptTube8.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube8, conn2CurvedBoot1, 0);
                for (var i = 0; i < 3; i++)
                    FabricationPart.CreateHanger(Doc, btHangerRound, 0, ptTube8.Id, conn1Tube8,
                        (i + 1) * lengthTube8 / 4, true);

                var ptSqToRound4 = FabricationPart.Create(Doc, btSqToRound, 0, levelOne.Id);
                var conn1SqToRound4 = GetPrimaryConnector(ptSqToRound4.ConnectorManager);
                var conn2SqToRound4 = GetSecondaryConnector(ptSqToRound4.ConnectorManager);
                SizeAlignCoupleConnect(conn1SqToRound4, conn1RectTee1, 0);
                SetDimValue(ptSqToRound4, "Length",
                    1.0 + (8.5 / 12.0));
                SetDimValue(ptSqToRound4, "Diameter", 1.0);

                var ptTube9 = CreateStraightPart(btTube, 0, levelOne.Id, 10.0);
                var conn1Tube9 = GetPrimaryConnector(ptTube9.ConnectorManager);
                var conn2Tube9 = GetSecondaryConnector(ptTube9.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube9, conn2SqToRound4, 0);

                FabricationPart.CreateHanger(Doc, btHangerRound, ptTube9.Id, conn1Tube9, 5.0, true);

                var ptCurvedBoot2 = FabricationPart.Create(Doc, btCurvedBoot, 0, levelOne.Id);
                var prodEntryCurvedBoot2 =
                    ptCurvedBoot2.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY);
                prodEntryCurvedBoot2.Set("12''x8''");
                var conn1CurvedBoot2 = GetPrimaryConnector(ptCurvedBoot2.ConnectorManager);
                var conn2CurvedBoot2 = GetSecondaryConnector(ptCurvedBoot2.ConnectorManager);
                FabricationPart.PlaceAsTap(Doc, conn1CurvedBoot2, conn1Tube9, 7.5, Math.PI, 0);

                var lengthTube10 = 8.0 + (1.0 / 12.0);
                var ptTube10 = CreateStraightPart(btTube, 0, levelOne.Id, lengthTube10);
                var conn1Tube10 = GetPrimaryConnector(ptTube10.ConnectorManager);
                var conn2Tube10 = GetSecondaryConnector(ptTube10.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube10, conn2CurvedBoot2, 0);

                FabricationPart.CreateHanger(Doc, btHangerRound, ptTube10.Id, conn1Tube10, lengthTube10 / 2,
                    true);

                var pt45Bend3 = FabricationPart.Create(Doc, bt45Bend, 0, levelOne.Id);
                var conn145Bend3 = GetPrimaryConnector(pt45Bend3.ConnectorManager);
                var conn245Bend3 = GetSecondaryConnector(pt45Bend3.ConnectorManager);
                SizeAlignCoupleConnect(conn145Bend3, conn2Tube10, 0);

                var ptTube11 = CreateStraightPart(btTube, 0, levelOne.Id, 20.0);
                var conn1Tube11 = GetPrimaryConnector(ptTube11.ConnectorManager);
                var conn2Tube11 = GetSecondaryConnector(ptTube11.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube11, conn245Bend3, 0);

                var pt45Bend4 = FabricationPart.Create(Doc, bt45Bend, 0, levelOne.Id);
                var conn145Bend4 = GetPrimaryConnector(pt45Bend4.ConnectorManager);
                var conn245Bend4 = GetSecondaryConnector(pt45Bend4.ConnectorManager);
                SizeAlignCoupleConnect(conn145Bend4, conn2Tube11, 0);

                var ptTube12 = CreateStraightPart(btTube, 0, levelOne.Id, 1.0 + (8.0 / 12.0));
                var conn1Tube12 = GetPrimaryConnector(ptTube12.ConnectorManager);
                GetSecondaryConnector(ptTube12.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube12, conn245Bend4, 0);

                var ptReducer2 = FabricationPart.Create(Doc, btReducer, 0, levelOne.Id);
                var conn1Reducer2 = GetPrimaryConnector(ptReducer2.ConnectorManager);
                var conn2Reducer2 = GetSecondaryConnector(ptReducer2.ConnectorManager);
                SizeAlignCoupleConnect(conn1Reducer2, conn2Tube9, 0);
                var prodEntryReducer2 = ptReducer2.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY);
                prodEntryReducer2.Set("12''x10''");

                var ptTube13 = CreateStraightPart(btTube, 0, levelOne.Id, 10.0);
                var conn1Tube13 = GetPrimaryConnector(ptTube13.ConnectorManager);
                var conn2Tube13 = GetSecondaryConnector(ptTube13.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube13, conn2Reducer2, 0);

                FabricationPart.CreateHanger(Doc, btHangerRound, ptTube13.Id, conn1Tube13, 5.0, true);

                var pt90Bend4 = FabricationPart.Create(Doc, bt90Bend, 0, levelOne.Id);
                var conn190Bend4 = GetPrimaryConnector(pt90Bend4.ConnectorManager);
                var conn290Bend4 = GetSecondaryConnector(pt90Bend4.ConnectorManager);
                SizeAlignCoupleConnect(conn190Bend4, conn2Tube13, 3.0 * Math.PI / 4.0);

                var ptTube14 = CreateStraightPart(btTube, 0, levelOne.Id, 1.0 + (2.5 / 12.0));
                var conn1Tube14 = GetPrimaryConnector(ptTube14.ConnectorManager);
                var conn2Tube14 = GetSecondaryConnector(ptTube14.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube14, conn290Bend4, 0);

                var pt45Bend5 = FabricationPart.Create(Doc, bt45Bend, 0, levelOne.Id);
                var conn145Bend5 = GetPrimaryConnector(pt45Bend5.ConnectorManager);
                var conn245Bend5 = GetSecondaryConnector(pt45Bend5.ConnectorManager);
                SizeAlignCoupleConnect(conn145Bend5, conn2Tube14, Math.PI / 2.0);

                var ptTube15 = CreateStraightPart(btTube, 0, levelOne.Id, 20.0);
                var conn1Tube15 = GetPrimaryConnector(ptTube15.ConnectorManager);
                GetSecondaryConnector(ptTube15.ConnectorManager);
                SizeAlignCoupleConnect(conn1Tube15, conn245Bend5, 0);
                for (var i = 0; i < 4; i++)
                    FabricationPart.CreateHanger(Doc, btHangerRound, ptTube15.Id, conn1Tube15,
                        (i + 1) * (20.0 / 5), true);

                var ptValve1 = FabricationPart.Create(Doc, btValve, 1, levelOne.Id);
                var prodEntryPtValve1 = ptValve1.get_Parameter(BuiltInParameter.FABRICATION_PRODUCT_ENTRY);
                prodEntryPtValve1.Set("6''");
                FabricationPart.AlignPartByInsertionPoint(Doc, ptValve1.Id, new XYZ(16, -10, 0), 0, 0, 0,
                    FabricationPartJustification.Middle, null);
                Doc.Regenerate();
                var conn2Valve1 = GetSecondaryConnector(ptValve1.ConnectorManager);

                var ptPipe1 = CreateStraightPart(btGroovedPipe, 0, levelOne.Id, 10.0);
                var conn1Pipe1 = GetPrimaryConnector(ptPipe1.ConnectorManager);
                var conn2Pipe1 = GetSecondaryConnector(ptPipe1.ConnectorManager);
                SizeAlignSlopeJustifyCoupleConnect(conn1Pipe1, conn2Valve1, 0, 0,
                    FabricationPartJustification.Middle);

                var pipe1Pos = (conn1Pipe1.Origin + conn2Pipe1.Origin) / 2.0;
                var ptValve2 = FabricationPart.Create(Doc, btValve, 1, levelOne.Id);
                // Cut-in valve sizes to the straight automatically.
                FabricationPart.AlignPartByInsertionPointAndCutInToStraight(Doc, ptPipe1.Id, ptValve2.Id,
                    pipe1Pos, Math.PI / 2.0, 0, false);
                Doc.Regenerate();

                var pt90Elbow1 = FabricationPart.Create(Doc, bt90Elbow, 0, levelOne.Id);
                var conn190Elbow1 = GetPrimaryConnector(pt90Elbow1.ConnectorManager);
                var conn290Elbow1 = GetSecondaryConnector(pt90Elbow1.ConnectorManager);
                SizeAlignSlopeJustifyCoupleConnect(conn190Elbow1, conn2Pipe1, Math.PI, 0.02,
                    FabricationPartJustification.Middle);

                var ptPipe2 = CreateStraightPart(btGroovedPipe, 0, levelOne.Id, 10.0);
                var conn1Pipe2 = GetPrimaryConnector(ptPipe2.ConnectorManager);
                GetSecondaryConnector(ptPipe2.ConnectorManager);
                SizeAlignSlopeJustifyCoupleConnect(conn1Pipe2, conn290Elbow1, 0, 0,
                    FabricationPartJustification.Middle);

                tr.Commit();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private double GetDimValue(FabricationPart part, string dimName)
        {
            double value = 0;
            if (part != null)
            {
                var dims = part.GetDimensions();
                foreach (var def in dims)
                {
                    if (def.Name.Equals(dimName))
                    {
                        value = part.GetDimensionValue(def);
                        break;
                    }
                }
            }

            return value;
        }

        private bool SetDimValue(FabricationPart part, string dimName, double dimValue)
        {
            var dims = part.GetDimensions();
            FabricationDimensionDefinition dim = null;
            foreach (var def in dims)
            {
                if (def.Name.Equals(dimName))
                {
                    dim = def;
                    break;
                }
            }

            if (dim == null)
                return false;

            part.SetDimensionValue(dim, dimValue);
            Doc.Regenerate();
            return true;
        }

        // rotation: angle between width vectors, in radians.
        private void SizeAlignCoupleConnect(Connector connFrom, Connector connTo, double rotation)
        {
            if (connFrom.Shape is ConnectorProfileType.Rectangular or ConnectorProfileType.Oval)
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

        // slope: fractional rise (positive up); fittings only—straights inherit from the connected piece.
        private void SizeAlignSlopeJustifyCoupleConnect(Connector connFrom, Connector connTo, double rotation,
            double slope, FabricationPartJustification justification)
        {
            if (connFrom.Shape is ConnectorProfileType.Rectangular or ConnectorProfileType.Oval)
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

        private void GetMaterials(FabricationConfiguration config)
        {
            MaterialIds = config.GetAllMaterials(null);

            MaterialGroups = [];
            MaterialNames = [];

            foreach (var materialId in MaterialIds)
            {
                MaterialGroups.Add(config.GetMaterialGroup(materialId));
                MaterialNames.Add(config.GetMaterialName(materialId));
            }
        }

        private void GetSpecs(FabricationConfiguration config)
        {
            SpecIds = config.GetAllSpecifications(null);

            SpecGroups = [];
            SpecNames = [];

            foreach (var specId in SpecIds)
            {
                SpecGroups.Add(config.GetSpecificationGroup(specId));
                SpecNames.Add(config.GetSpecificationName(specId));
            }
        }

        private void GetInsulationSpecs(FabricationConfiguration config)
        {
            InsSpecIds = config.GetAllInsulationSpecifications(null);

            InsSpecGroups = [];
            InsSpecNames = [];

            foreach (var specId in InsSpecIds)
            {
                InsSpecGroups.Add(config.GetInsulationSpecificationGroup(specId));
                InsSpecNames.Add(config.GetInsulationSpecificationName(specId));
            }
        }

        private void GetFabricationConnectors(FabricationConfiguration config)
        {
            ConnIds = config.GetAllFabricationConnectorDefinitions(ConnectorDomainType.Undefined,
                ConnectorProfileType.Invalid);

            ConnGroups = [];
            ConnNames = [];

            foreach (var connId in ConnIds)
            {
                ConnGroups.Add(config.GetFabricationConnectorGroup(connId));
                ConnNames.Add(config.GetFabricationConnectorName(connId));
            }
        }
    }
}
