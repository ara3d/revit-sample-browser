// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.ShaftHolePuncher.CS
{
    /// <summary>
    ///     ProfileWall class contains the information about profile of a wall,
    ///     and contains method to create Opening on a wall
    /// </summary>
    public class ProfileWall : Profile
    {
        private readonly Wall m_data;

        public ProfileWall(Wall wall, ExternalCommandData commandData)
            : base(commandData)
        {
            m_data = wall;
            var faces = GetFaces(m_data);
            Points = GetNeedPoints(faces);
            To2DMatrix = GetTo2DMatrix();
            MoveToCenterMatrix = ToCenterMatrix();
        }

        public override List<List<XYZ>> GetNeedPoints(List<List<Edge>> faces)
        {
            List<Edge> needFace = new();
            List<List<XYZ>> needPoints = new();
            var location = m_data.Location as LocationCurve;
            var curve = location.Curve;
            var xyzs = curve.Tessellate() as List<XYZ>;
            Vector4 zAxis = new(0, 0, 1);

            //if Location curve of wall is line, then return first face
            if (xyzs.Count == 2) needFace = faces[0];

            //else we return the face whose normal is Z axis
            foreach (var face in faces)
            {
                foreach (var edge in face)
                {
                    var edgexyzs = edge.Tessellate() as List<XYZ>;
                    if (xyzs.Count == edgexyzs.Count)
                    {
                        var normal = GetFaceNormal(face);
                        var cross = Vector4.CrossProduct(zAxis, normal);
                        cross.Normalize();
                        if (cross.Length() == 1) needFace = face;
                    }
                }
            }

            needFace = faces[0];

            foreach (var edge in needFace)
            {
                var edgexyzs = edge.Tessellate() as List<XYZ>;
                needPoints.Add(edgexyzs);
            }

            return needPoints;
        }

        public override Matrix4 GetTo2DMatrix()
        {
            Vector4 xAxis = new(1, 0, 0);
            Vector4 yAxis = new(0, 1, 0);
            Vector4 zAxis = new(0, 0, 1);
            Vector4 origin = new(0, 0, 0);
            if (m_data.Location is LocationCurve location)
            {
                var curve = location.Curve;

                if (curve is not Line) throw new Exception("Opening cannot build on this Wall");

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

        public override Opening CreateOpening(List<Vector4> points)
        {
            XYZ p1 = new(points[0].X, points[0].Y, points[0].Z);
            XYZ p2 = new(points[1].X, points[1].Y, points[1].Z);
            return DocCreator.NewOpening(m_data, p1, p2);
        }
    }
}
