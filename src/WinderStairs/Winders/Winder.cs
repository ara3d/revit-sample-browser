// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.WinderStairs.CS.Winders
{
    public abstract class Winder
    {
        public virtual IList<XYZ> ControlPoints { get; set; }

        public double RunWidth { get; set; }

        public double TreadDepth { get; set; }

        protected IList<Curve> OuterBoundary { get; private set; }

        protected IList<Curve> InnerBoundary { get; private set; }

        protected IList<Curve> CenterWalkpath { get; private set; }

        protected IList<Curve> RiserLines { get; set; }

        /// <summary>
        ///     This method creates the sketched winder run and it also deletes the input winderRunId.
        /// </summary>
        /// <param name="rvtDoc">Revit Document to create the sketched run</param>
        /// <param name="drawSketch">Flag to control whether to draw the sketch in document</param>
        /// <param name="winderRunId">Previous created winder, it will be deleted</param>
        public void Build(Document rvtDoc, bool drawSketch, ref ElementId winderRunId)
        {
            // Preparing the sketch containers.
            OuterBoundary = [];
            InnerBoundary = [];
            CenterWalkpath = [];
            RiserLines = [];

            // Fill the sketch containers
            GenerateSketch();

            // Draw the sketch with model curves if required so.
            if (drawSketch) DebugSketch(rvtDoc);

            // Create the sketched run
            CreateWinderRun(rvtDoc, ref winderRunId);
        }

        /// <summary>
        ///     This method generates the winder sketch and fills the sketch to the properties:
        ///     OuterBoundary, InnerBoundary, CenterWalkpath and RiserLines.
        /// </summary>
        protected abstract void GenerateSketch();

        /// <summary>
        ///     This method creates the sketched run in Revit document.
        /// </summary>
        /// <param name="rvtDoc">Revit Document</param>
        /// <param name="winderRunId">Created winder run</param>
        private void CreateWinderRun(Document rvtDoc, ref ElementId winderRunId)
        {
            using StairsEditScope stairsMode = new(rvtDoc, GetType().Name);
            ElementId stairsId;
            // Non-existed stairs, create a new one.
            if (rvtDoc.GetElement(winderRunId) is not StairsRun winderOldRun)
            {
                // Find two levels to create a stairs between them
                var levelList = rvtDoc.GetElements<Level>().ToList();
                levelList.Sort((a, b) => a.Elevation.CompareTo(b.Elevation));
                // Start the stairs edit mode
                stairsId = stairsMode.Start(levelList[0].Id, levelList[1].Id);
            }
            else // using the existed stairs
            {
                // Start the stairs edit mode
                stairsId = stairsMode.Start(winderOldRun.GetStairs().Id);
            }

            using (Transaction winderTransaction = new(rvtDoc))
            {
                // Start the winder creation transaction 
                winderTransaction.Start(GetType().Name);

                // The boundaries is consist of public and external boundaries.
                List<Curve> boundarys = [.. InnerBoundary, .. OuterBoundary];

                // Calculate the run elevation.
                var stairs = rvtDoc.GetElement(stairsId) as Stairs;
                var elevation = ControlPoints[0].Z;
                var actualElevation = Math.Max(elevation, stairs.BaseElevation);

                // Create the run
                var run = StairsRun.CreateSketchedRun(rvtDoc, stairsId,
                    actualElevation, boundarys, RiserLines, CenterWalkpath);
                if (ElementId.InvalidElementId != winderRunId)
                    // Delete the old run
                    rvtDoc.Delete(winderRunId);
                // output the new run
                winderRunId = run.Id;
                // Finish the winder run creation.
                winderTransaction.Commit();
            }

            // Finish the stairs Edit mode.
            stairsMode.Commit(new StairsEditScopeFailuresPreprocessor());
        }

        /// <summary>
        ///     This method draws sketch as model curves in Revit document for debug purpose.
        /// </summary>
        /// <param name="rvtDoc">Revit document</param>
        private void DebugSketch(Document rvtDoc)
        {
            try
            {
                using Transaction debugTransaction = new(rvtDoc);
                debugTransaction.Start("DEBUG API WINDER SKETCH");

                var skp = SketchPlane.Create(
                    rvtDoc, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, ControlPoints[0]));
                CurveArray curves = new();

                foreach (var curve in OuterBoundary)
                {
                    curves.Append(curve);
                }

                foreach (var curve in InnerBoundary)
                {
                    curves.Append(curve);
                }

                foreach (var curve in CenterWalkpath)
                {
                    curves.Append(curve);
                }

                foreach (var curve in RiserLines)
                {
                    curves.Append(curve);
                }

                rvtDoc.Create.NewModelCurveArray(curves, skp);
                debugTransaction.Commit();
            }
            catch (Exception)
            {
                // SWALLOW ALL DEBUG EXCEPTION
            }
        }
    }

    public class StairsEditScopeFailuresPreprocessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            failuresAccessor.DeleteAllWarnings();
            return FailureProcessingResult.Continue;
        }
    }
}
