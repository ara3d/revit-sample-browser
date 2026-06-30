// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
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
                        var normal = GetFaceNormal(face); //get the normal of face
                        var cross = Vector4.CrossProduct(zAxis, normal);
                        cross.Normalize();
                        if (cross.Length() == 1) needFace = face;
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

        public override Matrix4 GetTo2DMatrix()
        {
            //get the location curve
            Vector4 xAxis = new(1, 0, 0);
            Vector4 yAxis = new(0, 1, 0);
            Vector4 zAxis = new(0, 0, 1);
            Vector4 origin = new(0, 0, 0);
            if (m_data.Location is LocationCurve location)
            {
                var curve = location.Curve;

                if (curve is not Line) throw new Exception("Path Reinforcement cannot build on this Wall");

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

        public override Autodesk.Revit.DB.Structure.PathReinforcement CreatePathReinforcement(List<Vector4> points,
            bool flip)
        {
            IList<Curve> curves = [];
            for (var i = 0; i < points.Count - 1; i++)
            {
                XYZ p1 = new(points[i].X, points[i].Y, points[i].Z);
                XYZ p2 = new(points[i + 1].X, points[i + 1].Y, points[i + 1].Z);
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
