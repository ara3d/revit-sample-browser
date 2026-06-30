// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
namespace Ara3D.RevitSampleBrowser.Truss.CS
{
    /// <summary>
    ///     TrussGeometry class contains Geometry information of new created Truss,
    ///     and contains methods used to Edit profile of truss.
    /// </summary>
    public class TrussGeometry
    {
        private XYZ m_endLocation; //store the end point of truss location

        private Matrix4
            m_2dToTrussProfileMatrix; //store matrix use to transform point on pictureBox to truss (profile) plane

        private readonly LineTool m_bottomChord; //line tool used to draw top chord

        private XYZ[] m_boundPoints; // store array store bound point of truss

        private int m_clickMemberIndex = -1; // index of clicked truss member (beam), -1 when nothing clicked.

        private readonly ExternalCommandData m_commandData; //object which contains reference of Revit Application

        private readonly ArrayList m_graphicsPaths; //store all the GraphicsPath objects of each curve in truss.

        private Matrix4 m_moveToCenterMatrix; // store the Matrix used to move points to center

        private Vector4 m_origin; //base point of truss

        private List<XYZ> m_points; // store all the points on the needed face

        private Matrix4 m_restoreMatrix; // store the Matrix used to transform window UI coordinate to Revit

        private Matrix4 m_scaleMatrix; // store the Matrix used to scale profile fit to pictureBox

        private int m_selectMemberIndex = -1; // index of selected truss member (beam), -1 when nothing selected.

        private Matrix4 m_to2DMatrix; // store the Matrix used to transform 3D points to 2D

        private readonly LineTool m_topChord; //line tool used to draw top chord

        private Matrix4 m_transformMatrix; // store the Matrix used to transform Revit coordinate to window UI

        private readonly Autodesk.Revit.DB.Structure.Truss m_truss; //object of truss in Revit

        private XYZ m_startLocation; //store the start point of truss location

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="truss">new created truss object in Revit</param>
        public TrussGeometry(Autodesk.Revit.DB.Structure.Truss truss, ExternalCommandData commandData)
        {
            m_commandData = commandData;
            m_topChord = new LineTool();
            m_bottomChord = new LineTool();
            m_truss = truss;
            m_graphicsPaths = [];
            GetTrussGeometryInfo();
        }

        private void GetTrussGeometryInfo()
        {
            // get the start and end point of the basic line of the truss
            m_points = GetTrussPoints();
            // Get a matrix which can transform points to 2D
            m_to2DMatrix = GetTo2DMatrix();
            // get the boundary of all the points
            m_boundPoints = GetBoundsPoints();
            // get a matrix which can keep all the points in the center of the canvas
            m_moveToCenterMatrix = GetMoveToCenterMatrix();
            // get a matrix for scaling all the points and lines within the canvas
            m_scaleMatrix = GetScaleMatrix();
            // transform 3D points to 2D
            m_transformMatrix = Get3DTo2DMatrix();
            // transform from 2D to 3D
            m_restoreMatrix = Get2DTo3DMatrix();
            // transform from 2D (on picture box) to truss profile plane
            m_2dToTrussProfileMatrix = Get2DToTrussProfileMatrix();
            // create the graphics path which contains all the lines
            CreateGraphicsPath();
        }

        public List<XYZ> GetTrussPoints()
        {
            List<XYZ> xyzArray = [];
            try
            {
                IEnumerator iter = m_truss.Members.GetEnumerator();
                iter.Reset();
                while (iter.MoveNext())
                {
                    var id = (ElementId)iter.Current;
                    var elem =
                        m_commandData.Application.ActiveUIDocument.Document.GetElement(id);
                    var familyInstace = (FamilyInstance)elem;
                    Curve frame1Curve = null;

                    if (familyInstace.Location is LocationCurve curve)
                        frame1Curve = curve.Curve;

                    var line = (Line)frame1Curve;
                    xyzArray.Add(line.GetEndPoint(0));
                    xyzArray.Add(line.GetEndPoint(1));
                }
            }
            catch (ArgumentException)
            {
                TaskDialog.Show("Revit",
                    "The start point and the end point of the line are too close, please re-draw it.");
            }

            return xyzArray;
        }

