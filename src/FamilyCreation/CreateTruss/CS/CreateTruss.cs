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
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using View = Autodesk.Revit.DB.View;

namespace Revit.SDK.Samples.CreateTruss.CS
{
   /// <summary>
   /// A class inherits IExternalCommand interface.
   /// this class creates a mono truss in truss family document.
   /// </summary>
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
   [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
   public class Command : IExternalCommand
   {
      /// <summary>
      /// The Revit application
      /// </summary>
      private static Autodesk.Revit.ApplicationServices.Application m_application;

      /// <summary>
      /// The current document of the application
      /// </summary>
      private static Document m_document;

      /// <summary>
      /// The Application Creation object is used to create new instances of utility objects.
      /// </summary>
      private Autodesk.Revit.Creation.Application m_appCreator;

      /// <summary>
      /// the creation factory to create model lines, dimensions and alignments
      /// </summary>
      private Autodesk.Revit.Creation.FamilyItemFactory m_familyCreator;


      /// <summary>
      /// Implement this method as an external command for Revit.
      /// </summary>
      /// <param name="revit">An object that is passed to the external application 
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
      public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
      {
         try
         {
            m_application = revit.Application.Application;
            m_document = revit.Application.ActiveUIDocument.Document;

            // it can support in truss family document only
            if (!m_document.IsFamilyDocument
                || m_document.OwnerFamily.FamilyCategory.BuiltInCategory != BuiltInCategory.OST_Truss)
            {
               message = "Cannot execute truss creation in non-truss family document";
               return Result.Failed;
            }

            m_appCreator = m_application.Create;
            m_familyCreator = m_document.FamilyCreate;

            var newTran = new Transaction(m_document);
            newTran.Start("NewTrussCurve");

            // Start the truss creation
            MakeNewTruss();

            newTran.Commit();
         }
         catch (Exception ex)
         {
            message = ex.ToString();
            return Result.Failed;
         }
         return Result.Succeeded;
      }

      /// <summary>
      /// Example demonstrating truss creation in the Autodesk Revit API. This example constructs
      /// a "mono" truss aligned with the reference planes in the (already loaded) truss family
      /// document.
      /// </summary>
      private void MakeNewTruss()
      {
         // Constants for arranging the angular truss members
         var webAngle = 35.0;
         var webAngleRadians = (180 - webAngle) * Math.PI / 180.0;
         var angleDirection = new XYZ(Math.Cos(webAngleRadians), Math.Sin(webAngleRadians), 0);

         // Look up the reference planes and view in which to sketch 
         Autodesk.Revit.DB.ReferencePlane top = null, bottom = null, left = null, right = null, center = null;
         View level1 = null;
         var elements = new List<Element>();
         var refPlaneFilter = new ElementClassFilter(typeof(Autodesk.Revit.DB.ReferencePlane));
         var viewFilter = new ElementClassFilter(typeof(View));
         var filter = new LogicalOrFilter(refPlaneFilter, viewFilter);
         var collector = new FilteredElementCollector(m_document);
         elements.AddRange(collector.WherePasses(filter).ToElements());
         foreach (var e in elements)
         {
            // skip view templates because they're invisible invalid for truss creation
            var view = e as View;
            if (null != view && view.IsTemplate)
               continue;
            //
            switch (e.Name)
            {
               case "Top": top = e as Autodesk.Revit.DB.ReferencePlane; break;
               case "Bottom": bottom = e as Autodesk.Revit.DB.ReferencePlane; break;
               case "Right": right = e as Autodesk.Revit.DB.ReferencePlane; break;
               case "Left": left = e as Autodesk.Revit.DB.ReferencePlane; break;
               case "Center": center = e as Autodesk.Revit.DB.ReferencePlane; break;
               case "Level 1": level1 = e as View; break;
            }
         }
         if (top == null || bottom == null || left == null
             || right == null || center == null || level1 == null)
            throw new InvalidOperationException("Could not find prerequisite named reference plane or named view.");

         var sPlane = level1.SketchPlane;

         // Extract the geometry of each reference plane
         var bottomLine = GetReferencePlaneLine(bottom);
         var leftLine = GetReferencePlaneLine(left);
         var rightLine = GetReferencePlaneLine(right);
         var topLine = GetReferencePlaneLine(top);
         var centerLine = GetReferencePlaneLine(center);

         // Create bottom chord along "bottom" from "left" to "right"
         var bottomLeft = GetIntersection(bottomLine, leftLine);
         var bottomRight = GetIntersection(bottomLine, rightLine);
         var bottomChord = MakeTrussCurve(bottomLeft, bottomRight, sPlane, TrussCurveType.BottomChord);
         if (null != bottomChord)
         {
            // Add the alignment constraint to the bottom chord.
            var geometryCurve = bottomChord.GeometryCurve;
            // Lock the bottom chord to bottom reference plan
            m_familyCreator.NewAlignment(level1, bottom.GetReference(), geometryCurve.Reference);
         }

         // Create web connecting top and bottom chords on the right side
         var topRight = GetIntersection(topLine, rightLine);
         var rightWeb = MakeTrussCurve(bottomRight, topRight, sPlane, TrussCurveType.Web);
         if (null != rightWeb)
         {
            // Add the alignment constraint to the right web chord.
            var geometryCurve = rightWeb.GeometryCurve;
            // Lock the right web chord to right reference plan
            m_familyCreator.NewAlignment(level1, right.GetReference(), geometryCurve.Reference);
         }

         // Create top chord diagonally from bottom-left to top-right
         var topChord = MakeTrussCurve(bottomLeft, topRight, sPlane, TrussCurveType.TopChord);
         if (null != topChord)
         {
            // Add the alignment constraint to the top chord.
            var geometryCurve = topChord.GeometryCurve;
            // Lock the start point of top chord to the Intersection of left and bottom reference plan
            m_familyCreator.NewAlignment(level1, geometryCurve.GetEndPointReference(0), left.GetReference());
            m_familyCreator.NewAlignment(level1, geometryCurve.GetEndPointReference(0), bottom.GetReference());
            // Lock the end point of top chord to the Intersection of right and top reference plan
            m_familyCreator.NewAlignment(level1, geometryCurve.GetEndPointReference(1), top.GetReference());
            m_familyCreator.NewAlignment(level1, geometryCurve.GetEndPointReference(1), right.GetReference());
         }

         // Create angled web from midpoint to the narrow end of the truss
         var bottomMidPoint = GetIntersection(bottomLine, centerLine);
         var webDirection = Line.CreateUnbound(bottomMidPoint, angleDirection);
         var endOfWeb = GetIntersection(topChord.GeometryCurve as Line, webDirection);
         var angledWeb = MakeTrussCurve(bottomMidPoint, endOfWeb, sPlane, TrussCurveType.Web);

         // Add a dimension to force the angle to be stable even when truss length and height are modified
         var dimensionArc = Arc.Create(
             bottomMidPoint, angledWeb.GeometryCurve.Length / 2, webAngleRadians, Math.PI, XYZ.BasisX, XYZ.BasisY);
         var createdDim = m_familyCreator.NewAngularDimension(
             level1, dimensionArc, angledWeb.GeometryCurve.Reference, bottomChord.GeometryCurve.Reference);
         if (null != createdDim)
            createdDim.IsLocked = true;

         // Create angled web from corner to top of truss
         var bottomRight2 = GetIntersection(bottomLine, rightLine);
         webDirection = Line.CreateUnbound(bottomRight2, angleDirection);
         endOfWeb = GetIntersection(topChord.GeometryCurve as Line, webDirection);
         var angledWeb2 = MakeTrussCurve(bottomRight, endOfWeb, sPlane, TrussCurveType.Web);

         // Add a dimension to force the angle to be stable even when truss length and height are modified
         dimensionArc = Arc.Create(
             bottomRight, angledWeb2.GeometryCurve.Length / 2, webAngleRadians, Math.PI, XYZ.BasisX, XYZ.BasisY);
         createdDim = m_familyCreator.NewAngularDimension(
             level1, dimensionArc, angledWeb2.GeometryCurve.Reference, bottomChord.GeometryCurve.Reference);
         if (null != createdDim)
            createdDim.IsLocked = true;

         //Connect bottom midpoint to end of the angled web
         var braceWeb = MakeTrussCurve(bottomMidPoint, endOfWeb, sPlane, TrussCurveType.Web);
      }

      /// <summary>
      /// Utility method to create a truss model curve.
      /// </summary>
      /// <param name="start">The start point.</param>
      /// <param name="end">The end point.</param>
      /// <param name="sketchPlane">The sketch plane for the new curve.</param>
      /// <param name="type">The type of truss curve.</param>
      /// <returns>the created truss model curve.</returns>
      private ModelCurve MakeTrussCurve(XYZ start, XYZ end, SketchPlane sketchPlane, TrussCurveType type)
      {
         var line = Line.CreateBound(start, end);
         var trussCurve = m_familyCreator.NewModelCurve(line, sketchPlane);
         trussCurve.TrussCurveType = type;
         m_document.Regenerate();

         return trussCurve;
      }

      /// <summary>
      /// Utility method for to extract the geometry of a reference plane in a family.
      /// </summary>
      /// <param name="plane">The reference plane.</param>
      /// <returns>An unbounded line representing the location of the plane.</returns>
      private Line GetReferencePlaneLine(Autodesk.Revit.DB.ReferencePlane plane)
      {
         // Reset the "elevation" of the plane's line to Z=0, since that's where the lines will be placed.  
         // Otherwise, some intersection calculation may fail
         var origin = new XYZ(
             plane.BubbleEnd.X,
             plane.BubbleEnd.Y,
             0.0);

         var line = Line.CreateUnbound(origin, plane.Direction);

         return line;
      }

      /// <summary>
      /// Utility method for getting the intersection between two lines.
      /// </summary>
      /// <param name="line1">The first line.</param>
      /// <param name="line2">The second line.</param>
      /// <returns>The intersection point.</returns>
      /// <exception cref="InvalidOperationException">Thrown when an intersection can't be found.</exception>
      private XYZ GetIntersection(Line line1, Line line2)
      {
         IntersectionResultArray results;
         var result = line1.Intersect(line2, out results);

         if (result != SetComparisonResult.Overlap)
            throw new InvalidOperationException("Input lines did not intersect.");

         if (results == null || results.Size != 1)
            throw new InvalidOperationException("Could not extract intersection point for lines.");

         var iResult = results.get_Item(0);
         var intersectionPoint = iResult.XYZPoint;

         return intersectionPoint;
      }
   }
}
