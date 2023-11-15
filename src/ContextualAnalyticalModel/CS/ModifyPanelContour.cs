// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ContextualAnalyticalModel
{
    /// <summary>
    ///     Implements the interface IFailuresPreprocessor
    /// </summary>
    public class FailurePreproccessor : IFailuresPreprocessor
    {
        /// <summary>
        ///     This method is called when there have been failures found at the end of a transaction and Revit is about to start
        ///     processing them.
        /// </summary>
        /// <param name="failuresAccessor">The Interface class that provides access to the failure information. </param>
        /// <returns></returns>
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            return FailureProcessingResult.Continue;
        }
    }

    /// <summary>
    ///     Implements the Revit add-in interface IExternalCommand
    ///     Modify Analytical Panel contour using SketchEditScope
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class ModifyPanelContour : IExternalCommand
    {
        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var document = commandData.Application.ActiveUIDocument.Document;

                //create analytical panel
                var analyticalPanel = CreateAnalyticalPanel.CreateAmPanel(document);
                if (analyticalPanel != null)
                {
                    // Start a sketch edit scope
                    var sketchEditScope = new SketchEditScope(document, "Replace line with an arc");
                    sketchEditScope.StartWithNewSketch(analyticalPanel.Id);

                    using (var transaction = new Transaction(document, "Modify sketch"))
                    {
                        transaction.Start();

                        //replace a boundary line with an arc
                        Line line = null;
                        var sketch = document.GetElement(analyticalPanel.SketchId) as Sketch;
                        if (sketch != null)
                            foreach (CurveArray curveArray in sketch.Profile)
                            {
                                foreach (Curve curve in curveArray)
                                {
                                    line = curve as Line;
                                    if (line != null) break;
                                }

                                if (line != null) break;
                            }

                        // Create arc
                        var normal = line.Direction.CrossProduct(XYZ.BasisZ).Normalize().Negate();
                        var middle = line.GetEndPoint(0).Add(line.Direction.Multiply(line.Length / 2));
                        Curve arc = Arc.Create(line.GetEndPoint(0), line.GetEndPoint(1),
                            middle.Add(normal.Multiply(20)));

                        // Remove element referenced by the found line. 
                        document.Delete(line.Reference.ElementId);

                        // Model curve creation automatically puts the curve into the sketch, if sketch edit scope is running.
                        document.Create.NewModelCurve(arc, sketch.SketchPlane);

                        transaction.Commit();
                    }

                    sketchEditScope.Commit(new FailurePreproccessor());
                }

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
