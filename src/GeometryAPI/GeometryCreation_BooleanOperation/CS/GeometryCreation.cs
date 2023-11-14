// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.GeometryCreation_BooleanOperation.CS
{
    internal class GeometryCreation
    {
        /// <summary>
        ///     The direction of cylinder, only on X, Y, Z three axes forward direction
        /// </summary>
        public enum CylinderDirection
        {
            BasisX,
            BasisY,
            BasisZ
        }

        /// <summary>
        ///     The singleton instance of GeometryCreation
        /// </summary>
        private static GeometryCreation Instance;

        /// <summary>
        ///     revit application
        /// </summary>
        private Application m_app;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="app">Revit application</param>
        private GeometryCreation(Application app)
        {
            m_app = app;
        }

        /// <summary>
        ///     Get the singleton instance of GeometryCreation
        /// </summary>
        /// <param name="app">Revit application</param>
        /// <returns>The singleton instance of GeometryCreation</returns>
        public static GeometryCreation getInstance(Application app)
        {
            return Instance ?? (Instance = new GeometryCreation(app));
        }

        /// <summary>
        ///     Create an extrusion geometry
        /// </summary>
        /// <param name="profileLoops">The profile loops to be extruded</param>
        /// <param name="extrusionDir">The direction in which to extrude the profile loops</param>
        /// <param name="extrusionDist">The positive distance by which the loops are to be extruded</param>
        /// <returns>The created solid</returns>
        private Solid CreateExtrusion(List<CurveLoop> profileLoops, XYZ extrusionDir, double extrusionDist)
        {
            return GeometryCreationUtilities.CreateExtrusionGeometry(profileLoops, extrusionDir, extrusionDist);
        }

        /// <summary>
        ///     Create a revolved geometry
        /// </summary>
        /// <param name="coordinateFrame">A right-handed orthonormal frame of vectors</param>
        /// <param name="profileLoops">The profile loops to be revolved</param>
        /// <param name="startAngle">The start angle for the revolution</param>
        /// <param name="endAngle">The end angle for the revolution</param>
        /// <returns>The created solid</returns>
        private Solid CreateRevolved(Frame coordinateFrame, List<CurveLoop> profileLoops, double startAngle,
            double endAngle)
        {
            return GeometryCreationUtilities.CreateRevolvedGeometry(coordinateFrame, profileLoops, startAngle,
                endAngle);
        }

        /// <summary>
        ///     Create a swept geometry
        /// </summary>
        /// <param name="sweepPath">The sweep path, consisting of a set of contiguous curves</param>
        /// <param name="pathAttachmentCrvIdx">The index of the curve in the sweep path where the profile loops are situated</param>
        /// <param name="pathAttachmentParam">Parameter of the path curve specified by pathAttachmentCrvIdx</param>
        /// <param name="profileLoops">The curve loops defining the planar domain to be swept along the path</param>
        /// <returns>The created solid</returns>
        private Solid CreateSwept(CurveLoop sweepPath, int pathAttachmentCrvIdx, double pathAttachmentParam,
            List<CurveLoop> profileLoops)
        {
            return GeometryCreationUtilities.CreateSweptGeometry(sweepPath, pathAttachmentCrvIdx, pathAttachmentParam,
                profileLoops);
        }

        /// <summary>
        ///     Create a blend geometry
        /// </summary>
        /// <param name="firstLoop">The first curve loop</param>
        /// <param name="secondLoop">The second curve loop</param>
        /// <param name="vertexPairs">This input specifies how the two profile loops should be connected</param>
        /// <returns>The created solid</returns>
        private Solid CreateBlend(CurveLoop firstLoop, CurveLoop secondLoop, List<VertexPair> vertexPairs)
        {
            return GeometryCreationUtilities.CreateBlendGeometry(firstLoop, secondLoop, vertexPairs);
        }

        /// <summary>
        ///     Create a swept and blend geometry
        /// </summary>
        /// <param name="pathCurve">The sweep path</param>
        /// <param name="pathParams">An increasing sequence of parameters along the path curve</param>
        /// <param name="profileLoops">Closed, planar curve loops arrayed along the path</param>
        /// <param name="vertexPairs">This input specifies how adjacent profile loops should be connected</param>
        /// <returns>The created solid</returns>
        private Solid CreateSweptBlend(Curve pathCurve, List<double> pathParams, List<CurveLoop> profileLoops,
            List<ICollection<VertexPair>> vertexPairs)
        {
            return GeometryCreationUtilities.CreateSweptBlendGeometry(pathCurve, pathParams, profileLoops, vertexPairs);
        }

        /// <summary>
        ///     Create a centerbased box
        /// </summary>
        /// <param name="center">The given box center</param>
        /// <param name="edgelength">The given box's edge length</param>
        /// <returns>The created box</returns>
        public Solid CreateCenterbasedBox(XYZ center, double edgelength)
        {
            var halfedgelength = edgelength / 2.0;

            var profileloops = new List<CurveLoop>();
            var profileloop = new CurveLoop();
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

            var extrusiondir = new XYZ(0, 0, 1); // orthogonal

            var extrusiondist = edgelength;

            return GeometryCreationUtilities.CreateExtrusionGeometry(profileloops, extrusiondir, extrusiondist);
        }

        /// <summary>
        ///     Create a centerbased sphere
        /// </summary>
        /// <param name="center">The given sphere center</param>
        /// <param name="radius">The given sphere's radius</param>
        /// <returns>The created sphere</returns>
        public Solid CreateCenterbasedSphere(XYZ center, double radius)
        {
            var frame = new Frame(center,
                XYZ.BasisX,
                XYZ.BasisY,
                XYZ.BasisZ);

            var profileloops = new List<CurveLoop>();
            var profileloop = new CurveLoop();
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

        /// <summary>
        ///     Create a centerbased cylinder, only on X, Y, Z three axes forward direction
        /// </summary>
        /// <param name="center">The given cylinder center</param>
        /// <param name="bottomradius">The given cylinder's bottom radius</param>
        /// <param name="height">The given cylinder's height</param>
        /// <param name="cylinderdirection">Cylinder's extrusion direction</param>
        /// <returns>The created cylinder</returns>
        public Solid CreateCenterbasedCylinder(XYZ center, double bottomradius, double height,
            CylinderDirection cylinderdirection)
        {
            var halfheight = height / 2.0;
            var bottomcenter = new XYZ(
                cylinderdirection == CylinderDirection.BasisX ? center.X - halfheight : center.X,
                cylinderdirection == CylinderDirection.BasisY ? center.Y - halfheight : center.Y,
                cylinderdirection == CylinderDirection.BasisZ ? center.Z - halfheight : center.Z);
            var topcenter = new XYZ(
                cylinderdirection == CylinderDirection.BasisX ? center.X + halfheight : center.X,
                cylinderdirection == CylinderDirection.BasisY ? center.Y + halfheight : center.Y,
                cylinderdirection == CylinderDirection.BasisZ ? center.Z + halfheight : center.Z);

            var sweepPath = new CurveLoop();
            sweepPath.Append(Line.CreateBound(bottomcenter,
                topcenter));

            var profileloops = new List<CurveLoop>();
            var profileloop = new CurveLoop();
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
