// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
        /// <summary>
        ///     Application creator
        /// </summary>
        protected readonly Application AppCreator;

        /// <summary>
        ///     command data
        /// </summary>
        protected readonly ExternalCommandData CommandData;

        /// <summary>
        ///     Wall or Floor element
        /// </summary>
        protected readonly Element DataProfile;

        /// <summary>
        ///     Document creator
        /// </summary>
        protected readonly Document DocCreator;

        /// <summary>
        ///     geometry object [face]
        /// </summary>
        protected readonly List<Edge> Face;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="elem">Selected element</param>
        /// <param name="commandData">ExternalCommandData</param>
        public Profile(Element elem, ExternalCommandData commandData)
        {
            DataProfile = elem;
            CommandData = commandData;
            AppCreator = CommandData.Application.Application.Create;
            DocCreator = CommandData.Application.ActiveUIDocument.Document.Create;

            var faces = GetFaces(DataProfile);
            Face = GetNeedFace(faces);
        }

        /// <summary>
        ///     Abstract method to create Opening
        /// </summary>
        public abstract void DrawOpening(List<Vector4> points, ToolType type);

        /// <summary>
        ///     Draw profile of wall or floor in 2D
        /// </summary>
        /// <param name="graphics">form graphic</param>
        /// <param name="pen">pen use to draw line in pictureBox</param>
        /// <param name="matrix4">matrix used to transform points between 3d and 2d.</param>
        /// >
        public void Draw2D(Graphics graphics, Pen pen, Matrix4 matrix4)
        {
            foreach (var edge in Face)
            {
                var points = edge.Tessellate() as List<XYZ>;
                for (var i = 0; i < points.Count - 1; i++)
                {
                    var point1 = points[i];
                    var point2 = points[i + 1];

                    var v1 = new Vector4(point1);
                    var v2 = new Vector4(point2);

                    v1 = matrix4.TransForm(v1);
                    v2 = matrix4.TransForm(v2);
                    graphics.DrawLine(pen, new Point((int)v1.X, (int)v1.Y),
                        new Point((int)v2.X, (int)v2.Y));
                }
            }
        }

        /// <summary>
        ///     Get edges of element's profile
        /// </summary>
        /// <param name="elem">Selected element</param>
        public List<List<Edge>> GetFaces(Element elem)
        {
            var faceEdges = new List<List<Edge>>();
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
                            var edgesList = new List<Edge>();
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

        /// <summary>
        ///     Get Face Normal
        /// </summary>
        /// <param name="face">Edges in a face</param>
        private Vector4 GetFaceNormal(List<Edge> face)
        {
            var eg0 = face[0];
            var eg1 = face[1];

            var points = eg0.Tessellate() as List<XYZ>;
            var start = points[0];
            var end = points[1];

            var vStart = new Vector4((float)start.X, (float)start.Y, (float)start.Z);
            var vEnd = new Vector4((float)end.X, (float)end.Y, (float)end.Z);
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

        /// <summary>
        ///     Get First Face
        /// </summary>
        /// <param name="faces">edges in all faces</param>
        private List<Edge> GetNeedFace(List<List<Edge>> faces)
        {
            return DataProfile is Wall ? GetWallFace(faces) : faces[0];
        }

        /// <summary>
        ///     Get a matrix which can transform points to 2D
        /// </summary>
        public Matrix4 To2DMatrix()
        {
            if (DataProfile is Wall) return WallMatrix();
            var eg0 = Face[0].Tessellate() as List<XYZ>;
            var eg1 = Face[1].Tessellate() as List<XYZ>;

            var v1 = new Vector4((float)eg0[0].X,
                (float)eg0[0].Y, (float)eg0[0].Z);

            var v2 = new Vector4((float)eg0[1].X,
                (float)eg0[1].Y, (float)eg0[1].Z);
            var v21 = v1 - v2;
            v21.Normalize();

            var v3 = new Vector4((float)eg1[0].X,
                (float)eg1[0].Y, (float)eg1[0].Z);

            var v4 = new Vector4((float)eg1[1].X,
                (float)eg1[1].Y, (float)eg1[1].Z);
            var v43 = v4 - v3;
            v43.Normalize();

            var vZAxis = Vector4.CrossProduct(v43, v21);
            var vYAxis = Vector4.CrossProduct(vZAxis, v43);
            vYAxis.Normalize();
            vZAxis.Normalize();
            var vOrigin = (v4 + v1) / 2;

            var result = new Matrix4(v43, vYAxis, vZAxis, vOrigin);
            return result;
        }

        /// <summary>
        ///     Wall matrix
        /// </summary>
        /// <returns></returns>
        public Matrix4 WallMatrix()
        {
            //get the location curve
            var location = DataProfile.Location as LocationCurve;
            var xAxis = new Vector4(1, 0, 0);
            var yAxis = new Vector4(0, 1, 0);
            var zAxis = new Vector4(0, 0, 1);
            var origin = new Vector4(0, 0, 0);
            if (location != null)
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

        /// <summary>
        ///     Get wall face
        /// </summary>
        /// <param name="faces"></param>
        /// <returns></returns>
        private List<Edge> GetWallFace(List<List<Edge>> faces)
        {
            var location = DataProfile.Location as LocationCurve;
            var curve = location.Curve;
            var xyzs = curve.Tessellate() as List<XYZ>;
            var zAxis = new Vector4(0, 0, 1);

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

        /// <summary>
        ///     Get a matrix which can move points to origin
        /// </summary>
        public Matrix4 ToCenterMatrix()
        {
            //translate the origin to bound center
            var bounds = GetFaceBounds();
            var min = bounds[0];
            var max = bounds[1];
            var center = new PointF((min.X + max.X) / 2, (min.Y + max.Y) / 2);
            return new Matrix4(new Vector4(center.X, center.Y, 0));
        }

        /// <summary>
        ///     Get Face Bounds
        /// </summary>
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
                    var v = new Vector4(point);
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
                new PointF(minX, minY), new PointF(maxX, maxY)
            };
            return resultPoints;
        }
    }
}
