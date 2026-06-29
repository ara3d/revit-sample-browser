// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.GeometryCreation_BooleanOperation.CS
{
    [Transaction(TransactionMode.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData
            , ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                // Create a new transaction
                var tran = new Transaction(document, "GeometryCreation_BooleanOperation");
                tran.Start();

                // Create an object that is responsible for creating the solids
                var geometryCreation = GeometryCreation.GetInstance(commandData.Application.Application);

                // Create an object that is responsible for displaying the solids
                var avf = AnalysisVisualizationFramework.GetInstance(document);

                // Create a CSG tree solid
                CsgTree(geometryCreation, avf);

                tran.Commit();

                // Set the view which display the solid active
                commandData.Application.ActiveUIDocument.ActiveView =
                    document.GetElements<View>().First(e => e.Name == "CSGTree");

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Prepare 5 solids materials for CSG tree
        /// </summary>
        /// <param name="geometrycreation">The object that is responsible for creating the solids</param>
        /// <returns>The solids materials list</returns>
        private static List<Solid> PrepareSolids(GeometryCreation geometrycreation)
        {
            var resultSolids = new List<Solid>
            {
                geometrycreation.CreateCenterbasedBox(XYZ.Zero, 25),
                geometrycreation.CreateCenterbasedSphere(XYZ.Zero, 20),
                geometrycreation.CreateCenterbasedCylinder(XYZ.Zero, 5, 40,
                    GeometryCreation.CylinderDirection.BasisX),
                geometrycreation.CreateCenterbasedCylinder(XYZ.Zero, 5, 40,
                    GeometryCreation.CylinderDirection.BasisY),
                geometrycreation.CreateCenterbasedCylinder(XYZ.Zero, 5, 40,
                    GeometryCreation.CylinderDirection.BasisZ)
            };

            return resultSolids;
        }

        /// <summary>
        ///     Create a constructive solid geometry - CSG tree
        ///     http://en.wikipedia.org/wiki/Constructive_solid_geometry
        ///     http://en.wikipedia.org/wiki/File:Csg_tree.png
        /// </summary>
        /// <param name="geometrycreation">The object that is responsible for creating the solids</param>
        /// <param name="avf">The object that is responsible for displaying the solids</param>
        private static void CsgTree(GeometryCreation geometrycreation, AnalysisVisualizationFramework avf)
        {
            var materialSolids = PrepareSolids(geometrycreation);

            // Operation 1 : Intersect
            var csgTreeSolid1 = materialSolids[0].Intersect(materialSolids[1]);

            // Operation 2 : Union
            var csgTreeSolid2 = materialSolids[2].Union(materialSolids[3]);
            
            // Operation 3 : Union
            csgTreeSolid2.Union(materialSolids[4]);

            // Operation 4 : Difference
            csgTreeSolid1.Difference(csgTreeSolid2);

            avf.PaintSolid(csgTreeSolid1, "CSGTree");
        }
    }
}
