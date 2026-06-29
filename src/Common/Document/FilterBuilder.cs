// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;
using Ara3D.RevitSampleBrowser.ElementFilterSample.CS;
using System;
using System.Collections.Generic;
using System.Linq;

using Document = Autodesk.Revit.DB.Document;
using RevitView = Autodesk.Revit.DB.View;

namespace Ara3D.RevitSampleBrowser.Common.Documents
{
    public static class FilterBuilder
    {
        public static ICollection<ParameterFilterElement> GetViewFilters(Document doc) =>
                    new FilteredElementCollector(doc).WherePasses(new ElementClassFilter(typeof(ParameterFilterElement)))
                        .ToElements().Cast<ParameterFilterElement>().ToList();

        public static FilterRuleBuilder CreateFilterRuleBuilder(BuiltInParameter param, FilterRule rule)
                {
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
                            throw new NotImplementedException("The filter rule is not recognizable and supported yet!");
                    }
                }

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

        public static ElementFilter CreateElementFilterFromFilterRules(IList<FilterRule> filterRules)
                {
                    IList<ElementFilter> elemFilters = new List<ElementFilter>();
                    foreach (var filterRule in filterRules)
                        elemFilters.Add(new ElementParameterFilter(filterRule));
                    return new LogicalAndFilter(elemFilters);
                }

        public static IList<FilterRule> GetConjunctionOfFilterRulesFromElementFilter(ElementFilter elemFilter)
                {
                    if (!(elemFilter is LogicalAndFilter logicalAndFilter))
                        return null;

                    var childElemFilters = logicalAndFilter.GetFilters();
                    if (childElemFilters.Count == 0)
                        return null;

                    IList<FilterRule> filterRules = new List<FilterRule>();
                    foreach (var childElemFilter in childElemFilters)
                    {
                        if (!(childElemFilter is ElementParameterFilter elemParamFilter))
                            return null;
                        var rules = elemParamFilter.GetRules();
                        if (rules.Count != 1)
                            return null;
                        filterRules.Add(rules[0]);
                    }

                    return filterRules;
                }

        private static string GetEvaluatorCriteriaName(FilterStringRuleEvaluator fsre, bool inverted)
                {
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
                            return isInverseRule ? RuleCriteraNames.NotEquals : RuleCriteraNames.Equals;
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

        private static string GetEvaluatorCriteriaName(FilterNumericRuleEvaluator fsre, bool inverted)
                {
                    var isInverseRule = inverted;
                    switch (fsre)
                    {
                        case FilterNumericEquals _:
                            return isInverseRule ? RuleCriteraNames.NotEquals : RuleCriteraNames.Equals;
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

    }
}