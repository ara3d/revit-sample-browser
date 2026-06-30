using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BuildingCoder
{
    internal class WallOpening2d
    {
        public XYZ Start { get; set; }
        public XYZ End { get; set; }

        public override string ToString()
        {
            return $"({Util.PointString(Start)}-{Util.PointString(End)})";
        }
    }
    internal static partial class Util
    {
        private const double _wallOpeningOffset = 0.1;

        internal static bool IsEven(int i)
        {
            return 0 == i % 2;
        }

        internal static bool IsSurface(Reference r)
        {
            return ElementReferenceType.REFERENCE_TYPE_SURFACE
                   == r.ElementReferenceType;
        }

        internal static List<WallOpening2d> GetWallOpenings(
            Wall wall,
            View3D view)
        {
            var doc = wall.Document;
            var level = doc.GetElement(wall.LevelId) as Level;
            var elevation = level.Elevation;
            var c = (wall.Location as LocationCurve).Curve;
            var wallOrigin = c.GetEndPoint(0);
            var wallEndPoint = c.GetEndPoint(1);
            var wallDirection = wallEndPoint - wallOrigin;
            var wallLength = wallDirection.GetLength();
            wallDirection = wallDirection.Normalize();
            var offsetOut = _wallOpeningOffset * new UV(wallDirection.X, wallDirection.Y);

            XYZ rayStart = new(wallOrigin.X - offsetOut.U,
                wallOrigin.Y - offsetOut.V, elevation + _wallOpeningOffset);

            ReferenceIntersector intersector
                = new(wall.Id,
                    FindReferenceTarget.Face, view);

            var refs
                = intersector.Find(rayStart, wallDirection);

            List<XYZ> pointList = new(refs
                .Where(r => IsSurface(
                    r.GetReference()))
                .Where(r => r.Proximity
                            < wallLength + _wallOpeningOffset + _wallOpeningOffset)
                .OrderBy(
                    r => r.Proximity)
                .Select(r
                    => r.GetReference().GlobalPoint)
                .Distinct(new XyzEqualityComparer()));

            var q = wallOrigin + (_wallOpeningOffset * XYZ.BasisZ);

            var wallHasFaceAtStart = IsEqual(
                pointList[0], q);

            if (wallHasFaceAtStart)
                pointList.RemoveAll(p
                    => IsEqual(p, q));
            else
                pointList.Insert(0, wallOrigin);

            q = wallEndPoint + (_wallOpeningOffset * XYZ.BasisZ);

            var wallHasFaceAtEnd = IsEqual(
                pointList.Last(), q);

            if (wallHasFaceAtEnd)
                pointList.RemoveAll(p
                    => IsEqual(p, q));
            else
                pointList.Add(wallEndPoint);

            var n = pointList.Count;

            Debug.Assert(IsEven(n),
                "expected an even number of opening sides");

            List<WallOpening2d> wallOpenings = new(
                n / 2);

            for (var i = 0; i < n; i += 2)
                wallOpenings.Add(new WallOpening2d
                {
                    Start = pointList[i],
                    End = pointList[i + 1]
                });
            return wallOpenings;
        }
    }
}
