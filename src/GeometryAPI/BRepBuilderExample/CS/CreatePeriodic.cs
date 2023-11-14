// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.BRepBuilderExample.CS
{
    /// <summary>
    ///     Implement method Execute of this class as an external command for Revit.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class CreatePeriodic : IExternalCommand
    {
        private Document _dbdocument;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _dbdocument = commandData.Application.ActiveUIDocument.Document;

            try
            {
                CreateCylinder();
                CreateTruncatedCone();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        /// <summary>
        ///     Create a DirectShape element from a BRepBuilder object and keep it in the _dbdocument.
        ///     The main purpose is to display the BRepBuilder objects created.
        ///     In this function, the BrepBuilder is directly set to a DirectShape.
        /// </summary>
        /// <param name="myBRepBuilder"> The BRepBuilder object.</param>
        /// <param name="name"> Name of the BRepBuilder object, which will be passed on to the DirectShape creation method.</param>
        private void createDirectShapeElementFromBrepBuilderObject(BRepBuilder myBRepBuilder, string name)
        {
            if (!myBRepBuilder.IsResultAvailable())
                return;

            using (var tr = new Transaction(_dbdocument, "Create a DirectShape"))
            {
                tr.Start();

                var myDirectShape =
                    DirectShape.CreateElement(_dbdocument, new ElementId(BuiltInCategory.OST_GenericModel));
                myDirectShape.ApplicationId = "TestBRepBuilder";
                myDirectShape.ApplicationDataId = name;
                if (null != myDirectShape)
                    myDirectShape.SetShape(myBRepBuilder);
                tr.Commit();
            }
        }

        private void CreateCylinder()
        {
            // Naming convention for faces and edges: we assume that x is to the left and pointing down, y is horizontal and pointing to the right, z is up
            var brepBuilder = new BRepBuilder(BRepType.Solid);

            // The surfaces of the four faces.
            var basis = new Frame(new XYZ(50, -100, 0), new XYZ(0, 1, 0), new XYZ(-1, 0, 0), new XYZ(0, 0, 1));
            // Note that we do not have to create two identical surfaces here. The same surface can be used for multiple faces, 
            // since BRepBuilderSurfaceGeometry::Create() copies the input surface.
            // Thus, potentially we could have only one surface here, 
            // but we must create at least two faces below to account for periodicity. 
            var frontCylSurf = CylindricalSurface.Create(basis, 50);
            var backCylSurf = CylindricalSurface.Create(basis, 50);
            var top = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1),
                new XYZ(0, 0, 100)); // normal points outside the cylinder
            var bottom =
                Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1), new XYZ(0, 0, 0)); // normal points inside the cylinder
            // Note that the alternating of "inside/outside" matches the alternating of "true/false" in the next block that defines faces. 
            // There must be a correspondence to ensure that all faces are correctly oriented to point out of the solid.

            // Add the four faces
            var frontCylFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(frontCylSurf, null), false);
            var backCylFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(backCylSurf, null), false);
            var topFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(top, null), false);
            var bottomFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(bottom, null), true);

            // Geometry for the four semi-circular edges and two vertical linear edges
            var frontEdgeBottom =
                BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(0, -100, 0), new XYZ(100, -100, 0),
                    new XYZ(50, -50, 0)));
            var backEdgeBottom =
                BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(100, -100, 0), new XYZ(0, -100, 0),
                    new XYZ(50, -150, 0)));

            var frontEdgeTop = BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(0, -100, 100), new XYZ(100, -100, 100),
                new XYZ(50, -50, 100)));
            var backEdgeTop = BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(0, -100, 100), new XYZ(100, -100, 100),
                new XYZ(50, -150, 100)));

            var linearEdgeFront = BRepBuilderEdgeGeometry.Create(new XYZ(100, -100, 0), new XYZ(100, -100, 100));
            var linearEdgeBack = BRepBuilderEdgeGeometry.Create(new XYZ(0, -100, 0), new XYZ(0, -100, 100));

            // Add the six edges
            var frontEdgeBottomId = brepBuilder.AddEdge(frontEdgeBottom);
            var frontEdgeTopId = brepBuilder.AddEdge(frontEdgeTop);
            var linearEdgeFrontId = brepBuilder.AddEdge(linearEdgeFront);
            var linearEdgeBackId = brepBuilder.AddEdge(linearEdgeBack);
            var backEdgeBottomId = brepBuilder.AddEdge(backEdgeBottom);
            var backEdgeTopId = brepBuilder.AddEdge(backEdgeTop);

            // Loops of the four faces
            var loopId_Top = brepBuilder.AddLoop(topFaceId);
            var loopId_Bottom = brepBuilder.AddLoop(bottomFaceId);
            var loopId_Front = brepBuilder.AddLoop(frontCylFaceId);
            var loopId_Back = brepBuilder.AddLoop(backCylFaceId);

            // Add coedges for the loop of the front face
            brepBuilder.AddCoEdge(loopId_Front, linearEdgeBackId, false);
            brepBuilder.AddCoEdge(loopId_Front, frontEdgeTopId, false);
            brepBuilder.AddCoEdge(loopId_Front, linearEdgeFrontId, true);
            brepBuilder.AddCoEdge(loopId_Front, frontEdgeBottomId, true);
            brepBuilder.FinishLoop(loopId_Front);
            brepBuilder.FinishFace(frontCylFaceId);

            // Add coedges for the loop of the back face
            brepBuilder.AddCoEdge(loopId_Back, linearEdgeBackId, true);
            brepBuilder.AddCoEdge(loopId_Back, backEdgeBottomId, true);
            brepBuilder.AddCoEdge(loopId_Back, linearEdgeFrontId, false);
            brepBuilder.AddCoEdge(loopId_Back, backEdgeTopId, true);
            brepBuilder.FinishLoop(loopId_Back);
            brepBuilder.FinishFace(backCylFaceId);

            // Add coedges for the loop of the top face
            brepBuilder.AddCoEdge(loopId_Top, backEdgeTopId, false);
            brepBuilder.AddCoEdge(loopId_Top, frontEdgeTopId, true);
            brepBuilder.FinishLoop(loopId_Top);
            brepBuilder.FinishFace(topFaceId);

            // Add coedges for the loop of the bottom face
            brepBuilder.AddCoEdge(loopId_Bottom, frontEdgeBottomId, false);
            brepBuilder.AddCoEdge(loopId_Bottom, backEdgeBottomId, false);
            brepBuilder.FinishLoop(loopId_Bottom);
            brepBuilder.FinishFace(bottomFaceId);

            brepBuilder.Finish();

            createDirectShapeElementFromBrepBuilderObject(brepBuilder, "Full cylinder");
        }

        private void CreateTruncatedCone()
        {
            var brepBuilder = new BRepBuilder(BRepType.Solid);
            var bottom = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, -1), new XYZ(0, 0, 0));
            var top = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1), new XYZ(0, 0, 50));

            var basis = new Frame(new XYZ(0, 0, 100), new XYZ(0, 1, 0), new XYZ(1, 0, 0), new XYZ(0, 0, -1));

            // Note that we do not have to create two identical surfaces here. The same surface can be used for multiple faces, 
            // since BRepBuilderSurfaceGeometry::Create() copies the input surface.
            // Thus, potentially we could have only one surface here, 
            // but we must create at least two faces below to account for periodicity. 
            var rightConicalSurface = ConicalSurface.Create(basis, Math.Atan(0.5));
            var leftConicalSurface = ConicalSurface.Create(basis, Math.Atan(0.5));

            // Create 4 faces of the cone
            var topFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(top, null), false);
            var bottomFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(bottom, null), false);
            var rightSideFaceId =
                brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(rightConicalSurface, null), false);
            var leftSideFaceId =
                brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(leftConicalSurface, null), false);

            // Create 2 edges at the bottom of the cone
            var bottomRightEdgeGeom =
                BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(-50, 0, 0), new XYZ(50, 0, 0), new XYZ(0, 50, 0)));
            var bottomLeftEdgeGeom =
                BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(50, 0, 0), new XYZ(-50, 0, 0), new XYZ(0, -50, 0)));

            // Create 2 edges at the top of the cone
            var topLeftEdgeGeom =
                BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(-25, 0, 50), new XYZ(25, 0, 50),
                    new XYZ(0, -25, 50)));
            var topRightEdgeGeom =
                BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(25, 0, 50), new XYZ(-25, 0, 50), new XYZ(0, 25, 50)));

            // Create 2 side edges of the cone
            var sideFrontEdgeGeom = BRepBuilderEdgeGeometry.Create(new XYZ(25, 0, 50), new XYZ(50, 0, 0));
            var sideBackEdgeGeom = BRepBuilderEdgeGeometry.Create(new XYZ(-25, 0, 50), new XYZ(-50, 0, 0));

            var bottomRightId = brepBuilder.AddEdge(bottomRightEdgeGeom);
            var bottomLeftId = brepBuilder.AddEdge(bottomLeftEdgeGeom);
            var topRightEdgeId = brepBuilder.AddEdge(topRightEdgeGeom);
            var topLeftEdgeId = brepBuilder.AddEdge(topLeftEdgeGeom);
            var sideFrontEdgeid = brepBuilder.AddEdge(sideFrontEdgeGeom);
            var sideBackEdgeId = brepBuilder.AddEdge(sideBackEdgeGeom);

            // Create bottom face
            var bottomLoopId = brepBuilder.AddLoop(bottomFaceId);
            brepBuilder.AddCoEdge(bottomLoopId, bottomRightId, false);
            brepBuilder.AddCoEdge(bottomLoopId, bottomLeftId, false);
            brepBuilder.FinishLoop(bottomLoopId);
            brepBuilder.FinishFace(bottomFaceId);

            // Create top face
            var topLoopId = brepBuilder.AddLoop(topFaceId);
            brepBuilder.AddCoEdge(topLoopId, topLeftEdgeId, false);
            brepBuilder.AddCoEdge(topLoopId, topRightEdgeId, false);
            brepBuilder.FinishLoop(topLoopId);
            brepBuilder.FinishFace(topFaceId);

            // Create right face
            var rightLoopId = brepBuilder.AddLoop(rightSideFaceId);
            brepBuilder.AddCoEdge(rightLoopId, topRightEdgeId, true);
            brepBuilder.AddCoEdge(rightLoopId, sideFrontEdgeid, false);
            brepBuilder.AddCoEdge(rightLoopId, bottomRightId, true);
            brepBuilder.AddCoEdge(rightLoopId, sideBackEdgeId, true);
            brepBuilder.FinishLoop(rightLoopId);
            brepBuilder.FinishFace(rightSideFaceId);

            // Create left face
            var leftLoopId = brepBuilder.AddLoop(leftSideFaceId);
            brepBuilder.AddCoEdge(leftLoopId, topLeftEdgeId, true);
            brepBuilder.AddCoEdge(leftLoopId, sideBackEdgeId, false);
            brepBuilder.AddCoEdge(leftLoopId, bottomLeftId, true);
            brepBuilder.AddCoEdge(leftLoopId, sideFrontEdgeid, true);
            brepBuilder.FinishLoop(leftLoopId);
            brepBuilder.FinishFace(leftSideFaceId);

            brepBuilder.Finish();
            createDirectShapeElementFromBrepBuilderObject(brepBuilder, "Cone surface");
        }
    }
}
