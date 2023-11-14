// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.ViewFilters.CS
{
    /// <summary>
    ///     One utility class used to access or modify filter related
    /// </summary>
    public sealed class FiltersUtil
    {
        /// <summary>
        ///     Hide ctor, this class defines only static members, no need to be created
        /// </summary>
        private FiltersUtil()
        {
        }

        /// <summary>
        ///     Get all view filters(ParameterFilterElement) within current document
        /// </summary>
        /// <returns>All existing filters.</returns>
        public static ICollection<ParameterFilterElement> GetViewFilters(Document doc)
        {
            var filter = new ElementClassFilter(typeof(ParameterFilterElement));
            var collector = new FilteredElementCollector(doc);
            return collector.WherePasses(filter).ToElements()
                .Cast<ParameterFilterElement>().ToList();
        }

        /// <summary>
        ///     Convert FilterRule to our custom FilterRuleBuilder which will be displayed in form controls
        /// </summary>
        /// <param name="param">Parameter to which the FilterRule is applied.</param>
        /// <param name="rule">FilterRule to be converted.</param>
        /// <returns>Custom FilterRuleBuilder data converted from FilterRule</returns>
        public static FilterRuleBuilder CreateFilterRuleBuilder(BuiltInParameter param, FilterRule rule)
        {
            // Maybe FilterRule is inverse rule, we need to find its inner rule(FilterValueRule)
            // Note that the rule may be inversed more than once.
            var inverted = false;
            var innerRule = ReflectToInnerRule(rule, out inverted);
            switch (innerRule)
            {
                case FilterStringRule strRule:
                {
                    var evaluator = strRule.GetEvaluator();
                    return new FilterRuleBuilder(param, GetEvaluatorCriteriaName(evaluator, inverted), strRule.RuleString);
                }
                case FilterDoubleRule dbRule:
                {
                    var evaluator = dbRule.GetEvaluator();
                    return new FilterRuleBuilder(param, GetEvaluatorCriteriaName(evaluator, inverted), dbRule.RuleValue,
                        dbRule.Epsilon);
                }
                case FilterIntegerRule intRule:
                {
                    var evaluator = intRule.GetEvaluator();
                    return new FilterRuleBuilder(param, GetEvaluatorCriteriaName(evaluator, inverted), intRule.RuleValue);
                }
                case FilterElementIdRule idRule:
                {
                    var evaluator = idRule.GetEvaluator();
                    return new FilterRuleBuilder(param, GetEvaluatorCriteriaName(evaluator, inverted), idRule.RuleValue);
                }
                default:
                    // 
                    // for other rule, not supported yet
                    throw new NotImplementedException("The filter rule is not recognizable and supported yet!");
            }
        }

        /// <summary>
        ///     Get criteria(in string) from String Evaluator
        /// </summary>
        /// <param name="fsre">String Evaluator used to retrieve the criteria.</param>
        /// <param name="inverted">
        ///     Indicates whether rule to which Evaluator belong is inverse rule.
        ///     If inverted is true, inverse criteria for this evaluator will be returned.
        /// </param>
        /// <returns>criteria of this Evaluator.</returns>
        private static string GetEvaluatorCriteriaName(FilterStringRuleEvaluator fsre, bool inverted)
        {
            // indicate if inverse criteria should be returned
            var isInverseRule = inverted;
            switch (fsre)
            {
                case FilterStringBeginsWith _:
                    return isInverseRule ? RuleCriteraNames.NotBeginWith : RuleCriteraNames.BeginWith;
                case FilterStringContains _:
                    return isInverseRule ? RuleCriteraNames.NotContains : RuleCriteraNames.Contains;
                case FilterStringEndsWith _:
                    return isInverseRule ? RuleCriteraNames.NotEndsWith : RuleCriteraNames.EndsWith;
                case FilterStringEquals _:
                    return isInverseRule ? RuleCriteraNames.NotEquals : RuleCriteraNames.Equals_;
                case FilterStringGreater _:
                    return isInverseRule ? RuleCriteraNames.LessOrEqual : RuleCriteraNames.Greater;
                case FilterStringGreaterOrEqual _:
                    return isInverseRule ? RuleCriteraNames.Less : RuleCriteraNames.GreaterOrEqual;
                case FilterStringLess _:
                    return isInverseRule ? RuleCriteraNames.GreaterOrEqual : RuleCriteraNames.Less;
                case FilterStringLessOrEqual _:
                    return isInverseRule ? RuleCriteraNames.Greater : RuleCriteraNames.LessOrEqual;
                default:
                    return RuleCriteraNames.Invalid;
            }
        }

        /// <summary>
        ///     Get criteria(in string) from Numeric Evaluator
        /// </summary>
        /// <param name="fsre">String Evaluator used to retrieve the criteria.</param>
        /// <param name="inverted">
        ///     Indicates whether rule to which Evaluator belong is inverse rule.
        ///     If inverted is true, inverse criteria for this evaluator will be returned.
        /// </param>
        /// <returns>criteria of this Evaluator.</returns>
        private static string GetEvaluatorCriteriaName(FilterNumericRuleEvaluator fsre, bool inverted)
        {
            // indicate if inverse criteria should be returned
            var isInverseRule = inverted;
            switch (fsre)
            {
                case FilterNumericEquals _:
                    return isInverseRule ? RuleCriteraNames.NotEquals : RuleCriteraNames.Equals_;
                case FilterNumericGreater _:
                    return isInverseRule ? RuleCriteraNames.LessOrEqual : RuleCriteraNames.Greater;
                case FilterNumericGreaterOrEqual _:
                    return isInverseRule ? RuleCriteraNames.Less : RuleCriteraNames.GreaterOrEqual;
                case FilterNumericLess _:
                    return isInverseRule ? RuleCriteraNames.GreaterOrEqual : RuleCriteraNames.Less;
                case FilterNumericLessOrEqual _:
                    return isInverseRule ? RuleCriteraNames.Greater : RuleCriteraNames.LessOrEqual;
                default:
                    return RuleCriteraNames.Invalid;
            }
        }

        /// <summary>
        ///     Reflect filter rule to its inner rule, the final inner rule is FilterValueRule for this sample
        /// </summary>
        /// <param name="srcRule">Source filter to be checked.</param>
        /// <param name="inverted">
        ///     Indicates if source rule is inverse rule mapping to its inner rule.
        ///     Note that the rule may be inversed more than once, if inverse time is odd(1, 3, 5...), the inverted will be true.
        ///     If inverse time is even(0, 2, 4...), the inverted will be false.
        /// </param>
        /// <returns>Inner rule of source rule, the inner rule is FilterValueRule type for this sample.</returns>
        public static FilterRule ReflectToInnerRule(FilterRule srcRule, out bool inverted)
        {
            if (srcRule is FilterInverseRule rule)
            {
                inverted = true;
                var innerRule = rule.GetInnerRule();
                var invertedAgain = false;
                var returnRule = ReflectToInnerRule(innerRule, out invertedAgain);
                if (invertedAgain)
                    inverted = false;
                return returnRule;
            }

            inverted = false;
            return srcRule;
        }

        /// <summary>
        ///     Create an ElementFilter representing a conjunction ("ANDing together") of FilterRules.
        /// </summary>
        /// <param name="filterRules">A list of FilterRules</param>
        /// <returns>The ElementFilter.</returns>
        public static ElementFilter CreateElementFilterFromFilterRules(IList<FilterRule> filterRules)
        {
            // KEEP THIS FUNCTION SYNCHRONIZED WITH GetConjunctionOfFilterRulesFromElementFilter!!!

            // We use a LogicalAndFilter containing one ElementParameterFilter
            // for each FilterRule. We could alternatively create a single
            // ElementParameterFilter containing the entire list of FilterRules.
            IList<ElementFilter> elemFilters = new List<ElementFilter>();
            foreach (var filterRule in filterRules)
            {
                var elemParamFilter = new ElementParameterFilter(filterRule);
                elemFilters.Add(elemParamFilter);
            }

            var elemFilter = new LogicalAndFilter(elemFilters);

            return elemFilter;
        } // CreateElementFilterFromFilterRules

        /// <summary>
        ///     Given an ElementFilter representing a conjunction of FilterRules, return the list of FilterRules.
        /// </summary>
        /// <param name="elemFilter">An ElementFilter representing a conjunction of FilterRules.</param>
        /// <returns>A list of FilterRules whose conjunction the ElementFilter represents.</returns>
        public static IList<FilterRule> GetConjunctionOfFilterRulesFromElementFilter(ElementFilter elemFilter)
        {
            // KEEP THIS FUNCTION SYNCHRONIZED WITH CreateElementFilterFromFilterRules!!!

            // The ElementFilter is assumed to have been created by CreateElementFilterFromFilterRules,
            // which creates a LogicalAndFilter with each child being an ElementParameterFilter containing
            // just one FilterRule.
            if (!(elemFilter is LogicalAndFilter logicalAndFilter))
                return null; // Error

            var childElemFilters = logicalAndFilter.GetFilters();
            var numChildElemFilters = childElemFilters.Count;
            if (0 == numChildElemFilters)
                return null; // Error

            IList<FilterRule> filterRules = new List<FilterRule>();
            foreach (var childElemFilter in childElemFilters)
            {
                if (!(childElemFilter is ElementParameterFilter elemParamFilter))
                    return null;
                var rules = elemParamFilter.GetRules();
                if (1 != rules.Count)
                    return null;
                filterRules.Add(rules[0]);
            }

            return filterRules;
        } // GetConjunctionOfFilterRulesFromElementFilter
    } // class FiltersUtil
} // namespace
