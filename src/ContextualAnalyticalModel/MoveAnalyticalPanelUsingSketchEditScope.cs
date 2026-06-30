// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MoveAnalyticalPanelUsingSketchEditScope : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                // Create Analytical Panel
                var analyticalPanel = CreateAnalyticalPanel.CreateAmPanel(document);

                // Create an Analytical Member connected with the Analytical Panel above
                CreateAnalyticalMember.CreateMember(document);

                // Move the Analytical Panel using SketchEditScope 
                SketchEditScope sketchEditScope = new(document, "Move panel with SketchEditScope");
                sketchEditScope.StartWithNewSketch(analyticalPanel.Id);

                // Start transaction
                using (Transaction transaction = new(document, "Offset panel"))
                {
                    transaction.Start();

                    // Get Sketch
                    if (document.GetElement(analyticalPanel.SketchId) is Sketch sketch)
                        foreach (CurveArray curveArray in sketch.Profile)
                        // Iterate through the Curves forming the Analytical Panel and 
                        // create new ones with a slight offset from the original ones before deleting them
                        {
                            foreach (Curve curve in curveArray)
                            {
                                var line = curve as Line;
                                if (line != null)
                                {
                                    // Create new offseted Start and End points from the original line coordinates
                                    var offset = 5.0;
                                    XYZ newLineStart = new(line.GetEndPoint(0).X + offset,
                                        line.GetEndPoint(0).Y + offset, 0);
                                    XYZ newLineEnd = new(line.GetEndPoint(1).X + offset, line.GetEndPoint(1).Y + offset,
                                        0);

                                    Curve offsetedLine = Line.CreateBound(newLineStart, newLineEnd);

                                    // Remove the old line
                                    document.Delete(line.Reference.ElementId);

                                    document.Create.NewModelCurve(offsetedLine, sketch.SketchPlane);
                                }
                            }
                        }

                    transaction.Commit();
                }

                sketchEditScope.Commit(new FailurePreproccessor());

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
