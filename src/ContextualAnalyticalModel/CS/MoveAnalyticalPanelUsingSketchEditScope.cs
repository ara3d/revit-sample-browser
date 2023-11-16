// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Ara3D.RevitSampleBrowser.ContextualAnalyticalModel.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class MoveAnalyticalPanelUsingSketchEditScope : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Expected results: the Analytical Panel has been moved and the connection with the Analytical Member has been broken
            try
            {
                //Get the Document
                var document = commandData.Application.ActiveUIDocument.Document;

                // Create Analytical Panel
                var analyticalPanel = CreateAnalyticalPanel.CreateAmPanel(document);

                // Create an Analytical Member connected with the Analytical Panel above
                CreateAnalyticalMember.CreateMember(document);

                // Move the Analytical Panel using SketchEditScope 
                var sketchEditScope = new SketchEditScope(document, "Move panel with SketchEditScope");
                sketchEditScope.StartWithNewSketch(analyticalPanel.Id);

                // Start transaction
                using (var transaction = new Transaction(document, "Offset panel"))
                {
                    transaction.Start();

                    // Get Sketch
                    if (document.GetElement(analyticalPanel.SketchId) is Sketch sketch)
                        foreach (CurveArray curveArray in sketch.Profile)
                            // Iterate through the Curves forming the Analytical Panel and 
                            // create new ones with a slight offset from the original ones before deleting them
                        foreach (Curve curve in curveArray)
                        {
                            var line = curve as Line;
                            if (line != null)
                            {
                                // Create new offseted Start and End points from the original line coordinates
                                var offset = 5.0;
                                var newLineStart = new XYZ(line.GetEndPoint(0).X + offset,
                                    line.GetEndPoint(0).Y + offset, 0);
                                var newLineEnd = new XYZ(line.GetEndPoint(1).X + offset, line.GetEndPoint(1).Y + offset,
                                    0);

                                // Define the new line with offseted coordinates
                                Curve offsetedLine = Line.CreateBound(newLineStart, newLineEnd);

                                // Remove the old line
                                document.Delete(line.Reference.ElementId);

                                // Create the new line
                                document.Create.NewModelCurve(offsetedLine, sketch.SketchPlane);
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
