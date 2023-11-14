// (C) Copyright 2003-2022 by Autodesk, Inc.
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
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.SheetToView3D.CS
{
   /// <summary>
   /// Implements the Revit add-in interface IExternalCommand
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
   public class Command : IExternalCommand
   {
      #region IExternalCommand Members Implementation
      /// <summary>
      /// Implement this method as an external command for Revit.
      /// </summary>
      /// <param name="commandData">An object that is passed to the external application 
      /// which contains data related to the command, 
      /// such as the application object and active view.</param>
      /// <param name="message">A message that can be set by the external application 
      /// which will be displayed if a failure or cancellation is returned by 
      /// the external command.</param>
      /// <param name="elements">A set of elements to which the external application 
      /// can add elements that are to be highlighted in case of failure or cancellation.</param>
      /// <returns>Return the status of the external command. 
      /// A result of Succeeded means that the API external method functioned as expected. 
      /// Cancelled can be used to signify that the user cancelled the external operation 
      /// at some point. Failure should be returned if the application is unable to proceed with 
      /// the operation.</returns>
      public Result Execute(ExternalCommandData commandData,
        ref string message, ElementSet elements)
      {
         if (null == commandData)
         {
            throw new ArgumentNullException("commandData");
         }

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

      #endregion IExternalCommand Members Implementation
   }

   /// <summary>
   /// Generates a View3D from a click on a viewport on a sheet.
   /// </summary>
   public class MakeView3D
   {
      private static double CLICK_TOLERANCE = 0.0001;

      /// <summary>
      /// Makes a View3D from a click on a viewport on a sheet.
      /// </summary>
      /// <param name="uidoc">the currently active uidocument</param>
      /// <param name="doc">the currently active document</param>
      public static Result MakeFromViewportClick(UIDocument uidoc)
      {
         if (null == uidoc)
         {
            throw new ArgumentNullException("uidoc");
         }

         var doc = uidoc.Document;
         if (null == doc)
         {
            throw new InvalidOperationException("The document can't be found.");
         }

         var result = Result.Succeeded;

         // Have the user click on a plan view viewport on a sheet.
         var click = uidoc.Selection.PickPoint("Click on a plan view viewport on a sheet to create a perspective View3D with its camera at that point.");
         if (null == click)
         {
            throw new InvalidOperationException("Please click on a plan view viewport on a sheet.");
         }

         // Make sure the active view was a sheet view.
         var viewSheet = uidoc.ActiveGraphicalView as ViewSheet;
         if (null == viewSheet)
         {
            throw new InvalidOperationException("The click was not on a sheet.");
         }

         // Find which viewport was clicked.
         var clickedViewport = GetViewportAtClick(viewSheet, click);
         if (null == clickedViewport)
         {
            throw new InvalidOperationException("The click was not on a viewport.");
         }

         // Verify that the transforms are reported by the viewport and its view.
         var clickedView = doc.GetElement(clickedViewport.ViewId) as View;
         if (null == clickedView || !clickedView.HasViewTransforms() || !clickedViewport.HasViewportTransforms())
         {
            throw new InvalidOperationException("The clicked viewport doesn't report 3D model space to sheet space transforms.");
         }

         // Restrict application to plan view types.  
         // Note: Sections and Elevations report transforms but are not covered in this demo.
         if (ViewType.AreaPlan != clickedView.ViewType &&
             ViewType.CeilingPlan != clickedView.ViewType &&
             ViewType.EngineeringPlan != clickedView.ViewType &&
             ViewType.FloorPlan != clickedView.ViewType)
         {
            throw new InvalidOperationException("Only plan views are supported by this demo application.");
         }

         var plan = clickedView as ViewPlan;
         if (null == plan)
         {
            throw new InvalidOperationException("Only plan views are supported by this demo application.");
         }

         // Convert the viewport click into a ray through 3d model space.
         // Note: The output XYZ needs to be projected onto the view's cut plane before use.
         var clickAsModelRay = CalculateClickAsModelRay(clickedViewport, click);
         if (null == clickAsModelRay)
         {
            throw new InvalidOperationException("The click was outside the view crop regions.");
         }

         // Project the ray onto the view's cut plane.  
         // This picks a reasonable height in the model for the View3D camera.
         var cutPlane = GetViewPlanCutPlane(plan);
         if (null == cutPlane)
         {
            throw new InvalidOperationException("An error occured when getting the view's cut plane.");
         }
         var view3dCameraLocation = ProjectPointOnPlane(cutPlane, clickAsModelRay);
         if (null == view3dCameraLocation)
         {
            throw new InvalidOperationException("An error occured when calculating the View3D camera position.");
         }

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
      /// Find the viewport at a point on a sheet.
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
            var viewport = doc.GetElement(vpId) as Viewport;

            if (null != viewport && viewport.GetBoxOutline().Contains(click, CLICK_TOLERANCE))
            {
               // Click is within the viewport
               return viewport;
            }
         }

         // Click was not contained by any viewport
         return null;
      }

      /// <summary>
      /// Makes the sheet space --> 3D model space transform.
      /// </summary>
      /// <param name="trfModelToProjection">The 3D model space --> view projection space transform.</param>
      /// <param name="trfProjectionToSheet">The view projection space --> sheet space transform.</param>
      /// <returns>The sheet space --> 3D model space transform.</returns>
      private static Transform MakeSheetToModelTransform(Transform trfModelToProjection, Transform trfProjectionToSheet)
      {
         if (null == trfModelToProjection || null == trfProjectionToSheet)
            return null;

         var modelToSheetTrf = trfProjectionToSheet.Multiply(trfModelToProjection);
         return modelToSheetTrf.Inverse;
      }

      /// <summary>
      /// Projects a point on a plane.
      /// </summary>
      /// <param name="plane">The plane on which the point will be projected</param>
      /// <param name="point">The point to projected onto the plane.</param>
      /// <returns>The point projected onto the plane.</returns>
      private static XYZ ProjectPointOnPlane(Plane plane, XYZ point)
      {
         var uv = new UV();
         double ignored;
         plane.Project(point, out uv, out ignored);
         return plane.Origin + (plane.XVec * uv.U) + (plane.YVec * uv.V);
      }

      private static double RANDOM_X_SCALE = 5631;
      private static double RANDOM_Y_SCALE = 4369;

      /// <summary>
      /// Tests if a point is within a curveloop.  The point wil be projected onto the 
      /// plane which holds the curveloop before the test is executed.
      /// </summary>
      /// <param name="point">The point to test.</param>
      /// <param name="curveloop">A curveloop that lies in a plane.</param>
      /// <returns>True, if the point was inside the curveloop after the 
      /// point was projected onto the curveloop's plane. False, otherwise.</returns>
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
            projectedPoint + (RANDOM_X_SCALE * plane.XVec) + (RANDOM_Y_SCALE * plane.YVec));

         // Count how many edges of curveloop intersect veryLongLine.
         var intersectionCount = 0;
         foreach (var edge in curveloop)
         {
            IntersectionResultArray resultArray;
            var res = veryLongLine.Intersect(edge, out resultArray);
            if (SetComparisonResult.Overlap == res)
            {
               intersectionCount += resultArray.Size;
            }
         }

         // If the intersection count is ODD, then the point is inside the curveloop.
         return 1 == (intersectionCount % 2);
      }

      /// <summary>
      /// Starting with a click on a viewport on a sheet, this method calculates the corresponding
      /// ray through 3D model.
      /// </summary>
      /// <param name="viewport">The viewport on a sheet that was clicked.</param>
      /// <param name="click">The clicked point.</param>
      /// <returns>An XYZ that represents a ray through the model in the direction of the viewport view's
      /// view direction vector.</returns>
      private static XYZ CalculateClickAsModelRay(Viewport viewport, XYZ click)
      {
         if (null == viewport || null == click)
            return null;

         var doc = viewport.Document;
         if (null == doc)
            return null;

         var view = doc.GetElement(viewport.ViewId) as View;
         if (null == view)
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
            var trfSheetToModel = MakeSheetToModelTransform(trfWithBoundary.GetModelToProjectionTransform(), trfProjectionToSheet);
            if (null == trfSheetToModel)
            {
               throw new InvalidOperationException("An error occured when calculating the sheet-to-model transforms.");
            }

            // Transform the click point into 3D model space.
            var clickAsModelRay = trfSheetToModel.OfPoint(click);

            // Get the edges of the current crop region.
            var modelCurveLoop = trfWithBoundary.GetBoundary();

            if (null == modelCurveLoop)
            {
               // Views that are uncropped will have just one TransformWithBoundary and its
               // curveloop will be null.  All sheet points will be subject to the transform
               // from this TransformWithBoundary.
               return clickAsModelRay;
            }
            else if (IsPointInsideCurveLoop(clickAsModelRay, modelCurveLoop))
            {
               return clickAsModelRay;
            }
         }

         // The clicked point on the sheet is outside of all of the TransformWithBoundary crop regions.
         // The model ray can't be calculated.
         return null;
      }

      /// <summary>
      /// Gets the cut plane from a plan view.
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
      /// Creates a perspective View3D.
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
         if (null == view3d)
         {
            return null;
         }

         view3d.SetOrientation(new ViewOrientation3D(eyePosition, upDir, forwardDir));

         return view3d;
      }
   }
}
