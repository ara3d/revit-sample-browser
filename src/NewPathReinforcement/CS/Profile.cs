// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.DB.Document;
using Point = System.Drawing.Point;

namespace Ara3D.RevitSampleBrowser.NewPathReinforcement.CS
{
    /// <summary>
    ///     base class of ProfileFloor and ProfileWall
    ///     contains the profile information and can make matrix to transform point to 2D plane
    /// </summary>
    public abstract class Profile
    {
        /// <summary>
        ///     used to create new instances of utility objects.
        /// </summary>
        protected readonly Application AppCreator;

        /// <summary>
        ///     Revit DB document
        /// </summary>
        protected readonly Document Document;

        /// <summary>
        ///     store all the points on the needed face
        /// </summary>
        protected List<List<XYZ>> Points;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="commandData">object which contains reference to Revit Application</param>
        protected Profile(ExternalCommandData commandData)
        {
            CommandData = commandData;
            AppCreator = CommandData.Application.Application.Create;
            Document = CommandData.Application.ActiveUIDocument.Document;
        }

        /// <summary>
        ///     CommandData property get object which contains reference to Revit Application
        /// </summary>
        public ExternalCommandData CommandData { get; }

        /// <summary>
        ///     To2DMatrix property to get Matrix used to transform 3D points to 2D
        /// </summary>
        public Matrix4 To2DMatrix { get; protected set; }

        /// <summary>
        ///     abstract method to create PathReinforcement
        /// </summary>
        /// <returns>new created PathReinforcement</returns>
        /// <param name="points">points used to create PathReinforcement</param>
        /// <param name="flip">used to specify whether new PathReinforcement is Filp</param>
        public abstract Autodesk.Revit.DB.Structure.PathReinforcement CreatePathReinforcement(List<Vector4> points,
            bool flip);

        /// <summary>
        ///     Get points in first face
        /// </summary>
        /// <param name="faces">edges in all faces</param>
        /// <returns>points in first face</returns>
        public abstract List<List<XYZ>> GetNeedPoints(List<List<Edge>> faces);

        /// <summary>
        ///     Get a matrix which can transform points to 2D
        /// </summary>
        public abstract Matrix4 GetTo2DMatrix();

        /// <summary>
        ///     draw profile of wall or floor in 2D
        /// </summary>
        /// <param name="graphics">form graphic</param>
        /// <param name="pen">pen used to draw line in pictureBox</param>
        /// <param name="matrix4">
        ///     Matrix used to transform 3d to 2d
        ///     and make picture in right scale
        /// </param>
        public void Draw2D(Graphics graphics, Pen pen, Matrix4 matrix4)
        {
            foreach (var points in Points)
            {
                for (var j = 0; j < points.Count - 1; j++)
                {
                    var point1 = points[j];
                    var point2 = points[j + 1];

                    var v1 = new Vector4(point1);
                    var v2 = new Vector4(point2);

                    v1 = matrix4.Transform(v1);
                    v2 = matrix4.Transform(v2);
                    graphics.DrawLine(pen, new Point((int)v1.X, (int)v1.Y),
                        new Point((int)v2.X, (int)v2.Y));
                }
            }
        }

        /// <summary>
        ///     Get edges of element's profile
        /// </summary>
        /// <param name="elem">selected element</param>
        /// <returns>all the faces in the selected Element</returns>
        public List<List<Edge>> GetFaces(Element elem)
        {
            var faceEdges = new List<List<Edge>>();
            var options = AppCreator.NewGeometryOptions();
            options.DetailLevel = ViewDetailLevel.Medium;
            options.ComputeReferences = true; //make sure references to geometric objects are computed.
            var geoElem = elem.get_Geometry(options);
            //GeometryObjectArray gObjects = geoElem.Objects;
            var objects = geoElem.GetEnumerator();
            //get all the edges in the Geometry object
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
                            foreach (Edge edge in edgeArr) edgesList.Add(edge);
                            faceEdges.Add(edgesList);
                        }
                    }
                }
            }

            return faceEdges;
        }

        /// <summary>
        ///     Get normal of face
        /// </summary>
        /// <param name="face">edges in a face</param>
        /// <returns>vector stands for normal of the face</returns>
        public Vector4 GetFaceNormal(List<Edge> face)
        {
            var eg0 = face[0];
            var eg1 = face[1];

            //get two lines from the face
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

            //get the normal with two lines got from face
            var result = vSub.CrossProduct(vSub2);
            result.Normalize();
            return result;
        }

        /// <summary>
        ///     Get a matrix which can move points to center
        /// </summary>
        /// <returns>matrix used to move point to center of graphics</returns>
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
        ///     Get the bound of a face
        /// </summary>
        /// <returns>points array store the bound of the face</returns>
        public PointF[] GetFaceBounds()
        {
            var matrix = To2DMatrix;
            var inverseMatrix = matrix.Inverse();
            float minX = 0, maxX = 0, minY = 0, maxY = 0;
            var bFirstPoint = true;

            //get the max and min point on the face
            foreach (var points in Points)
            {
                foreach (var point in points)
                {
                    var v = new Vector4(point);
                    var v1 = inverseMatrix.Transform(v);

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

            //return an array with max and min value of face
            var resultPoints = new PointF[2]
            {
                new PointF(minX, minY), new PointF(maxX, maxY)
            };
            return resultPoints;
        }
    }
}
