// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using Point = System.Drawing.Point;
namespace Ara3D.RevitSampleBrowser.CurtainWallGrid.CS
{
    public class GridGeometry
    {
        // the active document of Revit
        private readonly Document m_activeDocument;


        // the referred drawing class for the curtain grid

        //object which contains reference of Revit Application
        private readonly ExternalCommandData m_commandData;

        private readonly MyDocument m_myDocument;

        public GridGeometry(MyDocument myDoc)
        {
            m_myDocument = myDoc;
            m_commandData = myDoc.CommandData;
            m_activeDocument = myDoc.Document;
            GridProperties = new GridProperties();
            //m_activeGrid = grid;
            Drawing = new GridDrawing(myDoc, this);
            UGridLines = [];
            VGridLines = [];
            GridVertexesXyz = [];
        }


        /// <summary>
        ///     stores the curtain grid information of the created curtain wall
        /// </summary>
        public CurtainGrid ActiveGrid { get; private set; }

        public GridDrawing Drawing { get; }

        public MullionType MullionType { get; set; }

        public List<CurtainGridLine> UGridLines { get; }

        public List<CurtainGridLine> VGridLines { get; }

        public List<XYZ> GridVertexesXyz { get; }

        public GridProperties GridProperties { get; }

        public CurtainGridLine LineToBeMoved { get; private set; }

        public int MoveOffset { get; set; }

        public void ReloadGridProperties()
        {
            if (null == ActiveGrid)
            {
                if (m_myDocument.WallCreated)
                    ActiveGrid = m_myDocument.CurtainWall.CurtainGrid;
                else
                    return;
            }

            // horizontal grid pattern 
            Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            switch (ActiveGrid.Grid2Justification)
            {
                case CurtainGridAlignType.Beginning:
                    GridProperties.HorizontalJustification = CurtainGridAlign.Beginning;
                    break;
                case CurtainGridAlignType.Center:
                    GridProperties.HorizontalJustification = CurtainGridAlign.Center;
                    break;
                case CurtainGridAlignType.End:
                    GridProperties.HorizontalJustification = CurtainGridAlign.End;
                    break;
            }

            GridProperties.HorizontalAngle = ActiveGrid.Grid2Angle * 180.0 / Math.PI;
            GridProperties.HorizontalOffset = ActiveGrid.Grid2Offset;
            GridProperties.HorizontalLinesNumber = ActiveGrid.NumULines;
            // vertical grid pattern
            switch (ActiveGrid.Grid1Justification)
            {
                case CurtainGridAlignType.Beginning:
                    GridProperties.VerticalJustification = CurtainGridAlign.Beginning;
                    break;
                case CurtainGridAlignType.Center:
                    GridProperties.VerticalJustification = CurtainGridAlign.Center;
                    break;
                case CurtainGridAlignType.End:
                    GridProperties.VerticalJustification = CurtainGridAlign.End;
                    break;
            }

            GridProperties.VerticalAngle = ActiveGrid.Grid1Angle * 180.0 / Math.PI;
            GridProperties.VerticalOffset = ActiveGrid.Grid1Offset;
            GridProperties.VerticalLinesNumber = ActiveGrid.NumVLines;
            // other data
            GridProperties.PanelNumber = ActiveGrid.NumPanels;
            GridProperties.UnlockedPanelsNumber = ActiveGrid.GetUnlockedPanelIds().Count;
            GridProperties.CellNumber = ActiveGrid.GetCurtainCells().Count;
            if (0 != ActiveGrid.GetMullionIds().Count)
            {
                GridProperties.MullionsNumber = ActiveGrid.GetMullionIds().Count;
                GridProperties.UnlockedmullionsNumber = ActiveGrid.GetUnlockedMullionIds().Count;
            }

            act.Commit();
        }

        public void ReloadGeometryData()
        {
            if (null == ActiveGrid)
            {
                if (m_myDocument.WallCreated)
                    ActiveGrid = m_myDocument.CurtainWall.CurtainGrid;
                else
                    return;
            }

            Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            //ElementSet mullions = m_activeGrid.Mullions;
            var mullions = ActiveGrid.GetMullionIds();

            act.Commit();
            if (0 == mullions.Count)
                foreach (var e in mullions)
                {
                    if (m_activeDocument.GetElement(e) is Mullion mullion)
                    {
                        MullionType = mullion.MullionType;
                        break;
                    }
                }

            GetULines();
            GetVLines();
            GetCurtainGridVertexes();
            // covert those lines to 2D format
            Drawing.GetLines2D();
        }

