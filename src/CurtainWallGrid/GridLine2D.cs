// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using System.Drawing;

namespace Ara3D.RevitSampleBrowser.CurtainWallGrid.CS
{
    public class Line2D
    {
        public Line2D()
        {
            StartPoint = Point.Empty;
            EndPoint = Point.Empty;
        }

        public Line2D(Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }

        public Line2D(Line2D line2D)
        {
            StartPoint = line2D.StartPoint;
            EndPoint = line2D.EndPoint;
        }
        // the start point of the line

        // the end point of the line

        public Point StartPoint { get; set; }

        public Point EndPoint { get; set; }
    }

    /// <summary>
    ///     the class stores the baseline data for curtain wall
    /// </summary>
    public class WallBaseline2D : Line2D
    {
        public WallBaseline2D()
        {
            AssistantPoint = Point.Empty;
        }

        public WallBaseline2D(Point startPoint, Point endPoint)
            : base(startPoint, endPoint)
        {
            AssistantPoint = Point.Empty;
        }

        public WallBaseline2D(WallBaseline2D wallLine2D)
            : base(wallLine2D)
        {
            AssistantPoint = wallLine2D.AssistantPoint;
        }
        // an assistant point for temp usage

        public Point AssistantPoint { get; set; }

        public void Clear()
        {
            StartPoint = Point.Empty;
            EndPoint = Point.Empty;
            AssistantPoint = Point.Empty;
        }
    }

    public class GridLine2D : Line2D
    {
        public GridLine2D()
        {
            Segments = [];
            Locked = false;
            RemovedNumber = 0;
            IsUGridLine = false;
        }

        public GridLine2D(Point startPoint, Point endPoint)
            : base(startPoint, endPoint)
        {
            Segments = [];
            Locked = false;
            RemovedNumber = 0;
            IsUGridLine = false;
        }

        public GridLine2D(GridLine2D gridLine2D)
            : base(gridLine2D)
        {
            Segments = [];
            Locked = gridLine2D.Locked;
            RemovedNumber = gridLine2D.RemovedNumber;
            IsUGridLine = gridLine2D.IsUGridLine;
            foreach (var segLine in gridLine2D.Segments)
            {
                Segments.Add(new SegmentLine2D(segLine));
            }
        }

        // all the segments for the grid line


        public bool Locked { get; set; }

        public List<SegmentLine2D> Segments { get; }

        public int RemovedNumber { get; set; }

        public bool IsUGridLine { get; set; }
    }

    public class SegmentLine2D : Line2D
    {
        public SegmentLine2D()
        {
            Removed = false;
            Isolated = false;
            SegmentIndex = -1;
            GridLineIndex = -1;
        }

        public SegmentLine2D(Point startPoint, Point endPoint)
            : base(startPoint, endPoint)
        {
            Removed = false;
            Isolated = false;
            SegmentIndex = -1;
            GridLineIndex = -1;
        }

        public SegmentLine2D(SegmentLine2D segLine2D)
            : base(segLine2D)
        {
            Removed = segLine2D.Removed;
            Isolated = segLine2D.Isolated;
            SegmentIndex = segLine2D.SegmentIndex;
            GridLineIndex = segLine2D.GridLineIndex;
        }


        // the index of the segment in the grid line

        // the index of its parent grid line in all the curtain grid's U/V grid lines


        public bool Isolated { get; set; }

        public bool Removed { get; set; }

        public int SegmentIndex { get; set; }

        public int GridLineIndex { get; set; }

        public bool IsUSegment { get; set; }
    }
}
