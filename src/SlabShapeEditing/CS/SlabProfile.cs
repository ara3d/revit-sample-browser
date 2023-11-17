// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.SlabShapeEditing.CS
{
    /// <summary>
    ///     SlabProfile class contains Geometry information of Slab,
    ///     and contains methods used to edit slab's Shape.
    /// </summary>
    public class SlabProfile
    {
        private const int SizeXPictureBox = 354; //save picture box's size.X

        private const int SizeYPictureBox = 280; //save picture box's size.Y

        private PointF[] m_boundPoints; // store array store bound point of Slab

        private readonly ExternalCommandData m_commandData; //contains reference of Revit Application

        private EdgeArray m_edges; // store all the edges of floor

        private readonly Floor m_floor; //object of truss in Revit

        private Matrix4 m_moveToCenterMatrix; // store the Matrix used to move points to center

        private Matrix4 m_moveToPictureBoxCenter; // store the Matrix used to move profile to center of pictureBox

        private Matrix4 m_restoreMatrix; // store the Matrix used to transform window UI coordinate to Revit

        private double m_rotateAngleX; //rotate angle in X direction

        private double m_rotateAngleY; //rotate angle in Y direction

        private Matrix4 m_rotateMatrix; //store the matrix which rotate object

        private Matrix4 m_scaleMatrix; // store the Matrix used to scale profile fit to pictureBox

        private readonly SlabShapeEditor m_slabShapeEditor; //SlabShapeEditor which use to editor shape of slab

        private Matrix4 m_to2DMatrix; // store the Matrix used to transform 3D points to 2D

        private Matrix4 m_transformMatrix; // store the Matrix used to transform Revit coordinate to window UI

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="floor">Floor object in Revit</param>
        /// <param name="commandData">contains reference of Revit Application</param>
        public SlabProfile(Floor floor, ExternalCommandData commandData)
        {
            m_floor = floor;
            m_commandData = commandData;
            m_slabShapeEditor = floor.GetSlabShapeEditor();
            GetSlabProfileInfo();
        }

        /// <summary>
        ///     Calculate geometry info for Slab
        /// </summary>
        public void GetSlabProfileInfo()
        {
            // get all the edges of the Slab
            m_edges = GetFloorEdges();
            // Get a matrix which can transform points to 2D
            m_to2DMatrix = GetTo2DMatrix();
            // get the boundary of all the points
            m_boundPoints = GetBoundsPoints();
            // get a matrix which can keep all the points in the center of the canvas
            m_moveToCenterMatrix = GetMoveToCenterMatrix();
            // get a matrix for scaling all the points and lines within the canvas
            m_scaleMatrix = GetScaleMatrix();
            // get a matrix for moving all point in the middle of PictureBox
            m_moveToPictureBoxCenter = GetMoveToCenterOfPictureBox();
            // transform 3D points to 2D
            m_transformMatrix = Get3DTo2DMatrix();
            // transform from 2D to 3D
            m_restoreMatrix = Get2DTo3DMatrix();
        }

        /// <summary>
        ///     Get all points of the Slab
        /// </summary>
        /// <returns>points array stores all the points on slab</returns>
        public EdgeArray GetFloorEdges()
        {
            var edges = new EdgeArray();
            var options = m_commandData.Application.Application.Create.NewGeometryOptions();
            options.DetailLevel = ViewDetailLevel.Medium;
            //make sure references to geometric objects are computed.
            options.ComputeReferences = true;
            var geoElem = m_floor.get_Geometry(options);
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
                        foreach (Edge edge in edgeArr)
                            edges.Append(edge);
                    }
                }
            }

            return edges;
        }

        /// <summary>
        ///     Get a matrix which can transform points to 2D
        /// </summary>
        /// <returns>matrix which can transform points to 2D</returns>
        public Matrix4 GetTo2DMatrix()
        {
            var xAxis = new Vector4(1, 0, 0);
            //Because Y axis in windows UI is downward, so we should Multiply(-1) here
            var yAxis = new Vector4(0, -1, 0);
            var zAxis = new Vector4(0, 0, 1);

            var result = new Matrix4(xAxis, yAxis, zAxis);
            return result;
        }

        /// <summary>
        ///     calculate the matrix use to scale
        /// </summary>
        /// <returns>maxtrix is use to scale the profile</returns>
        public Matrix4 GetScaleMatrix()
        {
            var xScale = 384 / (m_boundPoints[1].X - m_boundPoints[0].X);
            var yScale = 275 / (m_boundPoints[1].Y - m_boundPoints[0].Y);
            var factor = xScale <= yScale ? xScale : yScale;
            return new Matrix4((float)(factor * 0.85));
        }

        /// <summary>
        ///     Get a matrix which can move points to center of itself
        /// </summary>
        /// <returns>matrix used to move point to center of itself</returns>
        public Matrix4 GetMoveToCenterMatrix()
        {
            //translate the origin to bound center
            var bounds = GetBoundsPoints();
            var min = bounds[0];
            var max = bounds[1];
            var center = new PointF((min.X + max.X) / 2, (min.Y + max.Y) / 2);
            return new Matrix4(new Vector4(center.X, center.Y, 0));
        }

        /// <summary>
        ///     Get a matrix which can move points to center of picture box
        /// </summary>
        /// <returns>matrix used to move point to center of picture box</returns>
        private Matrix4 GetMoveToCenterOfPictureBox()
        {
            return new Matrix4(new Vector4(SizeXPictureBox / 2, SizeYPictureBox / 2, 0));
        }

        /// <summary>
        ///     calculate the matrix used to transform 3D to 2D
        /// </summary>
        /// <returns>maxtrix is use to transform 3d points to 2d</returns>
        public Matrix4 Get3DTo2DMatrix()
        {
            var result = Matrix4.Multiply(
                m_to2DMatrix.Inverse(), m_moveToCenterMatrix.Inverse());
            result = Matrix4.Multiply(result, m_scaleMatrix);
            return Matrix4.Multiply(result, m_moveToPictureBoxCenter);
        }

        /// <summary>
        ///     calculate the matrix used to transform 2D to 3D
        /// </summary>
        /// <returns>maxtrix is use to transform 2d points to 3d</returns>
        public Matrix4 Get2DTo3DMatrix()
        {
            var matrix = Matrix4.Multiply(
                m_moveToPictureBoxCenter.Inverse(), m_scaleMatrix.Inverse());
            matrix = Matrix4.Multiply(
                matrix, m_moveToCenterMatrix);
            return Matrix4.Multiply(matrix, m_to2DMatrix);
        }

        /// <summary>
        ///     Get max and min coordinates of all points
        /// </summary>
        /// <returns>points array stores the bound of all points</returns>
        public PointF[] GetBoundsPoints()
        {
            var matrix = m_to2DMatrix;
            var inverseMatrix = matrix.Inverse();
            double minX = 0, maxX = 0, minY = 0, maxY = 0;
            var bFirstPoint = true;

            //get all points on slab
            var points = new List<XYZ>();
            foreach (Edge edge in m_edges)
            {
                var edgexyzs = edge.Tessellate() as List<XYZ>;
                foreach (var xyz in edgexyzs) points.Add(xyz);
            }

            //get the max and min point on the face
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

            //return an array with max and min value of all points
            var resultPoints = new PointF[2]
            {
                new PointF((float)minX, (float)minY), new PointF((float)maxX, (float)maxY)
            };
            return resultPoints;
        }

        /// <summary>
        ///     draw profile of Slab in pictureBox
        /// </summary>
        /// <param name="graphics">form graphic</param>
        /// <param name="pen">pen used to draw line in pictureBox</param>
        public void Draw2D(Graphics graphics, Pen pen)
        {
            foreach (Edge edge in m_edges)
            {
                var edgexyzs = edge.Tessellate() as List<XYZ>;
                DrawCurve(graphics, pen, edgexyzs);
            }
        }

        /// <summary>
        ///     draw specific points in pictureBox
        /// </summary>
        /// <param name="graphics">form graphic</param>
        /// <param name="pen">pen used to draw line in pictureBox</param>
        /// <param name="points">points which need to be drawn</param>
        public void DrawCurve(Graphics graphics, Pen pen, List<XYZ> points)
        {
            //draw slab curves
            for (var i = 0; i < points.Count - 1; i += 1)
            {
                var point1 = points[i];
                var point2 = points[i + 1];

                var v1 = new Vector4(point1);
                var v2 = new Vector4(point2);

                v1 = m_transformMatrix.Transform(v1);
                v2 = m_transformMatrix.Transform(v2);
                if (m_rotateMatrix != null)
                {
                    v1 = m_rotateMatrix.Transform(v1);
                    v2 = m_rotateMatrix.Transform(v2);
                }

                graphics.DrawLine(pen, new PointF((int)v1.X, (int)v1.Y),
                    new PointF((int)v2.X, (int)v2.Y));
            }
        }

        /// <summary>
        ///     rotate slab with specific angle
        /// </summary>
        /// <param name="xAngle">rotate angle in X direction</param>
        /// <param name="yAngle">rotate angle in Y direction</param>
        public void RotateFloor(double xAngle, double yAngle)
        {
            if (0 == xAngle && 0 == yAngle) return;
            m_rotateAngleX += xAngle;
            m_rotateAngleY += yAngle;

            var rotateX = Matrix4.RotateX(m_rotateAngleX);
            var rotateY = Matrix4.RotateY(m_rotateAngleY);
            var rotateMatrix = Matrix4.Multiply(rotateX, rotateY);

            m_rotateMatrix = Matrix4.Multiply(m_moveToPictureBoxCenter.Inverse(), rotateMatrix);
            m_rotateMatrix = Matrix4.Multiply(m_rotateMatrix, m_moveToPictureBoxCenter);
        }

        /// <summary>
        ///     make rotate matrix null
        /// </summary>
        public void ClearRotateMatrix()
        {
            m_rotateMatrix = null;
        }

        /// <summary>
        ///     Reset index and clear line tool
        /// </summary>
        public void ResetSlabShape()
        {
            var transaction = new Transaction(
                m_commandData.Application.ActiveUIDocument.Document, "ResetSlabShape");
            transaction.Start();
            m_slabShapeEditor.ResetSlabShape();
            transaction.Commit();
            //re-calculate geometry info
            GetSlabProfileInfo();
        }

        /// <summary>
        ///     Add vertex on specific location
        /// </summary>
        /// <param name="point">location where vertex add on</param>
        /// <returns>new created vertex</returns>
        public SlabShapeVertex AddVertex(PointF point)
        {
            var transaction = new Transaction(
                m_commandData.Application.ActiveUIDocument.Document, "AddVertex");
            transaction.Start();
            var v1 = new Vector4(new XYZ(point.X, point.Y, 0));
            v1 = m_restoreMatrix.Transform(v1);
            var vertex = m_slabShapeEditor.DrawPoint(new XYZ(v1.X, v1.Y, v1.Z));
            transaction.Commit();
            //re-calculate geometry info
            GetSlabProfileInfo();

            return vertex;
        }

        /// <summary>
        ///     Add Crease on specific location
        /// </summary>
        /// <param name="point1">first point of location where Crease add on</param>
        /// <param name="point2">second point of location where Crease add on</param>
        /// <returns>new created Crease</returns>
        public SlabShapeCrease AddCrease(PointF point1, PointF point2)
        {
            //create first vertex
            var transaction = new Transaction(
                m_commandData.Application.ActiveUIDocument.Document, "AddCrease");
            transaction.Start();
            var v1 = new Vector4(new XYZ(point1.X, point1.Y, 0));
            v1 = m_restoreMatrix.Transform(v1);
            var vertex1 = m_slabShapeEditor.DrawPoint(new XYZ(v1.X, v1.Y, v1.Z));
            //create second vertex
            var v2 = new Vector4(new XYZ(point2.X, point2.Y, 0));
            v2 = m_restoreMatrix.Transform(v2);
            var vertex2 = m_slabShapeEditor.DrawPoint(new XYZ(v2.X, v2.Y, v2.Z));
            //create crease
            var creases = m_slabShapeEditor.DrawSplitLine(vertex1, vertex2);

            var crease = creases.get_Item(0);
            transaction.Commit();
            //re-calculate geometry info 
            GetSlabProfileInfo();
            return crease;
        }

        /// <summary>
        ///     judge whether point can use to create vertex on slab
        /// </summary>
        /// <param name="point1">location where vertex add on</param>
        /// <returns>whether point can use to create vertex on slab</returns>
        public bool CanCreateVertex(PointF pointF)
        {
            var createSuccess = false;
            var transaction = new Transaction(
                m_commandData.Application.ActiveUIDocument.Document, "CanCreateVertex");
            transaction.Start();
            var v1 = new Vector4(new XYZ(pointF.X, pointF.Y, 0));
            v1 = m_restoreMatrix.Transform(v1);
            var vertex = m_slabShapeEditor.DrawPoint(new XYZ(v1.X, v1.Y, v1.Z));
            if (null != vertex) createSuccess = true;
            transaction.RollBack();
            //re-calculate geometry info 
            GetSlabProfileInfo();
            return createSuccess;
        }
    }
}
