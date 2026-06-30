using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ViewsShowingElements sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     View extension predicate method: does
        ///     this view intersect the given bounding box?
        /// </summary>
        public static bool IntersectsBoundingBox(
            this View view,
            BoundingBoxXYZ targetBoundingBox)
        {
            var doc = view.Document;
            var viewBoundingBox = view.CropBox;

            if (!view.CropBoxActive)
            {
                using var tr = new Transaction(doc);
                tr.Start("Temp");
                view.CropBoxActive = true;
                viewBoundingBox = view.CropBox;
                tr.RollBack();
            }

            Outline viewOutline = null;

            if (view is ViewPlan plan)
            {
                var viewRange = plan.GetViewRange();

                var bottomXYZ = (doc.GetElement(viewRange
                                        .GetLevelId(PlanViewPlane.BottomClipPlane))
                                    as Level).Elevation
                                + viewRange.GetOffset(PlanViewPlane.BottomClipPlane);

                var topXYZ = (doc.GetElement(viewRange
                                     .GetLevelId(PlanViewPlane.CutPlane))
                                 as Level).Elevation
                             + viewRange.GetOffset(PlanViewPlane.CutPlane);

                viewOutline = new Outline(new XYZ(
                    viewBoundingBox.Min.X, viewBoundingBox.Min.Y,
                    bottomXYZ), new XYZ(viewBoundingBox.Max.X,
                    viewBoundingBox.Max.Y, topXYZ));
            }

            if (!viewBoundingBox.Transform.BasisY.IsAlmostEqualTo(
                XYZ.BasisY))
                viewOutline = new Outline(
                    new XYZ(viewBoundingBox.Min.X,
                        viewBoundingBox.Min.Z, viewBoundingBox.Min.Y),
                    new XYZ(viewBoundingBox.Max.X,
                        viewBoundingBox.Max.Z, viewBoundingBox.Max.Y));

            using var boundingBoxAsOutline = new Outline(
                targetBoundingBox.Min, targetBoundingBox.Max);
            return boundingBoxAsOutline.Intersects(
                viewOutline, 0);
        }

        /// <summary>
        ///     Return an enumeration of all views in this
        ///     document that can display elements at all.
        /// </summary>
        internal static IEnumerable<View>
            FindAllViewsThatCanDisplayElements(
                this Document doc)
        {
            var filter
                = new ElementMulticlassFilter(
                    new List<Type>
                    {
                        typeof(View3D),
                        typeof(ViewPlan),
                        typeof(ViewSection)
                    });

            return new FilteredElementCollector(doc)
                .WherePasses(filter)
                .Cast<View>()
                .Where(v => !v.IsTemplate);
        }

        /// <summary>
        ///     Return all views that display
        ///     any of the given elements.
        /// </summary>
        public static IEnumerable<View>
            FindAllViewsWhereAllElementsVisible(
                this IEnumerable<Element> elements)
        {
            if (null == elements) throw new ArgumentNullException("elements");

            var e1 = elements.FirstOrDefault();

            if (null == e1) return new List<View>();

            var doc = e1.Document;

            var relevantViewList
                = doc.FindAllViewsThatCanDisplayElements();

            var idsToCheck
                = from e in elements
                select e.Id;

            return from v in relevantViewList
                let idList
                    = new FilteredElementCollector(doc, v.Id)
                        .WhereElementIsNotElementType()
                        .ToElementIds()
                where !idsToCheck.Except(idList).Any()
                select v;
        }

        /// <summary>
        ///     Determine whether an element is visible in a view.
        /// </summary>
        public static bool IsElementVisibleInView(
            this View view,
            Element el)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

            if (el == null) throw new ArgumentNullException(nameof(el));

            var doc = el.Document;

            var elId = el.Id;

            var idRule = ParameterFilterRuleFactory
                .CreateEqualsRule(
                    new ElementId(BuiltInParameter.ID_PARAM),
                    elId);

            var idFilter = new ElementParameterFilter(idRule);

            var cat = el.Category;
            var catFilter = new ElementCategoryFilter(cat.Id);

            var collector =
                new FilteredElementCollector(doc, view.Id)
                    .WhereElementIsNotElementType()
                    .WherePasses(catFilter)
                    .WherePasses(idFilter);

            return collector.Any();
        }
    }
}
