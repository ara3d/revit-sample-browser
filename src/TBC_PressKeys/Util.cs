#region Namespaces

using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_PressKeys sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Return the first wall type with the given name.
        /// </summary>
        public static WallType GetFirstWallTypeNamed(
            Document doc,
            string name)
        {
            var bip
                = BuiltInParameter.SYMBOL_NAME_PARAM;

            var provider
                = new ParameterValueProvider(
                    new ElementId(bip));

            FilterStringRuleEvaluator evaluator
                = new FilterStringEquals();

            FilterRule rule = new FilterStringRule(
                provider, evaluator, name);

            var filter
                = new ElementParameterFilter(rule);

            var collector
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(WallType))
                    .WherePasses(filter);

            return collector.FirstElement() as WallType;
        }

        /// <summary>
        ///     Return the first wall found that
        ///     uses the given wall type.
        /// </summary>
        public static Wall GetFirstWallUsingType(
            Document doc,
            WallType wallType)
        {
            var bip
                = BuiltInParameter.ELEM_TYPE_PARAM;

            var provider
                = new ParameterValueProvider(
                    new ElementId(bip));

            FilterNumericRuleEvaluator evaluator
                = new FilterNumericEquals();

            FilterRule rule = new FilterElementIdRule(
                provider, evaluator, wallType.Id);

            var filter
                = new ElementParameterFilter(rule);

            var collector
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(Wall))
                    .WherePasses(filter);

            return collector.FirstElement() as Wall;
        }
    }
}
