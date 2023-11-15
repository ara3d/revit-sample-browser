// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.NewPathReinforcement.CS
{
    /// <summary>
    ///     ProfileWall class contains the information about profile of wall,
    ///     and contains method to create PathReinforcement on wall
    /// </summary>
    public class ProfileWall : Profile
    {
        private readonly Wall m_data;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="wall">wall to create reinforcement on</param>
        /// <param name="commandData">object which contains reference to Revit Application</param>
        public ProfileWall(Wall wall, ExternalCommandData commandData)
            : base(commandData)
        {
            m_data = wall;
            var faces = GetFaces(m_data);
            Points = GetNeedPoints(faces);
            To2DMatrix = GetTo2DMatrix();
        }

        /// <summary>
        ///     Get points of first face
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
            if (xyzs.Count == 2) needFace = faces[0];

            //else we return the face whose normal is Z axis
            foreach (var face in faces)
            foreach (var edge in face)
            {
                var edgexyzs = edge.Tessellate() as List<XYZ>;
                if (xyzs.Count == edgexyzs.Count)
                {
                    var normal = GetFaceNormal(face); //get the normal of face
                    var cross = Vector4.CrossProduct(zAxis, normal);
                    cross.Normalize();
                    if (cross.Length() == 1) needFace = face;
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
        ///     Get a matrix which can transform points to 2D
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

                if (!(curve is Line)) throw new Exception("Path Reinforcement cannot build on this Wall");

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
        ///     create PathReinforcement on wall
        /// </summary>
        /// <param name="points">points used to create PathReinforcement</param>
        /// <param name="flip">used to specify whether new PathReinforcement is Filp</param>
        /// <returns>new created PathReinforcement</returns>
        public override Autodesk.Revit.DB.Structure.PathReinforcement CreatePathReinforcement(List<Vector4> points,
            bool flip)
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
            var pathReinforcementTypeId = PathReinforcementType.CreateDefaultPathReinforcementType(Document);
            var rebarBarTypeId = RebarBarType.CreateDefaultRebarBarType(Document);
            var rebarHookTypeId = RebarHookType.CreateDefaultRebarHookType(Document);
            return Autodesk.Revit.DB.Structure.PathReinforcement.Create(Document, m_data, curves, flip,
                pathReinforcementTypeId, rebarBarTypeId, rebarHookTypeId, rebarHookTypeId);
        }
    }
}
