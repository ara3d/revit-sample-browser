using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>
    ///     A simple class with two coordinates
    ///     and some other basic info.
    /// </summary>
    internal class WallOpening2d
    {
        public XYZ Start { get; set; }
        public XYZ End { get; set; }

        public override string ToString()
        {
            return $"({Util.PointString(Start)}-{Util.PointString(End)})";
        }
    }

    /// <summary>Utilities extracted from TBC_WallOpenings sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Move out of wall and up from floor a bit.
        /// </summary>
        private const double _wallOpeningOffset = 0.1;

        /// <summary>
        ///     Predicate: is the given number even?
        /// </summary>
        internal static bool IsEven(int i)
        {
            return 0 == i % 2;
        }

        /// <summary>
        ///     Predicate: does the given reference refer to a surface?
        /// </summary>
        internal static bool IsSurface(Reference r)
        {
            return ElementReferenceType.REFERENCE_TYPE_SURFACE
                   == r.ElementReferenceType;
        }

        /// <summary>
        ///     Retrieve all wall openings,
        ///     including at start and end of wall.
        /// </summary>
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

            var rayStart = new XYZ(wallOrigin.X - offsetOut.U,
                wallOrigin.Y - offsetOut.V, elevation + _wallOpeningOffset);

            var intersector
                = new ReferenceIntersector(wall.Id,
                    FindReferenceTarget.Face, view);

            var refs
                = intersector.Find(rayStart, wallDirection);

            var pointList = new List<XYZ>(refs
                .Where(r => IsSurface(
                    r.GetReference()))
                .Where(r => r.Proximity
                            < wallLength + _wallOpeningOffset + _wallOpeningOffset)
                .OrderBy(
                    r => r.Proximity)
                .Select(r
                    => r.GetReference().GlobalPoint)
                .Distinct(new XyzEqualityComparer()));

            var q = wallOrigin + _wallOpeningOffset * XYZ.BasisZ;

            var wallHasFaceAtStart = IsEqual(
                pointList[0], q);

            if (wallHasFaceAtStart)
                pointList.RemoveAll(p
                    => IsEqual(p, q));
            else
                pointList.Insert(0, wallOrigin);

            q = wallEndPoint + _wallOpeningOffset * XYZ.BasisZ;

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

            var wallOpenings = new List<WallOpening2d>(
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
