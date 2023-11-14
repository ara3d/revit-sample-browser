// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitMultiSample.SheetToView3D.CS
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.NoCommandData)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            if (null == commandData) throw new ArgumentNullException(nameof(commandData));

            var result = Result.Succeeded;
            try
            {
                result = MakeView3D.MakeFromViewportClick(commandData.Application.ActiveUIDocument);
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }

            return result;
        }
    }

    /// <summary>
    ///     Generates a View3D from a click on a viewport on a sheet.
    /// </summary>
    public class MakeView3D
    {
        private static readonly double ClickTolerance = 0.0001;

        private static readonly double RandomXScale = 5631;
        private static readonly double RandomYScale = 4369;

        /// <summary>
        ///     Makes a View3D from a click on a viewport on a sheet.
        /// </summary>
        /// <param name="uidoc">the currently active uidocument</param>
        /// <param name="doc">the currently active document</param>
        public static Result MakeFromViewportClick(UIDocument uidoc)
        {
            if (null == uidoc) throw new ArgumentNullException(nameof(uidoc));

            var doc = uidoc.Document;
            if (null == doc) throw new InvalidOperationException("The document can't be found.");

            var result = Result.Succeeded;

            // Have the user click on a plan view viewport on a sheet.
            var click = uidoc.Selection.PickPoint(
                "Click on a plan view viewport on a sheet to create a perspective View3D with its camera at that point.");
            if (null == click) throw new InvalidOperationException("Please click on a plan view viewport on a sheet.");

            // Make sure the active view was a sheet view.
            if (!(uidoc.ActiveGraphicalView is ViewSheet viewSheet)) throw new InvalidOperationException("The click was not on a sheet.");

            // Find which viewport was clicked.
            var clickedViewport = GetViewportAtClick(viewSheet, click);
            if (null == clickedViewport) throw new InvalidOperationException("The click was not on a viewport.");

            // Verify that the transforms are reported by the viewport and its view.
            if (!(doc.GetElement(clickedViewport.ViewId) is View clickedView) || !clickedView.HasViewTransforms() || !clickedViewport.HasViewportTransforms())
                throw new InvalidOperationException(
                    "The clicked viewport doesn't report 3D model space to sheet space transforms.");

            // Restrict application to plan view types.  
            // Note: Sections and Elevations report transforms but are not covered in this demo.
            if (ViewType.AreaPlan != clickedView.ViewType &&
                ViewType.CeilingPlan != clickedView.ViewType &&
                ViewType.EngineeringPlan != clickedView.ViewType &&
                ViewType.FloorPlan != clickedView.ViewType)
                throw new InvalidOperationException("Only plan views are supported by this demo application.");

            if (!(clickedView is ViewPlan plan))
                throw new InvalidOperationException("Only plan views are supported by this demo application.");

            // Convert the viewport click into a ray through 3d model space.
            // Note: The output XYZ needs to be projected onto the view's cut plane before use.
            var clickAsModelRay = CalculateClickAsModelRay(clickedViewport, click);
            if (null == clickAsModelRay)
                throw new InvalidOperationException("The click was outside the view crop regions.");

            // Project the ray onto the view's cut plane.  
            // This picks a reasonable height in the model for the View3D camera.
            var cutPlane = GetViewPlanCutPlane(plan);
            if (null == cutPlane)
                throw new InvalidOperationException("An error occured when getting the view's cut plane.");
            var view3dCameraLocation = ProjectPointOnPlane(cutPlane, clickAsModelRay);
            if (null == view3dCameraLocation)
                throw new InvalidOperationException("An error occured when calculating the View3D camera position.");

            using (var tran = new Transaction(doc, "New 3D View"))
            {
                tran.Start();

                // Create a new perspective 3D View with its camera at the point.
                var view3d = Create3DView(doc, view3dCameraLocation, XYZ.BasisZ, XYZ.BasisY);
                if (null != view3d)
                {
                    tran.Commit();

                    // Activate the new 3D view.
                    uidoc.ActiveView = view3d;
                }
                else
                {
                    tran.RollBack();
                    throw new InvalidOperationException("Failed to generate the 3D view.");
                }
            }

            return result;
        }

        /// <summary>
        ///     Find the viewport at a point on a sheet.
        /// </summary>
        /// <param name="viewSheet">The ViewSheet that was clicked</param>
        /// <param name="click">The click point</param>
        /// <returns>The viewport which was clicked, or null if no viewport was clicked.</returns>
        private static Viewport GetViewportAtClick(ViewSheet viewSheet, XYZ click)
        {
            if (null == viewSheet || null == click)
                return null;

            var doc = viewSheet.Document;
            if (null == doc)
                return null;

            foreach (var vpId in viewSheet.GetAllViewports())
            {
                if (doc.GetElement(vpId) is Viewport viewport && viewport.GetBoxOutline().Contains(click, ClickTolerance))
                    // Click is within the viewport
                    return viewport;
            }

            // Click was not contained by any viewport
            return null;
        }

        /// <summary>
        ///     Makes the sheet space --> 3D model space transform.
        /// </summary>
        /// <param name="trfModelToProjection">The 3D model space --> view projection space transform.</param>
        /// <param name="trfProjectionToSheet">The view projection space --> sheet space transform.</param>
        /// <returns>The sheet space --> 3D model space transform.</returns>
        private static Transform MakeSheetToModelTransform(Transform trfModelToProjection,
            Transform trfProjectionToSheet)
        {
            if (null == trfModelToProjection || null == trfProjectionToSheet)
                return null;

            var modelToSheetTrf = trfProjectionToSheet.Multiply(trfModelToProjection);
            return modelToSheetTrf.Inverse;
        }

        /// <summary>
        ///     Projects a point on a plane.
        /// </summary>
        /// <param name="plane">The plane on which the point will be projected</param>
        /// <param name="point">The point to projected onto the plane.</param>
        /// <returns>The point projected onto the plane.</returns>
        private static XYZ ProjectPointOnPlane(Plane plane, XYZ point)
        {
            var uv = new UV();
            plane.Project(point, out uv, out _);
            return plane.Origin + plane.XVec * uv.U + plane.YVec * uv.V;
        }

        /// <summary>
        ///     Tests if a point is within a curveloop.  The point wil be projected onto the
        ///     plane which holds the curveloop before the test is executed.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <param name="curveloop">A curveloop that lies in a plane.</param>
        /// <returns>
        ///     True, if the point was inside the curveloop after the
        ///     point was projected onto the curveloop's plane. False, otherwise.
        /// </returns>
        private static bool IsPointInsideCurveLoop(XYZ point, CurveLoop curveloop)
        {
            // Starting at the point, shoot an infinite ray in one direction and count how many
            // times the ray intersects the edges of the curveloop.  If the ray intersects
            // an odd number of edges, then the point was inside the curveloop.  If it 
            // intersects an even number of times, then the point was outside the curveloop.
            //
            // Note: Revit doesn't have an infinite ray class, so a very long Line is used instead.
            // This test can fail if the curveloop is wider than the very long line.

            // Calculate the plane on which the edges of the curveloop lie.
            var plane = Plane.CreateByThreePoints(curveloop.ElementAt(0).GetEndPoint(0),
                curveloop.ElementAt(1).GetEndPoint(0),
                curveloop.ElementAt(2).GetEndPoint(0));

            // Project the test point on the plane.
            var projectedPoint = ProjectPointOnPlane(plane, point);

            // Create a very long bounded line that starts at projectedPoint and runs 
            // along the plane's surface.
            var veryLongLine = Line.CreateBound(projectedPoint,
                projectedPoint + RandomXScale * plane.XVec + RandomYScale * plane.YVec);

            // Count how many edges of curveloop intersect veryLongLine.
            var intersectionCount = 0;
            foreach (var edge in curveloop)
            {
                IntersectionResultArray resultArray;
                var res = veryLongLine.Intersect(edge, out resultArray);
                if (SetComparisonResult.Overlap == res) intersectionCount += resultArray.Size;
            }

            // If the intersection count is ODD, then the point is inside the curveloop.
            return 1 == intersectionCount % 2;
        }

        /// <summary>
        ///     Starting with a click on a viewport on a sheet, this method calculates the corresponding
        ///     ray through 3D model.
        /// </summary>
        /// <param name="viewport">The viewport on a sheet that was clicked.</param>
        /// <param name="click">The clicked point.</param>
        /// <returns>
        ///     An XYZ that represents a ray through the model in the direction of the viewport view's
        ///     view direction vector.
        /// </returns>
        private static XYZ CalculateClickAsModelRay(Viewport viewport, XYZ click)
        {
            if (null == viewport || null == click)
                return null;

            var doc = viewport.Document;

            if (!(doc?.GetElement(viewport.ViewId) is View view))
                return null;

            // Transform for view projection space --> sheet space
            var trfProjectionToSheet = new Transform(viewport.GetProjectionToSheetTransform());

            // Most views have just one model space --> view projection space transform. 
            // However, views whose view crops are broken into multiple regions
            // have more than one transform.
            //
            // Iterate all the model space --> view projection space transforms.  
            // Look for the region that contains the click as a model point.
            foreach (var trfWithBoundary in view.GetModelToProjectionTransforms())
            {
                // Make the sheet space --> 3D model space transform for the current crop region.
                var trfSheetToModel = MakeSheetToModelTransform(trfWithBoundary.GetModelToProjectionTransform(),
                    trfProjectionToSheet);
                if (null == trfSheetToModel)
                    throw new InvalidOperationException(
                        "An error occured when calculating the sheet-to-model transforms.");

                // Transform the click point into 3D model space.
                var clickAsModelRay = trfSheetToModel.OfPoint(click);

                // Get the edges of the current crop region.
                var modelCurveLoop = trfWithBoundary.GetBoundary();

                if (null == modelCurveLoop)
                    // Views that are uncropped will have just one TransformWithBoundary and its
                    // curveloop will be null.  All sheet points will be subject to the transform
                    // from this TransformWithBoundary.
                    return clickAsModelRay;
                if (IsPointInsideCurveLoop(clickAsModelRay, modelCurveLoop)) return clickAsModelRay;
            }

            // The clicked point on the sheet is outside of all of the TransformWithBoundary crop regions.
            // The model ray can't be calculated.
            return null;
        }

        /// <summary>
        ///     Gets the cut plane from a plan view.
        /// </summary>
        /// <param name="plan">The plan view containing the cut plane.</param>
        /// <returns>A plane representing the plan view's cut plane.</returns>
        private static Plane GetViewPlanCutPlane(ViewPlan plan)
        {
            if (null == plan)
                return null;

            var levelElevation = 0.0;
            if (null != plan.GenLevel)
                levelElevation = plan.GenLevel.Elevation;
            var cutPlaneOffset = plan.GetViewRange().GetOffset(PlanViewPlane.CutPlane);
            var viewCutPlaneElevation = levelElevation + cutPlaneOffset;

            return Plane.CreateByNormalAndOrigin(plan.ViewDirection, new XYZ(0.0, 0.0, viewCutPlaneElevation));
        }

        /// <summary>
        ///     Creates a perspective View3D.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="eyePosition">The eye position in 3D model space for the new 3D view.</param>
        /// <param name="upDir">The up direction in 3D model space for the new 3D view.</param>
        /// <param name="forwardDir">The forward direction in 3D model space for the new 3D view.</param>
        /// <returns>The new View3D.</returns>
        private static View3D Create3DView(Document doc, XYZ eyePosition, XYZ upDir, XYZ forwardDir)
        {
            if (null == doc || null == eyePosition || null == upDir || null == forwardDir)
                return null;

            var vft = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault(t => t.ViewFamily == ViewFamily.ThreeDimensional);

            var view3d = View3D.CreatePerspective(doc, vft.Id);
            if (null == view3d) return null;

            view3d.SetOrientation(new ViewOrientation3D(eyePosition, upDir, forwardDir));

            return view3d;
        }
    }
}
