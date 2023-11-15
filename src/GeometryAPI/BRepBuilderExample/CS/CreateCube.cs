// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.BRepBuilderExample.CS
{
    /// <summary>
    ///     Implement method Execute of this class as an external command for Revit.
    /// </summary>
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
            // create a BRepBuilder; add faces to build a cube

            var brepBuilder = new BRepBuilder(BRepType.Solid);

            // a cube 100x100x100, from (0,0,0) to (100, 100, 100)

            // 1. Planes.
            // naming convention for faces and planes:
            // We are looking at this cube in an isometric view. X is down and to the left of us, Y is horizontal and points to the right, Z is up.
            // front and back faces are along the X axis, left and right are along the Y axis, top and bottom are along the Z axis.
            var bottom =
                Plane.CreateByOriginAndBasis(new XYZ(50, 50, 0), new XYZ(1, 0, 0),
                    new XYZ(0, 1, 0)); // bottom. XY plane, Z = 0, normal pointing inside the cube.
            var top = Plane.CreateByOriginAndBasis(new XYZ(50, 50, 100), new XYZ(1, 0, 0),
                new XYZ(0, 1, 0)); // top. XY plane, Z = 100, normal pointing outside the cube.
            var front = Plane.CreateByOriginAndBasis(new XYZ(100, 50, 50), new XYZ(0, 0, 1),
                new XYZ(0, 1, 0)); // front side. ZY plane, X = 0, normal pointing inside the cube.
            var back = Plane.CreateByOriginAndBasis(new XYZ(0, 50, 50), new XYZ(0, 0, 1),
                new XYZ(0, 1, 0)); // back side. ZY plane, X = 0, normal pointing outside the cube.
            var left = Plane.CreateByOriginAndBasis(new XYZ(50, 0, 50), new XYZ(0, 0, 1),
                new XYZ(1, 0, 0)); // left side. ZX plane, Y = 0, normal pointing inside the cube
            var right = Plane.CreateByOriginAndBasis(new XYZ(50, 100, 50), new XYZ(0, 0, 1),
                new XYZ(1, 0, 0)); // right side. ZX plane, Y = 100, normal pointing outside the cube
            //Note that the alternating of "inside/outside" matches the alternating of "true/false" in the next block that defines faces. 
            //There must be a correspondence to ensure that all faces are correctly oriented to point out of the solid.
            // 2. Faces.
            var faceIdBottom = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(bottom, null), true);
            var faceIdTop = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(top, null), false);
            var faceIdFront = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(front, null), true);
            var faceIdBack = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(back, null), false);
            var faceIdLeft = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(left, null), true);
            var faceIdRight = brepBuilder.AddFace(BRepBuilderSurfaceGeometry.Create(right, null), false);

            // 3. Edges.

            // 3.a (define edge geometry)
            // walk around bottom face
            var edgeBottomFront = BRepBuilderEdgeGeometry.Create(new XYZ(100, 0, 0), new XYZ(100, 100, 0));
            var edgeBottomRight = BRepBuilderEdgeGeometry.Create(new XYZ(100, 100, 0), new XYZ(0, 100, 0));
            var edgeBottomBack = BRepBuilderEdgeGeometry.Create(new XYZ(0, 100, 0), new XYZ(0, 0, 0));
            var edgeBottomLeft = BRepBuilderEdgeGeometry.Create(new XYZ(0, 0, 0), new XYZ(100, 0, 0));

            // now walk around top face
            var edgeTopFront = BRepBuilderEdgeGeometry.Create(new XYZ(100, 0, 100), new XYZ(100, 100, 100));
            var edgeTopRight = BRepBuilderEdgeGeometry.Create(new XYZ(100, 100, 100), new XYZ(0, 100, 100));
            var edgeTopBack = BRepBuilderEdgeGeometry.Create(new XYZ(0, 100, 100), new XYZ(0, 0, 100));
            var edgeTopLeft = BRepBuilderEdgeGeometry.Create(new XYZ(0, 0, 100), new XYZ(100, 0, 100));

            // sides
            var edgeFrontRight = BRepBuilderEdgeGeometry.Create(new XYZ(100, 100, 0), new XYZ(100, 100, 100));
            var edgeRightBack = BRepBuilderEdgeGeometry.Create(new XYZ(0, 100, 0), new XYZ(0, 100, 100));
            var edgeBackLeft = BRepBuilderEdgeGeometry.Create(new XYZ(0, 0, 0), new XYZ(0, 0, 100));
            var edgeLeftFront = BRepBuilderEdgeGeometry.Create(new XYZ(100, 0, 0), new XYZ(100, 0, 100));

            // 3.b (define the edges themselves)
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

            // 4. Loops.
            var loopIdBottom = brepBuilder.AddLoop(faceIdBottom);
            var loopIdTop = brepBuilder.AddLoop(faceIdTop);
            var loopIdFront = brepBuilder.AddLoop(faceIdFront);
            var loopIdBack = brepBuilder.AddLoop(faceIdBack);
            var loopIdRight = brepBuilder.AddLoop(faceIdRight);
            var loopIdLeft = brepBuilder.AddLoop(faceIdLeft);

            // 5. Co-edges. 
            // Bottom face. All edges reversed
            brepBuilder.AddCoEdge(loopIdBottom, edgeIdBottomFront, true); // other direction in front loop
            brepBuilder.AddCoEdge(loopIdBottom, edgeIdBottomLeft, true); // other direction in left loop
            brepBuilder.AddCoEdge(loopIdBottom, edgeIdBottomBack, true); // other direction in back loop
            brepBuilder.AddCoEdge(loopIdBottom, edgeIdBottomRight, true); // other direction in right loop
            brepBuilder.FinishLoop(loopIdBottom);
            brepBuilder.FinishFace(faceIdBottom);

            // Top face. All edges NOT reversed.
            brepBuilder.AddCoEdge(loopIdTop, edgeIdTopFront, false); // other direction in front loop.
            brepBuilder.AddCoEdge(loopIdTop, edgeIdTopRight, false); // other direction in right loop
            brepBuilder.AddCoEdge(loopIdTop, edgeIdTopBack, false); // other direction in back loop
            brepBuilder.AddCoEdge(loopIdTop, edgeIdTopLeft, false); // other direction in left loop
            brepBuilder.FinishLoop(loopIdTop);
            brepBuilder.FinishFace(faceIdTop);

            // Front face.
            brepBuilder.AddCoEdge(loopIdFront, edgeIdBottomFront, false); // other direction in bottom loop
            brepBuilder.AddCoEdge(loopIdFront, edgeIdFrontRight, false); // other direction in right loop
            brepBuilder.AddCoEdge(loopIdFront, edgeIdTopFront, true); // other direction in top loop.
            brepBuilder.AddCoEdge(loopIdFront, edgeIdLeftFront, true); // other direction in left loop.
            brepBuilder.FinishLoop(loopIdFront);
            brepBuilder.FinishFace(faceIdFront);

            // Back face
            brepBuilder.AddCoEdge(loopIdBack, edgeIdBottomBack, false); // other direction in bottom loop
            brepBuilder.AddCoEdge(loopIdBack, edgeIdBackLeft, false); // other direction in left loop.
            brepBuilder.AddCoEdge(loopIdBack, edgeIdTopBack, true); // other direction in top loop
            brepBuilder.AddCoEdge(loopIdBack, edgeIdRightBack, true); // other direction in right loop.
            brepBuilder.FinishLoop(loopIdBack);
            brepBuilder.FinishFace(faceIdBack);

            // Right face
            brepBuilder.AddCoEdge(loopIdRight, edgeIdBottomRight, false); // other direction in bottom loop
            brepBuilder.AddCoEdge(loopIdRight, edgeIdRightBack, false); // other direction in back loop
            brepBuilder.AddCoEdge(loopIdRight, edgeIdTopRight, true); // other direction in top loop
            brepBuilder.AddCoEdge(loopIdRight, edgeIdFrontRight, true); // other direction in front loop
            brepBuilder.FinishLoop(loopIdRight);
            brepBuilder.FinishFace(faceIdRight);

            // Left face
            brepBuilder.AddCoEdge(loopIdLeft, edgeIdBottomLeft, false); // other direction in bottom loop
            brepBuilder.AddCoEdge(loopIdLeft, edgeIdLeftFront, false); // other direction in front loop
            brepBuilder.AddCoEdge(loopIdLeft, edgeIdTopLeft, true); // other direction in top loop
            brepBuilder.AddCoEdge(loopIdLeft, edgeIdBackLeft, true); // other direction in back loop
            brepBuilder.FinishLoop(loopIdLeft);
            brepBuilder.FinishFace(faceIdLeft);

            brepBuilder.Finish();
            return brepBuilder;
        }
    }
}
