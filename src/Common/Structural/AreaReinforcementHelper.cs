// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.Common.Structural
{
    public static class AreaReinforcementHelper
    {
        public static bool IsRectangular(CurveArray curves)
        {
            return IsRectangular(curves.Cast<Curve>().ToList());
        }

        public static bool IsRectangular(IList<Curve> curves)
        {
            if (curves == null || curves.Count != 4)
                return false;

            var lines = curves.OfType<Line>().ToList();
            if (lines.Count != 4)
                return false;

            for (var i = 0; i < 4; i++)
            {
                var line1 = lines[i];
                var line2 = lines[(i + 1) % 4];
                var vec1 = XyzMath.SubXyz(line1.GetEndPoint(1), line1.GetEndPoint(0));
                var vec2 = XyzMath.SubXyz(line2.GetEndPoint(1), line2.GetEndPoint(0));
                if (!XyzMath.IsEqual(XyzMath.DotMatrix(vec1, vec2), 0))
                    return false;
            }

            return true;
        }

        public enum HookOrientation
        {
            Up = 0,
            Down = 2
        }
    }
}
