// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from GetCentroid by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/GetCentroid

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.GetCentroid.CS
{
    internal static class GetCentroidSelection
    {
        public static IList<ElementId> GetElementIds(
            UIDocument uidoc,
            ref string message)
        {
            var doc = uidoc.Document;
            var ids = uidoc.Selection.GetElementIds().ToList();
            if (ids.Count > 0)
            {
                return ids;
            }

            if (doc.ActiveView.ViewType == ViewType.Internal)
            {
                message = "Cannot pick elements in this view: "
                          + doc.ActiveView.Name;
                return null;
            }

            try
            {
                return uidoc.Selection
                    .PickObjects(
                        Autodesk.Revit.UI.Selection.ObjectType.Element,
                        "Please select some elements")
                    .Select(reference => reference.ElementId)
                    .ToList();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                message = "Element selection was cancelled.";
                return null;
            }
        }
    }
}
