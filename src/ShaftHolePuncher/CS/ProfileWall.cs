// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.ShaftHolePuncher.CS
{
    /// <summary>
    ///     ProfileWall class contains the information about profile of a wall,
    ///     and contains method to create Opening on a wall
    /// </summary>
    public class ProfileWall : Profile
    {
        private readonly Wall m_data;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="wall">wall to create Opening on</param>
        /// <param name="commandData">object which contains reference of Revit Application</param>
        public ProfileWall(Wall wall, ExternalCommandData commandData)
            : base(commandData)
        {
            m_data = wall;
            var faces = GetFaces(m_data);
            Points = GetNeedPoints(faces);
            To2DMatrix = GetTo2DMatrix();
            MoveToCenterMatrix = ToCenterMatrix();
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
                    //get the normal of face
                    var normal = GetFaceNormal(face);
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

                if (!(curve is Line)) throw new Exception("Opening cannot build on this Wall");

                var start = curve.GetEndPoint(0);
                var end = curve.GetEndPoint(1);

                xAxis = new Vector4((float)(end.X - start.X),
                    (float)(end.Y - start.Y), (float)(end.Z - start.Z));
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
        ///     create Opening on wall
        /// </summary>
        /// <param name="points">points used to create Opening</param>
        /// <returns>newly created Opening</returns>
        public override Opening CreateOpening(List<Vector4> points)
        {
            //create Opening on wall
            var p1 = new XYZ(points[0].X, points[0].Y, points[0].Z);
            var p2 = new XYZ(points[1].X, points[1].Y, points[1].Z);
            return DocCreator.NewOpening(m_data, p1, p2);
        }
    }
}
