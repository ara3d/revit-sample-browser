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
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.CurtainWallGrid.CS
{
    /// <summary>
    /// manages the behaviors & operations of CurtainGrid
    /// </summary>
    public class GridGeometry
    {
        #region Fields
        // the document of this sample
        private MyDocument m_myDocument;

        // stores the curtain grid information of the created curtain wall
        private CurtainGrid m_activeGrid;

        // the referred drawing class for the curtain grid
        private GridDrawing m_drawing;

        //object which contains reference of Revit Application
        private ExternalCommandData m_commandData;

        // the active document of Revit
        private Document m_activeDocument;

        // store the mullion type used in this sample
        private MullionType m_mullionType;

        // all the grid lines of U direction (in CurtainGridLine format)
        private List<CurtainGridLine> m_uGridLines;

        // all the grid lines of V direction (in CurtainGridLine format)
        private List<CurtainGridLine> m_vGridLines;

        // stores all the vertexes of the curtain grid (in Autodesk.Revit.DB.XYZ format)
        private List<Autodesk.Revit.DB.XYZ> m_GridVertexesXYZ;

        // stores all the properties of the curtain grid
        private GridProperties m_gridProperties;

        // store the grid line to be removed
        private CurtainGridLine m_lineToBeMoved = null;

        // store the offset to be moved for the specified grid line
        private int m_moveOffset = 0;
        #endregion

        #region Properties
        /// <summary>
        /// stores the curtain grid information of the created curtain wall
        /// </summary>
        public CurtainGrid ActiveGrid
        {
            get
            {
                return m_activeGrid;
            }
        }

        /// <summary>
        /// the referred drawing class for the curtain grid
        /// </summary>
        public GridDrawing Drawing
        {
            get
            {
                return m_drawing;
            }
        }

        /// <summary>
        /// store the mullion type used in this sample
        /// </summary>
        public MullionType MullionType
        {
            get
            {
                return m_mullionType;
            }
            set
            {
                m_mullionType = value;
            }
        }

        /// <summary>
        /// all the grid lines of U direction (in CurtainGridLine format)
        /// </summary>
        public List<CurtainGridLine> UGridLines
        {
            get
            {
                return m_uGridLines;
            }
        }

        /// <summary>
        /// all the grid lines of V direction (in CurtainGridLine format)
        /// </summary>
        public List<CurtainGridLine> VGridLines
        {
            get
            {
                return m_vGridLines;
            }
        }

        /// <summary>
        /// stores all the vertexes of the curtain grid (in Autodesk.Revit.DB.XYZ format)
        /// </summary>
        public List<Autodesk.Revit.DB.XYZ> GridVertexesXYZ
        {
            get
            {
                return m_GridVertexesXYZ;
            }
        }

        /// <summary>
        /// stores all the properties of the curtain grid
        /// </summary>
        public GridProperties GridProperties
        {
            get
            {
                return m_gridProperties;
            }
        }

        public CurtainGridLine LineToBeMoved
        {
            get
            {
                return m_lineToBeMoved;
            }
        }

        /// <summary>
        /// store the offset to be moved for the specified grid line
        /// </summary>
        public int MoveOffset
        {
            get
            {
                return m_moveOffset;
            }
            set
            {
                m_moveOffset = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="myDoc">
        /// the document of the sample
        /// </param>
        public GridGeometry(MyDocument myDoc)
        {
            m_myDocument = myDoc;
            m_commandData = myDoc.CommandData;
            m_activeDocument = myDoc.Document;
            m_gridProperties = new GridProperties();
            //m_activeGrid = grid;
            m_drawing = new GridDrawing(myDoc, this);
            m_uGridLines = new List<CurtainGridLine>();
            m_vGridLines = new List<CurtainGridLine>();
            m_GridVertexesXYZ = new List<Autodesk.Revit.DB.XYZ>();
        }
        #endregion

        #region Public methods
        /// <summary>
        /// obtain all the properties of the curtain grid
        /// </summary>
        public void ReloadGridProperties()
        {
            if (null == m_activeGrid)
            {
                if (true == m_myDocument.WallCreated)
                {
                    m_activeGrid = m_myDocument.CurtainWall.CurtainGrid;
                }
                else
                {
                    return;
                }
            }

            // horizontal grid pattern 
            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            switch (m_activeGrid.Grid2Justification)
            {
                case CurtainGridAlignType.Beginning:
                    m_gridProperties.HorizontalJustification = CurtainGridAlign.Beginning;
                    break;
                case CurtainGridAlignType.Center:
                    m_gridProperties.HorizontalJustification = CurtainGridAlign.Center;
                    break;
                case CurtainGridAlignType.End:
                    m_gridProperties.HorizontalJustification = CurtainGridAlign.End;
                    break;
                default:
                    break;
            }
            m_gridProperties.HorizontalAngle = m_activeGrid.Grid2Angle * 180.0 / Math.PI;
            m_gridProperties.HorizontalOffset = m_activeGrid.Grid2Offset;
            m_gridProperties.HorizontalLinesNumber = m_activeGrid.NumULines;
            // vertical grid pattern
            switch (m_activeGrid.Grid1Justification)
            {
                case CurtainGridAlignType.Beginning:
                    m_gridProperties.VerticalJustification = CurtainGridAlign.Beginning;
                    break;
                case CurtainGridAlignType.Center:
                    m_gridProperties.VerticalJustification = CurtainGridAlign.Center;
                    break;
                case CurtainGridAlignType.End:
                    m_gridProperties.VerticalJustification = CurtainGridAlign.End;
                    break;
                default:
                    break;
            }
            m_gridProperties.VerticalAngle = m_activeGrid.Grid1Angle * 180.0 / Math.PI;
            m_gridProperties.VerticalOffset = m_activeGrid.Grid1Offset;
            m_gridProperties.VerticalLinesNumber = m_activeGrid.NumVLines;
            // other data
            m_gridProperties.PanelNumber = m_activeGrid.NumPanels;
            m_gridProperties.UnlockedPanelsNumber = m_activeGrid.GetUnlockedPanelIds().Count;
            m_gridProperties.CellNumber = m_activeGrid.GetCurtainCells().Count;
            if (0 != m_activeGrid.GetMullionIds().Count)
            {
                m_gridProperties.MullionsNumber = m_activeGrid.GetMullionIds().Count;
                m_gridProperties.UnlockedmullionsNumber = m_activeGrid.GetUnlockedMullionIds().Count;
            }
            act.Commit();
        }

        /// <summary>
        /// reload all the geometry data of the curtain grid (grid lines, vertexes, and convert them to 2D format)
        /// </summary>
        public void ReloadGeometryData()
        {
            if (null == m_activeGrid)
            {
                if (true == m_myDocument.WallCreated)
                {
                    m_activeGrid = m_myDocument.CurtainWall.CurtainGrid;
                }
                else
                {
                    return;
                }
            }

            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            //ElementSet mullions = m_activeGrid.Mullions;
            var mullions = m_activeGrid.GetMullionIds();

            act.Commit();
            if (0 == mullions.Count)
            {
                foreach (var e in mullions)
                {
                    var mullion = m_activeDocument.GetElement(e) as Mullion;

                    if (null != mullion)
                    {
                        m_mullionType = mullion.MullionType;
                        break;
                    }
                }
            }


            GetULines();
            GetVLines();
            GetCurtainGridVertexes();
            // covert those lines to 2D format
            m_drawing.GetLines2D();
        }

        /// <summary>
        /// remove the selected segment from the curtain grid
        /// </summary>
        public void RemoveSegment()
        {
            // verify that the mouse is inside the curtain grid area
            var lines2D = m_drawing.DrawObject.Lines2D;
            if (lines2D.Count < 1)
            {
                return;
            }

            var toBeRemovedList = new List<SegmentLine2D>();
            // check whether the deletion is valid
            var canRemove = true;
            MimicRemoveSegments(ref canRemove, toBeRemovedList);
            // in the "MimicRemove" process, we didn't find that we need to "Remove the last segment of the grid line"
            // so the "Remove" action can go on
            if (true == canRemove)
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
                        if (true == isUSegment)
                        {
                            line = m_uGridLines[gridLineIndex];
                        }
                        else
                        {
                            line = m_vGridLines[gridLineIndex];
                        }
                        var curve = line.AllSegmentCurves.get_Item(segIndex);
                        line.RemoveSegment(curve);
                    }
                    act.Commit();
                }
                catch (System.Exception e)
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

                    if (true == isUSegment)
                    {
                        gridLine2D = m_drawing.UGridLines2D[gridLineIndex];
                    }
                    else
                    {
                        gridLine2D = m_drawing.VGridLines2D[gridLineIndex];
                    }

                    gridLine2D.RemovedNumber--;
                    var segLine2D = gridLine2D.Segments[segIndex];
                    segLine2D.Removed = false;
                    gridLine2D.Segments[segIndex] = segLine2D;
                }
                var statusMsg = "Delete this segment will make some grid lines have no existent segments.";
                m_myDocument.Message = new KeyValuePair<string, bool>(statusMsg, true);
            }

            this.ReloadGeometryData();
            m_drawing.DrawObject.Clear();
        }

        /// <summary>
        /// add a new segment to the specified location
        /// </summary>
        public void AddSegment()
        {
            // verify that the mouse is inside the curtain grid area
            var lines2D = m_drawing.DrawObject.Lines2D;
            if (lines2D.Count < 1)
            {
                return;
            }

            // the selected segment location is on a U grid line
            if (-1 != m_drawing.SelectedUIndex)
            {
                var line = m_uGridLines[m_drawing.SelectedUIndex];
                var curve = line.AllSegmentCurves.get_Item(m_drawing.SelectedUSegmentIndex);
                if (null != line && null != curve)
                {
                    try
                    {
                        var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());

                        act.Start();
                        line.AddSegment(curve);
                        act.Commit();
                    }
                    catch (System.Exception e)
                    {
                        TaskDialog.Show("Exception", e.Message);
                        return;
                    }
                }

                var gridLine2D = m_drawing.UGridLines2D[m_drawing.SelectedUIndex];
                gridLine2D.RemovedNumber--;
                var segLine2D = gridLine2D.Segments[m_drawing.SelectedUSegmentIndex];
                segLine2D.Removed = false;
                gridLine2D.Segments[m_drawing.SelectedUSegmentIndex] = segLine2D;
            }
            // the selected segment location is on a V grid line
            else if (-1 != m_drawing.SelectedVIndex)
            {
                var line = m_vGridLines[m_drawing.SelectedVIndex];
                var curve = line.AllSegmentCurves.get_Item(m_drawing.SelectedVSegmentIndex);
                if (null != line && null != curve)
                {
                    try
                    {
                        var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                        act.Start();
                        line.AddSegment(curve);
                        act.Commit();
                    }
                    catch (System.Exception e)
                    {
                        TaskDialog.Show("Exception", e.Message);
                        return;
                    }
                }

                var gridLine2D = m_drawing.VGridLines2D[m_drawing.SelectedVIndex];
                gridLine2D.RemovedNumber--;
                var segLine2D = gridLine2D.Segments[m_drawing.SelectedVSegmentIndex];
                segLine2D.Removed = false;
                gridLine2D.Segments[m_drawing.SelectedVSegmentIndex] = segLine2D;
            }

            this.ReloadGeometryData();
            m_drawing.DrawObject.Clear();
        }

        /// <summary>
        /// add all the deleted segments back for a grid line
        /// </summary>
        public void AddAllSegments()
        {
            // verify that the mouse is inside the curtain grid area
            var lines2D = m_drawing.DrawObject.Lines2D;
            if (lines2D.Count < 1)
            {
                return;
            }

            if (-1 != m_drawing.SelectedUIndex)
            {
                var line = m_uGridLines[m_drawing.SelectedUIndex];
                if (null != line)
                {
                    try
                    {
                        var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                        act.Start();
                        line.AddAllSegments();
                        act.Commit();
                    }
                    catch (System.Exception e)
                    {
                        TaskDialog.Show("Exception", e.Message);
                        return;
                    }
                }

                var gridLine2D = m_drawing.UGridLines2D[m_drawing.SelectedUIndex];
                gridLine2D.RemovedNumber = 0;
                foreach (var segLine2D in gridLine2D.Segments)
                {
                    segLine2D.Removed = false;
                }
            }
            else if (-1 != m_drawing.SelectedVIndex)
            {
                var line = m_vGridLines[m_drawing.SelectedVIndex];
                if (null != line)
                {
                    try
                    {
                        var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                        act.Start();
                        line.AddAllSegments();
                        act.Commit();
                    }
                    catch (System.Exception e)
                    {
                        TaskDialog.Show("Exception", e.Message);
                        return;
                    }
                }

                var gridLine2D = m_drawing.VGridLines2D[m_drawing.SelectedVIndex];
                gridLine2D.RemovedNumber = 0;
                foreach (var segLine2D in gridLine2D.Segments)
                {
                    segLine2D.Removed = false;
                }
            }

            this.ReloadGeometryData();
            m_drawing.DrawObject.Clear();
        }

        /// <summary>
        /// add a new U grid line to the specified location
        /// </summary>
        public void AddUGridLine()
        {
            // verify that the mouse location is valid: it's inside the curtain grid area 
            // & it doesn't lap over another grid line (it's not allowed to add a grid line to lap over another one)
            if (false == m_drawing.MouseLocationValid)
            {
                return;
            }

            // all the assistant lines (in "Add U (Horizontal) Grid Line" operation, 
            // there's only one dash line, this line indicates the location to be added)
            var lines2D = m_drawing.DrawObject.Lines2D;
            if (lines2D.Count < 1)
            {
                return;
            }

            // the dash U line shown in the sample (incidates the location to be added)
            var line2D = lines2D[0].Key;
            if (System.Drawing.Point.Empty == line2D.StartPoint ||
                System.Drawing.Point.Empty == line2D.EndPoint)
            {
                return;
            }

            // get the point to be added
            var midX = (line2D.StartPoint.X + line2D.EndPoint.X) / 2;
            var midY = (line2D.StartPoint.Y + line2D.EndPoint.Y) / 2;
            // transform the 2D point to Autodesk.Revit.DB.XYZ format
            var pos = new Autodesk.Revit.DB.XYZ(midX, midY, 0);
            var vec = new Vector4(pos);
            vec = m_drawing.Coordinates.RestoreMatrix.Transform(vec);
            CurtainGridLine newLine;

            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            try
            {
                newLine = ActiveGrid.AddGridLine(true, new Autodesk.Revit.DB.XYZ(vec.X, vec.Y, vec.Z), false);
            }
            catch (System.Exception e)
            {
                TaskDialog.Show("Exception", e.Message);
                // "add U line" failed, so return directly
                return;
            }
            act.Commit();

            // U line added, the V line's segment information changed, so reload all the geometry data
            this.ReloadGeometryData();
        }

        /// <summary>
        /// add a new V grid line to the specified location
        /// </summary>
        public void AddVGridLine()
        {
            // verify that the mouse location is valid: it's inside the curtain grid area 
            // & it doesn't lap over another grid line (it's not allowed to add a grid line to lap over another one)
            if (false == m_drawing.MouseLocationValid)
            {
                return;
            }

            // all the assistant lines (in "Add V (Vertical) Grid Line" operation, 
            // there's only one dash line, this line indicates the location to be added)
            var lines2D = m_drawing.DrawObject.Lines2D;
            if (lines2D.Count < 1)
            {
                return;
            }

            // the dash V line shown in the sample (incidates the location to be added)
            var line2D = lines2D[0].Key;
            if (System.Drawing.Point.Empty == line2D.StartPoint ||
                System.Drawing.Point.Empty == line2D.EndPoint)
            {
                return;
            }

            // get the point to be added
            var midX = (line2D.StartPoint.X + line2D.EndPoint.X) / 2;
            var midY = (line2D.StartPoint.Y + line2D.EndPoint.Y) / 2;
            // transform the 2D point to Autodesk.Revit.DB.XYZ format
            var pos = new Autodesk.Revit.DB.XYZ(midX, midY, 0);
            var vec = new Vector4(pos);
            vec = m_drawing.Coordinates.RestoreMatrix.Transform(vec);
            CurtainGridLine newLine;

            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            try
            {
                newLine = ActiveGrid.AddGridLine(false, new Autodesk.Revit.DB.XYZ(vec.X, vec.Y, vec.Z), false);
            }
            catch (System.Exception e)
            {
                TaskDialog.Show("Exception", e.Message);
                // "add V line" failed, so return directly
                return;
            }
            act.Commit();

            // V line added, the U line's segment information changed, so reload all the geometry data
            this.ReloadGeometryData();
        }

        /// <summary>
        /// toggle the selected grid line's Lock status:  if it's locked, unlock it, vice versa
        /// </summary>
        public void LockOrUnlockSelectedGridLine()
        {
            CurtainGridLine line = null;
            var line2D = new GridLine2D();

            // get the selected grid line
            if (-1 != m_drawing.SelectedUIndex)
            {
                line = m_uGridLines[m_drawing.SelectedUIndex];
                line2D = m_drawing.UGridLines2D[m_drawing.SelectedUIndex];
            }
            else if (-1 != m_drawing.SelectedVIndex)
            {
                line = m_vGridLines[m_drawing.SelectedVIndex];
                line2D = m_drawing.VGridLines2D[m_drawing.SelectedVIndex];
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
            m_drawing.DrawObject.Clear();
        }

        /// <summary>
        /// get the grid line to be removed
        /// </summary>
        /// <returns>
        /// if the line obtained, return true; otherwise false
        /// </returns>
        public bool GetLineToBeMoved()
        {
            if (-1 != m_drawing.SelectedUIndex)
            {
                m_lineToBeMoved = m_uGridLines[m_drawing.SelectedUIndex];
                return true;
            }
            else if (-1 != m_drawing.SelectedVIndex)
            {
                m_lineToBeMoved = m_vGridLines[m_drawing.SelectedVIndex];
                return true;
            }
            else
            {
                m_lineToBeMoved = null;
                return false;
            }
        }

        /// <summary>
        /// move the selected grid line to the location of the mouse cursor
        /// </summary>
        /// <param name="mousePosition">
        /// indicates the destination position of the grid line
        /// </param>
        /// <returns>
        /// return whether the grid line be moved successfully
        /// </returns>
        public bool MoveGridLine(System.Drawing.Point mousePosition)
        {
            // verify that the mouse location is valid: it's inside the curtain grid area 
            // & it doesn't lap over another grid line (it's not allowed to move a grid line to lap over another one)
            if (false == m_drawing.MouseLocationValid)
            {
                return false;
            }

            if (null == m_lineToBeMoved)
            {
                return false;
            }

            // move a U line along the V direction
            if (-1 != m_drawing.SelectedUIndex)
            {
                // convert the 2D data to 3D
                var xyz = new Autodesk.Revit.DB.XYZ(mousePosition.X, mousePosition.Y, 0);
                var vec = new Vector4(xyz);
                vec = m_drawing.Coordinates.RestoreMatrix.Transform(vec);
                var offset = vec.Z - m_lineToBeMoved.FullCurve.GetEndPoint(0).Z;
                xyz = new Autodesk.Revit.DB.XYZ(0, 0, offset);
                var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                act.Start();
                try
                {
                    ElementTransformUtils.MoveElement(m_activeDocument, m_lineToBeMoved.Id, xyz);
                }
                catch (System.Exception e)
                {
                    TaskDialog.Show("Exception", e.Message);
                    return false;
                }
                act.Commit();

                // update the grid line 2d
                var line = m_drawing.UGridLines2D[m_drawing.SelectedUIndex];
                line.StartPoint = new System.Drawing.Point(line.StartPoint.X, line.StartPoint.Y + m_moveOffset);
                line.EndPoint = new System.Drawing.Point(line.EndPoint.X, line.EndPoint.Y + m_moveOffset);

                // update the mapped grid line graphics path
                var path = new GraphicsPath();
                path.AddLine(line.StartPoint, line.EndPoint);
                m_drawing.ULinePathList[m_drawing.SelectedUIndex] = path;

                // update the mapped segment line and its graphics path
                var pathList = m_drawing.USegLinePathListList[m_drawing.SelectedUIndex];
                var segLineList = line.Segments;
                for (var i = 0; i < segLineList.Count; i++)
                {
                    // update the segment
                    var segLine2D = segLineList[i];
                    segLine2D.StartPoint = new System.Drawing.Point(segLine2D.StartPoint.X, segLine2D.StartPoint.Y + m_moveOffset);
                    segLine2D.EndPoint = new System.Drawing.Point(segLine2D.EndPoint.X, segLine2D.EndPoint.Y + m_moveOffset);

                    // update the segment's graphics path
                    var gpath = new GraphicsPath();
                    path.AddLine(segLine2D.StartPoint, segLine2D.EndPoint);
                    pathList[i] = gpath;
                }
            }
            // move a V line along the U direction
            else if (-1 != m_drawing.SelectedVIndex)
            {
                // convert the 2D data to 3D
                var xyz = new Autodesk.Revit.DB.XYZ(mousePosition.X, mousePosition.Y, 0);
                var vec = new Vector4(xyz);
                vec = m_drawing.Coordinates.RestoreMatrix.Transform(vec);
                var offset = vec.X - m_lineToBeMoved.FullCurve.GetEndPoint(0).X;
                xyz = new Autodesk.Revit.DB.XYZ(offset, 0, 0);

                var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                act.Start();
                try
                {
                    ElementTransformUtils.MoveElement(m_activeDocument, m_lineToBeMoved.Id, xyz);
                }
                catch (System.Exception e)
                {
                    TaskDialog.Show("Exception", e.Message);
                    return false;
                }
                act.Commit();

                // update the grid line 2d
                var line = m_drawing.VGridLines2D[m_drawing.SelectedVIndex];
                line.StartPoint = new System.Drawing.Point(line.StartPoint.X + m_moveOffset, line.StartPoint.Y);
                line.EndPoint = new System.Drawing.Point(line.EndPoint.X + m_moveOffset, line.EndPoint.Y);

                // update the mapped grid line graphics path
                var path = new GraphicsPath();
                path.AddLine(line.StartPoint, line.EndPoint);
                m_drawing.VLinePathList[m_drawing.SelectedVIndex] = path;

                // update the mapped segment line and its graphics path
                var pathList = m_drawing.VSegLinePathListList[m_drawing.SelectedVIndex];
                var segLineList = line.Segments;
                for (var i = 0; i < segLineList.Count; i++)
                {
                    // update the segment
                    var segLine2D = segLineList[i];
                    segLine2D.StartPoint = new System.Drawing.Point(segLine2D.StartPoint.X + m_moveOffset, segLine2D.StartPoint.Y);
                    segLine2D.EndPoint = new System.Drawing.Point(segLine2D.EndPoint.X + m_moveOffset, segLine2D.EndPoint.Y);

                    // update the segment's graphics path
                    var gpath = new GraphicsPath();
                    path.AddLine(segLine2D.StartPoint, segLine2D.EndPoint);
                    pathList[i] = gpath;
                }
            }
            // line moved, the segment information changed, so reload all the geometry data
            this.ReloadGeometryData();

            m_drawing.DrawObject.Clear();
            return true;
        }

        /// <summary>
        /// add mullions to all the segments of the curtain grid
        /// due to the limitations of Mullions, it's not available yet to add mullions to the 
        /// edges of the curtain grid as Revit UI does
        /// </summary>
        public void AddAllMullions()
        {
            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            try
            {
                // add mullions to all U grid lines
                foreach (var line in m_uGridLines)
                {
                    line.AddMullions(line.AllSegmentCurves.get_Item(0), m_mullionType, false);
                }
                // add mullions to all V grid lines
                foreach (var line in m_vGridLines)
                {
                    line.AddMullions(line.AllSegmentCurves.get_Item(0), m_mullionType, false);
                }
            }
            catch (System.Exception e)
            {
                TaskDialog.Show("Exception", e.Message);
                return;
            }
            act.Commit();
        }

        /// <summary>
        /// delete all the mullions of the curtain grid
        /// </summary>
        public void DeleteAllMullions()
        {
            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            try
            {
                var mullions = m_activeGrid.GetMullionIds();

                foreach (var e in mullions)
                {
                    var mullion = m_activeDocument.GetElement(e) as Mullion;

                    // Exceptions may jump out if attempting to delete "Locked" mullions
                    if (true == mullion.Lockable && true == mullion.Lock)
                    {
                        mullion.Lock = false;
                    }

                    m_activeDocument.Delete(mullion.Id);
                }
            }
            catch (System.Exception e)
            {
                TaskDialog.Show("Exception", e.Message);
                return;
            }
            act.Commit();
        }
        #endregion

        #region Private methods
        /// <summary>
        /// get all the U grid lines' data of the curtain grid
        /// </summary>
        private void GetULines()
        {
            m_uGridLines.Clear();
            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            //ElementSet uLines = m_activeGrid.UGridLines;
            var uLines = m_activeGrid.GetUGridLineIds();
            act.Commit();
            if (0 == uLines.Count)
            {
                return;
            }

            foreach (var e in uLines)
            {
                var line = m_activeDocument.GetElement(e) as CurtainGridLine;

                m_uGridLines.Add(line);
            }
        }

        /// <summary>
        /// get all the V grid lines' data of the curtain grid
        /// </summary>
        private void GetVLines()
        {
            m_vGridLines.Clear();
            var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
            act.Start();
            //ElementSet vLines = m_activeGrid.VGridLines;
            var vLines = m_activeGrid.GetVGridLineIds();
            act.Commit();
            if (0 == vLines.Count)
            {
                return;
            }

            foreach (var e in vLines)
            {
                var line = m_activeDocument.GetElement(e) as CurtainGridLine;

                m_vGridLines.Add(line);
            }
        }

        /// <summary>
        /// get all of the 4 vertexes of the curtain grid
        /// </summary>
        /// <returns></returns>
        private bool GetCurtainGridVertexes()
        {
            // even in "ReloadGeometryData()" method, no need to reload the boundary information
            // (as the boundary of the curtain grid won't be changed in the sample)
            // just need to load it after the curtain wall been created
            if (null != m_GridVertexesXYZ && 0 < m_GridVertexesXYZ.Count)
            {
                return true;
            }

            // the curtain grid is from "Curtain Wall: Curtain Wall 1" (by default, the "Curtain Wall 1" has no U/V grid lines)
            if (m_uGridLines.Count <= 0 || m_vGridLines.Count <= 0)
            {
                // special handling for "Curtain Wall: Curtain Wall 1"
                // as the "Curtain Wall: Curtain Wall 1" has no U/V grid lines, so we can't compute the boundary from the grid lines
                // as that kind of curtain wall contains only one curtain cell
                // so we compute the boundary from the data of the curtain cell
                // Obtain the geometry information of the curtain wall
                // also works with some curtain grids with only U grid lines or only V grid lines
                var act = new Transaction(m_activeDocument, Guid.NewGuid().GetHashCode().ToString());
                act.Start();
                var cells = m_activeGrid.GetCurtainCells();
                act.Commit();
                var minXYZ = new Autodesk.Revit.DB.XYZ(Double.MaxValue, Double.MaxValue, Double.MaxValue);
                var maxXYZ = new Autodesk.Revit.DB.XYZ(Double.MinValue, Double.MinValue, Double.MinValue);
                GetVertexesByCells(cells, ref minXYZ, ref maxXYZ);

                // move the U & V lines to the boundary of the curtain grid, and get their end points as the vertexes of the curtain grid
                m_GridVertexesXYZ.Add(new Autodesk.Revit.DB.XYZ(minXYZ.X, minXYZ.Y, minXYZ.Z));
                m_GridVertexesXYZ.Add(new Autodesk.Revit.DB.XYZ(maxXYZ.X, maxXYZ.Y, minXYZ.Z));
                m_GridVertexesXYZ.Add(new Autodesk.Revit.DB.XYZ(maxXYZ.X, maxXYZ.Y, maxXYZ.Z));
                m_GridVertexesXYZ.Add(new Autodesk.Revit.DB.XYZ(minXYZ.X, minXYZ.Y, maxXYZ.Z));
                return true;
            }
            else
            {
                // handling for the other kinds of curtain walls (contains U&V grid lines by default)
                var uLine = m_uGridLines[0];
                var vLine = m_vGridLines[0];

                var points = new List<Autodesk.Revit.DB.XYZ>();

                var uStartPoint = uLine.FullCurve.GetEndPoint(0);
                var uEndPoint = uLine.FullCurve.GetEndPoint(1);

                var vStartPoint = vLine.FullCurve.GetEndPoint(0);
                var vEndPoint = vLine.FullCurve.GetEndPoint(1);

                points.Add(uStartPoint);
                points.Add(uEndPoint);
                points.Add(vStartPoint);
                points.Add(vEndPoint);

                //move the U & V lines to the boundary of the curtain grid, and get their end points as the vertexes of the curtain grid
                m_GridVertexesXYZ.Add(new Autodesk.Revit.DB.XYZ(uStartPoint.X, uStartPoint.Y, vStartPoint.Z));
                m_GridVertexesXYZ.Add(new Autodesk.Revit.DB.XYZ(uEndPoint.X, uEndPoint.Y, vStartPoint.Z));
                m_GridVertexesXYZ.Add(new Autodesk.Revit.DB.XYZ(uEndPoint.X, uEndPoint.Y, vEndPoint.Z));
                m_GridVertexesXYZ.Add(new Autodesk.Revit.DB.XYZ(uStartPoint.X, uStartPoint.Y, vEndPoint.Z));

                return true;
            }
        }

        /// <summary>
        /// get all the vertexes of the curtain cells
        /// </summary>
        /// <param name="cells">
        /// the curtain cells which need to be got the vertexes
        /// </param>
        /// <returns>
        /// the vertexes of the curtain cells
        /// </returns>
        private List<Autodesk.Revit.DB.XYZ> GetPoints(ICollection<CurtainCell> cells)
        {
            var points = new List<Autodesk.Revit.DB.XYZ>();

            if (null == cells || cells.Count == 0)
            {
                return points;
            }

            foreach (var cell in cells)
            {
                if (null == cell || 0 == cell.CurveLoops.Size)
                {
                    continue;
                }

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
        /// get a bounding box which covers all the input points 
        /// </summary>
        /// <param name="points">
        /// the source points
        /// </param>
        /// <param name="minXYZ">
        /// one of the bounding box points
        /// </param>
        /// <param name="maxXYZ">
        /// one of the bounding box points
        /// </param>
        private void GetVertexesByPoints(List<Autodesk.Revit.DB.XYZ> points, ref Autodesk.Revit.DB.XYZ minXYZ, ref Autodesk.Revit.DB.XYZ maxXYZ)
        {
            if (null == points || 0 == points.Count)
            {
                return;
            }

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

                if (xyz.Z < minZ)
                {
                    minZ = xyz.Z;
                }
                if (xyz.Z > maxZ)
                {
                    maxZ = xyz.Z;
                }
            } // end of loop

            minXYZ = new Autodesk.Revit.DB.XYZ(minX, minY, minZ);
            maxXYZ = new Autodesk.Revit.DB.XYZ(maxX, maxY, maxZ);
        }

        /// <summary>
        /// get the vertexes of the bounding box which covers all the curtain cells
        /// </summary>
        /// <param name="cells">
        /// the source curtain cells
        /// </param>
        /// <param name="minXYZ">
        /// the result bounding point
        /// </param>
        /// <param name="maxXYZ">
        /// the result bounding point
        /// </param>
        private void GetVertexesByCells(ICollection<CurtainCell> cells, ref Autodesk.Revit.DB.XYZ minXYZ, ref Autodesk.Revit.DB.XYZ maxXYZ)
        {
            if (null == cells || cells.Count == 0)
            {
                return;
            }

            var points = GetPoints(cells);
            GetVertexesByPoints(points, ref minXYZ, ref maxXYZ);
        }

        /// <summary>
        /// a simulative "Delete Segment" operation before real deletion
        /// as we may occur some situations that prevent us to delete the specific segment
        /// for example, delete the specific segment will make some other segments to be deleted automatically (the "conjoint" ones)
        /// and the "automatically deleted" segment is the last segment of its parent grid line
        /// in this situation, we should prevent deleting that specific segment and rollback all the simulative deletion
        /// </summary>
        /// <param name="removeList">
        /// the refferred to-be-removed list, in the simulative deletion operation, all the suitable (not the last segment) segments will
        /// be added to that list
        /// </param>
        private void MimicRemoveSegments(ref bool canRemove, List<SegmentLine2D> removeList)
        {
            // the currently operated is a U segment
            if (-1 != m_drawing.SelectedUIndex)
            {
                var gridLine2D = m_drawing.UGridLines2D[m_drawing.SelectedUIndex];
                var segLine2D = gridLine2D.Segments[m_drawing.SelectedUSegmentIndex];

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
                gridLine2D.Segments[m_drawing.SelectedUSegmentIndex] = segLine2D;
                removeList.Add(segLine2D);
                // the "regeneration" step: if there're only 2 segments existing in one joint and they're in the same line, delete one seg will cause the other 
                // been deleted automatically
                MimicRecursiveDelete(ref canRemove, segLine2D, removeList);
            }
            // the currently operated is a V segment
            else if (-1 != m_drawing.SelectedVIndex)
            {
                var gridLine2D = m_drawing.VGridLines2D[m_drawing.SelectedVIndex];
                var segLine2D = gridLine2D.Segments[m_drawing.SelectedVSegmentIndex];

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
                gridLine2D.Segments[m_drawing.SelectedVSegmentIndex] = segLine2D;
                removeList.Add(segLine2D);
                // the "regeneration" step: if there're only 2 segments existing in one joint and they're in the same line, delete one seg will cause the other 
                // been deleted automatically
                MimicRecursiveDelete(ref canRemove, segLine2D, removeList);
            }
        }

        /// <summary>
        /// the "regeneration" step: if there're only 2 segments existing in one joint and they're in the same line,
        /// delete one seg will cause the other been deleted automatically
        /// </summary>
        /// <param name="segLine2D">
        /// the to-be-automatically-deleted segment
        /// </param>
        /// <param name="removeList">
        /// the referred to-be-deleted list of the segments
        /// </param>
        /// <returns>
        /// returns the operation result: if there's no "last" segment in the deletion operation, return true; otherwise false
        /// </returns>
        private void MimicRecursiveDelete(ref bool canRemove, SegmentLine2D segLine2D, List<SegmentLine2D> removeList)
        {
            // the "regeneration" step: if there're only 2 segments existing in one joint 
            // and they're in the same line, delete one seg will cause the other 
            // been deleted automatically
            // get conjoint U line segments
            var removeSegments = new List<SegmentLine2D>();
            m_drawing.GetConjointSegments(segLine2D, removeSegments);

            // there's no isolated segment need to be removed automatically
            if (null == removeSegments || 0 == removeSegments.Count)
            {
                // didn't "remove last segment of the curtain grid line", all the operations are valid. so return true
                return;
            }

            // there're conjoint segments need to be removed automatically
            // add the segments to removeList first, and compute whether other segments need to be 
            // removed automatically because of the deletion of this newly removed segment
            if (true == segLine2D.Removed)
            {
                foreach (var seg in removeSegments)
                {
                    MimicRemoveSegment(ref canRemove, seg, removeList);

                    if (false == canRemove)
                    {
                        return;
                    }
                    // recursive calling
                    MimicRecursiveDelete(ref canRemove, seg, removeList);
                }
            }
        }

        /// <summary>
        /// remove the segment from the grid line
        /// </summary>
        /// <param name="canRemove">
        /// the returned result value, indicates whether the segment can be removed (is NOT the last segment)
        /// </param>
        /// <param name="seg">
        /// the to-be-removed segment
        /// </param>
        /// <param name="removeList">
        /// the referred to-be-deleted list of the segments
        /// </param>
        private void MimicRemoveSegment(ref bool canRemove, SegmentLine2D seg, List<SegmentLine2D> removeList)
        {
            var gridLineIndex = seg.GridLineIndex;
            var segIndex = seg.SegmentIndex;

            if (-1 != gridLineIndex && -1 != segIndex)
            {
                // update the gridline2d and segmentline2d data
                GridLine2D grid;
                if (true == seg.IsUSegment)
                {
                    grid = m_drawing.UGridLines2D[gridLineIndex];
                }
                else
                {
                    grid = m_drawing.VGridLines2D[gridLineIndex];
                }

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
        #endregion
    }// end of class
}
