// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Views;
using Ara3D.RevitSampleBrowser.WinderStairs.CS.Forms;
using Ara3D.RevitSampleBrowser.WinderStairs.CS.Winders;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
namespace Ara3D.RevitSampleBrowser.WinderStairs.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var rvtDoc = commandData.Application.ActiveUIDocument.Document;
                var selectionIds = commandData.Application.ActiveUIDocument.Selection.GetElementIds();
                if (selectionIds.Count is not 2 and not 3)
                {
                    message +=
                        "Please select two (or three) connected line elements. E.g. two (or three) connected straight Walls or Model Lines.";
                    return Result.Cancelled;
                }

                List<ElementId> selectedIds = new();
                selectedIds.AddRange(selectionIds);

                // Generate the winder creation parameters from selected elements.
                var controlPoints = StairsHelper.CalculateControlPoints(rvtDoc, selectedIds);
                double runWidth = 0, treadDepth = 0;
                GetStairsData(rvtDoc, out runWidth, out treadDepth);
                var maxCount = StairsHelper.CalculateMaxStepsCount(controlPoints, runWidth, treadDepth);
                uint numStepsInCorner = 3;
                var centerOffset = 0.0;
                switch (selectionIds.Count)
                {
                    // Create L-Winder
                    case 2:
                        {
                            using LWinderOptions options = new();
                            options.NumStepsAtStart = maxCount[0];
                            options.NumStepsAtEnd = maxCount[1];
                            options.NumStepsInCorner = numStepsInCorner;
                            options.RunWidth = runWidth;
                            options.CenterOffsetE = centerOffset;
                            options.CenterOffsetF = centerOffset;
                            if (options.ShowDialog() == DialogResult.OK)
                            {
                                LWinder lwinder = new()
                                {
                                    NumStepsAtStart = options.NumStepsAtStart,
                                    NumStepsInCorner = options.NumStepsInCorner,
                                    NumStepsAtEnd = options.NumStepsAtEnd,
                                    RunWidth = options.RunWidth,
                                    TreadDepth = treadDepth,
                                    CenterOffsetE = options.CenterOffsetE,
                                    CenterOffsetF = options.CenterOffsetF
                                };
                                var activeid = options.Dmu ? commandData.Application.ActiveAddInId : null;
                                new WinderUpdater(lwinder,
                                    selectedIds, rvtDoc, activeid, options.Sketch);
                            }

                            break;
                        }
                    // Create U-Winder
                    case 3:
                        {
                            using UWinderOptions options = new();
                            options.NumStepsAtStart = maxCount[0];
                            options.NumStepsInMiddle = maxCount[1];
                            options.NumStepsAtEnd = maxCount[2];
                            options.NumStepsInCorner1 = numStepsInCorner;
                            options.NumStepsInCorner2 = numStepsInCorner;
                            options.RunWidth = runWidth;
                            options.CenterOffsetE1 = centerOffset;
                            options.CenterOffsetF1 = centerOffset;
                            options.CenterOffsetE2 = centerOffset;
                            options.CenterOffsetF2 = centerOffset;
                            if (options.ShowDialog() == DialogResult.OK)
                            {
                                UWinder uwinder = new()
                                {
                                    NumStepsAtStart = options.NumStepsAtStart,
                                    NumStepsInCorner1 = options.NumStepsInCorner1,
                                    NumStepsInMiddle = options.NumStepsInMiddle,
                                    NumStepsInCorner2 = options.NumStepsInCorner2,
                                    NumStepsAtEnd = options.NumStepsAtEnd,
                                    RunWidth = options.RunWidth,
                                    TreadDepth = treadDepth,
                                    CenterOffsetE1 = options.CenterOffsetE1,
                                    CenterOffsetF1 = options.CenterOffsetF1,
                                    CenterOffsetE2 = options.CenterOffsetE2,
                                    CenterOffsetF2 = options.CenterOffsetF2
                                };
                                var activeid = options.Dmu ? commandData.Application.ActiveAddInId : null;
                                new WinderUpdater(uwinder,
                                    selectedIds, rvtDoc, activeid, options.Sketch);
                            }

                            break;
                        }
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        /// <summary>
        ///     Retrieve the system default stairs data, like run width and tread depth.
        /// </summary>
        /// <param name="rvtDoc">Revit Document</param>
        /// <param name="runWidth">Stairs run width</param>
        /// <param name="treadDepth">Stairs tread depth</param>
        private void GetStairsData(Document rvtDoc, out double runWidth, out double treadDepth)
        {
            FilteredElementCollector filterLevels = new(rvtDoc);
            var levels = filterLevels.OfClass(typeof(Level)).ToElements();
            if (levels.Count < 2) throw new InvalidOperationException("Need two Levels to create Stairs.");
            var levelList = levels.Cast<Level>().OrderBy(level => level.Elevation).Cast<Element>().ToList();
            using StairsEditScope stairsMode = new(rvtDoc, "DUMMY STAIRS SCOPE");
            var stairsId = stairsMode.Start(levelList[0].Id, levelList[1].Id);
            var stairs = rvtDoc.GetElement(stairsId) as Stairs;
            var stairsType = rvtDoc.GetElement(stairs.GetTypeId()) as StairsType;
            runWidth = stairsType.MinRunWidth;
            treadDepth = stairs.ActualTreadDepth;
        }
    }
}
