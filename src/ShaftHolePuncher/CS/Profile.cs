// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Document = Autodesk.Revit.Creation.Document;
using Point = System.Drawing.Point;

namespace Revit.SDK.Samples.ShaftHolePuncher.CS
{
    /// <summary>
    ///     base class of ProfileFloor, ProfileWall and ProfileNull.
    ///     contains the profile information and can calculate matrix to transform point to 2D plane
    /// </summary>
    public abstract class Profile
    {
        // used to create new instances of utility objects. 
        protected Application m_appCreator;

        // object which contains reference to Revit Application
        protected ExternalCommandData m_commandData;

        // used to create new instances of elements
        protected Document m_docCreator;

        // store the Matrix used to move points to center
        protected Matrix4 m_moveToCenterMatrix = null;

        // store all the points on the needed face
        protected List<List<XYZ>> m_points;

        // store the Matrix used to transform window UI coordinate to Revit
        protected Matrix4 m_restoreMatrix = null;

        // store the Matrix used to scale profile fit to pictureBox
        protected Matrix4 m_scaleMatrix;

        // store the size of pictureBox in UI
        protected Size m_sizePictureBox;

        // store the Matrix used to transform 3D points to 2D
        protected Matrix4 m_to2DMatrix = null;

        // store the Matrix used to transform Revit coordinate to window UI
        protected Matrix4 m_transformMatrix;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="commandData">object which contains reference to Revit Application</param>
        protected Profile(ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_appCreator = m_commandData.Application.Application.Create;
            m_docCreator = m_commandData.Application.ActiveUIDocument.Document.Create;
        }

        /// <summary>
        ///     abstract method to create Opening
        /// </summary>
        /// <returns>newly created Opening</returns>
        /// <param name="points">points used to create Opening</param>
        public abstract Opening CreateOpening(List<Vector4> points);

        /// <summary>
        ///     Get points in first face
        /// </summary>
        /// <param name="faces">edges in all faces</param>
        /// <returns>points in first face</returns>
        public virtual List<List<XYZ>> GetNeedPoints(List<List<Edge>> faces)
        {
            return null;
        }

        /// <summary>
        ///     Get a matrix which can transform points to 2D
        /// </summary>
        public virtual Matrix4 GetTo2DMatrix()
        {
            return null;
        }

        /// <summary>
        ///     draw profile of wall or floor in 2D
        /// </summary>
        /// <param name="graphics">form graphic</param>
        /// <param name="pen">pen used to draw line in pictureBox</param>
        /// <param name="matrix4">
        ///     Matrix used to transform 3d to 2d
        ///     and make picture in right scale
        /// </param>
        public virtual void Draw2D(Graphics graphics, Pen pen, Matrix4 matrix4)
        {
            //move the gdi origin to the picture center
            graphics.Transform = new Matrix(
                1, 0, 0, 1, m_sizePictureBox.Width / 2, m_sizePictureBox.Height / 2);

            //draw profile
            for (var i = 0; i < m_points.Count; i++)
            {
                var points = m_points[i];
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
        public virtual List<List<Edge>> GetFaces(Element elem)
        {
            var faceEdges = new List<List<Edge>>();
            var options = m_appCreator.NewGeometryOptions();
            options.DetailLevel = ViewDetailLevel.Medium;
            //make sure references to geometric objects are computed.
            options.ComputeReferences = true;
            var geoElem = elem.get_Geometry(options);
            //GeometryObjectArray gObjects = geoElem.Objects;
            var Objects = geoElem.GetEnumerator();
            //get all the edges in the Geometry object
            //foreach (GeometryObject geo in gObjects)
            while (Objects.MoveNext())
            {
                var geo = Objects.Current;

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
        /// <returns>points array stores the bound of the face</returns>
        public virtual PointF[] GetFaceBounds()
        {
            var matrix = m_to2DMatrix;
            var inverseMatrix = matrix.Inverse();
            float minX = 0, maxX = 0, minY = 0, maxY = 0;
            var bFirstPoint = true;

            //get the max and min point on the face
            for (var i = 0; i < m_points.Count; i++)
            {
                var points = m_points[i];
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

        /// <summary>
        ///     calculate the matrix use to scale
        /// </summary>
        /// <param name="size">pictureBox size</param>
        /// <returns>maxtrix is use to scale the profile</returns>
        public virtual Matrix4 ComputeScaleMatrix(Size size)
        {
            m_sizePictureBox = size;
            var boundPoints = GetFaceBounds();
            var width = size.Width / (boundPoints[1].X - boundPoints[0].X);
            var hight = size.Height / (boundPoints[1].Y - boundPoints[0].Y);
            var factor = width <= hight ? width : hight;
            //leave some margin, so multiply factor by 0.85
            m_scaleMatrix = new Matrix4((float)(factor * 0.85));
            return m_scaleMatrix;
        }

        /// <summary>
        ///     calculate the matrix used to transform 3D to 2D
        /// </summary>
        /// <returns>maxtrix is use to transform 3d points to 2d</returns>
        public virtual Matrix4 Compute3DTo2DMatrix()
        {
            var result = Matrix4.Multiply(
                m_to2DMatrix.Inverse(), m_moveToCenterMatrix.Inverse());
            m_transformMatrix = Matrix4.Multiply(result, m_scaleMatrix);
            return m_transformMatrix;
        }

        /// <summary>
        ///     transform the point on Form to 3d world coordinate of revit
        /// </summary>
        /// <param name="ps">contain the points to be transformed</param>
        /// <returns>Vector list contains points being transformed</returns>
        public virtual List<Vector4> Transform2DTo3D(Point[] ps)
        {
            var result = new List<Vector4>();
            TransformPoints(ps);
            var transformMatrix = Matrix4.Multiply(
                m_scaleMatrix.Inverse(), m_moveToCenterMatrix);
            transformMatrix = Matrix4.Multiply(transformMatrix, m_to2DMatrix);
            foreach (var point in ps)
            {
                var v = new Vector4(point.X, point.Y, 0);
                v = transformMatrix.Transform(v);
                result.Add(v);
            }

            return result;
        }

        /// <summary>
        ///     use matrix to transform point
        /// </summary>
        /// <param name="pts">contain the points to be transformed</param>
        private void TransformPoints(Point[] pts)
        {
            var matrix = new Matrix(
                1, 0, 0, 1, m_sizePictureBox.Width / 2, m_sizePictureBox.Height / 2);
            matrix.Invert();
            matrix.TransformPoints(pts);
        }
    }
}