        public void RemoveSegment()
        {
            // verify that the mouse is inside the curtain grid area
            var lines2D = Drawing.DrawObject.Lines2D;
            if (lines2D.Count < 1) return;

            List<SegmentLine2D> toBeRemovedList = [];
            var canRemove = true;
            MimicRemoveSegments(ref canRemove, toBeRemovedList);
            // in the "MimicRemove" process, we didn't find that we need to "Remove the last segment of the grid line"
            // so the "Remove" action can go on
            if (canRemove)
            {
                try
                {
                    Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                    act.Start();
                    foreach (var seg2D in toBeRemovedList)
                    {
                        var gridLineIndex = seg2D.GridLineIndex;
                        var segIndex = seg2D.SegmentIndex;
                        var isUSegment = seg2D.IsUSegment;

                        var line = isUSegment ? UGridLines[gridLineIndex] : VGridLines[gridLineIndex];
                        var curve = line.AllSegmentCurves.get_Item(segIndex);
                        line.RemoveSegment(curve);
                    }

                    act.Commit();
                }
                catch (Exception e)
                {
                    TaskDialog.Show("Exception", e.Message);
                    return;
                }
            }
            // in the "MimicRemove" process, we found that we would "Remove the last segment of the grid line"
            // so the whole "Remove" action will roll back
            else
            {
                foreach (var seg2D in toBeRemovedList)
                {
                    var gridLineIndex = seg2D.GridLineIndex;
                    var segIndex = seg2D.SegmentIndex;
                    var isUSegment = seg2D.IsUSegment;
                    var gridLine2D = isUSegment ? Drawing.UGridLines2D[gridLineIndex] : Drawing.VGridLines2D[gridLineIndex];
                    gridLine2D.RemovedNumber--;
                    var segLine2D = gridLine2D.Segments[segIndex];
                    segLine2D.Removed = false;
                    gridLine2D.Segments[segIndex] = segLine2D;
                }

                var statusMsg = "Delete this segment will make some grid lines have no existent segments.";
                m_myDocument.Message = new KeyValuePair<string, bool>(statusMsg, true);
            }

            ReloadGeometryData();
            Drawing.DrawObject.Clear();
        }

        public void AddSegment()
        {
            // verify that the mouse is inside the curtain grid area
            var lines2D = Drawing.DrawObject.Lines2D;
            if (lines2D.Count < 1) return;

            // the selected segment location is on a U grid line
            if (-1 != Drawing.SelectedUIndex)
            {
                var line = UGridLines[Drawing.SelectedUIndex];
                var curve = line.AllSegmentCurves.get_Item(Drawing.SelectedUSegmentIndex);
                if (null != line && null != curve)
                    try
                    {
                        Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());

                        act.Start();
                        line.AddSegment(curve);
                        act.Commit();
                    }
                    catch (Exception e)
                    {
                        TaskDialog.Show("Exception", e.Message);
                        return;
                    }

                var gridLine2D = Drawing.UGridLines2D[Drawing.SelectedUIndex];
                gridLine2D.RemovedNumber--;
                var segLine2D = gridLine2D.Segments[Drawing.SelectedUSegmentIndex];
                segLine2D.Removed = false;
                gridLine2D.Segments[Drawing.SelectedUSegmentIndex] = segLine2D;
            }
            // the selected segment location is on a V grid line
            else if (-1 != Drawing.SelectedVIndex)
            {
                var line = VGridLines[Drawing.SelectedVIndex];
                var curve = line.AllSegmentCurves.get_Item(Drawing.SelectedVSegmentIndex);
                if (null != line && null != curve)
                    try
                    {
                        Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                        act.Start();
                        line.AddSegment(curve);
                        act.Commit();
                    }
                    catch (Exception e)
                    {
                        TaskDialog.Show("Exception", e.Message);
                        return;
                    }

                var gridLine2D = Drawing.VGridLines2D[Drawing.SelectedVIndex];
                gridLine2D.RemovedNumber--;
                var segLine2D = gridLine2D.Segments[Drawing.SelectedVSegmentIndex];
                segLine2D.Removed = false;
                gridLine2D.Segments[Drawing.SelectedVSegmentIndex] = segLine2D;
            }