        public Matrix4 GetTo2DMatrix()
        {
            var trussLocation = (m_truss.Location as LocationCurve).Curve as Line;
            m_startLocation = trussLocation.GetEndPoint(0);
            m_endLocation = trussLocation.GetEndPoint(1);
            //use baseline of truss as the X axis
            var diff = m_endLocation - m_startLocation;
            Vector4 xAxis = new(new XYZ(diff.X, diff.Y, diff.Z));
            xAxis.Normalize();
            //get Z Axis
            var zAxis = Vector4.CrossProduct(xAxis, new Vector4(new XYZ(0, 0, 1)));
            zAxis.Normalize();
            //get Y Axis, downward
            var yAxis = Vector4.CrossProduct(xAxis, zAxis);
            yAxis.Normalize();
            //get original point, first point
            m_origin = new Vector4(m_points[0]);

            return new Matrix4(xAxis, yAxis, zAxis, m_origin);
        }

        public Matrix4 GetScaleMatrix()
        {
            var xScale = 384 / (m_boundPoints[1].X - m_boundPoints[0].X);
            var yScale = 275 / (m_boundPoints[1].Y - m_boundPoints[0].Y);
            var factor = xScale <= yScale ? xScale : yScale;
            return new Matrix4((float)(factor * 0.85));
        }

        public Matrix4 GetMoveToCenterMatrix()
        {
            //translate the origin to bound center
            var bounds = GetBoundsPoints();
            var min = bounds[0];
            var max = bounds[1];
            XYZ center = new((min.X + max.X) / 2, (min.Y + max.Y) / 2, 0);
            return new Matrix4(new Vector4((float)center.X, (float)center.Y, 0));
        }

        public Matrix4 Get3DTo2DMatrix()
        {
            var result = Matrix4.Multiply(
                m_to2DMatrix.Inverse(), m_moveToCenterMatrix.Inverse());
            result = Matrix4.Multiply(result, m_scaleMatrix);
            return Matrix4.Multiply(result, new Matrix4(new Vector4(192, 137, 0)));
        }

        public Matrix4 Get2DTo3DMatrix()
        {
            var matrix = Matrix4.Multiply(
                new Matrix4(new Vector4(-192, -137, 0)), m_scaleMatrix.Inverse());
            matrix = Matrix4.Multiply(matrix, m_moveToCenterMatrix);
            return Matrix4.Multiply(matrix, m_to2DMatrix);
        }

        public Matrix4 Get2DToTrussProfileMatrix()
        {
            var matrix = Matrix4.Multiply(
                new Matrix4(new Vector4(-192, -137, 0)), m_scaleMatrix.Inverse());
            return Matrix4.Multiply(matrix, m_moveToCenterMatrix);
            //Matrix4 upward =  new Matrix4(new Vector4(new Autodesk.Revit.DB.XYZ (1, 0, 0)),
            //    new Vector4(new Autodesk.Revit.DB.XYZ (0, -1, 0)), new Vector4(new Autodesk.Revit.DB.XYZ (0, 0, 1)));
            //return Matrix4.Multiply(matrix, upward);
        }

