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
using Autodesk.Revit.UI;

namespace Revit.SDK.Samples.CurtainWallGrid.CS
{
   /// <summary>
   /// the class manages the creation operation for the curtain wall
   /// </summary>
   public class WallGeometry
   {
      #region Fields
      // the document of this sample

      // the refferred drawing class for the curtain wall

      // the selected ViewPlan used for curtain wall creation

      // the selected wall type

      // store the start point of baseline (in PointD format)
      PointD m_startPointD;

      //store the start point of baseline (in Autodesk.Revit.DB.XYZ format)

      // store the end point of baseline (in PointD format)
      PointD m_endPointD;

      //store the end point of baseline (in Autodesk.Revit.DB.XYZ format)

      #endregion

      #region Properties
      /// <summary>
      /// the document of this sample
      /// </summary>
      public MyDocument MyDocument { get; }

      /// <summary>
      /// the refferred drawing class for the curtain wall
      /// </summary>
      public WallDrawing Drawing { get; }

      /// <summary>
      /// the selected ViewPlan used for curtain wall creation
      /// </summary>
      public ViewPlan SelectedView { get; set; }

      /// <summary>
      /// the selected wall type
      /// </summary>
      public WallType SelectedWallType { get; set; }

      /// <summary>
      /// store the start point of baseline (in PointD format)
      /// </summary>
      public PointD StartPointD
      {
         get => m_startPointD;
         set => m_startPointD = value;
      }

      /// <summary>
      /// Get start point of baseline
      /// </summary>
      public XYZ StartXYZ { get; set; }

      /// <summary>
      /// store the end point of baseline (in PointD format)
      /// </summary>
      public PointD EndPointD
      {
         get => m_endPointD;
         set => m_endPointD = value;
      }

      /// <summary>
      /// Get end point of baseline
      /// </summary>
      public XYZ EndXYZ { get; set; }

      #endregion

      #region Constructors
      /// <summary>
      /// default constructor
      /// </summary>
      /// <param name="myDoc">
      /// the document of the sample
      /// </param>
      public WallGeometry(MyDocument myDoc)
      {
         MyDocument = myDoc;
         Drawing = new WallDrawing(this);
      }
      #endregion

      #region Public methods
      /// <summary>
      /// create the curtain wall to the active document of Revit
      /// </summary>
      /// <returns>
      /// the created curtain wall
      /// </returns>
      public Wall CreateCurtainWall()
      {
         if (null == SelectedWallType || null == SelectedView)
         {
            return null;
         }

         //baseline
         //new baseline and transform coordinate on windows UI to Revit UI
         StartXYZ = new XYZ(m_startPointD.X, m_startPointD.Y, 0);
         EndXYZ = new XYZ(m_endPointD.X, m_endPointD.Y, 0);
         Line baseline = null;
         try
         {
            baseline = Line.CreateBound(StartXYZ, EndXYZ);
         }
         catch (ArgumentException)
         {
            TaskDialog.Show("Revit", "The start point and the end point of the line are too close, please re-draw it.");
         }
         var act = new Transaction(MyDocument.Document);
         act.Start(Guid.NewGuid().GetHashCode().ToString());
         var wall = Wall.Create(MyDocument.Document, baseline, SelectedWallType.Id,
             SelectedView.GenLevel.Id, 20, 0, false, false);
         act.Commit();
         var act2 = new Transaction(MyDocument.Document);
         act2.Start(Guid.NewGuid().GetHashCode().ToString());
         MyDocument.UIDocument.ShowElements(wall);
         act2.Commit();
         return wall;
      }
      #endregion
   }// end of class
}
