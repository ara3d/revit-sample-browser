// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from SpatialElementGeometryCalculator by Jeremy Tammik et al.
// https://github.com/jeremytammik/SpatialElementGeometryCalculator (MIT License)

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.SpatialElementGeometryCalculator.CS
{
    internal class SpatialBoundaryCache
    {
        public string roomName;
        public ElementId idElement;
        public ElementId idMaterial;
        public double dblNetArea;
        public double dblOpeningArea;

        public string AreaReport =>
            string.Format(
                "net {0}; opening {1}; gross {2}",
                dblNetArea, dblOpeningArea,
                dblNetArea + dblOpeningArea);
    }
}
