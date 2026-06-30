using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ListViews sample.</summary>
    internal static partial class Util
    {
        public static Element GetViewport(ViewSheet sheet, View view)
        {
            var doc = sheet.Document;

            var bip
                = BuiltInParameter.VIEW_NAME;

            ParameterValueProvider provider
                = new(
                    new ElementId(bip));

            FilterStringRuleEvaluator evaluator
                = new FilterStringEquals();

            FilterRule rule = new FilterStringRule(
                provider, evaluator, view.Name);

            ElementParameterFilter name_filter
                = new(rule);

            var bic
                = BuiltInCategory.OST_Viewports;

            List<Element> viewports
                = new(
                    new FilteredElementCollector(doc)
                        .OfCategory(bic)
                        .WherePasses(name_filter)
                        .ToElements());

            Debug.Assert(viewports[0].OwnerViewId.Equals(ElementId.InvalidElementId),
                "expected the first viewport to have an invalid owner view id");

            Debug.Assert(!viewports[1].OwnerViewId.Equals(ElementId.InvalidElementId),
                "expected the second viewport to have a valid owner view id");

            var i = 1;

            return viewports[i];
        }

        public static string GetViewSheetSetViewsBenchmark(Document doc)
        {
            var sheetSets = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheetSet));

            var n = sheetSets.GetElementCount();

            var result = $"Total of {n} sheet sets in this project.\n\n";

            Stopwatch stopWatch = new();
            stopWatch.Start();

            foreach (ViewSheetSet set in sheetSets)
            {
                result += set.Name;

                var views = set.Views;

                result += $" has {views.Size} views.\n";
            }

            stopWatch.Stop();

            double ms = stopWatch.ElapsedMilliseconds;

            result += $"\nOperation completed in {Math.Round(ms / 1000.0, 3)} seconds.\nAverage of {ms / n} ms per loop iteration.";

            return result;
        }
    }
}
