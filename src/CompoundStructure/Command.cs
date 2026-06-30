// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.CompoundStructure.CS
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class WallCompoundStructure : IExternalCommand
    {
        private UIApplication m_application;
        private UIDocument m_document;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_application = commandData.Application;
            m_document = m_application.ActiveUIDocument;

            var transaction = new Transaction(m_document.Document, "Create CompoundStructure for Wall");

            try
            {
                var selectedElements = new ElementSet();
                foreach (var elementId in m_document.Selection.GetElementIds())
                {
                    selectedElements.Insert(m_document.Document.GetElement(elementId));
                }

                if (selectedElements.IsEmpty)
                {
                    TaskDialog.Show("Error", "Please select one wall at least.");
                    return Result.Cancelled;
                }

                transaction.Start();
                if (selectedElements.IsEmpty)
                    return Result.Failed;

                foreach (Element elem in selectedElements)
                {
                    if (elem is Wall wall)
                    {
                        CreateCSforWall(wall);
                        break;
                    }
                }

                transaction.Commit();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                transaction.RollBack();
                return Result.Failed;
            }
        }

        public void CreateCSforWall(Wall wall)
        {
            var wallType = wall.WallType;
            //wallType.Name = wallType.Name + "_WithNewCompoundStructure";
            var wallCs = wallType.GetCompoundStructure();

            var masonryBrick = CreateSampleBrickMaterial();
            var concrete = CreateSampleConcreteMaterial();

            var csLayers = new List<CompoundStructureLayer>();
            var finish1Layer = new CompoundStructureLayer(0.2, MaterialFunctionAssignment.Finish1, masonryBrick.Id);
            var substrateLayer =
                new CompoundStructureLayer(0.1, MaterialFunctionAssignment.Substrate, ElementId.InvalidElementId);
            var structureLayer = new CompoundStructureLayer(0.5, MaterialFunctionAssignment.Structure, concrete.Id);
            var membraneLayer =
                new CompoundStructureLayer(0, MaterialFunctionAssignment.Membrane, ElementId.InvalidElementId);
            var finish2Layer = new CompoundStructureLayer(0.2, MaterialFunctionAssignment.Finish2, concrete.Id);
            csLayers.Add(finish1Layer);
            csLayers.Add(substrateLayer);
            csLayers.Add(structureLayer);
            csLayers.Add(membraneLayer);
            csLayers.Add(finish2Layer);

            wallCs.SetLayers(csLayers);

            wallCs.StructuralMaterialIndex = 2;

            wallCs.SetNumberOfShellLayers(ShellLayerType.Interior, 2);
            wallCs.SetNumberOfShellLayers(ShellLayerType.Exterior, 1);
            wallCs.SetParticipatesInWrapping(0, false);

            var sweepPoint = UV.Zero;
            var revealPoint = UV.Zero;

            var segId = wallCs.GetSegmentIds()[0];
            foreach (var regionId in wallCs.GetAdjacentRegions(segId))
            {
                var endPoint1 = UV.Zero;
                var endPoint2 = UV.Zero;
                wallCs.GetSegmentEndPoints(segId, regionId, out endPoint1, out endPoint2);

                var splitOrientation =
                    (RectangularGridSegmentOrientation)(((int)wallCs.GetSegmentOrientation(segId) + 1) % 2);
                var splitUv = (endPoint1 + endPoint2) / 2.0;
                var newRegionId = wallCs.SplitRegion(splitUv, splitOrientation);
                wallCs.IsValidRegionId(newRegionId);

                int segId1;
                int segId2;
                var findRegionId =
                    wallCs.FindEnclosingRegionAndSegments(splitUv, splitOrientation, out segId1, out segId2);

                var eP1 = UV.Zero;
                var eP2 = UV.Zero;
                wallCs.GetSegmentEndPoints(segId1, findRegionId, out eP1, out eP2);
                sweepPoint = (eP1 + eP2) / 4.0;

                var ep3 = UV.Zero;
                var ep4 = UV.Zero;
                wallCs.GetSegmentEndPoints(segId2, findRegionId, out ep3, out ep4);
                revealPoint = (ep3 + ep4) / 2.0;
            }

            var sweepInfo = new WallSweepInfo(true, WallSweepType.Sweep);
            PrepareWallSweepInfo(sweepInfo, sweepPoint.V);
            sweepInfo.ProfileId = GetProfile("8\" Wide").Id;
            sweepInfo.Id = 101;
            wallCs.AddWallSweep(sweepInfo);

            var revealInfo = new WallSweepInfo(true, WallSweepType.Reveal);
            PrepareWallSweepInfo(revealInfo, revealPoint.U);
            revealInfo.Id = 102;
            wallCs.AddWallSweep(revealInfo);

            wallType.SetCompoundStructure(wallCs);
        }

        private void PrepareWallSweepInfo(WallSweepInfo wallSweepInfo, double distance)
        {
            wallSweepInfo.DistanceMeasuredFrom = DistanceMeasuredFrom.Base;
            wallSweepInfo.Distance = distance;
            wallSweepInfo.WallSide = WallSide.Exterior;
            wallSweepInfo.Id = -1;
            wallSweepInfo.WallOffset = -0.1;
        }

        private Material GetMaterial(string name)
        {
            return m_document.Document.GetElements<Material>().FirstOrDefault(m => m.Name == name);
        }

        private Material CreateSampleBrickMaterial()
        {
            var createMaterial = new SubTransaction(m_document.Document);
            createMaterial.Start();
            Material materialNew;

            var masonryBrick = GetMaterial("Brick, Common");
            if (masonryBrick != null)
            {
                materialNew = masonryBrick.Duplicate($"{masonryBrick.Name}_new");
                Debug.WriteLine(masonryBrick.MaterialClass);
                materialNew.MaterialClass = "Brick";
            }
            else
            {
                var idNew = Material.Create(m_document.Document, "New Brick Sample");
                materialNew = m_document.Document.GetElement(idNew) as Material;
                materialNew.Color = new Color(255, 0, 0);
            }

            createMaterial.Commit();

            var createPropertySets = new SubTransaction(m_document.Document);
            createPropertySets.Start();

            var structuralAsssetBrick = new StructuralAsset("BrickStructuralAsset", StructuralAssetClass.Generic);

            var pseStructural = PropertySetElement.Create(m_document.Document, structuralAsssetBrick);

            var thermalAssetBrick = new ThermalAsset("BrickThermalAsset", ThermalMaterialType.Solid)
            {
                Porosity = 0.1,
                Permeability = 0.2,
                Compressibility = .5,
                ThermalConductivity = .5
            };

            var pseThermal = PropertySetElement.Create(m_document.Document, thermalAssetBrick);
            createPropertySets.Commit();
            var setPropertySets = new SubTransaction(m_document.Document);
            setPropertySets.Start();
            materialNew.SetMaterialAspectByPropertySet(MaterialAspect.Structural, pseStructural.Id);
            materialNew.SetMaterialAspectByPropertySet(MaterialAspect.Thermal, pseThermal.Id);

            //also try
            //materialNew.ThermalAssetId = pseThermal.Id;

            setPropertySets.Commit();
            return materialNew;
        }

        private Material CreateSampleConcreteMaterial()
        {
            Material materialNew = null;
            var masonryConcrete = GetMaterial("Concrete, Lightweight");
            if (masonryConcrete != null)
            {
                materialNew = masonryConcrete.Duplicate($"{masonryConcrete.Name}_new");
                materialNew.MaterialClass = "Concrete";
            }
            else
            {
                var idNew = Material.Create(m_document.Document, "New Concrete Sample");
                materialNew = m_document.Document.GetElement(idNew) as Material;
                materialNew.Color = new Color(130, 150, 120);
            }

            var structuralAsssetConcrete =
                new StructuralAsset("ConcreteStructuralAsset", StructuralAssetClass.Concrete)
                {
                    ConcreteBendingReinforcement = .5
                };

            var thermalAssetConcrete = new ThermalAsset("ConcreteThermalAsset", ThermalMaterialType.Solid)
            {
                Porosity = 0.2,
                Permeability = 0.3,
                Compressibility = .5,
                ThermalConductivity = .5
            };

            var pseThermal = PropertySetElement.Create(m_document.Document, thermalAssetConcrete);
            var pseStructural = PropertySetElement.Create(m_document.Document, structuralAsssetConcrete);

            materialNew.SetMaterialAspectByPropertySet(MaterialAspect.Structural, pseStructural.Id);
            materialNew.SetMaterialAspectByPropertySet(MaterialAspect.Thermal, pseThermal.Id);

            return materialNew;
        }

        private FamilySymbol GetProfile(string name)
        {
            var profiles = new FilteredElementCollector(m_document.Document);
            profiles.OfCategory(BuiltInCategory.OST_ProfileFamilies);
            var materialElement = from element in profiles
                where element.Name == name
                select element;
            return materialElement.First() as FamilySymbol;
        }
    }
}
