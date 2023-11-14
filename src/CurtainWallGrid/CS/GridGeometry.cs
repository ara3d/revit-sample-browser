//
// (C) Copyright 2003-2019 by Autodesk, Inc.
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE. AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.
//

using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Point = System.Drawing.Point;

namespace Revit.SDK.Samples.CurtainWallGrid.CS
{
    /// <summary>
    ///     manages the behaviors & operations of CurtainGrid
    /// </summary>
    public class GridGeometry
    {
        // the active document of Revit
        private readonly Document m_activeDocument;

        // stores the curtain grid information of the created curtain wall

        // the referred drawing class for the curtain grid

        //object which contains reference of Revit Application
        private ExternalCommandData m_commandData;

        // the document of this sample
        private readonly MyDocument m_myDocument;


        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="myDoc">
        ///     the document of the sample
        /// </param>
        public GridGeometry(MyDocument myDoc)
        {
            m_myDocument = myDoc;
            m_commandData = myDoc.CommandData;
            m_activeDocument = myDoc.Document;
            GridProperties = new GridProperties();
            //m_activeGrid = grid;
            Drawing = new GridDrawing(myDoc, this);
            UGridLines = new List<CurtainGridLine>();
            VGridLines = new List<CurtainGridLine>();
            GridVertexesXYZ = new List<XYZ>();
        }

        // store the mullion type used in this sample

        // all the grid lines of U direction (in CurtainGridLine format)

        // all the grid lines of V direction (in CurtainGridLine format)

        // stores all the vertexes of the curtain grid (in Autodesk.Revit.DB.XYZ format)

        // stores all the properties of the curtain grid

        // store the grid line to be removed

        // store the offset to be moved for the specified grid line


        /// <summary>
        ///     stores the curtain grid information of the created curtain wall
        /// </summary>
        public CurtainGrid ActiveGrid { get; private set; }

        /// <summary>
        ///     the referred drawing class for the curtain grid
        /// </summary>
        public GridDrawing Drawing { get; }

        /// <summary>
        ///     store the mullion type used in this sample
        /// </summary>
        public MullionType MullionType { get; set; }

        /// <summary>
        ///     all the grid lines of U direction (in CurtainGridLine format)
        /// </summary>
        public List<CurtainGridLine> UGridLines { get; }

        /// <summary>
        ///     all the grid lines of V direction (in CurtainGridLine format)
        /// </summary>
        public List<CurtainGridLine> VGridLines { get; }

        /// <summary>
        ///     stores all the vertexes of the curtain grid (in Autodesk.Revit.DB.XYZ format)
        /// </summary>
        public List<XYZ> GridVertexesXYZ { get; }

        /// <summary>
        ///     stores all the properties of the curtain grid
        /// </summary>
        public GridProperties GridProperties { get; }

        public CurtainGridLine LineToBeMoved { get; private set; }

        /// <summary>
        ///     store the offset to be moved for the specified grid line
        /// </summary>
        public int MoveOffset { get; set; }

        /// <summary>
        ///     obtain all the properties of the curtain grid
        /// </summary>
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
            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
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

