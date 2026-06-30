// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from CableTraySample by Gavin_WS / Jeremy Tammik (MIT License):
// https://github.com/jeremytammik/CableTraySample

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;

namespace Ara3D.RevitSampleBrowser.CableTraySample.CS
{
    static class Util
    {
        static ElementId GetNamedElementId(
            FilteredElementCollector collector,
            string name)
        {
            IList<Element> elements = collector
                .Where(x => x.Name.Equals(name))
                .Cast<Element>()
                .ToList();

            return elements.Count > 0
                ? elements[0].Id
                : ElementId.InvalidElementId;
        }

        public static ElementId FindLevelId(Document doc, string name)
        {
            var collector = new FilteredElementCollector(doc)
                .OfClass(typeof(Level));

            return GetNamedElementId(collector, name);
        }

        public static ElementId FindCableTrayTypeId(Document doc, string name)
        {
            var collector = new FilteredElementCollector(doc)
                .OfClass(typeof(CableTrayType));

            return GetNamedElementId(collector, name);
        }
    }
}
