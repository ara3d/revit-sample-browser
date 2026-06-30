#region Namespaces

using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_SpaceAdjacency sample.</summary>
    internal static partial class Util
    {
        private const double SpaceAdjacency_D2mm = 2.0 / 25.4 / 12;
        private const double SpaceAdjacency_MaxWallThickness = 14 / 12;

        public static void GetBoundaries(
            List<SpaceAdjacencySegment> segments,
            Space space)
        {
            var boundaries
                = space.GetBoundarySegments(
                    new SpatialElementBoundaryOptions());

            foreach (var b in boundaries)
            foreach (var s in b)
            {
                var curve = s.GetCurve();
                var a = curve.Tessellate();
                for (var i = 1; i < a.Count; ++i)
                {
                    var segment = new SpaceAdjacencySegment(
                        a[i - 1], a[i], space);

                    segments.Add(segment);
                }
            }
        }

        public static void FindClosestSegments(
            Dictionary<SpaceAdjacencySegment, SpaceAdjacencySegment> segmentPairs,
            List<SpaceAdjacencySegment> segments)
        {
            foreach (var segOuter in segments)
            {
                var first = true;
                double dist = 0;
                SpaceAdjacencySegment closest = null;

                foreach (var segInner in segments)
                {
                    if (segOuter == segInner)
                        continue;

                    if (segInner.Space == segOuter.Space)
                        continue;

                    var d = segOuter.Distance(
                        segInner);

                    if (first || d < dist)
                    {
                        dist = d;
                        first = false;
                        closest = segInner;
                    }
                }

                segmentPairs.Add(segOuter, closest);
            }
        }

        public static void DetermineAdjacencies(
            Dictionary<Space, List<Space>> a,
            Dictionary<SpaceAdjacencySegment, SpaceAdjacencySegment> segmentPairs)
        {
            foreach (var s in segmentPairs.Keys)
            {
                var t = segmentPairs[s];
                var d = s.Distance(t);
                if (d < SpaceAdjacency_MaxWallThickness)
                {
                    var direction = s.DirectionTo(t);
                    var startPt = t.MidPoint;
                    var testPoint = startPt + direction * SpaceAdjacency_D2mm;
                    if (t.Space.IsPointInSpace(testPoint))
                    {
                        if (!a.ContainsKey(s.Space)) a.Add(s.Space, new List<Space>());
                        if (!a[s.Space].Contains(t.Space)) a[s.Space].Add(t.Space);
                    }
                }
            }
        }

        public static void PrintSpaceInfo(
            string indent,
            Space space)
        {
            Debug.Print("{0}{1} {2}", indent,
                space.Name, space.Number);
        }

        public static void ReportAdjacencies(
            Dictionary<Space, List<Space>> spaceAdjacencies)
        {
            Debug.WriteLine("\nReport Space Adjacencies:");
            foreach (var space in spaceAdjacencies.Keys)
            {
                PrintSpaceInfo("", space);
                foreach (var adj in spaceAdjacencies[space]) PrintSpaceInfo("  ", adj);
            }
        }
    }

    internal class SpaceAdjacencySegment
    {
        public SpaceAdjacencySegment(XYZ sp, XYZ ep, Space space)
        {
            StartPoint = sp;
            EndPoint = ep;
            Space = space;
        }

        public XYZ StartPoint { get; }

        public XYZ EndPoint { get; }

        public Space Space { get; }

        public double Slope
        {
            get
            {
                var deltaX = StartPoint.X - EndPoint.X;
                var deltaY = StartPoint.Y - EndPoint.Y;
                if (deltaX != 0) return deltaY / deltaX;
                return 0;
            }
        }

        public bool IsHorizontal => StartPoint.Y == EndPoint.Y;

        public bool IsVertical => StartPoint.X == EndPoint.X;

        public XYZ MidPoint => Util.Midpoint(StartPoint, EndPoint);

        public new string ToString()
        {
            return $"{Util.PointString(StartPoint)} {Util.PointString(EndPoint)}";
        }

        public XYZ DirectionTo(SpaceAdjacencySegment a)
        {
            var v = a.MidPoint - MidPoint;
            return v.IsZeroLength() ? v : v.Normalize();
        }

        public double Distance(SpaceAdjacencySegment a)
        {
            return MidPoint.DistanceTo(a.MidPoint);
        }

        public bool Parallel(SpaceAdjacencySegment a)
        {
            return IsVertical && a.IsVertical
                   || IsHorizontal && a.IsHorizontal
                   || Util.IsEqual(Slope, a.Slope);
        }
    }
}
