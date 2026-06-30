// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;

namespace Ara3D.RevitSampleBrowser.GridCreation.CS
{
    public class CreateOrthogonalGridsData : CreateGridsData
    {
        public CreateOrthogonalGridsData(UIApplication application, ForgeTypeId unit, ArrayList labels)
            : base(application, labels, unit)
        {
        }
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

        public double XOrigin { get; set; }

        public double YOrigin { get; set; }

        public double XSpacing { get; set; }

        public double YSpacing { get; set; }

        public uint XNumber { get; set; }

        public uint YNumber { get; set; }

        public BubbleLocation XBubbleLoc { get; set; }

        public BubbleLocation YBubbleLoc { get; set; }

        public string XFirstLabel { get; set; }

        public string YFirstLabel { get; set; }

        public void CreateGrids()
        {
            ArrayList failureReasons = [];
            if (CreateXGrids(ref failureReasons) + CreateYGrids(ref failureReasons) != 0)
            {
                var failureReason = ResManager.GetString("FailedToCreateGrids");
                if (failureReasons.Count != 0)
                {
                    failureReason += $"{ResManager.GetString("Reasons")}\r";
                    failureReason += "\r";
                    foreach (string reason in failureReasons)
                    {
                        failureReason += $"{reason}\r";
                    }
                }

                failureReason += $"\r{ResManager.GetString("AjustValues")}";

                ShowMessage(failureReason, ResManager.GetString("FailureCaptionCreateGrids"));
            }
        }

        private int CreateXGrids(ref ArrayList failureReasons)
        {
            var errorCount = 0;

            // Curve array which stores all curves for batch creation
            CurveArray curves = new();

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
                        startPoint = new XYZ(XOrigin - (YSpacing / 2), YOrigin + (i * XSpacing), 0);
                        endPoint = new XYZ(XOrigin + ((YNumber - 1) * YSpacing) + (YSpacing / 2), YOrigin + (i * XSpacing),
                            0);
                    }
                    else
                    {
                        startPoint = new XYZ(XOrigin, YOrigin + (i * XSpacing), 0);
                        endPoint = new XYZ(XOrigin + (XSpacing / 2), YOrigin + (i * XSpacing), 0);
                    }

                    try
                    {
                        // Create a line according to the bubble location
                        line = XBubbleLoc == BubbleLocation.StartPoint ? NewLine(startPoint, endPoint) : NewLine(endPoint, startPoint);
                    }
                    catch (ArgumentException)
                    {
                        var failureReason = ResManager.GetString("SpacingsTooSmall");
                        if (!failureReasons.Contains(failureReason)) failureReasons.Add(failureReason);
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
                            ShowMessage($"{ResManager.GetString("FailedToSetLabel")}{XFirstLabel}!",
                                ResManager.GetString("FailureCaptionSetLabel"));
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
                }
            }

            // Create grids with curve array
            CreateGrids(curves);

            return errorCount;
        }

        private int CreateYGrids(ref ArrayList failureReasons)
        {
            var errorCount = 0;

            // Curve array which stores all curves for batch creation
            CurveArray curves = new();

            for (var j = 0; j < YNumber; ++j)
            {
                XYZ startPoint;
                XYZ endPoint;
                Line line;

                try
                {
                    if (XNumber != 0)
                    {
                        startPoint = new XYZ(XOrigin + (j * YSpacing), YOrigin - (XSpacing / 2), 0);
                        endPoint = new XYZ(XOrigin + (j * YSpacing), YOrigin + ((XNumber - 1) * XSpacing) + (XSpacing / 2),
                            0);
                    }
                    else
                    {
                        startPoint = new XYZ(XOrigin + (j * YSpacing), YOrigin, 0);
                        endPoint = new XYZ(XOrigin + (j * YSpacing), YOrigin + (YSpacing / 2), 0);
                    }

                    try
                    {
                        // Create a line according to the bubble location
                        line = YBubbleLoc == BubbleLocation.StartPoint ? NewLine(startPoint, endPoint) : NewLine(endPoint, startPoint);
                    }
                    catch (ArgumentException)
                    {
                        var failureReason = ResManager.GetString("SpacingsTooSmall");
                        if (!failureReasons.Contains(failureReason)) failureReasons.Add(failureReason);
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
                            ShowMessage($"{ResManager.GetString("FailedToSetLabel")}{YFirstLabel}!",
                                ResManager.GetString("FailureCaptionSetLabel"));
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
                }
            }

            // Create grids with curves
            CreateGrids(curves);

            return errorCount;
        }
    }
}
