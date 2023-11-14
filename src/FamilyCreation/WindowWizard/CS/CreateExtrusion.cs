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
using Autodesk.Revit.DB;
using Autodesk.Revit.ApplicationServices;

namespace Revit.SDK.Samples.WindowWizard.CS
{
   /// <summary>
   /// The class is used to create solid extrusion 
   /// </summary>
   class CreateExtrusion
   {
      #region Class Memeber Variables
      /// <summary>
      /// store the document
      /// </summary>
      Document m_document;

      /// <summary>
      /// store the application of creation
      /// </summary>
      Autodesk.Revit.Creation.Application m_appCreator;

      /// <summary>
      /// store the FamilyItemFactory of creation
      /// </summary>
      Autodesk.Revit.Creation.FamilyItemFactory m_familyCreator;
      #endregion

      /// <summary>
      /// The constructor of CreateExtrusion
      /// </summary>
      /// <param name="app">the application</param>
      /// <param name="doc">the document</param>
      public CreateExtrusion(Application app, Document doc)
      {
         m_document = doc;
         m_appCreator = app.Create;
         m_familyCreator = doc.FamilyCreate;
      }

      #region Class Implementation
      /// <summary>
      /// The method is used to create a CurveArray with four double parameters and one y coordinate value
      /// </summary>
      /// <param name="left">the left value</param>
      /// <param name="right">the right value</param>
      /// <param name="top">the top value</param>
      /// <param name="bottom">the bottom value</param>
      /// <param name="y_coordinate">the y_coordinate value</param>
      /// <returns>CurveArray</returns>
      public CurveArray CreateRectangle(double left, double right, double top, double bottom, double y_coordinate)
      {
         var curveArray = m_appCreator.NewCurveArray();
         try
         {
            var p0 = new XYZ(left, y_coordinate, top);
            var p1 = new XYZ(right, y_coordinate, top);
            var p2 = new XYZ(right, y_coordinate, bottom);
            var p3 = new XYZ(left, y_coordinate, bottom);
            var line1 = Line.CreateBound(p0, p1);
            var line2 = Line.CreateBound(p1, p2);
            var line3 = Line.CreateBound(p2, p3);
            var line4 = Line.CreateBound(p3, p0);
            curveArray.Append(line1);
            curveArray.Append(line2);
            curveArray.Append(line3);
            curveArray.Append(line4);
            return curveArray;
         }
         catch (Exception e)
         {
            System.Diagnostics.Debug.WriteLine(e.Message);
            return null;
         }
      }

      /// <summary>
      /// The method is used to create a CurveArray along to an origin CurveArray and an offset value
      /// </summary>
      /// <param name="origin">the original CurveArray</param>
      /// <param name="offset">the offset value</param>
      /// <returns>CurveArray</returns>
      public CurveArray CreateCurveArrayByOffset(CurveArray origin, double offset)
      {
          var counter = 0;
         var curveArr = m_appCreator.NewCurveArray();
         var offsetx = new XYZ(offset, 0, 0);
         var offsetz = new XYZ(0, 0, offset);
         var p0 = new XYZ();
         var p1 = new XYZ(); ;
         var p2 = new XYZ();
         var p3 = new XYZ();
         foreach (Curve curve in origin)
         {
            var temp = curve as Line;
            if (temp != null)
            {
               if (counter == 0)
               {
                  p0 = temp.GetEndPoint(0).Subtract(offsetz).Subtract(offsetx);
               }
               else if (counter == 1)
               {
                  p1 = temp.GetEndPoint(0).Subtract(offsetz).Add(offsetx);
               }
               else if (counter == 2)
               {
                  p2 = temp.GetEndPoint(0).Add(offsetx).Add(offsetz);
               }
               else
               {
                  p3 = temp.GetEndPoint(0).Subtract(offsetx).Add(offsetz);
               }
            }
            counter++;
         }
         var line = Line.CreateBound(p0, p1);
         curveArr.Append(line);
         line = Line.CreateBound(p1, p2);
         curveArr.Append(line);
         line = Line.CreateBound(p2, p3);
         curveArr.Append(line);
         line = Line.CreateBound(p3, p0);
         curveArr.Append(line);
         return curveArr;
      }

      /// <summary>
      /// The method is used to create extrusion using FamilyItemFactory.NewExtrusion()
      /// </summary>
      /// <param name="curveArrArray">the CurveArrArray parameter</param>
      /// <param name="workPlane">the reference plane is used to create SketchPlane</param>
      /// <param name="startOffset">the extrusion's StartOffset property</param>
      /// <param name="endOffset">the extrusion's EndOffset property</param>
      /// <returns>the new extrusion</returns>
      public Extrusion NewExtrusion(CurveArrArray curveArrArray, Autodesk.Revit.DB.ReferencePlane workPlane, double startOffset, double endOffset)
      {
          try
         {
            var subTransaction = new SubTransaction(m_document);
            subTransaction.Start();
            var sketch = SketchPlane.Create(m_document, workPlane.GetPlane());
            var rectExtrusion = m_familyCreator.NewExtrusion(true, curveArrArray, sketch, Math.Abs(endOffset - startOffset));
            rectExtrusion.StartOffset = startOffset;
            rectExtrusion.EndOffset = endOffset;
            subTransaction.Commit();
            return rectExtrusion;
         }
         catch
         {
            return null;
         }
      }
      #endregion
   }
}
