// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Drawing;
using Document = Autodesk.Revit.Creation.Document;
using Point = System.Drawing.Point;
namespace Ara3D.RevitSampleBrowser.NewOpenings.CS
{
    /// <summary>
    ///     Base class of ProfileFloor and ProfileWall
    ///     contain the profile information and can make matrix to transform point to 2D plane
    /// </summary>
    public abstract class Profile
    {
        protected readonly Application AppCreator;

        protected readonly ExternalCommandData CommandData;

        protected readonly Element DataProfile;

        protected readonly Document DocCreator;

        protected readonly List<Edge> Face;

        public Profile(Element elem, ExternalCommandData commandData)
        {
            DataProfile = elem;
            CommandData = commandData;
            AppCreator = CommandData.Application.Application.Create;
            DocCreator = CommandData.Application.ActiveUIDocument.Document.Create;

            var faces = GetFaces(DataProfile);
            Face = GetNeedFace(faces);
        }

        public abstract void DrawOpening(List<Vector4> points, ToolType type);

        public void Draw2D(Graphics graphics, Pen pen, Matrix4 matrix4)
        {
            foreach (var edge in Face)
            {
                var points = edge.Tessellate() as List<XYZ>;
                for (var i = 0; i < points.Count - 1; i++)
                {
                    var point1 = points[i];
                    var point2 = points[i + 1];

                    Vector4 v1 = new(point1);
                    Vector4 v2 = new(point2);

                    v1 = matrix4.TransForm(v1);
                    v2 = matrix4.TransForm(v2);
                    graphics.DrawLine(pen, new Point((int)v1.X, (int)v1.Y),
                        new Point((int)v2.X, (int)v2.Y));
                }
            }
        }

        public List<List<Edge>> GetFaces(Element elem)
        {
            List<List<Edge>> faceEdges = new();
            var options = AppCreator.NewGeometryOptions();
            options.DetailLevel = ViewDetailLevel.Medium;
            options.ComputeReferences = true;
            var geoElem = elem.get_Geometry(options);

            //GeometryObjectArray gObjects = geoElem.Objects;
            var objects = geoElem.GetEnumerator();
            //foreach (GeometryObject geo in gObjects)
            while (objects.MoveNext())
            {
                var geo = objects.Current;

                var solid = geo as Solid;
                if (solid != null)
                {
                    var faces = solid.Faces;
                    foreach (Face face in faces)
                    {
                        var edgeArrarr = face.EdgeLoops;
                        foreach (EdgeArray edgeArr in edgeArrarr)
                        {
                            List<Edge> edgesList = new();
                            foreach (Edge edge in edgeArr)
                            {
                                edgesList.Add(edge);
                            }

                            faceEdges.Add(edgesList);
                        }
                    }
                }
            }

            return faceEdges;
        }

        private Vector4 GetFaceNormal(List<Edge> face)
        {
            var eg0 = face[0];
            var eg1 = face[1];

            var points = eg0.Tessellate() as List<XYZ>;
            var start = points[0];
            var end = points[1];

            Vector4 vStart = new((float)start.X, (float)start.Y, (float)start.Z);
            Vector4 vEnd = new((float)end.X, (float)end.Y, (float)end.Z);
            var vSub = vEnd - vStart;

            points = eg1.Tessellate() as List<XYZ>;
            start = points[0];
            end = points[1];

            vStart = new Vector4((float)start.X, (float)start.Y, (float)start.Z);
            vEnd = new Vector4((float)end.X, (float)end.Y, (float)end.Z);
            var vSub2 = vEnd - vStart;

            var result = vSub.CrossProduct(vSub2);
            result.Normalize();
            return result;
        }

        private List<Edge> GetNeedFace(List<List<Edge>> faces)
        {
            return DataProfile is Wall ? GetWallFace(faces) : faces[0];
        }