            ReloadGeometryData();
            Drawing.DrawObject.Clear();
        }

        public void AddAllSegments()
        {
            // verify that the mouse is inside the curtain grid area
            var lines2D = Drawing.DrawObject.Lines2D;
            if (lines2D.Count < 1) return;

            if (-1 != Drawing.SelectedUIndex)
            {
                var line = UGridLines[Drawing.SelectedUIndex];
                if (null != line)
                    try
                    {
                        Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                        act.Start();
                        line.AddAllSegments();
                        act.Commit();
                    }
                    catch (Exception e)
                    {
                        TaskDialog.Show("Exception", e.Message);
                        return;
                    }

                var gridLine2D = Drawing.UGridLines2D[Drawing.SelectedUIndex];
                gridLine2D.RemovedNumber = 0;
                foreach (var segLine2D in gridLine2D.Segments)
                {
                    segLine2D.Removed = false;
                }
            }
            else if (-1 != Drawing.SelectedVIndex)
            {
                var line = VGridLines[Drawing.SelectedVIndex];
                if (null != line)
                    try
                    {
                        Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                        act.Start();
                        line.AddAllSegments();
                        act.Commit();
                    }
                    catch (Exception e)
                    {
                        TaskDialog.Show("Exception", e.Message);
                        return;
                    }

                var gridLine2D = Drawing.VGridLines2D[Drawing.SelectedVIndex];
                gridLine2D.RemovedNumber = 0;
                foreach (var segLine2D in gridLine2D.Segments)
                {
                    segLine2D.Removed = false;
                }
            }

            ReloadGeometryData();
            Drawing.DrawObject.Clear();
        }

        public void AddUGridLine()
        {
            // verify that the mouse location is valid: it's inside the curtain grid area 
            // & it doesn't lap over another grid line (it's not allowed to add a grid line to lap over another one)
            if (false == Drawing.MouseLocationValid) return;

            // all the assistant lines (in "Add U (Horizontal) Grid Line" operation, 
            // there's only one dash line, this line indicates the location to be added)
            var lines2D = Drawing.DrawObject.Lines2D;
            if (lines2D.Count < 1) return;

            // the dash U line shown in the sample (incidates the location to be added)
            var line2D = lines2D[0].Key;
            if (Point.Empty == line2D.StartPoint ||
                Point.Empty == line2D.EndPoint)
                return;

            var midX = (line2D.StartPoint.X + line2D.EndPoint.X) / 2;
            var midY = (line2D.StartPoint.Y + line2D.EndPoint.Y) / 2;
            // transform the 2D point to Autodesk.Revit.DB.XYZ format
            XYZ pos = new(midX, midY, 0);
            Vector4 vec = new(pos);
            vec = Drawing.Coordinates.RestoreMatrix.Transform(vec);

            Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            try
            {
                ActiveGrid.AddGridLine(true, new XYZ(vec.X, vec.Y, vec.Z), false);
            }
            catch (Exception e)
            {
                TaskDialog.Show("Exception", e.Message);
                // "add U line" failed, so return directly
                return;
            }

            act.Commit();

            // U line added, the V line's segment information changed, so reload all the geometry data
            ReloadGeometryData();
        }

        public void AddVGridLine()
        {
            // verify that the mouse location is valid: it's inside the curtain grid area 
            // & it doesn't lap over another grid line (it's not allowed to add a grid line to lap over another one)
            if (false == Drawing.MouseLocationValid) return;

            // all the assistant lines (in "Add V (Vertical) Grid Line" operation, 
            // there's only one dash line, this line indicates the location to be added)
            var lines2D = Drawing.DrawObject.Lines2D;
            if (lines2D.Count < 1) return;

            // the dash V line shown in the sample (incidates the location to be added)
            var line2D = lines2D[0].Key;
            if (Point.Empty == line2D.StartPoint ||
                Point.Empty == line2D.EndPoint)
                return;

            var midX = (line2D.StartPoint.X + line2D.EndPoint.X) / 2;
            var midY = (line2D.StartPoint.Y + line2D.EndPoint.Y) / 2;
            // transform the 2D point to Autodesk.Revit.DB.XYZ format
            XYZ pos = new(midX, midY, 0);
            Vector4 vec = new(pos);
            vec = Drawing.Coordinates.RestoreMatrix.Transform(vec);

            Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            try
            {
                ActiveGrid.AddGridLine(false, new XYZ(vec.X, vec.Y, vec.Z), false);
            }
            catch (Exception e)
            {
                TaskDialog.Show("Exception", e.Message);
                // "add V line" failed, so return directly
                return;
            }

            act.Commit();

            // V line added, the U line's segment information changed, so reload all the geometry data
            ReloadGeometryData();
        }

