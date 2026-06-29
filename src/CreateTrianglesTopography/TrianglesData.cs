// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CreateTrianglesTopography.CS
{
    public class TrianglesData
    {
        public IList<XYZ> Points { get; set; }
        public IList<IList<int>> Facets { get; set; }
    }
}
