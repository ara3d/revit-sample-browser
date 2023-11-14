//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;

namespace Revit.SDK.Samples.NewPathReinforcement.CS
{
    /// <summary>
    /// ProfileWall class contains the information about profile of wall,
    /// and contains method to create PathReinforcement on wall
    /// </summary>
    public class ProfileWall : Profile
    {
        private Wall m_data;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="wall">wall to create reinforcement on</param>
        /// <param name="commandData">object which contains reference to Revit Application</param>
        public ProfileWall(Wall wall, ExternalCommandData commandData)
            : base(commandData)
        {
            m_data = wall;
            var faces = GetFaces(m_data);
            m_points = GetNeedPoints(faces);
            m_to2DMatrix = GetTo2DMatrix();
        }

        /// <summary>
        /// Get points of first face
        /// </summary>
        /// <param name="faces">edges in all faces</param>
        /// <returns>points of first face</returns>
        public override List<List<XYZ>> GetNeedPoints(List<List<Edge>> faces)
        {
            var needFace = new List<Edge>();
            var needPoints = new List<List<XYZ>>();
            var location = m_data.Location as LocationCurve;
            var curve = location.Curve;
            var xyzs = curve.Tessellate() as List<XYZ>;
            var zAxis = new Vector4(0, 0, 1);

            //if Location curve of wall is line, then return first face
            if (xyzs.Count == 2)
            {
                needFace = faces[0];
            }

            //else we return the face whose normal is Z axis
            foreach (var face in faces)
            {
                foreach (var edge in face)
                {
                    var edgexyzs = edge.Tessellate() as List<XYZ>;
                    if (xyzs.Count == edgexyzs.Count)
                    {
                        var normal = GetFaceNormal(face); //get the normal of face
                        var cross = Vector4.CrossProduct(zAxis, normal);
                        cross.Normalize();
                        if (cross.Length() == 1)
                        {
                            needFace = face;
                        }
                    }
                }
            }
            needFace = faces[0];

            //get points array in edges 
            foreach (var edge in needFace)
            {
                var edgexyzs = edge.Tessellate() as List<XYZ>;
                needPoints.Add(edgexyzs);
            }
            return needPoints;
        }

        /// <summary>
        /// Get a matrix which can transform points to 2D
        /// </summary>
        /// <returns>matrix which can transform points to 2D</returns>
        public override Matrix4 GetTo2DMatrix()
        {
            //get the location curve
            var location = m_data.Location as LocationCurve;
            var xAxis = new Vector4(1, 0, 0);
            var yAxis = new Vector4(0, 1, 0);
            var zAxis = new Vector4(0, 0, 1);
            var origin = new Vector4(0, 0, 0);
            if (location != null)
            {
                var curve = location.Curve;

                if (!(curve is Line))
                {
                    throw new Exception("Path Reinforcement cannot build on this Wall");
                }

                var start = curve.GetEndPoint(0);
                var end = curve.GetEndPoint(1);

                //because we create PathReinforcement on the back of wall
                //so we need make X axis reverse
                xAxis = new Vector4(-(float)(end.X - start.X),
                    -(float)(end.Y - start.Y), -(float)(end.Z - start.Z));
                xAxis.Normalize();

                //because in the windows UI, Y axis is downward
                yAxis = new Vector4(0, 0, -1);

                zAxis = Vector4.CrossProduct(xAxis, yAxis);
                zAxis.Normalize();

                origin = new Vector4((float)(end.X + start.X) / 2,
                    (float)(end.Y + start.Y) / 2, (float)(end.Z + start.Z) / 2);
            }
            return new Matrix4(xAxis, yAxis, zAxis, origin);
        }

        /// <summary>
        /// create PathReinforcement on wall
        /// </summary>
        /// <param name="points">points used to create PathReinforcement</param>
        /// <param name="flip">used to specify whether new PathReinforcement is Filp</param>
        /// <returns>new created PathReinforcement</returns>
        public override Autodesk.Revit.DB.Structure.PathReinforcement CreatePathReinforcement(List<Vector4> points, bool flip)
        {
            IList<Curve> curves = new List<Curve>();
            for (var i = 0; i < points.Count - 1; i++)
            {
                var p1 = new XYZ(points[i].X, points[i].Y, points[i].Z);
                var p2 = new XYZ(points[i + 1].X, points[i + 1].Y, points[i + 1].Z);
                var curve = Line.CreateBound(p1, p2);
                curves.Add(curve);
            }

            //draw PathReinforcement on wall
            var pathReinforcementTypeId = PathReinforcementType.CreateDefaultPathReinforcementType(m_document);
            var rebarBarTypeId = RebarBarType.CreateDefaultRebarBarType(m_document);
            var rebarHookTypeId = RebarHookType.CreateDefaultRebarHookType(m_document);
            return Autodesk.Revit.DB.Structure.PathReinforcement.Create(m_document, m_data, curves, flip, pathReinforcementTypeId, rebarBarTypeId, rebarHookTypeId, rebarHookTypeId);
        }
    }
}
