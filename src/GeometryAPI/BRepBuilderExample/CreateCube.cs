// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.BRepBuilderExample.CS
{
    [Transaction(TransactionMode.Manual)]
    public class CreateCube : IExternalCommand
    {
        private Document m_dbdocument;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_dbdocument = commandData.Application.ActiveUIDocument.Document;

            try
            {
                var mySolid = CreateCubeImpl().GetResult();
                if (null == mySolid)
                    return Result.Failed;

                using (var tran = new Transaction(m_dbdocument, "CreateCube"))
                {
                    tran.Start();
                    var dsCubed = DirectShape.CreateElement(m_dbdocument, new ElementId(BuiltInCategory.OST_Walls));
                    if (null == dsCubed)
                        return Result.Failed;
                    dsCubed.ApplicationId = "TestCreateCube";
                    dsCubed.ApplicationDataId = "Cube";
                    var shapes = new List<GeometryObject> { mySolid };
                    dsCubed.SetShape(shapes, DirectShapeTargetViewType.Default);

                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private BRepBuilder CreateCubeImpl()
        {
            var brepBuilder = new BRepBuilder(BRepType.Solid);

            // Cube 100x100x100 from (0,0,0) to (100,100,100).
            // Isometric view: X down-left, Y right, Z up; front/back along X, left/right along Y, top/bottom along Z.
            var bottom =
                Plane.CreateByOriginAndBasis(new XYZ(50, 50, 0), new XYZ(1, 0, 0),
                    new XYZ(0, 1, 0));
            var top = Plane.CreateByOriginAndBasis(new XYZ(50, 50, 100), new XYZ(1, 0, 0),
                new XYZ(0, 1, 0));
            var front = Plane.CreateByOriginAndBasis(new XYZ(100, 50, 50), new XYZ(0, 0, 1),
                new XYZ(0, 1, 0));
            var back = Plane.CreateByOriginAndBasis(new XYZ(0, 50, 50), new XYZ(0, 0, 1),
                new XYZ(0, 1, 0));
            var left = Plane.CreateByOriginAndBasis(new XYZ(50, 0, 50), new XYZ(0, 0, 1),
                new XYZ(1, 0, 0));
            var right = Plane.CreateByOriginAndBasis(new XYZ(50, 100, 50), new XYZ(0, 0, 1),
                new XYZ(1, 0, 0));
            // Alternating inside/outside normals must match alternating true/false on AddFace so faces point outward.
            var faceIdBottom = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(bottom, null), true);
            var faceIdTop = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(top, null), false);
            var faceIdFront = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(front, null), true);
            var faceIdBack = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(back, null), false);
            var faceIdLeft = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(left, null), true);
            var faceIdRight = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(right, null), false);

            var edgeBottomFront = BRepBuilderEdgeGeometry.Create(new XYZ(100, 0, 0), new XYZ(100, 100, 0));
            var edgeBottomRight = BRepBuilderEdgeGeometry.Create(new XYZ(100, 100, 0), new XYZ(0, 100, 0));
            var edgeBottomBack = BRepBuilderEdgeGeometry.Create(new XYZ(0, 100, 0), new XYZ(0, 0, 0));
            var edgeBottomLeft = BRepBuilderEdgeGeometry.Create(new XYZ(0, 0, 0), new XYZ(100, 0, 0));

            var edgeTopFront = BRepBuilderEdgeGeometry.Create(new XYZ(100, 0, 100), new XYZ(100, 100, 100));
            var edgeTopRight = BRepBuilderEdgeGeometry.Create(new XYZ(100, 100, 100), new XYZ(0, 100, 100));
            var edgeTopBack = BRepBuilderEdgeGeometry.Create(new XYZ(0, 100, 100), new XYZ(0, 0, 100));
            var edgeTopLeft = BRepBuilderEdgeGeometry.Create(new XYZ(0, 0, 100), new XYZ(100, 0, 100));

            var edgeFrontRight = BRepBuilderEdgeGeometry.Create(new XYZ(100, 100, 0), new XYZ(100, 100, 100));
            var edgeRightBack = BRepBuilderEdgeGeometry.Create(new XYZ(0, 100, 0), new XYZ(0, 100, 100));
            var edgeBackLeft = BRepBuilderEdgeGeometry.Create(new XYZ(0, 0, 0), new XYZ(0, 0, 100));
            var edgeLeftFront = BRepBuilderEdgeGeometry.Create(new XYZ(100, 0, 0), new XYZ(100, 0, 100));

            var edgeIdBottomFront = brepBuilder.AddEdge(edgeBottomFront);
            var edgeIdBottomRight = brepBuilder.AddEdge(edgeBottomRight);
            var edgeIdBottomBack = brepBuilder.AddEdge(edgeBottomBack);
            var edgeIdBottomLeft = brepBuilder.AddEdge(edgeBottomLeft);
            var edgeIdTopFront = brepBuilder.AddEdge(edgeTopFront);
            var edgeIdTopRight = brepBuilder.AddEdge(edgeTopRight);
            var edgeIdTopBack = brepBuilder.AddEdge(edgeTopBack);
            var edgeIdTopLeft = brepBuilder.AddEdge(edgeTopLeft);
            var edgeIdFrontRight = brepBuilder.AddEdge(edgeFrontRight);
            var edgeIdRightBack = brepBuilder.AddEdge(edgeRightBack);
            var edgeIdBackLeft = brepBuilder.AddEdge(edgeBackLeft);
            var edgeIdLeftFront = brepBuilder.AddEdge(edgeLeftFront);

            var loopIdBottom = brepBuilder.AddLoop(faceIdBottom);
            var loopIdTop = brepBuilder.AddLoop(faceIdTop);
            var loopIdFront = brepBuilder.AddLoop(faceIdFront);
            var loopIdBack = brepBuilder.AddLoop(faceIdBack);
            var loopIdRight = brepBuilder.AddLoop(faceIdRight);
            var loopIdLeft = brepBuilder.AddLoop(faceIdLeft);

            brepBuilder.AddCoEdge(loopIdBottom, edgeIdBottomFront, true);
            brepBuilder.AddCoEdge(loopIdBottom, edgeIdBottomLeft, true);
            brepBuilder.AddCoEdge(loopIdBottom, edgeIdBottomBack, true);
            brepBuilder.AddCoEdge(loopIdBottom, edgeIdBottomRight, true);
            brepBuilder.FinishLoop(loopIdBottom);
            brepBuilder.FinishFace(faceIdBottom);

            brepBuilder.AddCoEdge(loopIdTop, edgeIdTopFront, false);
            brepBuilder.AddCoEdge(loopIdTop, edgeIdTopRight, false);
            brepBuilder.AddCoEdge(loopIdTop, edgeIdTopBack, false);
            brepBuilder.AddCoEdge(loopIdTop, edgeIdTopLeft, false);
            brepBuilder.FinishLoop(loopIdTop);
            brepBuilder.FinishFace(faceIdTop);

            brepBuilder.AddCoEdge(loopIdFront, edgeIdBottomFront, false);
            brepBuilder.AddCoEdge(loopIdFront, edgeIdFrontRight, false);
            brepBuilder.AddCoEdge(loopIdFront, edgeIdTopFront, true);
            brepBuilder.AddCoEdge(loopIdFront, edgeIdLeftFront, true);
            brepBuilder.FinishLoop(loopIdFront);
            brepBuilder.FinishFace(faceIdFront);

            brepBuilder.AddCoEdge(loopIdBack, edgeIdBottomBack, false);
            brepBuilder.AddCoEdge(loopIdBack, edgeIdBackLeft, false);
            brepBuilder.AddCoEdge(loopIdBack, edgeIdTopBack, true);
            brepBuilder.AddCoEdge(loopIdBack, edgeIdRightBack, true);
            brepBuilder.FinishLoop(loopIdBack);
            brepBuilder.FinishFace(faceIdBack);

            brepBuilder.AddCoEdge(loopIdRight, edgeIdBottomRight, false);
            brepBuilder.AddCoEdge(loopIdRight, edgeIdRightBack, false);
            brepBuilder.AddCoEdge(loopIdRight, edgeIdTopRight, true);
            brepBuilder.AddCoEdge(loopIdRight, edgeIdFrontRight, true);
            brepBuilder.FinishLoop(loopIdRight);
            brepBuilder.FinishFace(faceIdRight);

            brepBuilder.AddCoEdge(loopIdLeft, edgeIdBottomLeft, false);
            brepBuilder.AddCoEdge(loopIdLeft, edgeIdLeftFront, false);
            brepBuilder.AddCoEdge(loopIdLeft, edgeIdTopLeft, true);
            brepBuilder.AddCoEdge(loopIdLeft, edgeIdBackLeft, true);
            brepBuilder.FinishLoop(loopIdLeft);
            brepBuilder.FinishFace(faceIdLeft);

            brepBuilder.Finish();
            return brepBuilder;
        }
    }
}
