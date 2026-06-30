// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.GeometryCreation_BooleanOperation.CS
{
    public class GeometryCreation
    {
        /// <summary>Cylinder extrusion is limited to world X, Y, or Z.</summary>
        public enum CylinderDirection
        {
            BasisX,
            BasisY,
            BasisZ
        }

        private static GeometryCreation _instance;
        private readonly Application m_app;

        private GeometryCreation(Application app)
        {
            m_app = app;
        }

        public static GeometryCreation GetInstance(Application app)
        {
            return _instance ??= new GeometryCreation(app);
        }

        private Solid CreateExtrusion(List<CurveLoop> profileLoops, XYZ extrusionDir, double extrusionDist)
        {
            return GeometryCreationUtilities.CreateExtrusionGeometry(profileLoops, extrusionDir, extrusionDist);
        }

        private Solid CreateRevolved(Frame coordinateFrame, List<CurveLoop> profileLoops, double startAngle,
            double endAngle)
        {
            return GeometryCreationUtilities.CreateRevolvedGeometry(coordinateFrame, profileLoops, startAngle,
                endAngle);
        }

        private Solid CreateSwept(CurveLoop sweepPath, int pathAttachmentCrvIdx, double pathAttachmentParam,
            List<CurveLoop> profileLoops)
        {
            return GeometryCreationUtilities.CreateSweptGeometry(sweepPath, pathAttachmentCrvIdx, pathAttachmentParam,
                profileLoops);
        }

        private Solid CreateBlend(CurveLoop firstLoop, CurveLoop secondLoop, List<VertexPair> vertexPairs)
        {
            return GeometryCreationUtilities.CreateBlendGeometry(firstLoop, secondLoop, vertexPairs);
        }

        private Solid CreateSweptBlend(Curve pathCurve, List<double> pathParams, List<CurveLoop> profileLoops,
            List<ICollection<VertexPair>> vertexPairs)
        {
            return GeometryCreationUtilities.CreateSweptBlendGeometry(pathCurve, pathParams, profileLoops, vertexPairs);
        }

        public Solid CreateCenterbasedBox(XYZ center, double edgelength)
        {
            var halfedgelength = edgelength / 2.0;

            List<CurveLoop> profileloops = new();
            CurveLoop profileloop = new();
            profileloop.Append(Line.CreateBound(
                new XYZ(center.X - halfedgelength, center.Y - halfedgelength, center.Z - halfedgelength),
                new XYZ(center.X - halfedgelength, center.Y + halfedgelength, center.Z - halfedgelength)));
            profileloop.Append(Line.CreateBound(
                new XYZ(center.X - halfedgelength, center.Y + halfedgelength, center.Z - halfedgelength),
                new XYZ(center.X + halfedgelength, center.Y + halfedgelength, center.Z - halfedgelength)));
            profileloop.Append(Line.CreateBound(
                new XYZ(center.X + halfedgelength, center.Y + halfedgelength, center.Z - halfedgelength),
                new XYZ(center.X + halfedgelength, center.Y - halfedgelength, center.Z - halfedgelength)));
            profileloop.Append(Line.CreateBound(
                new XYZ(center.X + halfedgelength, center.Y - halfedgelength, center.Z - halfedgelength),
                new XYZ(center.X - halfedgelength, center.Y - halfedgelength, center.Z - halfedgelength)));
            profileloops.Add(profileloop);

            XYZ extrusiondir = new(0, 0, 1);

            var extrusiondist = edgelength;

            return GeometryCreationUtilities.CreateExtrusionGeometry(profileloops, extrusiondir, extrusiondist);
        }

        public Solid CreateCenterbasedSphere(XYZ center, double radius)
        {
            Frame frame = new(center,
                XYZ.BasisX,
                XYZ.BasisY,
                XYZ.BasisZ);

            List<CurveLoop> profileloops = new();
            CurveLoop profileloop = new();
            var cemiEllipse = Ellipse.CreateCurve(center, radius, radius,
                XYZ.BasisX,
                XYZ.BasisZ,
                -Math.PI / 2.0, Math.PI / 2.0);
            profileloop.Append(cemiEllipse);
            profileloop.Append(Line.CreateBound(
                new XYZ(center.X, center.Y, center.Z + radius),
                new XYZ(center.X, center.Y, center.Z - radius)));
            profileloops.Add(profileloop);

            return GeometryCreationUtilities.CreateRevolvedGeometry(frame, profileloops, -Math.PI, Math.PI);
        }

        public Solid CreateCenterbasedCylinder(XYZ center, double bottomradius, double height,
            CylinderDirection cylinderdirection)
        {
            var halfheight = height / 2.0;
            XYZ bottomcenter = new(
                cylinderdirection == CylinderDirection.BasisX ? center.X - halfheight : center.X,
                cylinderdirection == CylinderDirection.BasisY ? center.Y - halfheight : center.Y,
                cylinderdirection == CylinderDirection.BasisZ ? center.Z - halfheight : center.Z);
            XYZ topcenter = new(
                cylinderdirection == CylinderDirection.BasisX ? center.X + halfheight : center.X,
                cylinderdirection == CylinderDirection.BasisY ? center.Y + halfheight : center.Y,
                cylinderdirection == CylinderDirection.BasisZ ? center.Z + halfheight : center.Z);

            CurveLoop sweepPath = new();
            sweepPath.Append(Line.CreateBound(bottomcenter,
                topcenter));

            List<CurveLoop> profileloops = new();
            CurveLoop profileloop = new();
            var cemiEllipse1 = Ellipse.CreateCurve(bottomcenter, bottomradius, bottomradius,
                cylinderdirection == CylinderDirection.BasisX ? XYZ.BasisY : XYZ.BasisX,
                cylinderdirection == CylinderDirection.BasisZ ? XYZ.BasisY : XYZ.BasisZ,
                -Math.PI, 0);
            var cemiEllipse2 = Ellipse.CreateCurve(bottomcenter, bottomradius, bottomradius,
                cylinderdirection == CylinderDirection.BasisX ? XYZ.BasisY : XYZ.BasisX,
                cylinderdirection == CylinderDirection.BasisZ ? XYZ.BasisY : XYZ.BasisZ,
                0, Math.PI);
            profileloop.Append(cemiEllipse1);
            profileloop.Append(cemiEllipse2);
            profileloops.Add(profileloop);

            return GeometryCreationUtilities.CreateSweptGeometry(sweepPath, 0, 0, profileloops);
        }
    }
}
