// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using PFRF = Autodesk.Revit.DB.ParameterFilterRuleFactory;

namespace Ara3D.RevitSampleBrowser.ElementFilterSample.CS
{
    /// <summary>
    /// Mutable UI model for a Revit filter rule; converts to <see cref="FilterRule"/> via <see cref="AsFilterRule"/>.
    /// </summary>
    public sealed class FilterRuleBuilder
    {
        public FilterRuleBuilder(BuiltInParameter param, string ruleCriteria, string ruleValue)
        {
            InitializeMemebers();
            ParamType = StorageType.String;
            Parameter = param;
            RuleCriteria = ruleCriteria;
            RuleValue = ruleValue;
        }

        public FilterRuleBuilder(BuiltInParameter param, string ruleCriteria, double ruleValue, double tolearance)
        {
            InitializeMemebers();
            ParamType = StorageType.Double;
            Parameter = param;
            RuleCriteria = ruleCriteria;
            RuleValue = ruleValue.ToString();
            Epsilon = tolearance;
        }

        public FilterRuleBuilder(BuiltInParameter param, string ruleCriteria, int ruleValue)
        {
            InitializeMemebers();
            ParamType = StorageType.Integer;
            Parameter = param;
            RuleCriteria = ruleCriteria;
            RuleValue = ruleValue.ToString();
        }

        public FilterRuleBuilder(BuiltInParameter param, string ruleCriteria, ElementId ruleValue)
        {
            InitializeMemebers();
            ParamType = StorageType.ElementId;
            Parameter = param;
            RuleCriteria = ruleCriteria;
            RuleValue = ruleValue.ToString();
        }

        public BuiltInParameter Parameter { get; private set; }

        public string RuleCriteria { get; private set; }

        public string RuleValue { get; private set; }

        public StorageType ParamType { get; private set; }

        /// <summary>Comparison tolerance; used only when <see cref="ParamType"/> is <see cref="StorageType.Double"/>.</summary>
        public double Epsilon { get; private set; }

        public ElementId ParamId => new(Parameter);

        public FilterRule AsFilterRule()
        {
            ElementId paramId = new(Parameter);
            switch (ParamType)
            {
                case StorageType.String:
                    switch (RuleCriteria)
                    {
                        case RuleCriteraNames.BeginWith:
                            return PFRF.CreateBeginsWithRule(paramId, RuleValue);
                        case RuleCriteraNames.Contains:
                            return PFRF.CreateContainsRule(paramId, RuleValue);
                        case RuleCriteraNames.EndsWith:
                            return PFRF.CreateEndsWithRule(paramId, RuleValue);
                        case RuleCriteraNames.Equals:
                            return PFRF.CreateEqualsRule(paramId, RuleValue);
                        case RuleCriteraNames.Greater:
                            return PFRF.CreateGreaterRule(paramId, RuleValue);
                        case RuleCriteraNames.GreaterOrEqual:
                            return PFRF.CreateGreaterOrEqualRule(paramId, RuleValue);
                        case RuleCriteraNames.Less:
                            return PFRF.CreateLessRule(paramId, RuleValue);
                        case RuleCriteraNames.LessOrEqual:
                            return PFRF.CreateLessOrEqualRule(paramId, RuleValue);
                        case RuleCriteraNames.NotBeginWith:
                            return PFRF.CreateNotBeginsWithRule(paramId, RuleValue);
                        case RuleCriteraNames.NotContains:
                            return PFRF.CreateNotContainsRule(paramId, RuleValue);
                        case RuleCriteraNames.NotEndsWith:
                            return PFRF.CreateNotEndsWithRule(paramId, RuleValue);
                        case RuleCriteraNames.NotEquals:
                            return PFRF.CreateNotEqualsRule(paramId, RuleValue);
                    }

                    break;
                case StorageType.Double:
                    switch (RuleCriteria)
                    {
                        case RuleCriteraNames.Equals:
                            return PFRF.CreateEqualsRule(paramId, double.Parse(RuleValue), Epsilon);
                        case RuleCriteraNames.Greater:
                            return PFRF.CreateGreaterRule(paramId, double.Parse(RuleValue), Epsilon);
                        case RuleCriteraNames.GreaterOrEqual:
                            return PFRF.CreateGreaterOrEqualRule(paramId, double.Parse(RuleValue), Epsilon);
                        case RuleCriteraNames.Less:
                            return PFRF.CreateLessRule(paramId, double.Parse(RuleValue), Epsilon);
                        case RuleCriteraNames.LessOrEqual:
                            return PFRF.CreateLessOrEqualRule(paramId, double.Parse(RuleValue), Epsilon);
                        case RuleCriteraNames.NotEquals:
                            return PFRF.CreateNotEqualsRule(paramId, double.Parse(RuleValue), Epsilon);
                    }

                    break;
                case StorageType.Integer:
                    switch (RuleCriteria)
                    {
                        case RuleCriteraNames.Equals:
                            return PFRF.CreateEqualsRule(paramId, int.Parse(RuleValue));
                        case RuleCriteraNames.Greater:
                            return PFRF.CreateGreaterRule(paramId, int.Parse(RuleValue));
                        case RuleCriteraNames.GreaterOrEqual:
                            return PFRF.CreateGreaterOrEqualRule(paramId, int.Parse(RuleValue));
                        case RuleCriteraNames.Less:
                            return PFRF.CreateLessRule(paramId, int.Parse(RuleValue));
                        case RuleCriteraNames.LessOrEqual:
                            return PFRF.CreateLessOrEqualRule(paramId, int.Parse(RuleValue));
                        case RuleCriteraNames.NotEquals:
                            return PFRF.CreateNotEqualsRule(paramId, int.Parse(RuleValue));
                    }

                    break;
                case StorageType.ElementId:
                    switch (RuleCriteria)
                    {
                        case RuleCriteraNames.Equals:
                            return PFRF.CreateEqualsRule(paramId, ElementId.Parse(RuleValue));
                        case RuleCriteraNames.Greater:
                            return PFRF.CreateGreaterRule(paramId, ElementId.Parse(RuleValue));
                        case RuleCriteraNames.GreaterOrEqual:
                            return PFRF.CreateGreaterOrEqualRule(paramId, ElementId.Parse(RuleValue));
                        case RuleCriteraNames.Less:
                            return PFRF.CreateLessRule(paramId, ElementId.Parse(RuleValue));
                        case RuleCriteraNames.LessOrEqual:
                            return PFRF.CreateLessOrEqualRule(paramId, ElementId.Parse(RuleValue));
                        case RuleCriteraNames.NotEquals:
                            return PFRF.CreateNotEqualsRule(paramId, ElementId.Parse(RuleValue));
                    }

                    break;
            }

            throw new NotImplementedException("This filter rule or criteria is not implemented yet.");
        }

