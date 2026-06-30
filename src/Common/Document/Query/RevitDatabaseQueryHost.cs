// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.Common.Documents.Query
{
    /// <summary>
    /// Holds the active document during query parsing (replaces RevitDBExplorer.Application static state).
    /// </summary>
    public static class RevitDatabaseQueryHost
    {
        public static Document Document { get; set; }
    }
}
