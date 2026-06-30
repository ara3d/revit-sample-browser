// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from SpatialElementGeometryCalculator by Jeremy Tammik et al.
// https://github.com/jeremytammik/SpatialElementGeometryCalculator (MIT License)

using System.Diagnostics;

namespace Ara3D.RevitSampleBrowser.SpatialElementGeometryCalculator.CS
{
    internal static class LogCreator
    {
        public static void LogEntry(string s)
        {
            Debug.Print(s);
        }
    }
}