        public void LockOrUnlockSelectedGridLine()
        {
            CurtainGridLine line = null;
            GridLine2D line2D = new();

            if (-1 != Drawing.SelectedUIndex)
            {
                line = UGridLines[Drawing.SelectedUIndex];
                line2D = Drawing.UGridLines2D[Drawing.SelectedUIndex];
            }
            else if (-1 != Drawing.SelectedVIndex)
            {
                line = VGridLines[Drawing.SelectedVIndex];
                line2D = Drawing.VGridLines2D[Drawing.SelectedVIndex];
            }
            else
            {
                return;
            }

            // lock/unlock the grid line
            if (null != line)
            {
                Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                act.Start();
                line.Lock = !line.Lock;
                act.Commit();
            }

            line2D.Locked = line.Lock;

            // clear the intermediate variables and instances
            Drawing.DrawObject.Clear();
        }

        public bool GetLineToBeMoved()
        {
            if (-1 != Drawing.SelectedUIndex)
            {
                LineToBeMoved = UGridLines[Drawing.SelectedUIndex];
                return true;
            }

            if (-1 != Drawing.SelectedVIndex)
            {
                LineToBeMoved = VGridLines[Drawing.SelectedVIndex];
                return true;
            }

            LineToBeMoved = null;
            return false;
        }

        public bool MoveGridLine(Point mousePosition)
        {
            // verify that the mouse location is valid: it's inside the curtain grid area 
            // & it doesn't lap over another grid line (it's not allowed to move a grid line to lap over another one)
            if (false == Drawing.MouseLocationValid) return false;

            if (null == LineToBeMoved) return false;

            // move a U line along the V direction
            if (-1 != Drawing.SelectedUIndex)
            {
                // convert the 2D data to 3D
                XYZ xyz = new(mousePosition.X, mousePosition.Y, 0);
                Vector4 vec = new(xyz);
                vec = Drawing.Coordinates.RestoreMatrix.Transform(vec);
                var offset = vec.Z - LineToBeMoved.FullCurve.GetEndPoint(0).Z;
                xyz = new XYZ(0, 0, offset);
                Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                act.Start();
                try
                {
                    ElementTransformUtils.MoveElement(m_activeDocument, LineToBeMoved.Id, xyz);
                }
                catch (Exception e)
                {
                    TaskDialog.Show("Exception", e.Message);
                    return false;
                }

                act.Commit();

                var line = Drawing.UGridLines2D[Drawing.SelectedUIndex];
                line.StartPoint = new Point(line.StartPoint.X, line.StartPoint.Y + MoveOffset);
                line.EndPoint = new Point(line.EndPoint.X, line.EndPoint.Y + MoveOffset);

                GraphicsPath path = new();
                path.AddLine(line.StartPoint, line.EndPoint);
                Drawing.ULinePathList[Drawing.SelectedUIndex] = path;

                var pathList = Drawing.USegLinePathListList[Drawing.SelectedUIndex];
                var segLineList = line.Segments;
                for (var i = 0; i < segLineList.Count; i++)
                {
                    var segLine2D = segLineList[i];
                    segLine2D.StartPoint = new Point(segLine2D.StartPoint.X, segLine2D.StartPoint.Y + MoveOffset);
                    segLine2D.EndPoint = new Point(segLine2D.EndPoint.X, segLine2D.EndPoint.Y + MoveOffset);

                    GraphicsPath gpath = new();
                    path.AddLine(segLine2D.StartPoint, segLine2D.EndPoint);
                    pathList[i] = gpath;
                }
            }
            // move a V line along the U direction
            else if (-1 != Drawing.SelectedVIndex)
            {
                // convert the 2D data to 3D
                XYZ xyz = new(mousePosition.X, mousePosition.Y, 0);
                Vector4 vec = new(xyz);
                vec = Drawing.Coordinates.RestoreMatrix.Transform(vec);
                var offset = vec.X - LineToBeMoved.FullCurve.GetEndPoint(0).X;
                xyz = new XYZ(offset, 0, 0);

                Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                act.Start();
                try
                {
                    ElementTransformUtils.MoveElement(m_activeDocument, LineToBeMoved.Id, xyz);
                }
                catch (Exception e)
                {
                    TaskDialog.Show("Exception", e.Message);
                    return false;
                }

                act.Commit();

                var line = Drawing.VGridLines2D[Drawing.SelectedVIndex];
                line.StartPoint = new Point(line.StartPoint.X + MoveOffset, line.StartPoint.Y);
                line.EndPoint = new Point(line.EndPoint.X + MoveOffset, line.EndPoint.Y);

                GraphicsPath path = new();
                path.AddLine(line.StartPoint, line.EndPoint);
                Drawing.VLinePathList[Drawing.SelectedVIndex] = path;

                var pathList = Drawing.VSegLinePathListList[Drawing.SelectedVIndex];
                var segLineList = line.Segments;
                for (var i = 0; i < segLineList.Count; i++)
                {
                    var segLine2D = segLineList[i];
                    segLine2D.StartPoint = new Point(segLine2D.StartPoint.X + MoveOffset, segLine2D.StartPoint.Y);
                    segLine2D.EndPoint = new Point(segLine2D.EndPoint.X + MoveOffset, segLine2D.EndPoint.Y);

                    GraphicsPath gpath = new();
                    path.AddLine(segLine2D.StartPoint, segLine2D.EndPoint);
                    pathList[i] = gpath;
                }
            }

            // line moved, the segment information changed, so reload all the geometry data
            ReloadGeometryData();

            Drawing.DrawObject.Clear();
            return true;
        }

