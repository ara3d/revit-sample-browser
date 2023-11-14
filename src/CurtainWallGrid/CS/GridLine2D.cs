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

using System.Collections.Generic;
using System.Drawing;

namespace Revit.SDK.Samples.CurtainWallGrid.CS
{
   /// <summary>
   /// a linear line in 2D point format
   /// </summary>
   public class Line2D
   {
      #region Fields
      // the start point of the line

      // the end point of the line

      #endregion

      #region Properties
      /// <summary>
      /// the start point of the line
      /// </summary>
      public Point StartPoint { get; set; }

      /// <summary>
      /// the end point of the line
      /// </summary>
      public Point EndPoint { get; set; }

      #endregion

      #region Constructors
      /// <summary>
      /// default constructor
      /// </summary>
      public Line2D()
      {
         StartPoint = Point.Empty;
         EndPoint = Point.Empty;
      }

      /// <summary>
      /// constructor
      /// </summary>
      /// <param name="startPoint">
      /// the start point for the line
      /// </param>
      /// <param name="endPoint">
      /// the end point for the line
      /// </param>
      public Line2D(Point startPoint, Point endPoint)
      {
         StartPoint = startPoint;
         EndPoint = endPoint;
      }

      /// <summary>
      /// copy constructor
      /// </summary>
      /// <param name="line2D">
      /// the line to be copied
      /// </param>
      public Line2D(Line2D line2D)
      {
         StartPoint = line2D.StartPoint;
         EndPoint = line2D.EndPoint;
      }
      #endregion
   }

   /// <summary>
   /// the class stores the baseline data for curtain wall
   /// </summary>
   public class WallBaseline2D : Line2D
   {
      #region Fields
      // an assistant point for temp usage

      #endregion

      #region Properties
      /// <summary>
      /// an assistant point for temp usage
      /// </summary>
      public Point AssistantPoint { get; set; }

      #endregion

      #region Constructors
      /// <summary>
      /// default constructor
      /// </summary>
      public WallBaseline2D()
         : base()
      {
         AssistantPoint = Point.Empty;
      }

      /// <summary>
      /// constructor
      /// </summary>
      /// <param name="startPoint">
      /// the start point for the line
      /// </param>
      /// <param name="endPoint">
      /// the end point for the line
      /// </param>
      public WallBaseline2D(Point startPoint, Point endPoint)
         : base(startPoint, endPoint)
      {
         AssistantPoint = Point.Empty;
      }

      /// <summary>
      /// copy constructor
      /// </summary>
      /// <param name="wallLine2D">
      /// the line to be copied
      /// </param>
      public WallBaseline2D(WallBaseline2D wallLine2D)
         : base((Line2D)wallLine2D)
      {
         AssistantPoint = wallLine2D.AssistantPoint;
      }
      #endregion

      #region Public methods
      /// <summary>
      /// clear the stored data
      /// </summary>
      public void Clear()
      {
         StartPoint = Point.Empty;
         EndPoint = Point.Empty;
         AssistantPoint = Point.Empty;
      }
      #endregion
   }

   /// <summary>
   /// the 2D format for the curtain grid line, it inherits from the Line2D class
   /// </summary>
   public class GridLine2D : Line2D
   {
      #region Fields
      // indicate whether the grid line is locked

      // all the segments for the grid line

      // indicate how many segments have been removed from the grid line

      // indicate whether it's a U grid line

      #endregion

      #region Properties
      /// <summary>
      /// indicate whether the grid line is locked
      /// </summary>
      public bool Locked { get; set; }

      /// <summary>
      /// all the segments for the grid line
      /// </summary>
      public List<SegmentLine2D> Segments { get; }

      /// <summary>
      /// indicate how many segments have been removed from the grid line
      /// </summary>
      public int RemovedNumber { get; set; }

      /// <summary>
      /// indicate whether it's a U grid line
      /// </summary>
      public bool IsUGridLine { get; set; }

