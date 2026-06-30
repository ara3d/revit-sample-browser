// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from CreateAndPrintSheetsAndViews by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CreateAndPrintSheetsAndViews

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.CreateAndPrintSheetsAndViews.CS
{
    /// <summary>
    /// Restrict user to select only fabrication part elements.
    /// </summary>
    class FabricationPartSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            return e is FabricationPart;
        }

        public bool AllowReference(Reference r, XYZ p)
        {
            return true;
        }
    }
}
