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
using System.Collections;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.GridCreation.CS
{
    /// <summary>
    /// Data class which stores information for creating orthogonal grids
    /// </summary>
    public class CreateOrthogonalGridsData : CreateGridsData
    {
                // X coordinate of origin
        // Y coordinate of origin
        // Spacing between horizontal grids
        // Spacing between vertical grids
        // Number of horizontal grids
        // Number of vertical grids
        // Bubble location of horizontal grids
        // Bubble location of vertical grids
        // Label of first horizontal grid
        // Label of first vertical grid

        
                /// <summary>
        /// X coordinate of origin
        /// </summary>
        public double XOrigin { get; set; }

        /// <summary>
        /// Y coordinate of origin
        /// </summary>
        public double YOrigin { get; set; }

        /// <summary>
        /// Spacing between horizontal grids
        /// </summary>
        public double XSpacing { get; set; }

        /// <summary>
        /// Spacing between vertical grids
        /// </summary>
        public double YSpacing { get; set; }

        /// <summary>
        /// Number of horizontal grids
        /// </summary>
        public uint XNumber { get; set; }

        /// <summary>
        /// Number of vertical grids
        /// </summary>
        public uint YNumber { get; set; }

        /// <summary>
        /// Bubble location of horizontal grids
        /// </summary>
        public BubbleLocation XBubbleLoc { get; set; }

        /// <summary>
        /// Bubble location of vertical grids
        /// </summary>
        public BubbleLocation YBubbleLoc { get; set; }

        /// <summary>
        /// Label of first horizontal grid
        /// </summary>
        public string XFirstLabel { get; set; }

        /// <summary>
        /// Label of first vertical grid
        /// </summary>
        public string YFirstLabel { get; set; }

        
                /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="application">Application object</param>
        /// <param name="unit">Current length display unit type</param>
        /// <param name="labels">All existing labels in Revit's document</param>
        public CreateOrthogonalGridsData(UIApplication application, ForgeTypeId unit, ArrayList labels)
            : base(application, labels, unit)
        {
        }

        /// <summary>
        /// Create grids
        /// </summary>
        public void CreateGrids()
        {
            var failureReasons = new ArrayList();
            if (CreateXGrids(ref failureReasons) + CreateYGrids(ref failureReasons) != 0)
            {
                var failureReason = resManager.GetString("FailedToCreateGrids");
                if (failureReasons.Count != 0)
                {
                    failureReason += resManager.GetString("Reasons") + "\r";
                    failureReason += "\r";
                    foreach (string reason in failureReasons)
                    {
                        failureReason += reason + "\r";
                    }
                }
                
                failureReason += "\r" + resManager.GetString("AjustValues");

                ShowMessage(failureReason, resManager.GetString("FailureCaptionCreateGrids"));
            }
        }

        /// <summary>
        /// Create horizontal grids
        /// </summary>
        /// <param name="failureReasons">ArrayList contains failure reasons</param>
        /// <returns>Number of grids failed to create</returns>
        private int CreateXGrids(ref ArrayList failureReasons)
        {
            var errorCount = 0;

            // Curve array which stores all curves for batch creation
            var curves = new CurveArray();

            for (var i = 0; i < XNumber; ++i)
            {
                XYZ startPoint;
                XYZ endPoint;
                Line line;

                try
                {
                    if (YNumber != 0)
                    {
                        // Grids will have an extension distance of m_ySpacing / 2
                        startPoint = new XYZ (XOrigin - YSpacing / 2, YOrigin + i * XSpacing, 0);
                        endPoint = new XYZ (XOrigin + (YNumber - 1) * YSpacing + YSpacing / 2, YOrigin + i * XSpacing, 0);
                    }
                    else
                    {
                        startPoint = new XYZ (XOrigin, YOrigin + i * XSpacing, 0);
                        endPoint = new XYZ (XOrigin + XSpacing / 2, YOrigin + i * XSpacing, 0);

                    }

                    try
                    {
                        // Create a line according to the bubble location
                        if (XBubbleLoc == BubbleLocation.StartPoint)
                        {
                            line = NewLine(startPoint, endPoint);
                        }
                        else
                        {
                            line = NewLine(endPoint, startPoint);
                        }
                    }
                    catch (ArgumentException)
                    {
                        var failureReason = resManager.GetString("SpacingsTooSmall");
                        if (!failureReasons.Contains(failureReason))
                        {
                            failureReasons.Add(failureReason);
                        }
                        errorCount++;
                        continue;
                    }

                    if (i == 0)
                    {
                        var grid =
                            // Create grid with line
                            NewGrid(line);

                        try
                        {
                            // Set the label of first horizontal grid
                            grid.Name = XFirstLabel;
                        }
                        catch (ArgumentException)
                        {
                            ShowMessage(resManager.GetString("FailedToSetLabel") + XFirstLabel + "!",
                                        resManager.GetString("FailureCaptionSetLabel"));
                        }
                    }
                    else
                    {
                        // Add the line to curve array
                        curves.Append(line);
                    }
                }
                catch (Exception)
                {
                    ++errorCount;
                    continue;
                }                
            }

            // Create grids with curve array
            CreateGrids(curves);

            return errorCount;
        }

        /// <summary>
        /// Create vertical grids
        /// </summary>
        /// <param name="failureReasons">ArrayList contains failure reasons</param>
        /// <returns>Number of grids failed to create</returns>
        private int CreateYGrids(ref ArrayList failureReasons)
        {
            var errorCount = 0;

            // Curve array which stores all curves for batch creation
            var curves = new CurveArray();

            for (var j = 0; j < YNumber; ++j)
            {
                XYZ startPoint;
                XYZ endPoint;
                Line line;

                try
                {
                    if (XNumber != 0)
                    {
                        startPoint = new XYZ (XOrigin + j * YSpacing, YOrigin - XSpacing / 2, 0);
                        endPoint = new XYZ (XOrigin + j * YSpacing, YOrigin + (XNumber - 1) * XSpacing + XSpacing / 2, 0);
                    }
                    else
                    {
                        startPoint = new XYZ (XOrigin + j * YSpacing, YOrigin, 0);
                        endPoint = new XYZ (XOrigin + j * YSpacing, YOrigin + YSpacing / 2, 0);
                    }

                    try
                    {
                        // Create a line according to the bubble location
                        if (YBubbleLoc == BubbleLocation.StartPoint)
                        {
                            line = NewLine(startPoint, endPoint);
                        }
                        else
                        {
                            line = NewLine(endPoint, startPoint);
                        }
                    }
                    catch (ArgumentException)
                    {
                        var failureReason = resManager.GetString("SpacingsTooSmall");
                        if (!failureReasons.Contains(failureReason))
                        {
                            failureReasons.Add(failureReason);
                        }
                        errorCount++;
                        continue;
                    }

                    if (j == 0)
                    {
                        var grid =
                            // Create grid with line
                            NewGrid(line);

                        try
                        {
                            // Set label of first vertical grid
                            grid.Name = YFirstLabel;
                        }
                        catch (ArgumentException)
                        {
                            ShowMessage(resManager.GetString("FailedToSetLabel") + YFirstLabel + "!",
                                        resManager.GetString("FailureCaptionSetLabel"));
                        }
                    }
                    else
                    {
                        // Add the line to curve array
                        curves.Append(line);
                    }
                }
                catch (Exception)
                {
                    ++errorCount;
                    continue;
                }
            }

            // Create grids with curves
            CreateGrids(curves);

            return errorCount;
        }
            }
}
