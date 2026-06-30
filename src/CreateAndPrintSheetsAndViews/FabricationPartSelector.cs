// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CreateAndPrintSheetsAndViews by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CreateAndPrintSheetsAndViews

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.CreateAndPrintSheetsAndViews.CS
{
    /// <summary>
    /// Retrieve pre-selected or prompt user to select fabrication parts.
    /// </summary>
    class FabricationPartSelector
    {
        public FabricationPartSelector(UIDocument uidoc)
        {
            var doc = uidoc.Document;
            var sel = uidoc.Selection;

            Ids = [.. sel.GetElementIds().Where<ElementId>(
                    id => doc.GetElement(id) is FabricationPart)];

            var n = Ids.Count;

            while (0 == n)
            {
                try
                {
                    var refs = sel.PickObjects(
                        ObjectType.Element,
                        new FabricationPartSelectionFilter(),
                        "Please select fabrication part duct elements");

                    Ids = [.. refs.Select<Reference, ElementId>(
                            r => r.ElementId)];

                    n = Ids.Count;
                }
                catch (OperationCanceledException)
                {
                    Ids.Clear();
                    break;
                }
            }
        }

        public List<ElementId> Ids { get; }
    }
}
