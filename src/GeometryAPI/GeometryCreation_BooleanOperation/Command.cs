// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

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

                Transaction tran = new(document, "GeometryCreation_BooleanOperation");
                tran.Start();

                var geometryCreation = GeometryCreation.GetInstance(commandData.Application.Application);
                var avf = AnalysisVisualizationFramework.GetInstance(document);

                CsgTree(geometryCreation, avf);

                tran.Commit();

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

        private static List<Solid> PrepareSolids(GeometryCreation geometrycreation)
        {
            return [
            geometrycreation.CreateCenterbasedBox(XYZ.Zero, 25),
            geometrycreation.CreateCenterbasedSphere(XYZ.Zero, 20),
            geometrycreation.CreateCenterbasedCylinder(XYZ.Zero, 5, 40, GeometryCreation.CylinderDirection.BasisX),
            geometrycreation.CreateCenterbasedCylinder(XYZ.Zero, 5, 40, GeometryCreation.CylinderDirection.BasisY),
            geometrycreation.CreateCenterbasedCylinder(XYZ.Zero, 5, 40, GeometryCreation.CylinderDirection.BasisZ)
        ];
        }

        // CSG tree: https://en.wikipedia.org/wiki/Constructive_solid_geometry
        private static void CsgTree(GeometryCreation geometrycreation, AnalysisVisualizationFramework avf)
        {
            var materialSolids = PrepareSolids(geometrycreation);

            var csgTreeSolid1 = materialSolids[0].Intersect(materialSolids[1]);
            var csgTreeSolid2 = materialSolids[2].Union(materialSolids[3]);
            csgTreeSolid2.Union(materialSolids[4]);
            csgTreeSolid1.Difference(csgTreeSolid2);

            avf.PaintSolid(csgTreeSolid1, "CSGTree");
        }
    }
}
