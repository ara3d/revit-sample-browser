// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.BRepBuilderExample.CS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatePeriodic : IExternalCommand
    {
        private Document m_dbdocument;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_dbdocument = commandData.Application.ActiveUIDocument.Document;

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

        private void CreateDirectShapeElementFromBrepBuilderObject(BRepBuilder myBRepBuilder, string name)
        {
            if (!myBRepBuilder.IsResultAvailable())
                return;

            using Transaction tr = new(m_dbdocument, "Create a DirectShape");
            tr.Start();

            var myDirectShape =
                DirectShape.CreateElement(m_dbdocument, new ElementId(BuiltInCategory.OST_GenericModel));
            myDirectShape.ApplicationId = "TestBRepBuilder";
            myDirectShape.ApplicationDataId = name;
            if (null != myDirectShape)
                myDirectShape.SetShape(myBRepBuilder);
            tr.Commit();
        }

        private void CreateCylinder()
        {
            // x down-left, y right, z up.
            BRepBuilder brepBuilder = new(BRepType.Solid);

            Frame basis = new(new XYZ(50, -100, 0), new XYZ(0, 1, 0), new XYZ(-1, 0, 0), new XYZ(0, 0, 1));
            // Periodic surfaces need at least two faces even when the underlying surface is identical
            // (BRepBuilderSurfaceGeometry.Create copies the input surface).
            var frontCylSurf = CylindricalSurface.Create(basis, 50);
            var backCylSurf = CylindricalSurface.Create(basis, 50);
            var top = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1),
                new XYZ(0, 0, 100));
            var bottom =
                Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1), new XYZ(0, 0, 0));
            // Alternating inside/outside normals must match alternating true/false on AddFace.
            var frontCylFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(frontCylSurf, null), false);
            var backCylFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(backCylSurf, null), false);
            var topFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(top, null), false);
            var bottomFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(bottom, null), true);

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

            var frontEdgeBottomId = brepBuilder.AddEdge(frontEdgeBottom);
            var frontEdgeTopId = brepBuilder.AddEdge(frontEdgeTop);
            var linearEdgeFrontId = brepBuilder.AddEdge(linearEdgeFront);
            var linearEdgeBackId = brepBuilder.AddEdge(linearEdgeBack);
            var backEdgeBottomId = brepBuilder.AddEdge(backEdgeBottom);
            var backEdgeTopId = brepBuilder.AddEdge(backEdgeTop);

            var loopIdTop = brepBuilder.AddLoop(topFaceId);
            var loopIdBottom = brepBuilder.AddLoop(bottomFaceId);
            var loopIdFront = brepBuilder.AddLoop(frontCylFaceId);
            var loopIdBack = brepBuilder.AddLoop(backCylFaceId);

            brepBuilder.AddCoEdge(loopIdFront, linearEdgeBackId, false);
            brepBuilder.AddCoEdge(loopIdFront, frontEdgeTopId, false);
            brepBuilder.AddCoEdge(loopIdFront, linearEdgeFrontId, true);
            brepBuilder.AddCoEdge(loopIdFront, frontEdgeBottomId, true);
            brepBuilder.FinishLoop(loopIdFront);
            brepBuilder.FinishFace(frontCylFaceId);

            brepBuilder.AddCoEdge(loopIdBack, linearEdgeBackId, true);
            brepBuilder.AddCoEdge(loopIdBack, backEdgeBottomId, true);
            brepBuilder.AddCoEdge(loopIdBack, linearEdgeFrontId, false);
            brepBuilder.AddCoEdge(loopIdBack, backEdgeTopId, true);
            brepBuilder.FinishLoop(loopIdBack);
            brepBuilder.FinishFace(backCylFaceId);

            brepBuilder.AddCoEdge(loopIdTop, backEdgeTopId, false);
            brepBuilder.AddCoEdge(loopIdTop, frontEdgeTopId, true);
            brepBuilder.FinishLoop(loopIdTop);
            brepBuilder.FinishFace(topFaceId);

            brepBuilder.AddCoEdge(loopIdBottom, frontEdgeBottomId, false);
            brepBuilder.AddCoEdge(loopIdBottom, backEdgeBottomId, false);
            brepBuilder.FinishLoop(loopIdBottom);
            brepBuilder.FinishFace(bottomFaceId);

            brepBuilder.Finish();

            CreateDirectShapeElementFromBrepBuilderObject(brepBuilder, "Full cylinder");
        }

        private void CreateTruncatedCone()
        {
            BRepBuilder brepBuilder = new(BRepType.Solid);
            var bottom = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, -1), new XYZ(0, 0, 0));
            var top = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, 1), new XYZ(0, 0, 50));

            Frame basis = new(new XYZ(0, 0, 100), new XYZ(0, 1, 0), new XYZ(1, 0, 0), new XYZ(0, 0, -1));

            // Periodic surfaces need at least two faces even when the underlying surface is identical.
            var rightConicalSurface = ConicalSurface.Create(basis, Math.Atan(0.5));
            var leftConicalSurface = ConicalSurface.Create(basis, Math.Atan(0.5));

            var topFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(top, null), false);
            var bottomFaceId = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(bottom, null), false);
            var rightSideFaceId =
                brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(rightConicalSurface, null), false);
            var leftSideFaceId =
                brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(leftConicalSurface, null), false);

            var bottomRightEdgeGeom =
                BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(-50, 0, 0), new XYZ(50, 0, 0), new XYZ(0, 50, 0)));
            var bottomLeftEdgeGeom =
                BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(50, 0, 0), new XYZ(-50, 0, 0), new XYZ(0, -50, 0)));

            var topLeftEdgeGeom =
                BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(-25, 0, 50), new XYZ(25, 0, 50),
                    new XYZ(0, -25, 50)));
            var topRightEdgeGeom =
                BRepBuilderEdgeGeometry.Create(Arc.Create(new XYZ(25, 0, 50), new XYZ(-25, 0, 50), new XYZ(0, 25, 50)));

            var sideFrontEdgeGeom = BRepBuilderEdgeGeometry.Create(new XYZ(25, 0, 50), new XYZ(50, 0, 0));
            var sideBackEdgeGeom = BRepBuilderEdgeGeometry.Create(new XYZ(-25, 0, 50), new XYZ(-50, 0, 0));

            var bottomRightId = brepBuilder.AddEdge(bottomRightEdgeGeom);
            var bottomLeftId = brepBuilder.AddEdge(bottomLeftEdgeGeom);
            var topRightEdgeId = brepBuilder.AddEdge(topRightEdgeGeom);
            var topLeftEdgeId = brepBuilder.AddEdge(topLeftEdgeGeom);
            var sideFrontEdgeid = brepBuilder.AddEdge(sideFrontEdgeGeom);
            var sideBackEdgeId = brepBuilder.AddEdge(sideBackEdgeGeom);

            var bottomLoopId = brepBuilder.AddLoop(bottomFaceId);
            brepBuilder.AddCoEdge(bottomLoopId, bottomRightId, false);
            brepBuilder.AddCoEdge(bottomLoopId, bottomLeftId, false);
            brepBuilder.FinishLoop(bottomLoopId);
            brepBuilder.FinishFace(bottomFaceId);

            var topLoopId = brepBuilder.AddLoop(topFaceId);
            brepBuilder.AddCoEdge(topLoopId, topLeftEdgeId, false);
            brepBuilder.AddCoEdge(topLoopId, topRightEdgeId, false);
            brepBuilder.FinishLoop(topLoopId);
            brepBuilder.FinishFace(topFaceId);

            var rightLoopId = brepBuilder.AddLoop(rightSideFaceId);
            brepBuilder.AddCoEdge(rightLoopId, topRightEdgeId, true);
            brepBuilder.AddCoEdge(rightLoopId, sideFrontEdgeid, false);
            brepBuilder.AddCoEdge(rightLoopId, bottomRightId, true);
            brepBuilder.AddCoEdge(rightLoopId, sideBackEdgeId, true);
            brepBuilder.FinishLoop(rightLoopId);
            brepBuilder.FinishFace(rightSideFaceId);

            var leftLoopId = brepBuilder.AddLoop(leftSideFaceId);
            brepBuilder.AddCoEdge(leftLoopId, topLeftEdgeId, true);
            brepBuilder.AddCoEdge(leftLoopId, sideBackEdgeId, false);
            brepBuilder.AddCoEdge(leftLoopId, bottomLeftId, true);
            brepBuilder.AddCoEdge(leftLoopId, sideFrontEdgeid, true);
            brepBuilder.FinishLoop(leftLoopId);
            brepBuilder.FinishFace(leftSideFaceId);

            brepBuilder.Finish();
            CreateDirectShapeElementFromBrepBuilderObject(brepBuilder, "Cone surface");
        }
    }
}