        public XYZ[] GetBoundsPoints()
        {
            var matrix = m_to2DMatrix;
            var inverseMatrix = matrix.Inverse();
            double minX = 0, maxX = 0, minY = 0, maxY = 0;
            var bFirstPoint = true;

            //get the max and min point on the face
            foreach (var point in m_points)
            {
                Vector4 v = new(point);
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
            var resultPoints = new XYZ[2]
            {
                new(minX, minY, 0), new(maxX, maxY, 0)
            };
            return resultPoints;
        }

        public void Draw2D(Graphics graphics, Pen pen)
        {
            //draw truss curves
            for (var i = 0; i < m_points.Count - 1; i += 2)
            {
                var point1 = m_points[i];
                var point2 = m_points[i + 1];

                Vector4 v1 = new(point1);
                Vector4 v2 = new(point2);

                v1 = m_transformMatrix.Transform(v1);
                v2 = m_transformMatrix.Transform(v2);
                graphics.DrawLine(pen, new Point((int)v1.X, (int)v1.Y),
                    new Point((int)v2.X, (int)v2.Y));
            }

            //draw selected beam (line) by red pen
            DrawSelectedLineRed(graphics);

            //draw top chord and bottom chord
            m_topChord.Draw2D(graphics, Pens.Red);
            m_bottomChord.Draw2D(graphics, Pens.Black);
        }

        /// <summary>
        ///     Set profile of truss
        /// </summary>
        /// <param name="commandData">object which contains reference of Revit Application</param>
        public void SetProfile(ExternalCommandData commandData)
        {
            if (m_topChord.Points.Count < 2)
            {
                TaskDialog.Show("Truss API", "Haven't drawn top chord");
                return;
            }

            if (m_bottomChord.Points.Count < 2)
            {
                TaskDialog.Show("Truss API", "Haven't drawn bottom chord");
                return;
            }

            var createApp = commandData.Application.Application.Create;
            var curvesTop = createApp.NewCurveArray();
            var curvesBottom = createApp.NewCurveArray();
            //get coordinates of top (bottom) chord from lineTool
            GetChordPoints(m_topChord, curvesTop, createApp);
            GetChordPoints(m_bottomChord, curvesBottom, createApp);
            try
            {
                //set profile by top curve and bottom curve drawn by user in picture box
                m_truss.SetProfile(curvesTop, curvesBottom);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Truss API", ex.Message);
            }

            //re-calculate geometry info after truss profile changed
            GetTrussGeometryInfo();
            ClearChords();
        }

        private void GetChordPoints(LineTool chord, CurveArray curves, Application createApp)
        {
            //get coordinates of top chord from lineTool
            for (var i = 0; i < chord.Points.Count - 1; i++)
            {
                var point = (Point)chord.Points[i];
                var point2 = (Point)chord.Points[i + 1];

                XYZ xyz = new(point.X, point.Y, 0);
                XYZ xyz2 = new(point2.X, point2.Y, 0);

                Vector4 v1 = new(xyz);
                Vector4 v2 = new(xyz2);

                v1 = m_restoreMatrix.Transform(v1);
                v2 = m_restoreMatrix.Transform(v2);

                try
                {
                    var line = Line.CreateBound(
                        new XYZ(v1.X, v1.Y, v1.Z), new XYZ(v2.X, v2.Y, v2.Z));
                    curves.Append(line);
                }
                catch (ArgumentException)
                {
                    TaskDialog.Show("Revit",
                        "The start point and the end point of the line are too close, please re-draw it.");
                    ClearChords();
                }
            }
        }

        public void RemoveProfile()
        {
            m_truss.RemoveProfile();
            GetTrussGeometryInfo();
            ClearChords();
        }

        public void AddTopChordPoint(int x, int y)
        {
            // doesn't allow to add 2 points near-by
            if (m_topChord.Points.Count > 0)
            {
                var lastPoint = (Point)m_topChord.Points[m_topChord.Points.Count - 1];
                if (Math.Abs(lastPoint.X - x) < 1 ||
                    Math.Abs(lastPoint.Y - y) < 1)
                    return;
            }

            m_topChord.Points.Add(new Point(x, y));
        }

        public void AddBottomChordPoint(int x, int y)
        {
            // doesn't allow to add 2 points near-by
            if (m_topChord.Points.Count > 0)
            {
                var lastPoint = (Point)m_topChord.Points[m_topChord.Points.Count - 1];
                if (Math.Abs(lastPoint.X - x) < 1 ||
                    Math.Abs(lastPoint.Y - y) < 1)
                    return;
            }

            m_bottomChord.Points.Add(new Point(x, y));
        }

        public void AddTopChordMovePoint(int x, int y)
        {
            m_topChord.MovePoint = new Point(x, y);
            m_bottomChord.MovePoint = Point.Empty;
        }

        public void AddBottomChordMovePoint(int x, int y)
        {
            m_bottomChord.MovePoint = new Point(x, y);
            m_topChord.MovePoint = Point.Empty;
        }

        public void ClearMovePoint()
        {
            m_topChord.MovePoint = Point.Empty;
            m_bottomChord.MovePoint = Point.Empty;
        }

        public void ClearChords()
        {
            m_topChord.Points.Clear();
            m_bottomChord.Points.Clear();
        }

        public void CreateGraphicsPath()
        {
            m_graphicsPaths.Clear();
            //create path for all the curves of Truss
            for (var i = 0; i < m_points.Count - 1; i += 2)
            {
                var point1 = m_points[i];
                var point2 = m_points[i + 1];

                Vector4 v1 = new(point1);
                Vector4 v2 = new(point2);

                v1 = m_transformMatrix.Transform(v1);
                v2 = m_transformMatrix.Transform(v2);

                GraphicsPath path = new();
                path.AddLine(new Point((int)v1.X, (int)v1.Y), new Point((int)v2.X, (int)v2.Y));
                m_graphicsPaths.Add(path);
            }
        }

        public int SelectTrussMember(int x, int y)
        {
            Point point = new(x, y);
            for (var i = 0; i < m_graphicsPaths.Count; i++)
            {
                var path = (GraphicsPath)m_graphicsPaths[i];
                if (path.IsOutlineVisible(point, Pens.Blue))
                {
                    m_selectMemberIndex = i;
                    return m_selectMemberIndex;
                }
            }

            m_selectMemberIndex = -1;
            return m_selectMemberIndex;
        }

        public void DrawSelectedLineRed(Graphics graphics)
        {
            Pen redPen = new(Color.Red, (float)2.0);
            //draw the selected beam as red line
            if (m_selectMemberIndex != -1)
            {
                var selectPath = (GraphicsPath)m_graphicsPaths[m_selectMemberIndex];
                var startPointOfSelectedLine = (PointF)selectPath.PathPoints.GetValue(0);
                var endPointOfSelectedLine = (PointF)selectPath.PathPoints.GetValue(1);
                graphics.DrawLine(redPen, startPointOfSelectedLine, endPointOfSelectedLine);
            }

            //draw clicked beam red
            if (m_clickMemberIndex != -1)
            {
                var selectPath = (GraphicsPath)m_graphicsPaths[m_clickMemberIndex];
                var startPointOfSelectedLine = (PointF)selectPath.PathPoints.GetValue(0);
                var endPointOfSelectedLine = (PointF)selectPath.PathPoints.GetValue(1);
                graphics.DrawLine(redPen, startPointOfSelectedLine, endPointOfSelectedLine);
            }
        }

        /// <summary>
        ///     Get selected beam (truss member) by select index
        /// </summary>
        /// <param name="commandData">object which contains reference of Revit Application</param>
        /// <returns>index of selected member</returns>
        public FamilyInstance GetSelectedBeam(ExternalCommandData commandData)
        {
            m_clickMemberIndex = m_selectMemberIndex;
            ElementId id = null;
            var idSet = m_truss.Members as List<ElementId>;
            IEnumerator iter = idSet.GetEnumerator();
            iter.Reset();
            var i = 0;
            while (iter.MoveNext())
            {
                if (i == m_selectMemberIndex)
                {
                    id = iter.Current as ElementId;
                    break;
                }

                i++;
            }

            return (FamilyInstance)commandData.Application.ActiveUIDocument.Document.GetElement(id);
        }

        public void Reset()
        {
            m_clickMemberIndex = -1;
            m_selectMemberIndex = -1;
            m_topChord.Points.Clear();
            m_bottomChord.Points.Clear();
        }
    }
}
