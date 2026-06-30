// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Create Various Solids

        public static Solid CreateSphereAt(
            XYZ centre,
            double radius)
        {
            // Use the standard global coordinate system 
            // as a frame, translated to the sphere centre.

            var frame = new Frame(centre, XYZ.BasisX,
                XYZ.BasisY, XYZ.BasisZ);

            // that must be in the frame location.

            var arc = Arc.Create(
                centre - radius * XYZ.BasisZ,
                centre + radius * XYZ.BasisZ,
                centre + radius * XYZ.BasisX);

            var line = Line.CreateBound(
                arc.GetEndPoint(1),
                arc.GetEndPoint(0));

            var halfCircle = new CurveLoop();
            halfCircle.Append(arc);
            halfCircle.Append(line);

            var loops = new List<CurveLoop>(1);
            loops.Add(halfCircle);

            return GeometryCreationUtilities
                .CreateRevolvedGeometry(frame, loops,
                    0, 2 * Math.PI);
        }

        public static Solid CreateCylinder(
            XYZ origin,
            XYZ axis_vector,
            double radius,
            double height)
        {
            var az = axis_vector.Normalize();

            XYZ ax, ay;
            GetArbitraryAxes(az, out ax, out ay);

            // Define a rectangle in XZ plane

            var px = origin + radius * ax;
            var pxz = origin + radius * ax + height * az;
            var pz = origin + height * az;

            var profile = new List<Curve>();

            profile.Add(Line.CreateBound(origin, px));
            profile.Add(Line.CreateBound(px, pxz));
            profile.Add(Line.CreateBound(pxz, pz));
            profile.Add(Line.CreateBound(pz, origin));

            var curveLoop = CurveLoop.Create(profile);

            var frame = new Frame(origin, ax, ay, az);

            var cone = GeometryCreationUtilities
                .CreateRevolvedGeometry(frame,
                    new[] {curveLoop},
                    0, 2 * Math.PI);

            return cone;
        }

        public static Solid CreateCone(
            XYZ center,
            XYZ axis_vector,
            double radius,
            double height)
        {
            var az = axis_vector.Normalize();

            XYZ ax, ay;
            GetArbitraryAxes(az, out ax, out ay);

            // Define a triangle in XZ plane

            var px = center + radius * ax;
            var pz = center + height * az;

            var profile = new List<Curve>();

            profile.Add(Line.CreateBound(center, px));
            profile.Add(Line.CreateBound(px, pz));
            profile.Add(Line.CreateBound(pz, center));

            var curveLoop = CurveLoop.Create(profile);

            var frame = new Frame(center, ax, ay, az);

            //SolidOptions options = new SolidOptions( 
            //  ElementId.InvalidElementId, 
            //  ElementId.InvalidElementId );

            var cone = GeometryCreationUtilities
                .CreateRevolvedGeometry(frame,
                    new[] {curveLoop},
                    0, 2 * Math.PI);

            return cone;

            //using( Transaction t = new Transaction( Command.Doc, "Create cone" ) )
            //{
            //  t.Start();
            //  DirectShape ds = DirectShape.CreateElement( Command.Doc, new ElementId( BuiltInCategory.OST_GenericModel ) );
            //  ds.SetShape( new GeometryObject[] { cone } );
            //  t.Commit();
            //}
        }

        public static Solid CreateArcSolid(Arc arc)
        {
            var p = arc.GetEndPoint(0);
            var q = arc.GetEndPoint(1);
            var r = q - q.Z * XYZ.BasisZ;

            var frame = new Frame(r,
                -XYZ.BasisX, -XYZ.BasisY, XYZ.BasisZ);

            var line2 = Line.CreateBound(q, r);
            var line3 = Line.CreateBound(r, p);

            var loop = new CurveLoop();
            loop.Append(arc);
            loop.Append(line2);
            loop.Append(line3);

            var loops = new List<CurveLoop>(1);
            loops.Add(loop);

            return GeometryCreationUtilities
                .CreateRevolvedGeometry(frame,
                    loops, 0, 2 * Math.PI);
        }

        private static Solid CreateCube(double d)
        {
            return CreateRectangularPrism(
                XYZ.Zero, d, d, d);
        }

        private static Solid CreateRectangularPrism(
            XYZ center,
            double d1,
            double d2,
            double d3)
        {
            var profile = new List<Curve>();
            var profile00 = new XYZ(-d1 / 2, -d2 / 2, -d3 / 2);
            var profile01 = new XYZ(-d1 / 2, d2 / 2, -d3 / 2);
            var profile11 = new XYZ(d1 / 2, d2 / 2, -d3 / 2);
            var profile10 = new XYZ(d1 / 2, -d2 / 2, -d3 / 2);

            profile.Add(Line.CreateBound(profile00, profile01));
            profile.Add(Line.CreateBound(profile01, profile11));
            profile.Add(Line.CreateBound(profile11, profile10));
            profile.Add(Line.CreateBound(profile10, profile00));

            var curveLoop = CurveLoop.Create(profile);

            var options = new SolidOptions(
                ElementId.InvalidElementId,
                ElementId.InvalidElementId);

            return GeometryCreationUtilities
                .CreateExtrusionGeometry(
                    new[] {curveLoop},
                    XYZ.BasisZ, d3, options);
        }

        public static Solid CreateSolidFromBoundingBox(
            Solid inputSolid)
        {
            var bbox = inputSolid.GetBoundingBox();

            // Corners in BBox coords

            var pt0 = new XYZ(bbox.Min.X, bbox.Min.Y, bbox.Min.Z);
            var pt1 = new XYZ(bbox.Max.X, bbox.Min.Y, bbox.Min.Z);
            var pt2 = new XYZ(bbox.Max.X, bbox.Max.Y, bbox.Min.Z);
            var pt3 = new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Min.Z);

            // Edges in BBox coords

            var edge0 = Line.CreateBound(pt0, pt1);
            var edge1 = Line.CreateBound(pt1, pt2);
            var edge2 = Line.CreateBound(pt2, pt3);
            var edge3 = Line.CreateBound(pt3, pt0);

            // Create loop, still in BBox coords

            var edges = new List<Curve>();
            edges.Add(edge0);
            edges.Add(edge1);
            edges.Add(edge2);
            edges.Add(edge3);

            var height = bbox.Max.Z - bbox.Min.Z;

            var baseLoop = CurveLoop.Create(edges);

            var loopList = new List<CurveLoop>();
            loopList.Add(baseLoop);

            var preTransformBox = GeometryCreationUtilities
                .CreateExtrusionGeometry(loopList, XYZ.BasisZ,
                    height);

            var transformBox = SolidUtils.CreateTransformed(
                preTransformBox, bbox.Transform);

            return transformBox;
        }

        #endregion
    }
}