      #endregion

      #region Constructors
      /// <summary>
      /// default constructor, initialize all the members with default value
      /// </summary>
      public GridLine2D()
         : base()
      {
         Segments = new List<SegmentLine2D>();
         Locked = false;
         RemovedNumber = 0;
         IsUGridLine = false;
      }

      /// <summary>
      /// constructor, initialize the grid line with end points
      /// </summary>
      /// <param name="startPoint">
      /// the start point of the curtain grid line 2D
      /// </param>
      /// <param name="endPoint">
      /// the end point of the curtain grid line 2D
      /// </param>
      public GridLine2D(Point startPoint, Point endPoint)
         : base(startPoint, endPoint)
      {
         Segments = new List<SegmentLine2D>();
         Locked = false;
         RemovedNumber = 0;
         IsUGridLine = false;
      }

      /// <summary>
      /// copy constructor, initialize the curtain grid line with another grid line 2D
      /// </summary>
      /// <param name="gridLine2D">
      /// the source line 
      /// </param>
      public GridLine2D(GridLine2D gridLine2D)
         : base((Line2D)gridLine2D)
      {
         Segments = new List<SegmentLine2D>();
         Locked = gridLine2D.Locked;
         RemovedNumber = gridLine2D.RemovedNumber;
         IsUGridLine = gridLine2D.IsUGridLine;
         foreach (var segLine in gridLine2D.Segments)
         {
            Segments.Add(new SegmentLine2D(segLine));
         }
      }
      #endregion
   }

   /// <summary>
   /// the line class for the segment of grid line, it inherits from Line2D class
   /// </summary>
   public class SegmentLine2D : Line2D
   {
      #region Fields
      // indicates whether the segment is "isolated" 

      // indicate whether the segment has been removed from the grid line

      // the index of the segment in the grid line

      // the index of its parent grid line in all the curtain grid's U/V grid lines

      // indicates whether the segment is in a U grid line

      #endregion

      #region Properties
      /// <summary>
      /// indicates whether the segment is "isolated" 
      /// </summary>
      public bool Isolated { get; set; }

      #endregion

      #region
      /// <summary>
      /// indicate whether the segment has been removed from the grid line
      /// </summary>
      public bool Removed { get; set; }

      /// <summary>
      /// the index of the segment in the grid line
      /// </summary>
      public int SegmentIndex { get; set; }

      /// <summary>
      /// the index of its parent grid line in all the curtain grid's U/V grid lines
      /// </summary>
      public int GridLineIndex { get; set; }

      /// <summary>
      /// indicates whether the segment is in a U grid line
      /// </summary>
      public bool IsUSegment { get; set; }

      #endregion

      #region Constructors
      /// <summary>
      /// default constructor
      /// </summary>
      public SegmentLine2D()
         : base()
      {
         Removed = false;
         Isolated = false;
         SegmentIndex = -1;
         GridLineIndex = -1;
      }

      /// <summary>
      /// constructor, initialize the segment with end points
      /// </summary>
      /// <param name="startPoint">
      /// the start point of the segment
      /// </param>
      /// <param name="endPoint">
      /// the end point of the segment
      /// </param>
      public SegmentLine2D(Point startPoint, Point endPoint)
         : base(startPoint, endPoint)
      {
         Removed = false;
         Isolated = false;
         SegmentIndex = -1;
         GridLineIndex = -1;
      }

      /// <summary>
      /// copy constructor
      /// </summary>
      /// <param name="segLine2D">
      /// the source segment line 2D
      /// </param>
      public SegmentLine2D(SegmentLine2D segLine2D)
         : base((Line2D)segLine2D)
      {
         Removed = segLine2D.Removed;
         Isolated = segLine2D.Isolated;
         SegmentIndex = segLine2D.SegmentIndex;
         GridLineIndex = segLine2D.GridLineIndex;
      }
      #endregion
   }

}
