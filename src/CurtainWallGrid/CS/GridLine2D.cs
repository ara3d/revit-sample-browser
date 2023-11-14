// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;

namespace RevitMultiSample.CurtainWallGrid.CS
{
    /// <summary>
    ///     a linear line in 2D point format
    /// </summary>
    public class Line2D
    {
        /// <summary>
        ///     default constructor
        /// </summary>
        public Line2D()
        {
            StartPoint = Point.Empty;
            EndPoint = Point.Empty;
        }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="startPoint">
        ///     the start point for the line
        /// </param>
        /// <param name="endPoint">
        ///     the end point for the line
        /// </param>
        public Line2D(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        /// <summary>
        ///     copy constructor
        /// </summary>
        /// <param name="line2D">
        ///     the line to be copied
        /// </param>
        public Line2D(Line2D line2D)
        {
            StartPoint = line2D.StartPoint;
            EndPoint = line2D.EndPoint;
        }
        // the start point of the line

        // the end point of the line

        /// <summary>
        ///     the start point of the line
        /// </summary>
        public Point StartPoint { get; set; }

        /// <summary>
        ///     the end point of the line
        /// </summary>
        public Point EndPoint { get; set; }
    }

    /// <summary>
    ///     the class stores the baseline data for curtain wall
    /// </summary>
    public class WallBaseline2D : Line2D
    {
        /// <summary>
        ///     default constructor
        /// </summary>
        public WallBaseline2D()
        {
            AssistantPoint = Point.Empty;
        }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="startPoint">
        ///     the start point for the line
        /// </param>
        /// <param name="endPoint">
        ///     the end point for the line
        /// </param>
        public WallBaseline2D(Point startPoint, Point endPoint)
            : base(startPoint, endPoint)
        {
            AssistantPoint = Point.Empty;
        }

        /// <summary>
        ///     copy constructor
        /// </summary>
        /// <param name="wallLine2D">
        ///     the line to be copied
        /// </param>
        public WallBaseline2D(WallBaseline2D wallLine2D)
            : base(wallLine2D)
        {
            AssistantPoint = wallLine2D.AssistantPoint;
        }
        // an assistant point for temp usage

        /// <summary>
        ///     an assistant point for temp usage
        /// </summary>
        public Point AssistantPoint { get; set; }

        /// <summary>
        ///     clear the stored data
        /// </summary>
        public void Clear()
        {
            StartPoint = Point.Empty;
            EndPoint = Point.Empty;
            AssistantPoint = Point.Empty;
        }
    }

    /// <summary>
    ///     the 2D format for the curtain grid line, it inherits from the Line2D class
    /// </summary>
    public class GridLine2D : Line2D
    {
        /// <summary>
        ///     default constructor, initialize all the members with default value
        /// </summary>
        public GridLine2D()
        {
            Segments = new List<SegmentLine2D>();
            Locked = false;
            RemovedNumber = 0;
            IsUGridLine = false;
        }

        /// <summary>
        ///     constructor, initialize the grid line with end points
        /// </summary>
        /// <param name="startPoint">
        ///     the start point of the curtain grid line 2D
        /// </param>
        /// <param name="endPoint">
        ///     the end point of the curtain grid line 2D
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
        ///     copy constructor, initialize the curtain grid line with another grid line 2D
        /// </summary>
        /// <param name="gridLine2D">
        ///     the source line
        /// </param>
        public GridLine2D(GridLine2D gridLine2D)
            : base(gridLine2D)
        {
            Segments = new List<SegmentLine2D>();
            Locked = gridLine2D.Locked;
            RemovedNumber = gridLine2D.RemovedNumber;
            IsUGridLine = gridLine2D.IsUGridLine;
            foreach (var segLine in gridLine2D.Segments) Segments.Add(new SegmentLine2D(segLine));
        }
        // indicate whether the grid line is locked

        // all the segments for the grid line

        // indicate how many segments have been removed from the grid line

        // indicate whether it's a U grid line

        /// <summary>
        ///     indicate whether the grid line is locked
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        ///     all the segments for the grid line
        /// </summary>
        public List<SegmentLine2D> Segments { get; }

        /// <summary>
        ///     indicate how many segments have been removed from the grid line
        /// </summary>
        public int RemovedNumber { get; set; }

        /// <summary>
        ///     indicate whether it's a U grid line
        /// </summary>
        public bool IsUGridLine { get; set; }
    }

    /// <summary>
    ///     the line class for the segment of grid line, it inherits from Line2D class
    /// </summary>
    public class SegmentLine2D : Line2D
    {
        /// <summary>
        ///     default constructor
        /// </summary>
        public SegmentLine2D()
        {
            Removed = false;
            Isolated = false;
            SegmentIndex = -1;
            GridLineIndex = -1;
        }

        /// <summary>
        ///     constructor, initialize the segment with end points
        /// </summary>
        /// <param name="startPoint">
        ///     the start point of the segment
        /// </param>
        /// <param name="endPoint">
        ///     the end point of the segment
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
        ///     copy constructor
        /// </summary>
        /// <param name="segLine2D">
        ///     the source segment line 2D
        /// </param>
        public SegmentLine2D(SegmentLine2D segLine2D)
            : base(segLine2D)
        {
            Removed = segLine2D.Removed;
            Isolated = segLine2D.Isolated;
            SegmentIndex = segLine2D.SegmentIndex;
            GridLineIndex = segLine2D.GridLineIndex;
        }
        // indicates whether the segment is "isolated" 

        // indicate whether the segment has been removed from the grid line

        // the index of the segment in the grid line

        // the index of its parent grid line in all the curtain grid's U/V grid lines

        // indicates whether the segment is in a U grid line

        /// <summary>
        ///     indicates whether the segment is "isolated"
        /// </summary>
        public bool Isolated { get; set; }

        /// <summary>
        ///     indicate whether the segment has been removed from the grid line
        /// </summary>
        public bool Removed { get; set; }

        /// <summary>
        ///     the index of the segment in the grid line
        /// </summary>
        public int SegmentIndex { get; set; }

        /// <summary>
        ///     the index of its parent grid line in all the curtain grid's U/V grid lines
        /// </summary>
        public int GridLineIndex { get; set; }

        /// <summary>
        ///     indicates whether the segment is in a U grid line
        /// </summary>
        public bool IsUSegment { get; set; }
    }
}
