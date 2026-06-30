#region Namespaces

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    internal static partial class Util
    {
        public static List<List<CurveLoop>> SortCurveLoops(Face face)
        {
            var allLoops = face.GetEdgesAsCurveLoops().Select(loop => new CurveLoopUV(loop, face)).ToList();

            var outerLoops = allLoops.Where(loop => loop.IsCounterclockwise).ToList();
            var innerLoops = allLoops.Where(loop => !outerLoops.Contains(loop)).ToList();

            bool somethingHasChanged;
            do
            {
                somethingHasChanged = false;
                for (var i = 1; i < outerLoops.Count(); i++)
                {
                    var point = outerLoops[i].StartPointUV;
                    var loop = outerLoops[i - 1];
                    if (loop.IsPointInside(point) is CurveLoopUV.PointLocation.Inside)
                    {
                        var tmp = outerLoops[i];
                        outerLoops[i] = outerLoops[i - 1];
                        outerLoops[i - 1] = tmp;

                        somethingHasChanged = true;
                    }
                }
            } while (somethingHasChanged);

            var result = new List<List<CurveLoop>>();
            foreach (var outerLoop in outerLoops)
            {
                var list = new List<CurveLoop> {outerLoop.Loop3d};

                for (var i = innerLoops.Count - 1; i >= 0; i--)
                {
                    var innerLoop = innerLoops[i];
                    if (outerLoops.Count == 1
                        || outerLoop.IsPointInside(innerLoop.StartPointUV) == CurveLoopUV.PointLocation.Inside)
                    {
                        list.Add(innerLoop.Loop3d);
                        innerLoops.RemoveAt(i);
                    }
                }

                result.Add(list);
            }

            return result;
        }
    }

    internal class CurveLoopUV : IEnumerable<Curve>
    {
        public enum PointLocation
        {
            Outside,
            OnTheEdge,
            Inside
        }

        private const double Epsilon = 0.000001;
        private readonly CurveLoop _loop2d;

        public readonly double MinX, MaxX, MinY, MaxY;

        public CurveLoopUV(CurveLoop curveLoop, Face face)
        {
            Loop3d = curveLoop;
            _loop2d = new CurveLoop();

            var points3d = Loop3d.SelectMany(curve => curve.Tessellate().Skip(1));
            var pointsUv = points3d.Select(point3d => face.Project(point3d).UVPoint);
            var points2d = pointsUv.Select(pointUv => new XYZ(pointUv.U, pointUv.V, 0)).ToList();

            MinX = MinY = 1.0e100;
            MaxX = MaxY = -1.0e100;
            var nPoints = points2d.Count;
            for (var i = 0; i < nPoints; i++)
            {
                var p1 = points2d[i];
                var p2 = points2d[(i + 1) % nPoints];
                _loop2d.Append(Line.CreateBound(p1, p2));
                if (p1.X < MinX)
                    MinX = p1.X;
                if (p1.Y < MinY)
                    MinY = p1.Y;
                if (p1.X > MaxX)
                    MaxX = p1.X;
                if (p1.Y > MaxY)
                    MaxY = p1.Y;
            }
        }

        public CurveLoop Loop3d { get; }

        public bool IsCounterclockwise => _loop2d.IsCounterclockwise(XYZ.BasisZ);

        public XYZ StartPointUV => _loop2d.First().GetEndPoint(0);

        public IEnumerator<Curve> GetEnumerator()
        {
            return _loop2d.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public PointLocation IsPointInside(XYZ point)
        {
            if (point.X - Epsilon < MinX
                || point.X + Epsilon > MaxX
                || point.Y - Epsilon < MinY
                || point.Y + Epsilon > MaxY)
                return PointLocation.Outside;

            if (_loop2d.Any(curve => curve.Distance(point) < Epsilon))
                return PointLocation.OnTheEdge;

            var line = Line.CreateBound(point, new XYZ(MaxX + 0.1234, MaxY + 0.3456, 0));

            var nIntersections = _loop2d
                .Where(edge => edge.Intersect(line) == SetComparisonResult.Overlap)
                .Count(edge => line.Distance(edge.GetEndPoint(0)) > Epsilon);

            return nIntersections % 2 == 1 ? PointLocation.Inside : PointLocation.Outside;
        }
    }
}