        /// <summary>
        ///     reload all the geometry data of the curtain grid (grid lines, vertexes, and convert them to 2D format)
        /// </summary>
        public void ReloadGeometryData()
        {
            if (null == ActiveGrid)
            {
                if (m_myDocument.WallCreated)
                    ActiveGrid = m_myDocument.CurtainWall.CurtainGrid;
                else
                    return;
            }

            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            //ElementSet mullions = m_activeGrid.Mullions;
            var mullions = ActiveGrid.GetMullionIds();

            act.Commit();
            if (0 == mullions.Count)
                foreach (var e in mullions)
                {
                    var mullion = m_activeDocument.GetElement(e) as Mullion;

                    if (null != mullion)
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

        /// <summary>
        ///     remove the selected segment from the curtain grid
        /// </summary>
        public void RemoveSegment()
        {
            // verify that the mouse is inside the curtain grid area
            var lines2D = Drawing.DrawObject.Lines2D;
            if (lines2D.Count < 1) return;

            var toBeRemovedList = new List<SegmentLine2D>();
            // check whether the deletion is valid
            var canRemove = true;
            MimicRemoveSegments(ref canRemove, toBeRemovedList);
            // in the "MimicRemove" process, we didn't find that we need to "Remove the last segment of the grid line"
            // so the "Remove" action can go on
            if (canRemove)
            {
                try
                {
                    var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                    act.Start();
                    foreach (var seg2D in toBeRemovedList)
                    {
                        var gridLineIndex = seg2D.GridLineIndex;
                        var segIndex = seg2D.SegmentIndex;
                        var isUSegment = seg2D.IsUSegment;

                        CurtainGridLine line;
                        if (isUSegment)
                            line = UGridLines[gridLineIndex];
                        else
                            line = VGridLines[gridLineIndex];
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
                    GridLine2D gridLine2D;

                    if (isUSegment)
                        gridLine2D = Drawing.UGridLines2D[gridLineIndex];
                    else
                        gridLine2D = Drawing.VGridLines2D[gridLineIndex];

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

        /// <summary>
        ///     add a new segment to the specified location
        /// </summary>
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
                        var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());

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
                        var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
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

        /// <summary>
        ///     add all the deleted segments back for a grid line
        /// </summary>
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
                        var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
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
                foreach (var segLine2D in gridLine2D.Segments) segLine2D.Removed = false;
            }
            else if (-1 != Drawing.SelectedVIndex)
            {
                var line = VGridLines[Drawing.SelectedVIndex];
                if (null != line)
                    try
                    {
                        var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
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
                foreach (var segLine2D in gridLine2D.Segments) segLine2D.Removed = false;
            }

            ReloadGeometryData();
            Drawing.DrawObject.Clear();
        }

        /// <summary>
        ///     add a new U grid line to the specified location
        /// </summary>
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

            // get the point to be added
            var midX = (line2D.StartPoint.X + line2D.EndPoint.X) / 2;
            var midY = (line2D.StartPoint.Y + line2D.EndPoint.Y) / 2;
            // transform the 2D point to Autodesk.Revit.DB.XYZ format
            var pos = new XYZ(midX, midY, 0);
            var vec = new Vector4(pos);
            vec = Drawing.Coordinates.RestoreMatrix.Transform(vec);

            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
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

        /// <summary>
        ///     add a new V grid line to the specified location
        /// </summary>
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

            // get the point to be added
            var midX = (line2D.StartPoint.X + line2D.EndPoint.X) / 2;
            var midY = (line2D.StartPoint.Y + line2D.EndPoint.Y) / 2;
            // transform the 2D point to Autodesk.Revit.DB.XYZ format
            var pos = new XYZ(midX, midY, 0);
            var vec = new Vector4(pos);
            vec = Drawing.Coordinates.RestoreMatrix.Transform(vec);

            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
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

        /// <summary>
        ///     toggle the selected grid line's Lock status:  if it's locked, unlock it, vice versa
        /// </summary>
        public void LockOrUnlockSelectedGridLine()
        {
            CurtainGridLine line = null;
            var line2D = new GridLine2D();

            // get the selected grid line
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
                var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                act.Start();
                line.Lock = !line.Lock;
                act.Commit();
            }

            // update the mapped line2D's data
            line2D.Locked = line.Lock;

            // clear the intermediate variables and instances
            Drawing.DrawObject.Clear();
        }

        /// <summary>
        ///     get the grid line to be removed
        /// </summary>
        /// <returns>
        ///     if the line obtained, return true; otherwise false
        /// </returns>
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

        /// <summary>
        ///     move the selected grid line to the location of the mouse cursor
        /// </summary>
        /// <param name="mousePosition">
        ///     indicates the destination position of the grid line
        /// </param>
        /// <returns>
        ///     return whether the grid line be moved successfully
        /// </returns>
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
                var xyz = new XYZ(mousePosition.X, mousePosition.Y, 0);
                var vec = new Vector4(xyz);
                vec = Drawing.Coordinates.RestoreMatrix.Transform(vec);
                var offset = vec.Z - LineToBeMoved.FullCurve.GetEndPoint(0).Z;
                xyz = new XYZ(0, 0, offset);
                var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
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

                // update the grid line 2d
                var line = Drawing.UGridLines2D[Drawing.SelectedUIndex];
                line.StartPoint = new Point(line.StartPoint.X, line.StartPoint.Y + MoveOffset);
                line.EndPoint = new Point(line.EndPoint.X, line.EndPoint.Y + MoveOffset);

                // update the mapped grid line graphics path
                var path = new GraphicsPath();
                path.AddLine(line.StartPoint, line.EndPoint);
                Drawing.ULinePathList[Drawing.SelectedUIndex] = path;

                // update the mapped segment line and its graphics path
                var pathList = Drawing.USegLinePathListList[Drawing.SelectedUIndex];
                var segLineList = line.Segments;
                for (var i = 0; i < segLineList.Count; i++)
                {
                    // update the segment
                    var segLine2D = segLineList[i];
                    segLine2D.StartPoint = new Point(segLine2D.StartPoint.X, segLine2D.StartPoint.Y + MoveOffset);
                    segLine2D.EndPoint = new Point(segLine2D.EndPoint.X, segLine2D.EndPoint.Y + MoveOffset);

                    // update the segment's graphics path
                    var gpath = new GraphicsPath();
                    path.AddLine(segLine2D.StartPoint, segLine2D.EndPoint);
                    pathList[i] = gpath;
                }
            }
            // move a V line along the U direction
            else if (-1 != Drawing.SelectedVIndex)
            {
                // convert the 2D data to 3D
                var xyz = new XYZ(mousePosition.X, mousePosition.Y, 0);
                var vec = new Vector4(xyz);
                vec = Drawing.Coordinates.RestoreMatrix.Transform(vec);
                var offset = vec.X - LineToBeMoved.FullCurve.GetEndPoint(0).X;
                xyz = new XYZ(offset, 0, 0);

                var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
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

                // update the grid line 2d
                var line = Drawing.VGridLines2D[Drawing.SelectedVIndex];
                line.StartPoint = new Point(line.StartPoint.X + MoveOffset, line.StartPoint.Y);
                line.EndPoint = new Point(line.EndPoint.X + MoveOffset, line.EndPoint.Y);

                // update the mapped grid line graphics path
                var path = new GraphicsPath();
                path.AddLine(line.StartPoint, line.EndPoint);
                Drawing.VLinePathList[Drawing.SelectedVIndex] = path;

                // update the mapped segment line and its graphics path
                var pathList = Drawing.VSegLinePathListList[Drawing.SelectedVIndex];
                var segLineList = line.Segments;
                for (var i = 0; i < segLineList.Count; i++)
                {
                    // update the segment
                    var segLine2D = segLineList[i];
                    segLine2D.StartPoint = new Point(segLine2D.StartPoint.X + MoveOffset, segLine2D.StartPoint.Y);
                    segLine2D.EndPoint = new Point(segLine2D.EndPoint.X + MoveOffset, segLine2D.EndPoint.Y);

                    // update the segment's graphics path
                    var gpath = new GraphicsPath();
                    path.AddLine(segLine2D.StartPoint, segLine2D.EndPoint);
                    pathList[i] = gpath;
                }
            }

            // line moved, the segment information changed, so reload all the geometry data
            ReloadGeometryData();

            Drawing.DrawObject.Clear();
            return true;
        }

        /// <summary>
        ///     add mullions to all the segments of the curtain grid
        ///     due to the limitations of Mullions, it's not available yet to add mullions to the
        ///     edges of the curtain grid as Revit UI does
        /// </summary>
        public void AddAllMullions()
        {
            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            try
            {
                // add mullions to all U grid lines
                foreach (var line in UGridLines)
                    line.AddMullions(line.AllSegmentCurves.get_Item(0), MullionType, false);
                // add mullions to all V grid lines
                foreach (var line in VGridLines)
                    line.AddMullions(line.AllSegmentCurves.get_Item(0), MullionType, false);
            }
            catch (Exception e)
            {
                TaskDialog.Show("Exception", e.Message);
                return;
            }

            act.Commit();
        }

        /// <summary>
        ///     delete all the mullions of the curtain grid
        /// </summary>
        public void DeleteAllMullions()
        {
            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
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

        /// <summary>
        ///     get all the U grid lines' data of the curtain grid
        /// </summary>
        private void GetULines()
        {
            UGridLines.Clear();
            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
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

        /// <summary>
        ///     get all the V grid lines' data of the curtain grid
        /// </summary>
        private void GetVLines()
        {
            VGridLines.Clear();
            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
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

        /// <summary>
        ///     get all of the 4 vertexes of the curtain grid
        /// </summary>
        /// <returns></returns>
        private bool GetCurtainGridVertexes()
        {
            // even in "ReloadGeometryData()" method, no need to reload the boundary information
            // (as the boundary of the curtain grid won't be changed in the sample)
            // just need to load it after the curtain wall been created
            if (null != GridVertexesXYZ && 0 < GridVertexesXYZ.Count) return true;

            // the curtain grid is from "Curtain Wall: Curtain Wall 1" (by default, the "Curtain Wall 1" has no U/V grid lines)
            if (UGridLines.Count <= 0 || VGridLines.Count <= 0)
            {
                // special handling for "Curtain Wall: Curtain Wall 1"
                // as the "Curtain Wall: Curtain Wall 1" has no U/V grid lines, so we can't compute the boundary from the grid lines
                // as that kind of curtain wall contains only one curtain cell
                // so we compute the boundary from the data of the curtain cell
                // Obtain the geometry information of the curtain wall
                // also works with some curtain grids with only U grid lines or only V grid lines
                var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                act.Start();
                var cells = ActiveGrid.GetCurtainCells();
                act.Commit();
                var minXYZ = new XYZ(double.MaxValue, double.MaxValue, double.MaxValue);
                var maxXYZ = new XYZ(double.MinValue, double.MinValue, double.MinValue);
                GetVertexesByCells(cells, ref minXYZ, ref maxXYZ);

                // move the U & V lines to the boundary of the curtain grid, and get their end points as the vertexes of the curtain grid
                GridVertexesXYZ.Add(new XYZ(minXYZ.X, minXYZ.Y, minXYZ.Z));
                GridVertexesXYZ.Add(new XYZ(maxXYZ.X, maxXYZ.Y, minXYZ.Z));
                GridVertexesXYZ.Add(new XYZ(maxXYZ.X, maxXYZ.Y, maxXYZ.Z));
                GridVertexesXYZ.Add(new XYZ(minXYZ.X, minXYZ.Y, maxXYZ.Z));
                return true;
            }

            // handling for the other kinds of curtain walls (contains U&V grid lines by default)
            var uLine = UGridLines[0];
            var vLine = VGridLines[0];

            var points = new List<XYZ>();

            var uStartPoint = uLine.FullCurve.GetEndPoint(0);
            var uEndPoint = uLine.FullCurve.GetEndPoint(1);

            var vStartPoint = vLine.FullCurve.GetEndPoint(0);
            var vEndPoint = vLine.FullCurve.GetEndPoint(1);

            points.Add(uStartPoint);
            points.Add(uEndPoint);
            points.Add(vStartPoint);
            points.Add(vEndPoint);

            //move the U & V lines to the boundary of the curtain grid, and get their end points as the vertexes of the curtain grid
            GridVertexesXYZ.Add(new XYZ(uStartPoint.X, uStartPoint.Y, vStartPoint.Z));
            GridVertexesXYZ.Add(new XYZ(uEndPoint.X, uEndPoint.Y, vStartPoint.Z));
            GridVertexesXYZ.Add(new XYZ(uEndPoint.X, uEndPoint.Y, vEndPoint.Z));
            GridVertexesXYZ.Add(new XYZ(uStartPoint.X, uStartPoint.Y, vEndPoint.Z));

            return true;
        }

        /// <summary>
        ///     get all the vertexes of the curtain cells
        /// </summary>
        /// <param name="cells">
        ///     the curtain cells which need to be got the vertexes
        /// </param>
        /// <returns>
        ///     the vertexes of the curtain cells
        /// </returns>
        private List<XYZ> GetPoints(ICollection<CurtainCell> cells)
        {
            var points = new List<XYZ>();

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

        /// <summary>
        ///     get a bounding box which covers all the input points
        /// </summary>
        /// <param name="points">
        ///     the source points
        /// </param>
        /// <param name="minXYZ">
        ///     one of the bounding box points
        /// </param>
        /// <param name="maxXYZ">
        ///     one of the bounding box points
        /// </param>
        private void GetVertexesByPoints(List<XYZ> points, ref XYZ minXYZ, ref XYZ maxXYZ)
        {
            if (null == points || 0 == points.Count) return;

            var minX = minXYZ.X;
            var minY = minXYZ.Y;
            var minZ = minXYZ.Z;
            var maxX = maxXYZ.X;
            var maxY = maxXYZ.Y;
            var maxZ = maxXYZ.Z;

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

            minXYZ = new XYZ(minX, minY, minZ);
            maxXYZ = new XYZ(maxX, maxY, maxZ);
        }

        /// <summary>
        ///     get the vertexes of the bounding box which covers all the curtain cells
        /// </summary>
        /// <param name="cells">
        ///     the source curtain cells
        /// </param>
        /// <param name="minXYZ">
        ///     the result bounding point
        /// </param>
        /// <param name="maxXYZ">
        ///     the result bounding point
        /// </param>
        private void GetVertexesByCells(ICollection<CurtainCell> cells, ref XYZ minXYZ, ref XYZ maxXYZ)
        {
            if (null == cells || cells.Count == 0) return;

            var points = GetPoints(cells);
            GetVertexesByPoints(points, ref minXYZ, ref maxXYZ);
        }

        /// <summary>
        ///     a simulative "Delete Segment" operation before real deletion
        ///     as we may occur some situations that prevent us to delete the specific segment
        ///     for example, delete the specific segment will make some other segments to be deleted automatically (the "conjoint"
        ///     ones)
        ///     and the "automatically deleted" segment is the last segment of its parent grid line
        ///     in this situation, we should prevent deleting that specific segment and rollback all the simulative deletion
        /// </summary>
        /// <param name="removeList">
        ///     the refferred to-be-removed list, in the simulative deletion operation, all the suitable (not the last segment)
        ///     segments will
        ///     be added to that list
        /// </param>
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

        /// <summary>
        ///     the "regeneration" step: if there're only 2 segments existing in one joint and they're in the same line,
        ///     delete one seg will cause the other been deleted automatically
        /// </summary>
        /// <param name="segLine2D">
        ///     the to-be-automatically-deleted segment
        /// </param>
        /// <param name="removeList">
        ///     the referred to-be-deleted list of the segments
        /// </param>
        /// <returns>
        ///     returns the operation result: if there's no "last" segment in the deletion operation, return true; otherwise false
        /// </returns>
        private void MimicRecursiveDelete(ref bool canRemove, SegmentLine2D segLine2D, List<SegmentLine2D> removeList)
        {
            // the "regeneration" step: if there're only 2 segments existing in one joint 
            // and they're in the same line, delete one seg will cause the other 
            // been deleted automatically
            // get conjoint U line segments
            var removeSegments = new List<SegmentLine2D>();
            Drawing.GetConjointSegments(segLine2D, removeSegments);

            // there's no isolated segment need to be removed automatically
            if (null == removeSegments || 0 == removeSegments.Count)
                // didn't "remove last segment of the curtain grid line", all the operations are valid. so return true
                return;

            // there're conjoint segments need to be removed automatically
            // add the segments to removeList first, and compute whether other segments need to be 
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

        /// <summary>
        ///     remove the segment from the grid line
        /// </summary>
        /// <param name="canRemove">
        ///     the returned result value, indicates whether the segment can be removed (is NOT the last segment)
        /// </param>
        /// <param name="seg">
        ///     the to-be-removed segment
        /// </param>
        /// <param name="removeList">
        ///     the referred to-be-deleted list of the segments
        /// </param>
        private void MimicRemoveSegment(ref bool canRemove, SegmentLine2D seg, List<SegmentLine2D> removeList)
        {
            var gridLineIndex = seg.GridLineIndex;
            var segIndex = seg.SegmentIndex;

            if (-1 != gridLineIndex && -1 != segIndex)
            {
                // update the gridline2d and segmentline2d data
                GridLine2D grid;
                if (seg.IsUSegment)
                    grid = Drawing.UGridLines2D[gridLineIndex];
                else
                    grid = Drawing.VGridLines2D[gridLineIndex];

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