        private void InitializeMemebers()
        {
            Parameter = BuiltInParameter.INVALID;
            RuleCriteria = string.Empty;
            RuleValue = string.Empty;
            ParamType = StorageType.None;
            Epsilon = 0.0f;
        }
    }

    /// <summary>
    /// UI model for one parameter filter: categories and rules backed by the active document.
    /// </summary>
    public sealed class FilterData
    {
        private readonly Document m_doc;

        public FilterData(Document doc,
            ICollection<BuiltInCategory> categories, ICollection<FilterRuleBuilder> filterRules)
        {
            m_doc = doc;
            FilterCategories = [.. categories];
            RuleData = [.. filterRules];
        }

        public FilterData(Document doc,
            ICollection<ElementId> categories, ICollection<FilterRuleBuilder> filterRules)
        {
            m_doc = doc;
            FilterCategories = [];
            foreach (var catId in categories)
            {
                FilterCategories.Add((BuiltInCategory)catId.Value);
            }

            RuleData = [.. filterRules];
        }

        public List<BuiltInCategory> FilterCategories { get; private set; }

        public List<FilterRuleBuilder> RuleData { get; }

        public IList<ElementId> GetCategoryIds()
        {
            List<ElementId> catIds = [];
            foreach (var cat in FilterCategories)
            {
                catIds.Add(new ElementId(cat));
            }

            return catIds;
        }

        /// <returns>true when categories changed or rules were cleared because new categories no longer support them.</returns>
        public bool SetNewCategories(List<BuiltInCategory> newCats)
        {
            if (ListCompareUtility<BuiltInCategory>.Equals(newCats, FilterCategories))
                return false;
            FilterCategories = newCats;

            List<ElementId> newCatIds = [];
            foreach (var cat in newCats)
            {
                newCatIds.Add(new ElementId(cat));
            }

            var supportParams =
                ParameterFilterUtilities.GetFilterableParametersInCommon(m_doc, newCatIds);
            foreach (var rule in RuleData)
            {
                if (!supportParams.Contains(new ElementId(rule.Parameter)))
                {
                    RuleData.Clear();
                    break;
                }
            }

            return true;
        }
    }

    public sealed class RuleCriteraNames
    {
        public const string BeginWith = "begins with";
        public const string Contains = "contains";
        public const string EndsWith = "ends with";
        public const string Equals = "equals";
        public const string Greater = "is greater than";
        public const string GreaterOrEqual = "is greater than or equal to";
        public const string LessOrEqual = "is less than or equal to";
        public const string Less = "is less than";
        public const string NotBeginWith = "does not begin with";
        public const string NotContains = "does not contain";
        public const string NotEndsWith = "does not end with";
        public const string NotEquals = "does not equal";
        public const string Invalid = "n/a";

        private RuleCriteraNames()
        {
        }

        public static ICollection<string> Criterions(StorageType paramType)
        {
            ICollection<string> returns =
            [
                Equals,
                Greater,
                GreaterOrEqual,
                LessOrEqual,
                Less,
                NotEquals
            ];
            // String parameters also support substring criteria.
            if (paramType == StorageType.String)
            {
                returns.Add(BeginWith);
                returns.Add(Contains);
                returns.Add(EndsWith);
                returns.Add(NotBeginWith);
                returns.Add(NotContains);
                returns.Add(NotEndsWith);
            }

            return returns;
        }
    }
}