        public void AddAllMullions()
        {
            Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            try
            {
                // add mullions to all U grid lines
                foreach (var line in UGridLines)
                {
                    line.AddMullions(line.AllSegmentCurves.get_Item(0), MullionType, false);
                }

                // add mullions to all V grid lines
                foreach (var line in VGridLines)
                {
                    line.AddMullions(line.AllSegmentCurves.get_Item(0), MullionType, false);
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("Exception", e.Message);
                return;
            }

            act.Commit();
        }

        public void DeleteAllMullions()
        {
            Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            try
            {
                var mullions = ActiveGrid.GetMullionIds();

                foreach (var e in mullions)
                {
                    var mullion = m_activeDocument.GetElement(e) as Mullion;

                    // Exceptions may jump out if attempting to delete "Locked" mullions
                    if (mullion.Lockable && mullion.Lock) mullion.Lock = false;

                    m_activeDocument.Delete(mullion.Id);
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("Exception", e.Message);
                return;
            }

            act.Commit();
        }

        private void GetULines()
        {
            UGridLines.Clear();
            Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            //ElementSet uLines = m_activeGrid.UGridLines;
            var uLines = ActiveGrid.GetUGridLineIds();
            act.Commit();
            if (0 == uLines.Count) return;

            foreach (var e in uLines)
            {
                var line = m_activeDocument.GetElement(e) as CurtainGridLine;

                UGridLines.Add(line);
            }
        }

        private void GetVLines()
        {
            VGridLines.Clear();
            Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            //ElementSet vLines = m_activeGrid.VGridLines;
            var vLines = ActiveGrid.GetVGridLineIds();
            act.Commit();
            if (0 == vLines.Count) return;

            foreach (var e in vLines)
            {
                var line = m_activeDocument.GetElement(e) as CurtainGridLine;

                VGridLines.Add(line);
            }
        }

        private bool GetCurtainGridVertexes()
        {
            // even in "ReloadGeometryData()" method, no need to reload the boundary information
            // (as the boundary of the curtain grid won't be changed in the sample)
            // just need to load it after the curtain wall been created
            if (null != GridVertexesXyz && 0 < GridVertexesXyz.Count) return true;

            // the curtain grid is from "Curtain Wall: Curtain Wall 1" (by default, the "Curtain Wall 1" has no U/V grid lines)
            if (UGridLines.Count <= 0 || VGridLines.Count <= 0)
            {
                // special handling for "Curtain Wall: Curtain Wall 1"
                // as the "Curtain Wall: Curtain Wall 1" has no U/V grid lines, so we can't compute the boundary from the grid lines
                // as that kind of curtain wall contains only one curtain cell
                // so we compute the boundary from the data of the curtain cell
                // Obtain the geometry information of the curtain wall
                // also works with some curtain grids with only U grid lines or only V grid lines
                Transaction act = new(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                act.Start();
                var cells = ActiveGrid.GetCurtainCells();
                act.Commit();
                XYZ minXyz = new(double.MaxValue, double.MaxValue, double.MaxValue);
                XYZ maxXyz = new(double.MinValue, double.MinValue, double.MinValue);
                GetVertexesByCells(cells, ref minXyz, ref maxXyz);

                // move the U & V lines to the boundary of the curtain grid, and get their end points as the vertexes of the curtain grid
                GridVertexesXyz.Add(new XYZ(minXyz.X, minXyz.Y, minXyz.Z));
                GridVertexesXyz.Add(new XYZ(maxXyz.X, maxXyz.Y, minXyz.Z));
                GridVertexesXyz.Add(new XYZ(maxXyz.X, maxXyz.Y, maxXyz.Z));
                GridVertexesXyz.Add(new XYZ(minXyz.X, minXyz.Y, maxXyz.Z));
                return true;
            }

            // handling for the other kinds of curtain walls (contains U&V grid lines by default)
            var uLine = UGridLines[0];
            var vLine = VGridLines[0];

            List<XYZ> points = [];

            var uStartPoint = uLine.FullCurve.GetEndPoint(0);
            var uEndPoint = uLine.FullCurve.GetEndPoint(1);

            var vStartPoint = vLine.FullCurve.GetEndPoint(0);
            var vEndPoint = vLine.FullCurve.GetEndPoint(1);

            points.Add(uStartPoint);
            points.Add(uEndPoint);
            points.Add(vStartPoint);
            points.Add(vEndPoint);

            //move the U & V lines to the boundary of the curtain grid, and get their end points as the vertexes of the curtain grid
            GridVertexesXyz.Add(new XYZ(uStartPoint.X, uStartPoint.Y, vStartPoint.Z));
            GridVertexesXyz.Add(new XYZ(uEndPoint.X, uEndPoint.Y, vStartPoint.Z));
            GridVertexesXyz.Add(new XYZ(uEndPoint.X, uEndPoint.Y, vEndPoint.Z));
            GridVertexesXyz.Add(new XYZ(uStartPoint.X, uStartPoint.Y, vEndPoint.Z));

            return true;
        }

        private List<XYZ> GetPoints(ICollection<CurtainCell> cells)
        {
            List<XYZ> points = [];

            if (null == cells || cells.Count == 0) return points;

            foreach (var cell in cells)
            {
                if (null == cell || 0 == cell.CurveLoops.Size) continue;

                var curves = cell.CurveLoops.get_Item(0);

                foreach (Curve curve in curves)
                {
                    points.Add(curve.GetEndPoint(0));
                    points.Add(curve.GetEndPoint(1));
                }
            }

            return points;
        }

        private void GetVertexesByPoints(List<XYZ> points, ref XYZ minXyz, ref XYZ maxXyz)
        {
            if (null == points || 0 == points.Count) return;

            var minX = minXyz.X;
            var minY = minXyz.Y;
            var minZ = minXyz.Z;
            var maxX = maxXyz.X;
            var maxY = maxXyz.Y;
            var maxZ = maxXyz.Z;

            foreach (var xyz in points)
            {
                // compare the values and update the min and max value
                if (xyz.X < minX)
                {
                    minX = xyz.X;
                    minY = xyz.Y;
                }

                if (xyz.X > maxX)
                {
                    maxX = xyz.X;
                    maxY = xyz.Y;
                }

                if (xyz.Z < minZ) minZ = xyz.Z;
                if (xyz.Z > maxZ) maxZ = xyz.Z;
            } // end of loop

            minXyz = new XYZ(minX, minY, minZ);
            maxXyz = new XYZ(maxX, maxY, maxZ);
        }

        private void GetVertexesByCells(ICollection<CurtainCell> cells, ref XYZ minXyz, ref XYZ maxXyz)
        {
            if (null == cells || cells.Count == 0) return;

            var points = GetPoints(cells);
            GetVertexesByPoints(points, ref minXyz, ref maxXyz);
        }

        private void MimicRemoveSegments(ref bool canRemove, List<SegmentLine2D> removeList)
        {
            // the currently operated is a U segment
            if (-1 != Drawing.SelectedUIndex)
            {
                var gridLine2D = Drawing.UGridLines2D[Drawing.SelectedUIndex];
                var segLine2D = gridLine2D.Segments[Drawing.SelectedUSegmentIndex];

                // the to-be-deleted segment is the last one of the grid line, it's not allowed to delete it
                var existingNumber = gridLine2D.Segments.Count - gridLine2D.RemovedNumber;
                if (1 == existingNumber)
                {
                    canRemove = false;
                    return;
                }

                // simulative deletion
                gridLine2D.RemovedNumber++;
                segLine2D.Removed = true;
                gridLine2D.Segments[Drawing.SelectedUSegmentIndex] = segLine2D;
                removeList.Add(segLine2D);
                // the "regeneration" step: if there're only 2 segments existing in one joint and they're in the same line, delete one seg will cause the other 
                // been deleted automatically
                MimicRecursiveDelete(ref canRemove, segLine2D, removeList);
            }
            // the currently operated is a V segment
            else if (-1 != Drawing.SelectedVIndex)
            {
                var gridLine2D = Drawing.VGridLines2D[Drawing.SelectedVIndex];
                var segLine2D = gridLine2D.Segments[Drawing.SelectedVSegmentIndex];

                var existingNumber = gridLine2D.Segments.Count - gridLine2D.RemovedNumber;
                // the to-be-deleted segment is the last one of the grid line, it's not allowed to delete it
                if (1 == existingNumber)
                {
                    canRemove = false;
                    return;
                }

                // simulative deletion
                gridLine2D.RemovedNumber++;
                segLine2D.Removed = true;
                gridLine2D.Segments[Drawing.SelectedVSegmentIndex] = segLine2D;
                removeList.Add(segLine2D);
                // the "regeneration" step: if there're only 2 segments existing in one joint and they're in the same line, delete one seg will cause the other 
                // been deleted automatically
                MimicRecursiveDelete(ref canRemove, segLine2D, removeList);
            }
        }

        private void MimicRecursiveDelete(ref bool canRemove, SegmentLine2D segLine2D, List<SegmentLine2D> removeList)
        {
            // the "regeneration" step: if there're only 2 segments existing in one joint 
            // and they're in the same line, delete one seg will cause the other 
            // been deleted automatically
            // get conjoint U line segments
            List<SegmentLine2D> removeSegments = [];
            Drawing.GetConjointSegments(segLine2D, removeSegments);

            // there's no isolated segment need to be removed automatically
            if (null == removeSegments || 0 == removeSegments.Count)
                // didn't "remove last segment of the curtain grid line", all the operations are valid. so return true
                return;

            // there're conjoint segments need to be removed automatically
            // removed automatically because of the deletion of this newly removed segment
            if (segLine2D.Removed)
                foreach (var seg in removeSegments)
                {
                    MimicRemoveSegment(ref canRemove, seg, removeList);

                    if (false == canRemove) return;
                    // recursive calling
                    MimicRecursiveDelete(ref canRemove, seg, removeList);
                }
        }

        private void MimicRemoveSegment(ref bool canRemove, SegmentLine2D seg, List<SegmentLine2D> removeList)
        {
            var gridLineIndex = seg.GridLineIndex;
            var segIndex = seg.SegmentIndex;

            if (-1 != gridLineIndex && -1 != segIndex)
            {
                var grid = seg.IsUSegment ? Drawing.UGridLines2D[gridLineIndex] : Drawing.VGridLines2D[gridLineIndex];

                // the last segment of the grid line
                var existingNumber = grid.Segments.Count - grid.RemovedNumber;
                if (1 == existingNumber)
                {
                    canRemove = false;
                    return;
                }

                grid.RemovedNumber++;
                var seg2D = grid.Segments[segIndex];
                seg2D.Removed = true;
                grid.Segments[segIndex] = seg2D;

                removeList.Add(seg2D);
            }
        }
    } // end of class
}
