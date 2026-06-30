// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.BRepBuilderExample.CS
{
    [Transaction(TransactionMode.Manual)]
    public class CreateNurbs : IExternalCommand
    {
        private Document m_dbdocument;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            m_dbdocument = commandData.Application.ActiveUIDocument.Document;

            try
            {
                using Transaction tr = new(m_dbdocument, "CreateNURBS");
                tr.Start();

                var myDirectShape =
                    DirectShape.CreateElement(m_dbdocument, new ElementId(BuiltInCategory.OST_GenericModel));
                myDirectShape.ApplicationId = "TestCreateNURBS";
                myDirectShape.ApplicationDataId = "NURBS";

                if (null != myDirectShape)
                    myDirectShape.SetShape(CreateNurbsSurface());
                tr.Commit();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private BRepBuilder CreateNurbsSurface()
        {
            // Open shell, not a closed solid.
            BRepBuilder brepBuilder = new(BRepType.OpenShell);

            IList<double> knotsU = [0, 0, 0, 0, 0, 1, 1, 1, 1, 1];
            IList<double> knotsV = [0, 0, 0, 0, 0, 1, 1, 1, 1, 1];
            var degreeU = 4;
            var degreeV = 4;

            IList<XYZ> controlPoints =
            [
                new XYZ(0, 0, 0), new XYZ(0, 20, 0), new XYZ(0, 40, 0), new XYZ(0, 60, 0), new XYZ(0, 80, 0),
                new XYZ(20, 0, 100), new XYZ(20, 20, 200), new XYZ(20, 40, 300), new XYZ(20, 60, 200),
                new XYZ(20, 80, 100),
                new XYZ(40, 0, -100), new XYZ(40, 20, -250), new XYZ(40, 40, -300), new XYZ(40, 60, -250),
                new XYZ(40, 80, -100),
                new XYZ(60, 0, 100), new XYZ(60, 20, 200), new XYZ(60, 40, 300), new XYZ(60, 60, 200),
                new XYZ(60, 80, 100),
                new XYZ(80, 0, 0), new XYZ(80, 20, 0), new XYZ(80, 40, 0), new XYZ(80, 60, 0), new XYZ(80, 80, 0)
            ];

            var nurbsSurface = BRepBuilderSurfaceGeometry.CreateNURBSSurface(degreeU, degreeV, knotsU, knotsV,
                controlPoints, false /*bReverseOrientation*/, null /*pSurfaceEnvelope*/);

            IList<double> weights = [1, 1, 1, 1, 1];

            IList<XYZ> backEdgeControlPoints = [new XYZ(0, 0, 0), new XYZ(0, 20, 0), new XYZ(0, 40, 0), new XYZ(0, 60, 0), new XYZ(0, 80, 0)];
            var backNurbs = NurbSpline.CreateCurve(4, knotsU, backEdgeControlPoints, weights);
            var backEdge = BRepBuilderEdgeGeometry.Create(backNurbs);

            IList<XYZ> frontEdgeControlPoints = [new XYZ(80, 0, 0), new XYZ(80, 20, 0), new XYZ(80, 40, 0), new XYZ(80, 60, 0), new XYZ(80, 80, 0)];
            var frontNurbs = NurbSpline.CreateCurve(4, knotsU, frontEdgeControlPoints, weights);
            var frontEdge = BRepBuilderEdgeGeometry.Create(frontNurbs);

            IList<XYZ> leftEdgeControlPoints = [new XYZ(0, 0, 0), new XYZ(20, 0, 100), new XYZ(40, 0, -100), new XYZ(60, 0, 100), new XYZ(80, 0, 0)];
            var leftNurbs = NurbSpline.CreateCurve(4, knotsU, leftEdgeControlPoints, weights);
            var leftEdge = BRepBuilderEdgeGeometry.Create(leftNurbs);

            IList<XYZ> rightEdgeControlPoints =
            [
                new XYZ(0, 80, 0), new XYZ(20, 80, 100), new XYZ(40, 80, -100), new XYZ(60, 80, 100), new XYZ(80, 80, 0)
            ];
            var rightNurbs = NurbSpline.CreateCurve(4, knotsU, rightEdgeControlPoints, weights);
            var rightEdge = BRepBuilderEdgeGeometry.Create(rightNurbs);

            var nurbSplineFaceId = brepBuilder.AddFace(nurbsSurface, false);
            var loopId = brepBuilder.AddLoop(nurbSplineFaceId);

            var backEdgeId = brepBuilder.AddEdge(backEdge);
            var frontEdgeId = brepBuilder.AddEdge(frontEdge);
            var leftEdgeId = brepBuilder.AddEdge(leftEdge);
            var rightEdgeId = brepBuilder.AddEdge(rightEdge);

            brepBuilder.AddCoEdge(loopId, backEdgeId, true);
            brepBuilder.AddCoEdge(loopId, leftEdgeId, false);
            brepBuilder.AddCoEdge(loopId, frontEdgeId, false);
            brepBuilder.AddCoEdge(loopId, rightEdgeId, true);
            brepBuilder.FinishLoop(loopId);
            brepBuilder.FinishFace(nurbSplineFaceId);

            brepBuilder.Finish();
            return brepBuilder;
        }
    }
}
