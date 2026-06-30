// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

using Ara3D.RevitSampleBrowser.Common.Geometry;
namespace Ara3D.RevitSampleBrowser.CurtainWallGrid.CS
{
    public class GridCoordinates
    {

        // will be used in the scale matrix and move-to-center matrix
        private Rectangle m_boundary;


        private List<PointF> m_boundPoints;

        // will be used in the scale matrix and move-to-center matrix
        private Point m_center;


        private Matrix4 m_moveToCenterMatrix;

        private readonly MyDocument m_myDocument;

        // scale the size of the image to let it shown in the canvas
        private readonly float m_scaleFactor = 0.85f;

        private Matrix4 m_scaleMatrix;

        public GridCoordinates(MyDocument myDoc, GridDrawing drawing)
        {
            m_myDocument = myDoc;

            if (null == drawing) TaskDialog.Show("Revit", "Error! There's no grid information in the curtain wall.");
            Drawing = drawing;
            drawing.Coordinates = this;
        }

        public GridDrawing Drawing { get; set; }

        public Matrix4 To2DMatrix { get; private set; }

        public Matrix4 TransformMatrix { get; private set; }

        public Matrix4 RestoreMatrix { get; private set; }

        public void GetMatrix()
        {
            // initialize the class members, obtain the location information of the canvas
            m_boundary = Drawing.Boundary;
            m_center = Drawing.Center;

            // Get a matrix which can transform points to 2D
            To2DMatrix = GetTo2DMatrix();

            m_boundPoints = GetBoundsPoints();

            // get a matrix which can keep all the points in the center of the canvas
            m_moveToCenterMatrix = GetMoveToCenterMatrix();

            // get a matrix for scaling all the points and lines within the canvas
            m_scaleMatrix = GetScaleMatrix();

            // transform 3D points to 2D
            TransformMatrix = Get3DTo2DMatrix();

            // transform from 2D to 3D
            RestoreMatrix = Get2DTo3DMatrix();
        }

        private Matrix4 Get2DTo3DMatrix()
        {
            var matrix = Matrix4.Multiply(
                new Matrix4(new Vector4(-m_center.X, -m_center.Y, 0)), m_scaleMatrix.Inverse());
            matrix = Matrix4.Multiply(
                matrix, m_moveToCenterMatrix);
            return Matrix4.Multiply(matrix, To2DMatrix);
        }

        private Matrix4 Get3DTo2DMatrix()
        {
            var result = Matrix4.Multiply(
                To2DMatrix.Inverse(), m_moveToCenterMatrix.Inverse());
            result = Matrix4.Multiply(result, m_scaleMatrix);
            return Matrix4.Multiply(result, new Matrix4(new Vector4(m_center.X, m_center.Y, 0)));
        }

        private Matrix4 GetScaleMatrix()
        {
            var xScale = m_boundary.Width / (m_boundPoints[1].X - m_boundPoints[0].X);
            var yScale = m_boundary.Height / (m_boundPoints[1].Y - m_boundPoints[0].Y);
            var factor = xScale <= yScale ? xScale : yScale;
            return new Matrix4(factor * m_scaleFactor);
        }

        private Matrix4 GetMoveToCenterMatrix()
        {
            //translate the origin to bound center
            var min = m_boundPoints[0];
            var max = m_boundPoints[1];
            var center = new PointF((min.X + max.X) / 2, (min.Y + max.Y) / 2);
            return new Matrix4(new Vector4(center.X, center.Y, 0));
        }

        private List<PointF> GetBoundsPoints()
        {
            var matrix = To2DMatrix;
            var inverseMatrix = matrix.Inverse();
            float minX = 0, maxX = 0, minY = 0, maxY = 0;
            var bFirstPoint = true;
            var resultPoints = new List<PointF>();

            foreach (var point in Drawing.Geometry.GridVertexesXyz)
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

            resultPoints.Add(new PointF(minX, minY));
            resultPoints.Add(new PointF(maxX, maxY));
            return resultPoints;
        }

        private Matrix4 GetTo2DMatrix()
        {
            var startXyz = m_myDocument.WallGeometry.StartXyz;
            var endXyz = m_myDocument.WallGeometry.EndXyz;
            var sub = endXyz - startXyz;
            var xAxis = new Vector4(new XYZ(sub.X, sub.Y, sub.Z));
            xAxis.Normalize();
            //because in the windows UI, Y axis is downward
            var yAxis = new Vector4(new XYZ(0, 0, -1));
            yAxis.Normalize();
            var zAxis = Vector4.CrossProduct(xAxis, yAxis);
            zAxis.Normalize();
            var origin = new Vector4(Drawing.Geometry.GridVertexesXyz[0]);

            return new Matrix4(xAxis, yAxis, zAxis, origin);
        }
    } // end of class
}
