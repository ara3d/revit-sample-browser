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
using System.Drawing;
using System.Drawing.Drawing2D;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.CurtainWallGrid.CS
{
    /// <summary>
    /// Maintain the appearance of the 2D projection of the curtain grid and its behaviors
    /// </summary>
    public class GridDrawing
    {
                //the document of the sample
        private MyDocument m_myDocument;

        // stores the reference to the parent geometry

        // stores the matrix transform system used in the image drawing

        // stores the client rectangle of the canvas of the curtain grid

        // stores the midpoint of the client rectangle 

        // all the grid lines of U ("Horizontal" in curtain wall) direction (in GridLine2D format)

        // all the grid lines of V ("Vertical" in curtain wall) direction (in GridLine2D format)

        // stores the boundary lines of the curtain grid of the curtain wall(in GridLine2D format)

        // stores the graphics path of all the U ("Horizontal") lines

        // stores the graphics paths of all the segments of the U lines, 
        // each element in the "m_uSegLinePathListList" is a list, which contains all the segments of a grid line

        // stores the graphics path of all the V ("Vertical") lines

        // stores the graphics paths of all the segments of the V lines, 
        // each element in the "m_uSegLinePathListList" is a list, which contains all the segments of a grid line

        // stores the graphics path of the boundary of the curtain grid
        private List<GraphicsPath> m_boundPath;

        // stores all the assistant lines & hints

        // stores the boundary coordinate of the curtain grid of the curtain wall
        int m_minX;
        int m_minY;
        int m_maxX;
        int m_maxY;

        // stores the index of the currently selected U grid line

        // stores the index of the currently selected V grid line

        // stores the index of the currently selected segment of a specified U grid line

        // stores the index of the currently selected segment of a specified V grid line

        // indicates whether the current mouse is valid
        // if the mouse location is outside the boundary of the curtain grid, it's invalid
        // if the current operation is "Add Horizontal/Vertical grid line" and the mouse location
        // is on another grid line, it's invalid (it's not allowed to add a grid line overlap another)
        // if the current operation is "Move grid line" and the destination location to be moved (indicated by the mouse location)
        // is on another grid line, it's invalid (it's not allowed to move a grid line to lap over another)
        // except these, the mouse location is valid

        // specify the pen width used in different kind of lines
        //////////////////////////////////////////////////////////////////////////
        // used in select a line/a segment
        // in this situation, use a Pen of width 10.0f to paint the graphics path, if the mouse location
        // is in the outline of the graphics path, we can say that the mouse "selects" a grid line/segment
        private float m_outlineSelectPenWidth = 10.0f;
        // the width of the pen which is used to paint the boundary lines
        private float m_boundaryPenWidth = 1.5f;
        // the width of the pen which is used to paint the locked grid lines
        private float m_lockedPenWidth = 2.0f;
        // the width of the pen which is used to paint the unlocked grid lines
        private float m_unlockedPenWidth = 1.0f;
        // the width of the pen which is used to paint the currently selected grid lines
        private float m_selectedLinePenWidth = 2.5f;
        // the width of the pen which is used to paint the currently selected segments
        private float m_selectedSegmentPenWidth = 3.0f;
        // the width of the pen which is used to paint the sketch lines
        private float m_sketchPenWidth = 2.5f;
        //////////////////////////////////////////////////////////////////////////
        
                /// <summary>
        /// stores the reference to the parent geometry
        /// </summary>
        public GridGeometry Geometry { get; }

        /// <summary>
        /// stores the matrix transform system used in the image drawing
        /// </summary>
        public GridCoordinates Coordinates { get; set; }

        /// <summary>
        /// stores the client rectangle of the canvas of the curtain grid
        /// </summary>
        public System.Drawing.Rectangle Boundary { get; set; }

        /// <summary>
        /// stores the midpoint of the client rectangle 
        /// </summary>
        public System.Drawing.Point Center { get; set; }

        /// <summary>
        /// all the grid lines of U ("Horizontal" in curtain wall) direction (in GridLine2D format)
        /// </summary>
        public List<GridLine2D> UGridLines2D { get; }

        /// <summary>
        /// all the grid lines of V ("Vertical" in curtain wall) direction (in GridLine2D format)
        /// </summary>
        public List<GridLine2D> VGridLines2D { get; }

        /// <summary>
        /// stores the boundary lines of the curtain grid of the curtain wall(in GridLine2D format)
        /// </summary>
        public List<GridLine2D> BoundLines2D { get; }

        /// <summary>
        /// stores the graphics path of all the U ("Horizontal") lines
        /// </summary>
        public List<GraphicsPath> ULinePathList { get; }

        /// <summary>
        /// stores the graphics paths of all the segments of the U lines, 
        /// each element in the "m_uSegLinePathListList" is a list, which contains all the segments of a grid line
        /// </summary>
        public List<List<GraphicsPath>> USegLinePathListList { get; }

        /// <summary>
        /// stores the graphics path of all the V ("Vertical") lines
        /// </summary>
        public List<GraphicsPath> VLinePathList { get; }

        /// <summary>
        /// stores the graphics paths of all the segments of the V lines, 
        /// each element in the "m_uSegLinePathListList" is a list, which contains all the segments of a grid line
        /// </summary>
        public List<List<GraphicsPath>> VSegLinePathListList { get; }

        /// <summary>
        /// stores all the assistant lines & hints
        /// </summary>
        public DrawObject DrawObject { get; private set; }

        /// <summary>
        /// stores the index of the currently selected U grid line
        /// </summary>
        public int SelectedUIndex { get; private set; } = -1;

        /// <summary>
        /// stores the index of the currently selected V grid line
        /// </summary>
        public int SelectedVIndex { get; private set; } = -1;

        /// <summary>
        /// stores the index of the currently selected segment of a specified U grid line
        /// </summary>
        public int SelectedUSegmentIndex { get; private set; } = -1;

        /// <summary>
        /// stores the index of the currently selected segment of a specified V grid line
        /// </summary>
        public int SelectedVSegmentIndex { get; private set; } = -1;

        /// <summary>
        /// indicates whether the current mouse is valid
        /// if the mouse location is outside the boundary of the curtain grid, it's invalid
        /// if the current operation is "Add Horizontal/Vertical grid line" and the mouse location
        /// is on another grid line, it's invalid (it's not allowed to add a grid line overlap another)
        /// if the current operation is "Move grid line" and the destination location to be moved (indicated by the mouse location)
        /// is on another grid line, it's invalid (it's not allowed to move a grid line to lap over another)
        /// except these, the mouse location is valid
        /// </summary>
        public bool MouseLocationValid { get; private set; }

        
                // occurs only when the mouse is inside the curtain grid area
        public delegate void MouseInGridHandler();
        public event MouseInGridHandler MouseInGridEvent;

        // occurs only when the mouse is outside (out of or at the edge) the curtain grid area
        public delegate void MouseOutGridHandler();
        public event MouseOutGridHandler MouseOutGridEvent;
        
                /// <summary>
        /// constructor
        /// </summary>
        /// <param name="geometry">
        /// the referred parent geometry of the curtain grid
        /// </param>
        public GridDrawing(MyDocument myDoc, GridGeometry geometry)
        {
            m_myDocument = myDoc;

            if (null == geometry)
            {
                TaskDialog.Show("Revit", "Error! There's no grid information in the curtain wall.");
            }
            else
            {
                Geometry = geometry;
                Coordinates = new GridCoordinates(myDoc, this);
                UGridLines2D = new List<GridLine2D>();
                VGridLines2D = new List<GridLine2D>();
                BoundLines2D = new List<GridLine2D>();
                ULinePathList = new List<GraphicsPath>();
                USegLinePathListList = new List<List<GraphicsPath>>();
                VSegLinePathListList = new List<List<GraphicsPath>>();
                VLinePathList = new List<GraphicsPath>();
                m_boundPath = new List<GraphicsPath>();
                DrawObject = new DrawObject();
            }
        }
        
                /// <summary>
        /// get the 2D data of the curtain grid
        /// the original data is in CurtainGridLine/XYZ/Curve format of Revit, change it to Point/GridLine2D format
        /// </summary>
        public void GetLines2D()
        {
            // clear the data container first to delete all the obsolete data
            UGridLines2D.Clear();
            VGridLines2D.Clear();
            BoundLines2D.Clear();
            ULinePathList.Clear();
            USegLinePathListList.Clear();
            VSegLinePathListList.Clear();
            VLinePathList.Clear();
            m_boundPath.Clear();
            DrawObject.Clear();
            // initialize the matrixes used in the code
            Coordinates.GetMatrix();
            // get the U grid lines and their segments (in GridLine2D format)
            GetULines2D();
            // get the V grid lines and their segments (in GridLine2D format)
            GetVLines2D();
            // get all the boundary lines (in GridLine2D format)
            GetBoundLines2D();
            // check whether the segments are "isolated" 
            // (the "isolated" segments will be displayed especially in the sample)
            // how a segment is "isolated": for a segment, at least one of its end points doesn't
            // have any other segments connected.
            UpdateIsolate();
        }

        /// <summary>
        /// show the candicate new U grid line to be added. In "add horizontal grid line" operation,
        /// there'll be a dash line following the movement of the mouse, it's drawn by this method
        /// </summary>
        /// <param name="mousePosition">
        /// the location of the mouse cursor
        /// </param>
        /// <returns>
        /// if added successfully, return true; otherwise false (if the mouse location is invalid, it will return false)
        /// </returns>
        public bool AddDashULine(System.Drawing.Point mousePosition)
        {
            var mouseInGrid = VerifyMouseLocation(mousePosition);

            // mouse is outside the curtain grid boundary
            if (false == mouseInGrid)
            {
                MouseLocationValid = false;
                return false;
            }

            // if the mouse laps over another grid line, it's invalid
            var isOverlapped = IsOverlapped(mousePosition, ULinePathList);
            if (true == isOverlapped)
            {
                MouseLocationValid = false;
                var msg = "It's not allowed to add grid line lapping over another grid line";
                m_myDocument.Message = new KeyValuePair<string, bool>(msg, true);
                return false;
            }
            // there's no "overlap", it's valid
            else
            {
                var msg = "Specify a point within the curtain grid to locate the grid line";
                m_myDocument.Message = new KeyValuePair<string, bool>(msg, false);
            }

            MouseLocationValid = true;
            // get a parallel U line first
            GridLine2D uLine2D;
            // for "Curtain Wall: Curtain Wall 1", there's no initial U/V grid lines, so we use the boundary
            // line instead (the same result)
            if (null == UGridLines2D || 0 == UGridLines2D.Count)
            {
                uLine2D = BoundLines2D[0];
            }
            else
            {
                uLine2D = UGridLines2D[0];
            }

            var startPoint = uLine2D.StartPoint;
            var endPoint = uLine2D.EndPoint;

            // move the start point and the end point parallelly
            startPoint.Y = mousePosition.Y;
            endPoint.Y = mousePosition.Y;
            // get the dash u line
            var dashULine = new GridLine2D(startPoint, endPoint);

            // initialize the pan
            var redPen = new Pen(System.Drawing.Color.Red, m_sketchPenWidth);
            redPen.DashCap = DashCap.Flat;
            redPen.DashStyle = DashStyle.Dash;

            // add the dash line to the assistant line list for drawing
            DrawObject = new DrawObject(dashULine, redPen);
            return true;
        }

        /// <summary>
        /// draw curtain grid in the canvas
        /// </summary>
        /// <param name="graphics">
        /// form graphic
        /// </param>
        public void DrawCurtainGrid(Graphics graphics)
        {
            // draw the U grid lines
            var lockBluePen = new Pen(System.Drawing.Color.Blue, m_lockedPenWidth);
            var unlockBluePen = new Pen(System.Drawing.Color.Blue, m_unlockedPenWidth);
            DrawULines(graphics, lockBluePen, unlockBluePen);

            // draw the V grid lines
            var lockbrownPen = new Pen(System.Drawing.Color.Brown, m_lockedPenWidth);
            var unlockbrownPen = new Pen(System.Drawing.Color.Brown, m_unlockedPenWidth);
            DrawVLines(graphics, lockbrownPen, unlockbrownPen);

            // draw the boundary lines
            var blackPen = new Pen(System.Drawing.Color.Black, m_boundaryPenWidth);
            DrawBoundLines(graphics, blackPen);

            // draw all the assistant line & display the hints
            DrawAssistLine(graphics);
        }

        /// <summary>
        /// show the candicate new V grid line to be added. In "add vertical grid line" operation,
        /// there'll be a dash line following the movement of the mouse, it's drawn by this method
        /// </summary>
        /// <param name="mousePosition">
        /// the location of the mouse cursor
        /// </param>
        /// <returns>
        /// if added successfully, return true; otherwise false (if the mouse location is invalid, it will return false)
        /// </returns>
        public bool AddDashVLine(System.Drawing.Point mousePosition)
        {
            var mouseInGrid = VerifyMouseLocation(mousePosition);

            // mouse is outside the curtain grid boundary
            if (false == mouseInGrid)
            {
                MouseLocationValid = false;
                return false;
            }

            // if the mouse laps over another grid line, it's invalid
            var isOverlapped = IsOverlapped(mousePosition, VLinePathList);
            if (true == isOverlapped)
            {
                MouseLocationValid = false;
                var msg = "It's not allowed to add grid line lapping over another grid line";
                m_myDocument.Message = new KeyValuePair<string, bool>(msg, true);
                return false;
            }
            // there's no "overlap", it's valid
            else
            {
                var msg = "Specify a point within the curtain grid to locate the grid line";
                m_myDocument.Message = new KeyValuePair<string, bool>(msg, false);
            }

            MouseLocationValid = true;
            // get a parallel V line first
            GridLine2D vLine2D;
            // for "Curtain Wall: Curtain Wall 1", there's no initial U/V grid lines, so we use the boundary
            // line instead (the same result)
            if (null == VGridLines2D || 0 == VGridLines2D.Count)
            {
                vLine2D = BoundLines2D[1];
            }
            else
            {
                vLine2D = VGridLines2D[0];
            }
            var startPoint = vLine2D.StartPoint;
            var endPoint = vLine2D.EndPoint;

            // move the start point and the end point parallelly
            startPoint.X = mousePosition.X;
            endPoint.X = mousePosition.X;
            // get the dash u line
            var dashVLine = new GridLine2D(startPoint, endPoint);

            // initialize the pan
            var redPen = new Pen(System.Drawing.Color.Red, m_sketchPenWidth);
            redPen.DashCap = DashCap.Flat;
            redPen.DashStyle = DashStyle.Dash;

            // add the dash line to the assistant line list for drawing
            DrawObject = new DrawObject(dashVLine, redPen);
            return true;
        }

        /// <summary>
        /// add the dash U/V line (used only in "Move grid line" operation)
        /// </summary>
        /// <param name="mousePosition">
        /// the location of the mouse cursor
        /// </param>
        public void AddDashLine(System.Drawing.Point mousePosition)
        {
            var mouseInGrid = VerifyMouseLocation(mousePosition);


            // mouse is outside the curtain grid boundary
            if (false == mouseInGrid)
            {
                return;
            }

            var offset = 0.0;

            // the selected grid line is a U grid line (it's the grid line to be moved)
            if (-1 != SelectedUIndex)
            {
                var succeeded = AddDashULine(mousePosition);
                // add failed (for example, the mouse locates on another grid line)
                if (false == succeeded)
                {
                    return;
                }

                // add the selected grid line (the line to be moved) to the assistant line list
                // (it will be painted in bold and with red color)
                var line = UGridLines2D[SelectedUIndex];
                var redPen = new Pen(System.Drawing.Color.Red, m_selectedLinePenWidth);
                DrawObject.Lines2D.Add(new KeyValuePair<Line2D, Pen>(line, redPen));
                Geometry.MoveOffset = mousePosition.Y - line.StartPoint.Y;

                // convert the 2D data to 3D
                var xyz = new XYZ(mousePosition.X, mousePosition.Y, 0);
                var vec = new Vector4(xyz);
                vec = Coordinates.RestoreMatrix.Transform(vec);
                offset = vec.Z - Geometry.LineToBeMoved.FullCurve.GetEndPoint(0).Z;
                offset = Unit.CovertFromAPI(m_myDocument.LengthUnit, offset);

                // showing the move offset
                DrawObject.Text = "Offset: " + Math.Round(offset, 1) + Unit.GetUnitLabel(m_myDocument.LengthUnit);
                DrawObject.TextPosition = mousePosition;
                DrawObject.TextPen = redPen;
                return;
            }
            // the selected grid line is a V grid line (it's the grid line to be moved)
            else if (-1 != SelectedVIndex)
            {
                var succeeded = AddDashVLine(mousePosition);
                // add failed (for example, the mouse locates on another grid line)
                if (false == succeeded)
                {
                    return;
                }

                // add the selected grid line (the line to be moved) to the assistant line list
                // (it will be painted in bold and with red color)
                var line = VGridLines2D[SelectedVIndex];
                var redPen = new Pen(System.Drawing.Color.Red, m_selectedLinePenWidth);
                DrawObject.Lines2D.Add(new KeyValuePair<Line2D, Pen>(line, redPen));
                Geometry.MoveOffset = mousePosition.X - line.StartPoint.X;
                // convert the 2D data to 3D
                var xyz = new XYZ(mousePosition.X, mousePosition.Y, 0);
                var vec = new Vector4(xyz);
                vec = Coordinates.RestoreMatrix.Transform(vec);
                offset = vec.X - Geometry.LineToBeMoved.FullCurve.GetEndPoint(0).X;
                offset = Unit.CovertFromAPI(m_myDocument.LengthUnit, offset);

                // showing the move offset
                DrawObject.Text = "Offset: " + Math.Round(offset, 1) + Unit.GetUnitLabel(m_myDocument.LengthUnit);
                DrawObject.TextPosition = mousePosition;
                DrawObject.TextPen = redPen;
            }
        }

        /// <summary>
        /// pick up a grid line by mouse
        /// </summary>
        /// <param name="mousePosition">
        /// the location of the mouse cursor
        /// </param>
        /// <param name="verifyLock">
        /// will locked grid lines be picked (if verifyLock is true, won't pick up locked ones)
        /// </param>
        /// <param name="verifyRemove">
        /// whether grid line without skipped segments be picked (if verifyRemove is true, won't pick up the grid line without skipped segments)
        /// </param>
        public void SelectLine(System.Drawing.Point mousePosition, bool verifyLock, bool verifyRemove)
        {
            var mouseInGrid = VerifyMouseLocation(mousePosition);

            // mouse is outside the curtain grid boundary
            if (false == mouseInGrid)
            {
                return;
            }

            // select the U grid line
            SelectULine(mousePosition, verifyLock, verifyRemove);

            // necessary
            // supposing the mouse hovers on the cross point of a U line and a V line, just handle
            // the U line, skip the V line 
            // otherwise it allows users to select "2" cross lines at one time
            if (-1 != SelectedUIndex)
            {
                return;
            }

            // select the V grid line
            SelectVLine(mousePosition, verifyLock, verifyRemove);
        }

        /// <summary>
        /// pick up a U grid line by mouse
        /// </summary>
        /// <param name="mousePosition">
        /// the location of the mouse cursor
        /// </param>
        /// <param name="verifyLock">
        /// will locked grid lines be picked (if verifyLock is true, won't pick up locked ones)
        /// </param>
        /// <param name="verifyRemove">
        /// whether grid line without skipped segments be picked (if verifyRemove is true, won't pick up the grid line without skipped segments)
        /// </param>
        public void SelectULine(System.Drawing.Point mousePosition, bool verifyLock, bool verifyRemove)
        {
            for (var i = 0; i < ULinePathList.Count; i++)
            {
                var path = ULinePathList[i];
                var line2D = UGridLines2D[i];
                // the verifyLock is true (won't pick up locked ones) and the current pointed grid line is locked
                // so can't select it
                if (true == verifyLock &&
                    true == line2D.Locked)
                {
                    continue;
                }

                // the verifyRemove is true (only pick up the grid line with skipped segments) and the current pointed grid line
                // has no skipped segments
                if (true == verifyRemove && line2D.RemovedNumber == 0)
                {
                    continue;
                }

                var redPen = new Pen(System.Drawing.Color.Red, m_outlineSelectPenWidth);

                // the mouse is in the outline of the graphics path
                if (path.IsOutlineVisible(mousePosition, redPen))
                {
                    SelectedUIndex = i;
                    DrawObject = new DrawObject(line2D, new Pen(System.Drawing.Color.Red, m_selectedLinePenWidth));
                    // show the lock status of the grid line
                    if (false == verifyLock && false == verifyRemove)
                    {
                        DrawObject.Text = (true == line2D.Locked) ? "Locked" : "Unlocked";
                        DrawObject.TextPosition = mousePosition;
                        DrawObject.TextPen = redPen;
                    }
                    return;
                }
            }

            DrawObject.Clear();
            SelectedUIndex = -1;
        }

        /// <summary>
        /// pick up a V grid line by mouse
        /// </summary>
        /// <param name="mousePosition">
        /// the location of the mouse cursor
        /// </param>
        /// <param name="verifyLock">
        /// will locked grid lines be picked (if verifyLock is true, won't pick up locked ones)
        /// </param>
        /// <param name="verifyRemove">
        /// whether grid line without skipped segments be picked (if verifyRemove is true, won't pick up the grid line without skipped segments)
        /// </param>
        public void SelectVLine(System.Drawing.Point mousePosition, bool verifyLock, bool verifyRemove)
        {
            for (var i = 0; i < VLinePathList.Count; i++)
            {
                var path = VLinePathList[i];
                var line2D = VGridLines2D[i];
                // the verifyLock is true (won't pick up locked ones) and the current pointed grid line is locked
                // so can't select it
                if (true == verifyLock &&
                    true == line2D.Locked)
                {
                    continue;
                }

                // the verifyRemove is true (only pick up the grid line with skipped segments) and the current pointed grid line
                // has no skipped segments
                if (true == verifyRemove && line2D.RemovedNumber == 0)
                {
                    continue;
                }

                var redPen = new Pen(System.Drawing.Color.Red, m_outlineSelectPenWidth);

                // the mouse is in the outline of the graphics path
                if (path.IsOutlineVisible(mousePosition, redPen))
                {
                    SelectedVIndex = i;
                    DrawObject = new DrawObject(line2D, new Pen(System.Drawing.Color.Red, m_selectedLinePenWidth));
                    // show the lock status of the grid line
                    if (false == verifyLock && false == verifyRemove)
                    {
                        DrawObject.Text = (true == line2D.Locked) ? "Locked" : "Unlocked";
                        DrawObject.TextPosition = mousePosition;
                        DrawObject.TextPen = redPen;
                    }
                    return;
                }
            }

            DrawObject.Clear();
            SelectedVIndex = -1;
        }

        /// <summary>
        /// pick up a segment
        /// </summary>
        /// <param name="mousePosition">
        /// the location of the mouse cursor
        /// </param>
        public void SelectSegment(System.Drawing.Point mousePosition)
        {
            var mouseInGrid = VerifyMouseLocation(mousePosition);
            // mouse is outside the curtain grid boundary
            if (false == mouseInGrid)
            {
                return;
            }

            // select a segment of the U grid line
            SelectUSegment(mousePosition);

            // necessary
            // supposing the mouse hovers on the cross point of a U line and a V line, just handle
            // the U line, skip the V line 
            // otherwise it allows users to select "2" cross lines at one time
            if (-1 != SelectedUIndex)
            {
                return;
            }

            // select a segment of the V grid line
            SelectVSegment(mousePosition);
        }

        /// <summary>
        /// pick up a segment of a U grid line
        /// </summary>
        /// <param name="mousePosition">
        /// the location of the mouse cursor
        /// </param>
        public void SelectUSegment(System.Drawing.Point mousePosition)
        {
            for (var i = 0; i < USegLinePathListList.Count; i++)
            {
                var gridLine2D = UGridLines2D[i];
                var pathList = USegLinePathListList[i];
                var redPen = new Pen(System.Drawing.Color.Red, m_outlineSelectPenWidth);

                // find out which segment it's on and which grid line does the segment belong to
                for (var j = 0; j < pathList.Count; j++)
                {
                    var segLine2D = UGridLines2D[i].Segments[j];
                    var path = pathList[j];
                    if (path.IsOutlineVisible(mousePosition, redPen))
                    {
                        if (LineOperationType.AddSegment == m_myDocument.ActiveOperation.OpType)
                        {
                            // the operation is add segment, but the selected segment hasn't been removed
                            // so skip this segment
                            if (false == segLine2D.Removed)
                            {
                                var msg = "It's only allowed to add segment on a removed segment";
                                var statusMsg = new KeyValuePair<string, bool>(msg, true);
                                m_myDocument.Message = statusMsg;
                                return;
                            }
                        }
                        else if (LineOperationType.RemoveSegment == m_myDocument.ActiveOperation.OpType)
                        {
                            // the operation is remove segment, but the selected segment has been removed
                            // so skip this segment
                            if (true == segLine2D.Removed)
                            {
                                return;
                            }
                            // if there's only segment existing, forbid to delete it
                            if (gridLine2D.RemovedNumber == gridLine2D.Segments.Count - 1)
                            {
                                var msg = "It's not allowed to delete the last segment";
                                var statusMsg = new KeyValuePair<string, bool>(msg, true);
                                m_myDocument.Message = statusMsg;
                                return;
                            }
                        }

                        SelectedUIndex = i;
                        SelectedUSegmentIndex = j;
                        DrawObject = new DrawObject(segLine2D, new Pen(System.Drawing.Color.Red, m_selectedSegmentPenWidth));
                        // update the status strip hint
                        {
                            var msg = "Left-click to finish the operation";
                            var statusMsg = new KeyValuePair<string, bool>(msg, false);
                            m_myDocument.Message = statusMsg;
                        }
                        return;
                    }
                }
            }

            DrawObject.Clear();
            SelectedUIndex = -1;
            SelectedUSegmentIndex = -1;
            // update the hints
            {
                var msg = "Select a segment";
                var statusMsg = new KeyValuePair<string, bool>(msg, false);
                m_myDocument.Message = statusMsg;
            }
        }

        /// <summary>
        /// pick up a segment of a V grid line
        /// </summary>
        /// <param name="mousePosition">
        /// the location of the mouse cursor
        /// </param>
        public void SelectVSegment(System.Drawing.Point mousePosition)
        {
            for (var i = 0; i < VSegLinePathListList.Count; i++)
            {
                var gridLine2D = VGridLines2D[i];
                var pathList = VSegLinePathListList[i];
                var redPen = new Pen(System.Drawing.Color.Red, m_outlineSelectPenWidth);

                // find out which segment it's on and which grid line does the segment belong to
                for (var j = 0; j < pathList.Count; j++)
                {
                    var segLine2D = VGridLines2D[i].Segments[j];
                    var path = pathList[j];

                    if (path.IsOutlineVisible(mousePosition, redPen))
                    {
                        if (LineOperationType.AddSegment == m_myDocument.ActiveOperation.OpType)
                        {
                            // the operation is add segment, but the selected segment hasn't been removed
                            // so skip this segment
                            if (false == segLine2D.Removed)
                            {
                                var msg = "It's only allowed to add segment on a removed segment";
                                var statusMsg = new KeyValuePair<string, bool>(msg, true);
                                m_myDocument.Message = statusMsg;
                                return;
                            }
                        }
                        else if (LineOperationType.RemoveSegment == m_myDocument.ActiveOperation.OpType)
                        {
                            // the operation is remove segment, but the selected segment has been removed
                            // so skip this segment
                            if (true == segLine2D.Removed)
                            {
                                return;
                            }
                            // if there's only segment existing, forbid to delete it
                            if (gridLine2D.RemovedNumber == gridLine2D.Segments.Count - 1)
                            {
                                var msg = "It's not allowed to delete the last segment";
                                var statusMsg = new KeyValuePair<string, bool>(msg, true);
                                m_myDocument.Message = statusMsg;
                                return;
                            }
                        }

                        SelectedVIndex = i;
                        SelectedVSegmentIndex = j;
                        DrawObject = new DrawObject(segLine2D, new Pen(System.Drawing.Color.Red, m_selectedSegmentPenWidth));

                        // update the status strip hint
                        {
                            var msg = "Left-click to finish the operation";
                            var statusMsg = new KeyValuePair<string, bool>(msg, false);
                            m_myDocument.Message = statusMsg;
                        }
                        return;
                    }
                }
            }

            DrawObject.Clear();
            // selection failed
            SelectedVIndex = -1;
            SelectedVSegmentIndex = -1;
            // update the status hint
            {
                var msg = "Select a segment";
                var statusMsg = new KeyValuePair<string, bool>(msg, false);
                m_myDocument.Message = statusMsg;
            }
        }

        /// <summary>
        /// check whether the segments which have the same junction with the specified segment 
        /// will be isolated if we delete the specified segment
        /// for example: 2 grid line has one junction, so there'll be 4 segments connecting to this junction
        /// let's delete 2 segments out of the 4 first, then if we want to delete the 3rd segment, the 4th 
        /// segment will be a "isolated" one, so we will pick this kind of segment out
        /// </summary>
        /// <param name="segLine">
        /// the specified segment used to checking
        /// </param>
        /// <param name="removeSegments">
        /// the result seg list (all the segments in this list is to-be-deleted)
        /// </param>
        public void GetConjointSegments(SegmentLine2D segLine,
            List<SegmentLine2D> removeSegments)
        {
            var startPoint = segLine.StartPoint;
            var endPoint = segLine.EndPoint;

            // get the "isolated" segment in the location of start point
            var startRemoveSegLine = new SegmentLine2D();
            GetConjointSegment(startPoint, segLine.IsUSegment, ref startRemoveSegLine);
            // get the "isolated" segment in the location of end point
            var endRemoveSegLine = new SegmentLine2D();
            GetConjointSegment(endPoint, segLine.IsUSegment, ref endRemoveSegLine);

            if (null != startRemoveSegLine)
            {
                removeSegments.Add(startRemoveSegLine);
            }
            if (null != endRemoveSegLine)
            {
                removeSegments.Add(endRemoveSegLine);
            }

        }
        
                /// <summary>
        /// get the U ("Horizontal") grid lines and their segments in GridLine2D format
        /// </summary>
        private void GetULines2D()
        {
            var gridLineIndex = -1;
            foreach (var line in Geometry.UGridLines)
            {
                gridLineIndex++;
                // store the segment paths
                var segPaths = new List<GraphicsPath>();

                // get the line2D and its segments
                var line2D = ConvertToLine2D(line, segPaths, gridLineIndex);
                UGridLines2D.Add(line2D);

                // convert the grid line of GridLine2D format to GraphicsPath format
                var path = new GraphicsPath();
                path.AddLine(line2D.StartPoint, line2D.EndPoint);
                ULinePathList.Add(path);

                // store the segment paths to a list
                USegLinePathListList.Add(segPaths);
            }
        }

        /// <summary>
        /// convert the grid line in CurtainGridLine format to GridLine2D format
        /// in the Canvas area, the "System.Drawing.Point" instances are directly used, it's hard
        /// for us to use CurtainGridLine and Autodesk.Revit.DB.XYZ directly, so convert them to 2D data first
        /// </summary>
        /// <param name="line">
        /// the grid line in CurtainGridLine format 
        /// </param>
        /// <param name="segPaths">
        /// the grid line in GraphicsPath format (the GraphicsPath contains the grid line in GridLine2D format)
        /// </param>
        /// <param name="gridLineIndex">
        /// the index of the grid line
        /// </param>
        /// <returns>
        /// the converted grid line in GridLine2D format
        /// </returns>
        private GridLine2D ConvertToLine2D(CurtainGridLine line, List<GraphicsPath> segPaths, int gridLineIndex)
        {
            var curve = line.FullCurve;
            var point1 = curve.GetEndPoint(0);
            var point2 = curve.GetEndPoint(1);

            var v1 = new Vector4(point1);
            var v2 = new Vector4(point2);

            // transform from 3D point to 2D point
            v1 = Coordinates.TransformMatrix.Transform(v1);
            v2 = Coordinates.TransformMatrix.Transform(v2);

            // create a new line in GridLine2D format
            var line2D = new GridLine2D();
            line2D.StartPoint = new System.Drawing.Point((int)v1.X, (int)v1.Y);
            line2D.EndPoint = new System.Drawing.Point((int)v2.X, (int)v2.Y);
            line2D.Locked = line.Lock;
            line2D.IsUGridLine = line.IsUGridLine;
            // get which segments are skipped
            var skippedSegments = ConvertCurveToSegment(line.SkippedSegmentCurves);
            // get all the segments for the curtain grid (and tag the skipped ones out)
            GetSegments(line2D, line.AllSegmentCurves, skippedSegments, segPaths, gridLineIndex);
            return line2D;
        }

        /// <summary>
        /// convert the segment lines in Curve format to SegmentLine2D format
        /// </summary>
        /// <param name="curveArray">
        /// the skipped segments in Curve (used in RevitAPI) format 
        /// </param>
        /// <returns>
        /// the skipped segments in SegmentLine2D format (converted from 3D to 2D)
        /// </returns>
        private List<SegmentLine2D> ConvertCurveToSegment(CurveArray curveArray)
        {
            var resultList = new List<SegmentLine2D>();

            // convert the skipped segments (in Curve format) to SegmentLine2D format
            foreach (Curve curve in curveArray)
            {
                var point1 = curve.GetEndPoint(0);
                var point2 = curve.GetEndPoint(1);

                var v1 = new Vector4(point1);
                var v2 = new Vector4(point2);

                // transform from 3D point to 2D point
                v1 = Coordinates.TransformMatrix.Transform(v1);
                v2 = Coordinates.TransformMatrix.Transform(v2);

                // add the segment data
                var segLine2D = new SegmentLine2D();
                segLine2D.StartPoint = new System.Drawing.Point((int)v1.X, (int)v1.Y);
                segLine2D.EndPoint = new System.Drawing.Point((int)v2.X, (int)v2.Y);
                resultList.Add(segLine2D);
            }

            return resultList;
        }

        /// <summary>
        /// identify whether the segment is contained in the segments of a grid line
        /// </summary>
        /// <param name="lines">
        /// the grid line (may contain the specified segment)
        /// </param>
        /// <param name="lineB">
        /// the segment which needs to be identified
        /// </param>
        /// <returns>
        /// if the segment is contained in the grid line, return true; otherwise false
        /// </returns>
        private bool IsSegLineContained(List<SegmentLine2D> lines, SegmentLine2D lineB)
        {
            foreach (var lineA in lines)
            {
                var lineAStartPoint = lineA.StartPoint;
                var lineAEndPoint = lineA.EndPoint;
                var lineBStartPoint = lineB.StartPoint;
                var lineBEndPoint = lineB.EndPoint;

                // the 2 lines have the same start point and the same end point
                if ((true == IsPointsEqual(lineAStartPoint, lineBStartPoint) && true == IsPointsEqual(lineAEndPoint, lineBEndPoint)) ||
                    (true == IsPointsEqual(lineAStartPoint, lineBEndPoint) && true == IsPointsEqual(lineAEndPoint, lineBStartPoint)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// get all the segments of the specified grid line
        /// </summary>
        /// <param name="gridLine2D">
        /// the grid line which wants to get all its segments
        /// </param>
        /// <param name="allCurves">
        /// all the segments (include existent ones and skipped one)
        /// </param>
        /// <param name="skippedSegments">
        /// the skipped segments
        /// </param>
        /// <param name="segPaths">
        /// the GraphicsPath list contains all the segments
        /// </param>
        /// <param name="gridLineIndex">
        /// the index of the grid line
        /// </param>
        private void GetSegments(GridLine2D gridLine2D, CurveArray allCurves,
            List<SegmentLine2D> skippedSegments, List<GraphicsPath> segPaths,
            int gridLineIndex)
        {
            var segIndex = -1;
            // convert the segments from Curve format to SegmentLine2D format (from 3D to 2D)
            foreach (Curve curve in allCurves)
            {
                // store the index of the segment in the grid line
                segIndex++;
                var point1 = curve.GetEndPoint(0);
                var point2 = curve.GetEndPoint(1);

                var v1 = new Vector4(point1);
                var v2 = new Vector4(point2);

                // transform from 3D point to 2D point
                v1 = Coordinates.TransformMatrix.Transform(v1);
                v2 = Coordinates.TransformMatrix.Transform(v2);

                // add the segment data
                var segLine2D = new SegmentLine2D();
                segLine2D.StartPoint = new System.Drawing.Point((int)v1.X, (int)v1.Y);
                segLine2D.EndPoint = new System.Drawing.Point((int)v2.X, (int)v2.Y);
                // if the segment is contained in the skipped list, set the Removed flag to true; otherwise false
                segLine2D.Removed = IsSegLineContained(skippedSegments, segLine2D);
                // if the segment is in a U grid line, set it true; otherwise false
                segLine2D.IsUSegment = gridLine2D.IsUGridLine;
                // the index of the parent grid line in the grid line list
                segLine2D.GridLineIndex = gridLineIndex;
                // the index of the segment in its parent grid line
                segLine2D.SegmentIndex = segIndex;
                if (true == segLine2D.Removed)
                {
                    gridLine2D.RemovedNumber++;
                }
                gridLine2D.Segments.Add(segLine2D);

                // store the mapped graphics path
                var path = new GraphicsPath();
                path.AddLine(segLine2D.StartPoint, segLine2D.EndPoint);
                segPaths.Add(path);
            }
        }

        /// <summary>
        /// get the V ("Vertical") grid lines and their segments in GridLine2D format
        /// </summary>
        private void GetVLines2D()
        {
            var gridLineIndex = -1;
            foreach (var line in Geometry.VGridLines)
            {
                gridLineIndex++;
                // store the segment paths
                var segPaths = new List<GraphicsPath>();
                // get the line2D and its segments
                var line2D = ConvertToLine2D(line, segPaths, gridLineIndex);
                VGridLines2D.Add(line2D);

                var path = new GraphicsPath();
                path.AddLine(line2D.StartPoint, line2D.EndPoint);
                VLinePathList.Add(path);

                // store the segment paths to a list
                VSegLinePathListList.Add(segPaths);
            }
        }

        /// <summary>
        /// get the boundary lines of the curtain grid
        /// </summary>
        private void GetBoundLines2D()
        {
            for (var i = 0; i < Geometry.GridVertexesXYZ.Count; i += 1)
            {
                XYZ point1, point2;

                // connect the last point with the first point as a boundary line
                if (i == Geometry.GridVertexesXYZ.Count - 1)
                {
                    point1 = Geometry.GridVertexesXYZ[i];
                    point2 = Geometry.GridVertexesXYZ[0];
                }
                else
                {
                    point1 = Geometry.GridVertexesXYZ[i];
                    point2 = Geometry.GridVertexesXYZ[i + 1];
                }

                var v1 = new Vector4(point1);
                var v2 = new Vector4(point2);

                // transform from 3D point to 2D point
                v1 = Coordinates.TransformMatrix.Transform(v1);
                v2 = Coordinates.TransformMatrix.Transform(v2);

                // stores the bounding coordinate
                var v1X = (int)v1.X;
                var v1Y = (int)v1.Y;
                var v2X = (int)v2.X;
                var v2Y = (int)v2.Y;

                // obtain the min and max point
                m_minX = v1X;
                m_minY = v1Y;

                if (v1X > m_maxX)
                {
                    m_maxX = v1X;
                }
                else if (v1X < m_minX)
                {
                    m_minX = v1X;
                }

                if (v2X > m_maxX)
                {
                    m_maxX = v2X;
                }
                else if (v2X < m_minX)
                {
                    m_minX = v2X;
                }

                if (v1Y > m_maxY)
                {
                    m_maxY = v1Y;
                }
                if (v1Y < m_minY)
                {
                    m_minY = v1Y;
                }

                if (v2Y > m_maxY)
                {
                    m_maxY = v2Y;
                }
                if (v2Y < m_minY)
                {
                    m_minY = v2Y;
                }

                // create the boundary line
                var line2D = new GridLine2D();
                line2D.StartPoint = new System.Drawing.Point((int)v1.X, (int)v1.Y);
                line2D.EndPoint = new System.Drawing.Point((int)v2.X, (int)v2.Y);
                BoundLines2D.Add(line2D);

                // add the line to the mapped GraphicsPath list
                var path = new GraphicsPath();
                path.AddLine(line2D.StartPoint, line2D.EndPoint);
                m_boundPath.Add(path);
            }
        }

        /// <summary>
        /// draw the U grid lines
        /// </summary>
        /// <param name="graphics">
        /// used in drawing the lines
        /// </param>
        /// <param name="lockPen">
        /// the pen used to draw the locked grid line
        /// </param>
        /// <param name="unlockPen">
        /// the pen used to draw the unlocked grid line
        /// </param>
        private void DrawULines(Graphics graphics, Pen lockPen, Pen unlockPen)
        {
            foreach (var line2D in UGridLines2D)
            {
                var pen = (true == line2D.Locked) ? lockPen : unlockPen;
                var isolatedPen = new Pen(Brushes.Gray, pen.Width);

                // won't draw the grid lines at GridLine2D level, draw them at SegmentLine2D level
                // at the skipped segments in the grid line won't be painted to the canvas
                foreach (var segLine2D in line2D.Segments)
                {
                    // skip the removed segments, won't draw them
                    if (true == segLine2D.Removed)
                    {
                        continue;
                    }
                    else if (true == segLine2D.Isolated)
                    {
                        graphics.DrawLine(isolatedPen, segLine2D.StartPoint, segLine2D.EndPoint);
                    }
                    else
                    {
                        graphics.DrawLine(pen, segLine2D.StartPoint, segLine2D.EndPoint);
                    }
                }
            }
        }

        /// <summary>
        /// draw the V grid lines
        /// </summary>
        /// <param name="graphics">
        /// used in drawing the lines
        /// </param>
        /// <param name="lockPen">
        /// the pen used to draw the locked grid line
        /// </param>
        /// <param name="unlockPen">
        /// the pen used to draw the unlocked grid line
        /// </param>
        private void DrawVLines(Graphics graphics, Pen lockPen, Pen unlockPen)
        {
            foreach (var line2D in VGridLines2D)
            {
                var pen = (true == line2D.Locked) ? lockPen : unlockPen;
                var isolatedPen = new Pen(Brushes.Gray, pen.Width);
                // won't draw the grid lines at GridLine2D level, draw them at SegmentLine2D level
                // at the skipped segments in the grid line won't be painted to the canvas
                foreach (var segLine2D in line2D.Segments)
                {
                    // skip the removed segments, won't draw them
                    if (true == segLine2D.Removed)
                    {
                        continue;
                    }
                    else if (true == segLine2D.Isolated)
                    {
                        graphics.DrawLine(isolatedPen, segLine2D.StartPoint, segLine2D.EndPoint);
                    }
                    else
                    {
                        graphics.DrawLine(pen, segLine2D.StartPoint, segLine2D.EndPoint);
                    }
                }
            }
        }

        /// <summary>
        /// draw the boundary lines of the curtain grid
        /// </summary>
        /// <param name="graphics">
        /// used in drawing the lines
        /// </param>
        /// <param name="pen">
        /// the pen used to draw the boundary line
        /// </param>
        private void DrawBoundLines(Graphics graphics, Pen pen)
        {
            foreach (var line2D in BoundLines2D)
            {
                graphics.DrawLine(pen, line2D.StartPoint, line2D.EndPoint);
            }
        }

        /// <summary>
        /// draw the assistant lines used to highlight some special lines
        /// for example: in add Horizontal/Vertical grid line operations, the to-be-added lines will
        /// be shown in bold with red color, this assistant line is drawn in this method
        /// </summary>
        /// <param name="graphics">
        /// used in drawing the lines
        /// </param>
        private void DrawAssistLine(Graphics graphics)
        {
            if (null != DrawObject)
            {
                DrawObject.Draw(graphics);
            }
        }

        /// <summary>
        /// if mouse insiede the curtain grid area, returns true; otherwise false
        /// </summary>
        /// <param name="mousePosition">
        /// the position of the mouse cursor
        /// </param>
        /// <returns>
        /// if mouse insiede the curtain grid area, returns true; otherwise false
        /// </returns>
        private bool VerifyMouseLocation(System.Drawing.Point mousePosition)
        {
            var x = mousePosition.X;
            var y = mousePosition.Y;

            // the mouse is outside the curtain grid area or at the edge of the curtain grid, just do nothing
            if (x <= m_minX ||
                y <= m_minY ||
                x >= m_maxX ||
                y >= m_maxY)
            {
                // set the cursor to the default cursor
                // indicating that the mouse is outside the curtain grid area, and disables to draw U line
                if (null != MouseOutGridEvent)
                {
                    MouseOutGridEvent();
                }

                return false;
            }

            // set the cursor to the cursor of "Cross"
            // indicating that the mouse is inside the curtain grid area, and enables to draw U line
            if (null != MouseInGridEvent)
            {
                MouseInGridEvent();
            }
            return true;
        }

        /// <summary>
        /// check whether the point is in the outline of the paths
        /// </summary>
        /// <param name="point">
        /// the point to be checked
        /// </param>
        /// <param name="paths">
        /// the paths of the lines
        /// </param>
        /// <returns>
        /// if the point is in the outline of one of the paths, return true; otherwise false
        /// </returns>
        private bool IsOverlapped(System.Drawing.Point point, List<GraphicsPath> paths)
        {
            var pen = new Pen(System.Drawing.Color.Red, m_lockedPenWidth);

            foreach (var path in paths)
            {
                // the point is in the outline of the path, so the isOverlapped is true
                if (path.IsOutlineVisible(point, pen))
                {
                    return true;
                }
            }

            // no overlap found, so return false
            return false;
        }

        private void UpdateIsolate()
        {
            foreach (var line2D in UGridLines2D)
            {
                foreach (var seg in line2D.Segments)
                {
                    var startPoint = seg.StartPoint;
                    // if we get all the U line's start points, we can cover all the conjoints
                    IsPointIsolate(startPoint);
                }
            }
        }

        /// <summary>
        /// check whether the segments which have the same junction with the specified junction 
        /// will be isolated if we delete the specified segment
        /// for example: 2 grid line has one junction, so there'll be 4 segments connecting to this junction
        /// let's delete 2 segments out of the 4 first, then if we want to delete the 3rd segment, the 4th 
        /// segment will be a "isolated" one, so we will pick this kind of segment out
        /// </summary>
        /// <param name="point">
        /// the junction of several segments
        /// </param>
        private bool IsPointIsolate(System.Drawing.Point point)
        {
            var uSegIndexes = new List<int>();
            // get which U grid line contains the point
            var uIndex = GetOutlineIndex(ULinePathList, point);

            var vSegIndexes = new List<int>();
            // get which V grid line contains the point
            var vIndex = GetOutlineIndex(VLinePathList, point);

            if (-1 == uIndex || -1 == vIndex)
            {
                return false;
            }

            if (-1 != uIndex)
            {
                // get the grid line containing the point and its segments
                var segList = UGridLines2D[uIndex].Segments;
                // get which segments of the grid line contains the point
                uSegIndexes = GetOutlineIndexes(segList, point);
            }

            if (-1 != vIndex)
            {
                // get the grid line containing the point and its segments
                var segList = VGridLines2D[vIndex].Segments;
                // get which segments of the grid line contains the point
                vSegIndexes = GetOutlineIndexes(segList, point);
            }

            // TODO: improve the comments
            // there's only 1 v segment contains the point and no u segment, so the segment is an isolated one
            if (0 == uSegIndexes.Count && 1 == vSegIndexes.Count)
            {
                var seg = VGridLines2D[vIndex].Segments[vSegIndexes[0]];
                seg.Isolated = true;

                // recursive check
                IsPointIsolate(seg.StartPoint);
                IsPointIsolate(seg.EndPoint);
            }
            else if (1 == uSegIndexes.Count && 0 == vSegIndexes.Count)
            {
                var seg = UGridLines2D[uIndex].Segments[uSegIndexes[0]];
                seg.Isolated = true;

                // recursive check
                IsPointIsolate(seg.StartPoint);
                IsPointIsolate(seg.EndPoint);
            }

            return false;
        }

        /// <summary>
        /// get which segment contains the specified point, if the segment contains the point, 
        /// return the index of the segment in the grid line
        /// </summary>
        /// <param name="segList">
        /// the segment list to be checked with the specified point
        /// </param>
        /// <param name="point">
        /// the point which need to be checked
        /// </param>
        /// <returns>
        /// the index of the segment in the grid line
        /// </returns>
        private List<int> GetOutlineIndexes(List<SegmentLine2D> segList, System.Drawing.Point point)
        {
            var resultIndexes = new List<int>();

            for (var i = 0; i < segList.Count; i++)
            {
                var seg = segList[i];

                if (true == seg.Removed ||
                    true == seg.Isolated)
                {
                    continue;
                }

                // the specified point is one of the end points of the current segment
                System.Drawing.Point[] points = { seg.StartPoint, seg.EndPoint };
                foreach (var p in points)
                {
                    var equal = IsPointsEqual(p, point);
                    if (true == equal)
                    {
                        resultIndexes.Add(i);
                    }
                }
            }

            return resultIndexes;
        }

        /// <summary>
        /// judge whether the 2 points are equal (have the same coordinate)
        /// (as the X & Y of the Point class are both int values, so needn't use AlmostEqual)
        /// </summary>
        /// <param name="pa">
        /// the point to be checked with another for equality
        /// </param>
        /// <param name="pb">
        /// the point to be checked with another for equality
        /// </param>
        /// <returns>
        /// if the 2 points have the same coordinate, return true; otherwise false
        /// </returns>
        private bool IsPointsEqual(System.Drawing.Point pa, System.Drawing.Point pb)
        {
            var ax = pa.X;
            var ay = pa.Y;
            var bx = pb.X;
            var by = pb.Y;

            float result = (ax - bx) * (ax - bx) + (ay - by) * (ay - by);

            // the distance of the 2 points is greater than 0, they're not equal
            if (/*result > 1*/result != 0)
            {
                return false;
            }
            // the distance of the 2 points is 0, they're equal
            else
            {
                return true;
            }
        }

        /// <summary>
        /// get which grid line (in GraphicsPath format) contains the specified point, 
        /// if the grid line contains the point, 
        /// return the index of the grid line
        /// </summary>
        /// <param name="pathList">
        /// the grid line list to be checked with the specified point
        /// </param>
        /// <param name="point">
        /// the point which need to be checked
        /// </param>
        /// <returns>
        /// the index of the grid line
        /// </returns>
        private int GetOutlineIndex(List<GraphicsPath> pathList, System.Drawing.Point point)
        {
            for (var i = 0; i < pathList.Count; i++)
            {
                var redPen = Pens.Red;
                var path = pathList[i];
                if (path.IsOutlineVisible(point, redPen))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// get all the segments which is connected to the end point of the segment and is in the same line as the segment
        /// for example: 2 grid line has one junction, so there'll be 4 segments connecting to this junction
        /// 2 U segments and 2 V segments, delete the 2 U segments first, then delete one of the V segment, the other V segment
        /// will be marked as the "ConjointSegment" and will be deleted automatically.
        /// this method is used to pick the "the other V segment" out
        /// </summary>
        /// <param name="point">
        /// the junction of several segments
        /// </param>
        /// <param name="isUSegment">
        /// the U/V status of the segment
        /// </param>
        /// <param name="removeSegLine">
        /// the result segment line to be removed
        /// </param>
        private void GetConjointSegment(System.Drawing.Point point, bool isUSegment,
             ref SegmentLine2D removeSegLine)
        {
            var uSegIndexes = new List<int>();
            // get which U grid line contains the point
            var uIndex = GetOutlineIndex(ULinePathList, point);

            var vSegIndexes = new List<int>();
            // get which V grid line contains the point
            var vIndex = GetOutlineIndex(VLinePathList, point);

            if (-1 != uIndex)
            {
                // get the grid line containing the point and its segments
                var segList = UGridLines2D[uIndex].Segments;
                // get which segments of the grid line contains the point
                uSegIndexes = GetOutlineIndexes(segList, point);
            }

            if (-1 != vIndex)
            {
                // get the grid line containing the point and its segments
                var segList = VGridLines2D[vIndex].Segments;
                // get which segments of the grid line contains the point
                vSegIndexes = GetOutlineIndexes(segList, point);
            }

            if ((0 == uSegIndexes.Count && 1 == vSegIndexes.Count))
            {
                // the source segment is an V segment, and the result segment is a V segment too.
                // they're connected and in one line, so the result V segment should be removed, 
                // according to the UI rule
                if (false == isUSegment)
                {
                    removeSegLine = VGridLines2D[vIndex].Segments[vSegIndexes[0]];
                    return;
                }
            }
            else if (1 == uSegIndexes.Count && 0 == vSegIndexes.Count)
            {
                // the source segment is an U segment, and the result segment is a U segment too.
                // they're connected and in one line, so the result U segment should be removed, 
                // according to the UI rule
                if (true == isUSegment)
                {
                    removeSegLine = UGridLines2D[uIndex].Segments[uSegIndexes[0]];
                    return;
                }
            }
        }
            }// end of class

    /// <summary>
    /// the class is designed to help draw the hints and the assistant lines of the curtain grid
    /// </summary>
    public class DrawObject
    {
                // the lines to be drawn with the mapping pen

        // the hint to be drawn

        // the location to draw the hint
        private System.Drawing.Point m_textPosition;

        // the pen to draw the hint

        
                /// <summary>
        /// the lines to be drawn with the mapping pen
        /// </summary>
        public List<KeyValuePair<Line2D, Pen>> Lines2D { get; set; }

        /// <summary>
        /// the hint to be drawn
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// the location to draw the hint
        /// </summary>
        public System.Drawing.Point TextPosition
        {
            get => m_textPosition;
            set => m_textPosition = value;
        }

        /// <summary>
        /// the pen to draw the hint
        /// </summary>
        public Pen TextPen { get; set; }

        
                /// <summary>
        /// default constructor
        /// </summary>
        public DrawObject()
        {
            Lines2D = new List<KeyValuePair<Line2D, Pen>>();
            Text = string.Empty;
            m_textPosition = System.Drawing.Point.Empty;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="line">
        /// the line to be drawn
        /// </param>
        /// <param name="pen">
        /// the pen used to draw the line
        /// </param>
        public DrawObject(Line2D line, Pen pen)
        {
            Lines2D = new List<KeyValuePair<Line2D, Pen>>();
            Lines2D.Add(new KeyValuePair<Line2D, Pen>(new Line2D(line), pen));
            Text = string.Empty;
            m_textPosition = System.Drawing.Point.Empty;
        }
        
                /// <summary>
        /// clear all the to-be-drawn lines
        /// </summary>
        public void Clear()
        {
            Lines2D.Clear();
            Text = string.Empty;
            m_textPosition = System.Drawing.Point.Empty;
        }

        /// <summary>
        /// draw the assistant lines and hint text
        /// </summary>
        /// <param name="graphics">
        /// the graphics used to draw the lines
        /// </param>
        public void Draw(Graphics graphics)
        {
            // draw the assistant lines
            foreach (var pair in Lines2D)
            {
                var line2D = pair.Key;
                if (System.Drawing.Point.Empty == line2D.StartPoint ||
                    System.Drawing.Point.Empty == line2D.EndPoint)
                {
                    continue;
                }

                var pen = pair.Value;
                graphics.DrawLine(pen, line2D.StartPoint, line2D.EndPoint);
            }

            // draw the hint text
            if (false == string.IsNullOrEmpty(Text) &&
                System.Drawing.Point.Empty != m_textPosition)
            {
                var font = new Font("Verdana", 10, FontStyle.Regular);
                graphics.DrawString(Text, font, TextPen.Brush, new PointF(m_textPosition.X + 2, m_textPosition.Y + 2));
            }
        }
            }

}
