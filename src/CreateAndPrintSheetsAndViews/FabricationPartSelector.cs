// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from CreateAndPrintSheetsAndViews by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CreateAndPrintSheetsAndViews

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Ara3D.RevitSampleBrowser.CreateAndPrintSheetsAndViews.CS
{
    /// <summary>
    /// Retrieve pre-selected or prompt user to select fabrication parts.
    /// </summary>
    class FabricationPartSelector
    {
        readonly List<ElementId> _ids;

        public FabricationPartSelector(UIDocument uidoc)
        {
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;

            _ids = new List<ElementId>(
                sel.GetElementIds().Where<ElementId>(
                    id => doc.GetElement(id) is FabricationPart));

            int n = _ids.Count;

            while (0 == n)
            {
                try
                {
                    IList<Reference> refs = sel.PickObjects(
                        ObjectType.Element,
                        new FabricationPartSelectionFilter(),
                        "Please select fabrication part duct elements");

                    _ids = new List<ElementId>(
                        refs.Select<Reference, ElementId>(
                            r => r.ElementId));

                    n = _ids.Count;
                }
                catch (OperationCanceledException)
                {
                    _ids.Clear();
                    break;
                }
            }
        }

        public List<ElementId> Ids => _ids;
    }
}