        public Matrix4 To2DMatrix()
        {
            if (DataProfile is Wall) return WallMatrix();
            var eg0 = Face[0].Tessellate() as List<XYZ>;
            var eg1 = Face[1].Tessellate() as List<XYZ>;

            Vector4 v1 = new((float)eg0[0].X,
                (float)eg0[0].Y, (float)eg0[0].Z);

            Vector4 v2 = new((float)eg0[1].X,
                (float)eg0[1].Y, (float)eg0[1].Z);
            var v21 = v1 - v2;
            v21.Normalize();

            Vector4 v3 = new((float)eg1[0].X,
                (float)eg1[0].Y, (float)eg1[0].Z);

            Vector4 v4 = new((float)eg1[1].X,
                (float)eg1[1].Y, (float)eg1[1].Z);
            var v43 = v4 - v3;
            v43.Normalize();

            var vZAxis = Vector4.CrossProduct(v43, v21);
            var vYAxis = Vector4.CrossProduct(vZAxis, v43);
            vYAxis.Normalize();
            vZAxis.Normalize();
            var vOrigin = (v4 + v1) / 2;

            Matrix4 result = new(v43, vYAxis, vZAxis, vOrigin);
            return result;
        }

        public Matrix4 WallMatrix()
        {
            //get the location curve
            Vector4 xAxis = new(1, 0, 0);
            Vector4 yAxis = new(0, 1, 0);
            Vector4 zAxis = new(0, 0, 1);
            Vector4 origin = new(0, 0, 0);
            if (DataProfile.Location is LocationCurve location)
            {
                var curve = location.Curve;
                var start = curve.GetEndPoint(0);
                var end = curve.GetEndPoint(1);

                xAxis = new Vector4((float)(end.X - start.X),
                    (float)(end.Y - start.Y), (float)(end.Z - start.Z));
                xAxis.Normalize();

                yAxis = new Vector4(0, 0, 1);

                zAxis = Vector4.CrossProduct(xAxis, yAxis);
                zAxis.Normalize();

                origin = new Vector4((float)(end.X + start.X) / 2,
                    (float)(end.Y + start.Y) / 2, (float)(end.Z + start.Z) / 2);
            }

            return new Matrix4(xAxis, yAxis, zAxis, origin);
        }

        private List<Edge> GetWallFace(List<List<Edge>> faces)
        {
            var location = DataProfile.Location as LocationCurve;
            var curve = location.Curve;
            var xyzs = curve.Tessellate() as List<XYZ>;
            Vector4 zAxis = new(0, 0, 1);

            if (xyzs.Count == 2) return faces[0];

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
                        if (cross.Length() == 1) return face;
                    }
                }
            }

            return faces[0];
        }

        public Matrix4 ToCenterMatrix()
        {
            //translate the origin to bound center
            var bounds = GetFaceBounds();
            var min = bounds[0];
            var max = bounds[1];
            PointF center = new((min.X + max.X) / 2, (min.Y + max.Y) / 2);
            return new Matrix4(new Vector4(center.X, center.Y, 0));
        }

        public PointF[] GetFaceBounds()
        {
            var matrix = To2DMatrix();
            var inverseMatrix = matrix.Inverse();
            float minX = 0, maxX = 0, minY = 0, maxY = 0;
            var bFirstPoint = true;
            foreach (var edge in Face)
            {
                var points = edge.Tessellate() as List<XYZ>;

                foreach (var point in points)
                {
                    Vector4 v = new(point);
                    var v1 = inverseMatrix.TransForm(v);

                    if (bFirstPoint)
                    {
                        minX = maxX = v1.X;
                        minY = maxY = v1.Y;
                        bFirstPoint = false;
                    }
                    else
                    {
                        if (v1.X < minX)
                            minX = v1.X;
                        else if (v1.X > maxX) maxX = v1.X;

                        if (v1.Y < minY)
                            minY = v1.Y;
                        else if (v1.Y > maxY) maxY = v1.Y;
                    }
                }
            }

            var resultPoints = new PointF[2]
            {
                new(minX, minY), new(maxX, maxY)
            };
            return resultPoints;
        }
    }
}
