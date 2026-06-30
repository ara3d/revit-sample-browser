// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;


namespace BuildingCoder
{
    public static class JtFilteredElementCollectorExtensions
    {
        public static FilteredElementCollector OfClass<T>(
            this FilteredElementCollector collector)
            where T : Element
        {
            return collector.OfClass(typeof(T));
        }

        public static IEnumerable<T> OfType<T>(
            this FilteredElementCollector collector)
            where T : Element
        {
            return Enumerable.OfType<T>(
                collector.OfClass<T>());
        }
    }
}
