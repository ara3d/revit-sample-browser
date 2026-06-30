// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from RevitFindExteriorWalls by Pekshev / Jeremy Tammik (MIT).
// https://github.com/jeremytammik/RevitFindExteriorWalls

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.FindExteriorWalls.CS
{
    internal class WallsSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem) => elem is Wall;

        public bool AllowReference(Reference reference, XYZ position) => false;
    }
}
