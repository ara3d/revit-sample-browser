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
    /// <summary>
    ///     This command allows to create vertical CompoundStructure and applying to walls.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class WallCompoundStructure : IExternalCommand
    {
        /// <summary>
        ///     store the application
        /// </summary>
        private UIApplication m_application;

        /// <summary>
        ///     store the document
        /// </summary>
        private UIDocument m_document;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_application = commandData.Application;
            m_document = m_application.ActiveUIDocument;

            var transaction = new Transaction(m_document.Document, "Create CompoundStructure for Wall");

            try
            {
                // Select at least a wall.
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

                // Create the CompoundStructure for wall.
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
                // If any error, give error information and return failed
                message = ex.Message;
                transaction.RollBack();
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Create vertical CompoundStructure for wall: new layers, split new region, new sweep and new reveal.
        /// </summary>
        /// <param name="wall">The wall applying the new CompoundStructure.</param>
        public void CreateCSforWall(Wall wall)
        {
            // Get CompoundStructure

            var wallType = wall.WallType;
            //wallType.Name = wallType.Name + "_WithNewCompoundStructure";
            var wallCs = wallType.GetCompoundStructure();
            // Get material for CompoundStructureLayer

            var masonryBrick = CreateSampleBrickMaterial();
            var concrete = CreateSampleConcreteMaterial();

            // Create CompoundStructureLayers and add the materials created above to them.
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

            // Set the created layers to CompoundStructureLayer
            wallCs.SetLayers(csLayers);

            //Set which layer is used for structural analysis
            wallCs.StructuralMaterialIndex = 2;

            // Set shell layers and wrapping.
            wallCs.SetNumberOfShellLayers(ShellLayerType.Interior, 2);
            wallCs.SetNumberOfShellLayers(ShellLayerType.Exterior, 1);
            wallCs.SetParticipatesInWrapping(0, false);

            // Points for adding wall sweep and reveal.
            var sweepPoint = UV.Zero;
            var revealPoint = UV.Zero;

            // split the region containing segment 0.
            var segId = wallCs.GetSegmentIds()[0];
            foreach (var regionId in wallCs.GetAdjacentRegions(segId))
            {
                // Get the end points of segment 0.
                var endPoint1 = UV.Zero;
                var endPoint2 = UV.Zero;
                wallCs.GetSegmentEndPoints(segId, regionId, out endPoint1, out endPoint2);

                // Split a new region in split point and orientation.
                var splitOrientation =
                    (RectangularGridSegmentOrientation)(((int)wallCs.GetSegmentOrientation(segId) + 1) % 2);
                var splitUv = (endPoint1 + endPoint2) / 2.0;
                var newRegionId = wallCs.SplitRegion(splitUv, splitOrientation);
                wallCs.IsValidRegionId(newRegionId);

                // Find the enclosing region and the two segments intersected by a line through the split point
                int segId1;
                int segId2;
                var findRegionId =
                    wallCs.FindEnclosingRegionAndSegments(splitUv, splitOrientation, out segId1, out segId2);

                // Get the end points of finding segment 1 and compute the wall sweep point.
                var eP1 = UV.Zero;
                var eP2 = UV.Zero;
                wallCs.GetSegmentEndPoints(segId1, findRegionId, out eP1, out eP2);
                sweepPoint = (eP1 + eP2) / 4.0;

                // Get the end points of finding segment 2 and compute the wall reveal point.
                var ep3 = UV.Zero;
                var ep4 = UV.Zero;
                wallCs.GetSegmentEndPoints(segId2, findRegionId, out ep3, out ep4);
                revealPoint = (ep3 + ep4) / 2.0;
            }

            // Create a WallSweepInfo for wall sweep
            var sweepInfo = new WallSweepInfo(true, WallSweepType.Sweep);
            PrepareWallSweepInfo(sweepInfo, sweepPoint.V);
            // Set sweep profile: Sill-Precast : 8" Wide
            sweepInfo.ProfileId = GetProfile("8\" Wide").Id;
            sweepInfo.Id = 101;
            wallCs.AddWallSweep(sweepInfo);

            // Create a WallSweepInfo for wall reveal
            var revealInfo = new WallSweepInfo(true, WallSweepType.Reveal);
            PrepareWallSweepInfo(revealInfo, revealPoint.U);
            revealInfo.Id = 102;
            wallCs.AddWallSweep(revealInfo);

            // Set the new wall CompoundStructure to the type of wall.
            wallType.SetCompoundStructure(wallCs);
        }

        /// <summary>
        ///     Setting the WallSweepInfo for sweep and reveal.
        /// </summary>
        /// <param name="wallSweepInfo">The WallSweepInfo.</param>
        /// <param name="distance">The distance from either the top or base of the wall</param>
        private void PrepareWallSweepInfo(WallSweepInfo wallSweepInfo, double distance)
        {
            wallSweepInfo.DistanceMeasuredFrom = DistanceMeasuredFrom.Base;
            wallSweepInfo.Distance = distance;
            wallSweepInfo.WallSide = WallSide.Exterior;
            wallSweepInfo.Id = -1;
            wallSweepInfo.WallOffset = -0.1;
        }

        /// <summary>
        ///     Getting the specific material by name.
        /// </summary>
        /// <param name="name">The name of specific material.</param>
        /// <returns>The specific material</returns>
        private Material GetMaterial(string name)
        {
            var collector = new FilteredElementCollector(m_document.Document);
            collector.WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Materials));
            var materialElement = from element in collector
                where element.Name == name
                select element;

            if (!materialElement.Any())
                return null;
            return materialElement.First() as Material;
        }

        /// <summary>
        ///     Create a new brick material
        /// </summary>
        /// <returns>The specific material</returns>
        private Material CreateSampleBrickMaterial()
        {
            var createMaterial = new SubTransaction(m_document.Document);
            createMaterial.Start();
            Material materialNew = null;

            //Try to copy an existing material.  If it is not available, create a new one.
            var masonryBrick = GetMaterial("Brick, Common");
            if (masonryBrick != null)
            {
                materialNew = masonryBrick.Duplicate(masonryBrick.Name + "_new");
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

            //Create a new structural asset and set properties on it.
            var structuralAsssetBrick = new StructuralAsset("BrickStructuralAsset", StructuralAssetClass.Generic);

            var pseStructural = PropertySetElement.Create(m_document.Document, structuralAsssetBrick);

            //Create a new thermal asset and set properties on it.
            var thermalAssetBrick = new ThermalAsset("BrickThermalAsset", ThermalMaterialType.Solid)
            {
                Porosity = 0.1,
                Permeability = 0.2,
                Compressibility = .5,
                ThermalConductivity = .5
            };

            //Create PropertySets from assets and assign them to the material.
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

        /// <summary>
        ///     Create a new concrete material.
        /// </summary>
        /// <returns>The specific material</returns>
        private Material CreateSampleConcreteMaterial()
        {
            Material materialNew = null;
            //Try to copy an existing material.  If it is not available, create a new one.
            var masonryConcrete = GetMaterial("Concrete, Lightweight");
            if (masonryConcrete != null)
            {
                materialNew = masonryConcrete.Duplicate(masonryConcrete.Name + "_new");
                materialNew.MaterialClass = "Concrete";
            }
            else
            {
                var idNew = Material.Create(m_document.Document, "New Concrete Sample");
                materialNew = m_document.Document.GetElement(idNew) as Material;
                materialNew.Color = new Color(130, 150, 120);
            }

            //Create a new structural asset and set properties on it.
            var structuralAsssetConcrete =
                new StructuralAsset("ConcreteStructuralAsset", StructuralAssetClass.Concrete)
                {
                    ConcreteBendingReinforcement = .5
                };

            //Create a new thermal asset and set properties on it.
            var thermalAssetConcrete = new ThermalAsset("ConcreteThermalAsset", ThermalMaterialType.Solid)
            {
                Porosity = 0.2,
                Permeability = 0.3,
                Compressibility = .5,
                ThermalConductivity = .5
            };

            //Create PropertySets from assets and assign them to the material.
            var pseThermal = PropertySetElement.Create(m_document.Document, thermalAssetConcrete);
            var pseStructural = PropertySetElement.Create(m_document.Document, structuralAsssetConcrete);

            materialNew.SetMaterialAspectByPropertySet(MaterialAspect.Structural, pseStructural.Id);
            materialNew.SetMaterialAspectByPropertySet(MaterialAspect.Thermal, pseThermal.Id);

            return materialNew;
        }

        /// <summary>
        ///     Getting the profile for sweep or reveal.
        /// </summary>
        /// <param name="name">The name of specific profile.</param>
        /// <returns>The specific profile.</returns>
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
