// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.GridCreation.CS
{
    /// <summary>
    ///     The dialog which provides the options of creating radial and arc grids
    /// </summary>
    public class CreateRadialAndArcGridsData : CreateGridsData
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="application">Application object</param>
        /// <param name="unit">Current length display unit type</param>
        /// <param name="labels">All existing labels in Revit's document</param>
        public CreateRadialAndArcGridsData(UIApplication application, ForgeTypeId unit, ArrayList labels)
            : base(application, labels, unit)
        {
        }
        // X coordinate of origin
        // Y coordinate of origin
        // Start degree of arc grids and radial grids
        // End degree of arc grids and radial grids
        // Spacing between arc grids
        // Number of arc grids
        // Number of radial grids
        // Radius of first arc grid
        // Distance from origin to start point
        // Bubble location of arc grids
        // Bubble location of radial grids
        // Label of first arc grid
        // Label of first radial grid

        /// <summary>
        ///     X coordinate of origin
        /// </summary>
        public double XOrigin { get; set; }

        /// <summary>
        ///     Y coordinate of origin
        /// </summary>
        public double YOrigin { get; set; }

        /// <summary>
        ///     Start degree of arc grids and radial grids
        /// </summary>
        public double StartDegree { get; set; }

        /// <summary>
        ///     End degree of arc grids and radial grids
        /// </summary>
        public double EndDegree { get; set; }

        /// <summary>
        ///     Spacing between arc grids
        /// </summary>
        public double ArcSpacing { get; set; }

        /// <summary>
        ///     Number of arc grids
        /// </summary>
        public uint ArcNumber { get; set; }

        /// <summary>
        ///     Number of radial grids
        /// </summary>
        public uint LineNumber { get; set; }

        /// <summary>
        ///     Radius of first arc grid
        /// </summary>
        public double ArcFirstRadius { get; set; }

        /// <summary>
        ///     Distance from origin to start point
        /// </summary>
        public double LineFirstDistance { get; set; }

        /// <summary>
        ///     Bubble location of arc grids
        /// </summary>
        public BubbleLocation ArcFirstBubbleLoc { get; set; }

        /// <summary>
        ///     Bubble location of radial grids
        /// </summary>
        public BubbleLocation LineFirstBubbleLoc { get; set; }

        /// <summary>
        ///     Label of first arc grid
        /// </summary>
        public string ArcFirstLabel { get; set; }

        /// <summary>
        ///     Label of first radial grid
        /// </summary>
        public string LineFirstLabel { get; set; }

        /// <summary>
        ///     Create grids
        /// </summary>
        public void CreateGrids()
        {
            if (CreateRadialGrids() != 0)
            {
                var failureReason = ResManager.GetString("FailedToCreateRadialGrids") + "\r";
                failureReason += ResManager.GetString("AjustValues");

                ShowMessage(failureReason, ResManager.GetString("FailureCaptionCreateGrids"));
            }

            var failureReasons = new ArrayList();
            if (CreateArcGrids(ref failureReasons) != 0)
            {
                var failureReason = ResManager.GetString("FailedToCreateArcGrids") +
                                    ResManager.GetString("Reasons") + "\r";
                if (failureReasons.Count != 0)
                {
                    failureReason += "\r";
                    foreach (string reason in failureReasons) failureReason += reason + "\r";
                }

                failureReason += "\r" + ResManager.GetString("AjustValues");

                ShowMessage(failureReason, ResManager.GetString("FailureCaptionCreateGrids"));
            }
        }

        /// <summary>
        ///     Create radial grids
        /// </summary>
        /// <returns>Number of grids failed to create</returns>
        private int CreateRadialGrids()
        {
            var errorCount = 0;

            // Curve array which stores all curves for batch creation
            var curves = new CurveArray();

            for (var i = 0; i < LineNumber; ++i)
                try
                {
                    double angel;
                    if (LineNumber == 1)
                    {
                        angel = (StartDegree + EndDegree) / 2;
                    }
                    else
                    {
                        // The number of space between radial grids will be m_lineNumber if arc is a circle
                        if (EndDegree - StartDegree == 2 * Values.Pi)
                            angel = StartDegree + i * (EndDegree - StartDegree) / LineNumber;
                        // The number of space between radial grids will be m_lineNumber-1 if arc is not a circle
                        else
                            angel = StartDegree + i * (EndDegree - StartDegree) / (LineNumber - 1);
                    }

                    XYZ startPoint;
                    XYZ endPoint;
                    var cos = Math.Cos(angel);
                    var sin = Math.Sin(angel);

                    if (ArcNumber != 0)
                    {
                        // Grids will have an extension distance of m_ySpacing / 2
                        startPoint = new XYZ(XOrigin + LineFirstDistance * cos, YOrigin + LineFirstDistance * sin, 0);
                        endPoint = new XYZ(
                            XOrigin + (ArcFirstRadius + (ArcNumber - 1) * ArcSpacing + ArcSpacing / 2) * cos,
                            YOrigin + (ArcFirstRadius + (ArcNumber - 1) * ArcSpacing + ArcSpacing / 2) * sin, 0);
                    }
                    else
                    {
                        startPoint = new XYZ(XOrigin + LineFirstDistance * cos, YOrigin + LineFirstDistance * sin, 0);
                        endPoint = new XYZ(XOrigin + (ArcFirstRadius + 5) * cos, YOrigin + (ArcFirstRadius + 5) * sin,
                            0);
                    }

                    Line line;
                    // Create a line according to the bubble location
                    if (LineFirstBubbleLoc == BubbleLocation.StartPoint)
                        line = NewLine(startPoint, endPoint);
                    else
                        line = NewLine(endPoint, startPoint);

                    if (i == 0)
                    {
                        // Create grid with line
                        var grid = NewGrid(line);

                        try
                        {
                            // Set label of first radial grid
                            grid.Name = LineFirstLabel;
                        }
                        catch (ArgumentException)
                        {
                            ShowMessage(ResManager.GetString("FailedToSetLabel") + LineFirstLabel + "!",
                                ResManager.GetString("FailureCaptionSetLabel"));
                        }
                    }
                    else
                    {
                        // Add the line to curve array
                        AddCurveForBatchCreation(ref curves, line);
                    }
                }
                catch (Exception)
                {
                    ++errorCount;
                }

            // Create grids with curves
            CreateGrids(curves);

            return errorCount;
        }

        /// <summary>
        ///     Create Arc Grids
        /// </summary>
        /// <param name="failureReasons">ArrayList contains failure reasons</param>
        /// <returns>Number of grids failed to create</returns>
        private int CreateArcGrids(ref ArrayList failureReasons)
        {
            var errorCount = 0;

            // Curve array which stores all curves for batch creation
            var curves = new CurveArray();

            for (var i = 0; i < ArcNumber; ++i)
                try
                {
                    var origin = new XYZ(XOrigin, YOrigin, 0);
                    var radius = ArcFirstRadius + i * ArcSpacing;

                    // In Revit UI user can select a circle to create a grid, but actually two grids 
                    // (One from 0 to 180 degree and the other from 180 degree to 360) will be created. 
                    // In RevitAPI using NewGrid method with a circle as its argument will raise an exception. 
                    // Therefore in this sample we will create two arcs from the upper and lower parts of the 
                    // circle, and then create two grids on the base of the two arcs to accord with UI.
                    if (EndDegree - StartDegree == 2 * Values.Pi) // Create circular grids
                    {
                        var upperArcToCreate = TransformArc(origin, radius, 0, Values.Pi, ArcFirstBubbleLoc);

                        if (i == 0)
                        {
                            var gridUpper = NewGrid(upperArcToCreate);
                            if (gridUpper != null)
                                try
                                {
                                    // Set label of first grid
                                    gridUpper.Name = ArcFirstLabel;
                                }
                                catch (ArgumentException)
                                {
                                    ShowMessage(ResManager.GetString("FailedToSetLabel") + ArcFirstLabel + "!",
                                        ResManager.GetString("FailureCaptionSetLabel"));
                                }
                        }
                        else
                        {
                            curves.Append(upperArcToCreate);
                        }

                        var lowerArcToCreate =
                            TransformArc(origin, radius, Values.Pi, 2 * Values.Pi, ArcFirstBubbleLoc);
                        curves.Append(lowerArcToCreate);
                    }
                    else // Create arc grids
                    {
                        // Each arc grid will has extension degree of 15 degree
                        var extensionDegree = 15 * Values.Degtorad;
                        Arc arcToCreate;

                        if (LineNumber != 0)
                        {
                            // If the range of arc degree is too close to a circle, the arc grids will not have 
                            // extension degrees.
                            // Also the room for bubble should be considered, so a room size of 3 * extensionDegree
                            // is reserved here
                            if (EndDegree - StartDegree < 2 * Values.Pi - 3 * extensionDegree)
                            {
                                var startDegreeWithExtension = StartDegree - extensionDegree;
                                var endDegreeWithExtension = EndDegree + extensionDegree;

                                arcToCreate = TransformArc(origin, radius, startDegreeWithExtension,
                                    endDegreeWithExtension, ArcFirstBubbleLoc);
                            }
                            else
                            {
                                try
                                {
                                    arcToCreate = TransformArc(origin, radius, StartDegree, EndDegree,
                                        ArcFirstBubbleLoc);
                                }
                                catch (ArgumentException)
                                {
                                    var failureReason = ResManager.GetString("EndPointsTooClose");
                                    if (!failureReasons.Contains(failureReason)) failureReasons.Add(failureReason);
                                    errorCount++;
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                arcToCreate = TransformArc(origin, radius, StartDegree, EndDegree, ArcFirstBubbleLoc);
                            }
                            catch (ArgumentException)
                            {
                                var failureReason = ResManager.GetString("EndPointsTooClose");
                                if (!failureReasons.Contains(failureReason)) failureReasons.Add(failureReason);
                                errorCount++;
                                continue;
                            }
                        }

                        if (i == 0)
                        {
                            var grid = NewGrid(arcToCreate);
                            if (grid != null)
                                try
                                {
                                    grid.Name = ArcFirstLabel;
                                }
                                catch (ArgumentException)
                                {
                                    ShowMessage(ResManager.GetString("FailedToSetLabel") + ArcFirstLabel + "!",
                                        ResManager.GetString("FailureCaptionSetLabel"));
                                }
                        }
                        else
                        {
                            curves.Append(arcToCreate);
                        }
                    }
                }
                catch (Exception)
                {
                    ++errorCount;
                }

            // Create grids with curves
            CreateGrids(curves);

            return errorCount;
        }
    }
}
