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

namespace Revit.SDK.Samples.Openings.CS
{
   /// <summary>
   /// This class which inherit from Autodesk.Revit.DB.BoundingBoxXYZ
   /// store the information about Max (Min) coordinate of object
   /// can get all the corner point coordinate and create X model line
   /// </summary>
   public class BoundingBox : BoundingBoxXYZ
   {
      /// <summary>
      /// define whether we have created Model Line on this BoundingBox
      /// </summary>
      private bool m_isCreated;

      /// <summary>
      /// store all the corner points in BoundingBox
      /// </summary>
      private readonly List<XYZ> m_points = new List<XYZ>();

      /// <summary>
      /// property to get all the points
      /// </summary>
      public List<XYZ> Points => m_points;

      /// <summary>
      /// property to get width of BoundingBox (short side)
      /// </summary>
      public double Width
      {
         get
         {
            double yDistance = 0;
            double xDistance = 0;
            yDistance = m_points[2].Y - m_points[1].Y;
            xDistance = m_points[5].X - m_points[2].X;
            return xDistance < yDistance ? xDistance : yDistance;
         }
      }

      /// <summary>
      /// property to get Length of BoundingBox (long side)
      /// </summary>
      public double Length
      {
         get
         {
            double yDistance = 0;
            double xDistance = 0;
            yDistance = m_points[2].Y - m_points[1].Y;
            xDistance = m_points[5].X - m_points[2].X;
            return xDistance > yDistance ? xDistance : yDistance;
         }
      }

      /// <summary>
      /// The default constructor
      /// </summary>
      /// <param name="boundBoxXYZ">The reference of the application in revit</param>
      public BoundingBox(BoundingBoxXYZ boundBoxXYZ)
      {
         Min = boundBoxXYZ.Min;
         Max = boundBoxXYZ.Max;

         GetCorners();
      }

      /// <summary>
      /// Create X model line with the BoundBox
      /// Create 12 lines to makeup an cube
      /// </summary>
      /// <param name="app">Application get from RevitAPI</param>
      public void CreateLines(UIApplication app)
      {
         if (m_isCreated)
         {
            return;
         }

         //create 12 lines
         for (var i = 0; i < 7; i++)
         {
            NewModelLine(app, i, i + 1);
         }

         for (var i = 0; i < 5; i = i + 2)
         {
            NewModelLine(app, i, i + 3);
         }

         NewModelLine(app, 1, 6);
         NewModelLine(app, 0, 7);

         m_isCreated = true;
      }

      /// <summary>
      /// get all the Corner points of Cube Box via Min and Max
      /// </summary>
      private void GetCorners()
      {
         m_points.Add(Min);

         var point = new XYZ(
             Min.X,
             Min.Y,
             Max.Z);
         m_points.Add(point);

         var point2 = new XYZ(
             Min.X,
             Max.Y,
             Max.Z);
         m_points.Add(point2);

         var point3 = new XYZ(
             Min.X,
             Max.Y,
             Min.Z);
         m_points.Add(point3);

         var point4 = new XYZ(
             Max.X,
             Max.Y,
             Min.Z);
         m_points.Add(point4);

         m_points.Add(Max);

         var point5 = new XYZ(
             Max.X,
             Min.Y,
             Max.Z);
         m_points.Add(point5);

         var point6 = new XYZ(
             Max.X,
             Min.Y,
             Min.Z);
         m_points.Add(point6);
      }

      /// <summary>
      /// Create a Sketch Plane which pass the defined line
      /// the defined line must be one of BoundingBox Profile
      /// </summary>
      /// <param name="app">Application get from RevitAPI</param>
      /// <param name="aline">a line which sketch plane pass</param>
      private SketchPlane NewSketchPlanePassLine(Line aline, UIApplication app)
      {
         //in a cube only
         XYZ norm;
         if (aline.GetEndPoint(0).X == aline.GetEndPoint(1).X)
         {
            norm = new XYZ(1, 0, 0);
         }
         else if (aline.GetEndPoint(0).Y == aline.GetEndPoint(1).Y)
         {
            norm = new XYZ(0, 1, 0);
         }
         else
         {
            norm = new XYZ(0, 0, 1);
         }

         var point = aline.GetEndPoint(0);
         var plane = Plane.CreateByNormalAndOrigin(norm, point);
         var sketchPlane = SketchPlane.Create(app.ActiveUIDocument.Document, plane);
         return sketchPlane;
      }

      /// <summary>
      /// new ModelLine in BoundingBox
      /// </summary>
      /// <param name="app">Application get from RevitAPI</param>
      /// <param name="pointIndex1">index of point in BoundingBox acme</param>
      /// <param name="pointIndex2">index of another point in BoundingBox acme</param>
      private void NewModelLine(UIApplication app, int pointIndex1, int pointIndex2)
      {
         var startP2 = m_points[pointIndex1];
         var endP2 = m_points[pointIndex2];

         try
         {
            var line = Line.CreateBound(startP2, endP2);
            var sketchPlane = NewSketchPlanePassLine(line, app);
            var line2 = Line.CreateBound(startP2, endP2);
            app.ActiveUIDocument.Document.Create.NewModelCurve(line2, sketchPlane);
         }
         catch (Exception)
         {
            return;
         }
      }
   }
}
