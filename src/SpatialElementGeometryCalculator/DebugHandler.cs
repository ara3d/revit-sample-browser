// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from SpatialElementGeometryCalculator by Jeremy Tammik et al.
// https://github.com/jeremytammik/SpatialElementGeometryCalculator (MIT License)

namespace Ara3D.RevitSampleBrowser.SpatialElementGeometryCalculator.CS
{
    internal static class DebugHandler
    {
        static bool _enableSolidUtilityVolumes;

        public static bool EnableSolidUtilityVolumes
        {
            get => _enableSolidUtilityVolumes;
            set => _enableSolidUtilityVolumes = value;
        }
    }
}
