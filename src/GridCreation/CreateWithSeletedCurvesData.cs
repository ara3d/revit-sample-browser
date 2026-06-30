// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.GridCreation.CS
{
    public class CreateWithSelectedCurvesData : CreateGridsData
    {
        private readonly CurveArray m_selectedCurves;

        public CreateWithSelectedCurvesData(UIApplication application, CurveArray selectedCurves, ArrayList labels)
            : base(application, labels)
        {
            m_selectedCurves = selectedCurves;
        }

        public bool DeleteSelectedElements { get; set; }

        public BubbleLocation BubbleLocation { get; set; }

        public string FirstLabel { get; set; }

        public void CreateGrids()
        {
            var errorCount = 0;

            var curves = new CurveArray();

            var i = 0;
            foreach (Curve curve in m_selectedCurves)
            {
                try
                {
                    var line = curve as Line;
                    if (line != null) // Selected curve is a line
                    {
                        var lineToCreate = TransformLine(line, BubbleLocation);
                        if (i == 0)
                        {
                            var grid = CreateLinearGrid(lineToCreate);

                            try
                            {
                                grid.Name = FirstLabel;
                            }
                            catch (ArgumentException)
                            {
                                ShowMessage($"{ResManager.GetString("FailedToSetLabel")}{FirstLabel}!",
                                    ResManager.GetString("FailureCaptionSetLabel"));
                            }
                        }
                        else
                        {
                            AddCurveForBatchCreation(ref curves, lineToCreate);
                        }
                    }
                    else // Selected curve is an arc
                    {
                        var arc = curve as Arc;
                        if (arc != null)
                        {
                            if (arc.IsBound) // Part of a circle
                            {
                                var arcToCreate = TransformArc(arc, BubbleLocation);

                                if (i == 0)
                                {
                                    var grid = NewGrid(arcToCreate);

                                    try
                                    {
                                        grid.Name = FirstLabel;
                                    }
                                    catch (ArgumentException)
                                    {
                                        ShowMessage($"{ResManager.GetString("FailedToSetLabel")}{FirstLabel}!",
                                            ResManager.GetString("FailureCaptionSetLabel"));
                                    }
                                }
                                else
                                {
                                    AddCurveForBatchCreation(ref curves, arcToCreate);
                                }
                            }
                            else
                            {
                                // NewGrid(circle) throws; UI creates two semicircle grids instead.
                                Arc upperArc = null;
                                Arc lowerArc = null;

                                TransformCircle(arc, ref upperArc, ref lowerArc, BubbleLocation);
                                if (i == 0)
                                {
                                    var gridUpper = NewGrid(upperArc);
                                    try
                                    {
                                        // Set label of first grid
                                        gridUpper.Name = FirstLabel;
                                    }
                                    catch (ArgumentException)
                                    {
                                        ShowMessage($"{ResManager.GetString("FailedToSetLabel")}{FirstLabel}!",
                                            ResManager.GetString("FailureCaptionSetLabel"));
                                    }

                                    AddCurveForBatchCreation(ref curves, lowerArc);
                                }
                                else
                                {
                                    AddCurveForBatchCreation(ref curves, upperArc);
                                    AddCurveForBatchCreation(ref curves, lowerArc);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    ++errorCount;
                    continue;
                }

                ++i;
            }

            CreateGrids(curves);

            if (DeleteSelectedElements)
                try
                {
                    foreach (Element e in Command.GetSelectedModelLinesAndArcs(RevitDoc))
                    {
                        RevitDoc.Delete(e.Id);
                    }
                }
                catch (Exception)
                {
                    ShowMessage(ResManager.GetString("FailedToDeletedLinesOrArcs"),
                        ResManager.GetString("FailureCaptionDeletedLinesOrArcs"));
                }

            if (errorCount != 0)
                ShowMessage(ResManager.GetString("FailedToCreateGrids"),
                    ResManager.GetString("FailureCaptionCreateGrids"));
        }
    }
}
