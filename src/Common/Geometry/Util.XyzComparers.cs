// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Collections.Generic;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Consolidated XYZ comparers

        public class XyzEqualityComparer : IEqualityComparer<XYZ>
        {
            private readonly double _eps;

            public XyzEqualityComparer()
                : this(0)
            {
            }

            public XyzEqualityComparer(double eps)
            {
                _eps = eps;
            }

            public bool Equals(XYZ p, XYZ q)
            {
                return 0 < _eps
                    ? _eps > p.DistanceTo(q)
                    : p.IsAlmostEqualTo(q);
            }

            public int GetHashCode(XYZ p)
            {
                return PointString(p).GetHashCode();
            }
        }

        #endregion
    }
}
