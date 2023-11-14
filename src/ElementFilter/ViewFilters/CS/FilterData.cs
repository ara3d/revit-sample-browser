// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using PFRF = Autodesk.Revit.DB.ParameterFilterRuleFactory;

namespace RevitMultiSample.ViewFilters.CS
{
    /// <summary>
    ///     Sample custom immutable class used to represents Revit internal FilterRule.
    ///     This class and its variables will help display the contents of UI controls.
    ///     This class can build its data caches to Revit FilterRule object.
    /// </summary>
    public sealed class FilterRuleBuilder
    {
        /// <summary>
        ///     Create FilterRuleBuilder for String FilterRule
        /// </summary>
        /// <param name="param">Parameter of FilterRule.</param>
        /// <param name="ruleCriteria">Rule criteria.</param>
        /// <param name="ruleValue">Rule value.</param>
        public FilterRuleBuilder(BuiltInParameter param, string ruleCriteria, string ruleValue)
        {
            InitializeMemebers();
            //
            // set data with specified values
            ParamType = StorageType.String;
            Parameter = param;
            RuleCriteria = ruleCriteria;
            RuleValue = ruleValue;
        }

        /// <summary>
        ///     Create FilterRuleBuilder for double FilterRule
        /// </summary>
        /// <param name="param">Parameter of FilterRule.</param>
        /// <param name="ruleCriteria">Rule criteria.</param>
        /// <param name="ruleValue">Rule value.</param>
        /// <param name="tolerance">Epsilon for double values comparison.</param>
        public FilterRuleBuilder(BuiltInParameter param, string ruleCriteria, double ruleValue, double tolearance)
        {
            InitializeMemebers();
            //
            // set data with specified values
            ParamType = StorageType.Double;
            Parameter = param;
            RuleCriteria = ruleCriteria;
            RuleValue = ruleValue.ToString();
            Epsilon = tolearance;
        }

        /// <summary>
        ///     Create FilterRuleBuilder for int FilterRule
        /// </summary>
        /// <param name="param">Parameter of FilterRule.</param>
        /// <param name="ruleCriteria">Rule criteria.</param>
        /// <param name="ruleValue">Rule value.</param>
        public FilterRuleBuilder(BuiltInParameter param, string ruleCriteria, int ruleValue)
        {
            InitializeMemebers();
            //
            // set data with specified values
            ParamType = StorageType.Integer;
            Parameter = param;
            RuleCriteria = ruleCriteria;
            RuleValue = ruleValue.ToString();
        }

        /// <summary>
        ///     Create FilterRuleBuilder for ElementId FilterRule
        /// </summary>
        /// <param name="param">Parameter of FilterRule.</param>
        /// <param name="ruleCriteria">Rule criteria.</param>
        /// <param name="ruleValue">Rule value.</param>
        public FilterRuleBuilder(BuiltInParameter param, string ruleCriteria, ElementId ruleValue)
        {
            InitializeMemebers();
            //
            // set data with specified values
            ParamType = StorageType.ElementId;
            Parameter = param;
            RuleCriteria = ruleCriteria;
            RuleValue = ruleValue.ToString();
        }

        /// <summary>
        ///     Parameter of filter rule
        /// </summary>
        public BuiltInParameter Parameter { get; private set; }

        /// <summary>
        ///     Filter rule criteria(in String type)
        /// </summary>
        public string RuleCriteria { get; private set; }

        /// <summary>
        ///     Rule values in string
        /// </summary>
        public string RuleValue { get; private set; }

        /// <summary>
        ///     Parameter storage type of current FilterRule.
        /// </summary>
        public StorageType ParamType { get; private set; }

        /// <summary>
        ///     Tolerance of double comparison, valid only when ParamType is double
        /// </summary>
        public double Epsilon { get; private set; }

        /// <summary>
        ///     Get ElementId of current parameter
        /// </summary>
        public ElementId ParamId => new ElementId(Parameter);

        /// <summary>
        ///     Create API FilterRule according to sample's FilterRuleBuilder
        /// </summary>
        /// <returns>API FilterRule converted from current FilterRuleBuilder.</returns>
        public FilterRule AsFilterRule()
        {
            var paramId = new ElementId(Parameter);
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

            //
            // Throw exception for others
            throw new NotImplementedException("This filter rule or criteria is not implemented yet.");
        }

        /// <summary>
        ///     Make sure all members are initialized with expected values.
        /// </summary>
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
    ///     This class used to represents data for one API filter.
    ///     It consists of BuiltInCategory and filter rules
    /// </summary>
    public sealed class FilterData
    {
        /// <summary>
        ///     Reserves current active document
        /// </summary>
        private readonly Document m_doc;

        /// <summary>
        ///     BuiltInCategories of filter
        /// </summary>
        private List<BuiltInCategory> m_filterCategories;

        /// <summary>
        ///     Filer rules of filter
        /// </summary>
        private readonly List<FilterRuleBuilder> m_filterRules;

        /// <summary>
        ///     Create sample custom FilterData with specified categories and FilterRuleBuilder
        /// </summary>
        /// <param name="doc">Revit active document.</param>
        /// <param name="categories">BuilInCategories of filter.</param>
        /// <param name="filterRules">FilterRuleBuilder set of filter.</param>
        public FilterData(Document doc,
            ICollection<BuiltInCategory> categories, ICollection<FilterRuleBuilder> filterRules)
        {
            m_doc = doc;
            m_filterCategories = new List<BuiltInCategory>();
            m_filterCategories.AddRange(categories);
            m_filterRules = new List<FilterRuleBuilder>();
            m_filterRules.AddRange(filterRules);
        }

        /// <summary>
        ///     Create sample custom FilterData with specified category id and FilterRuleBuilder
        /// </summary>
        /// <param name="doc">Revit active document.</param>
        /// <param name="categories">BuilInCategory ids of filter.</param>
        /// <param name="filterRules">FilterRuleBuilder set of filter.</param>
        public FilterData(Document doc,
            ICollection<ElementId> categories, ICollection<FilterRuleBuilder> filterRules)
        {
            m_doc = doc;
            m_filterCategories = new List<BuiltInCategory>();
            foreach (var catId in categories)
                m_filterCategories.Add((BuiltInCategory)catId.Value);
            m_filterRules = new List<FilterRuleBuilder>();
            m_filterRules.AddRange(filterRules);
        }

        /// <summary>
        ///     Get BuiltInCategories of filter
        /// </summary>
        public List<BuiltInCategory> FilterCategories => m_filterCategories;

        /// <summary>
        ///     Get FilterRuleBuilder of API filter's rules
        /// </summary>
        public List<FilterRuleBuilder> RuleData => m_filterRules;

        /// <summary>
        ///     Get BuiltInCategory Ids of filter
        /// </summary>
        public IList<ElementId> GetCategoryIds()
        {
            var catIds = new List<ElementId>();
            foreach (var cat in m_filterCategories)
                catIds.Add(new ElementId(cat));
            return catIds;
        }

        /// <summary>
        ///     Set new categories, this method will possibly update existing criteria
        /// </summary>
        /// <param name="newCatIds">New categories for current filter.</param>
        /// <returns>true if categories or criteria are changed; otherwise false.</returns>
        /// <remarks>
        ///     If someone parameter of criteria cannot be supported by new categories,
        ///     the old criteria will be cleaned and set to empty
        /// </remarks>
        public bool SetNewCategories(List<BuiltInCategory> newCats)
        {
            // do nothing if new categories are equals to old categories
            if (ListCompareUtility<BuiltInCategory>.Equals(newCats, m_filterCategories))
                return false;
            m_filterCategories = newCats; // update categories

            var newCatIds = new List<ElementId>();
            foreach (var cat in newCats) newCatIds.Add(new ElementId(cat));
            //
            // Check if need to update file rules:
            // . if filer rule is empty, do nothing
            // . if some parameters of rules cannot be supported by new categories, clean all old rules
            var supportParams =
                ParameterFilterUtilities.GetFilterableParametersInCommon(m_doc, newCatIds);
            foreach (var rule in m_filterRules)
                if (!supportParams.Contains(new ElementId(rule.Parameter)))
                {
                    m_filterRules.Clear();
                    break;
                }

            return true;
        }
    }

    /// <summary>
    ///     This class define constant strings to map rule criteria
    /// </summary>
    public sealed class RuleCriteraNames
    {
        /// <summary>
        ///     String represents BeginWith criteria
        /// </summary>
        public const string BeginWith = "begins with";

        /// <summary>
        ///     String represents Contains criteria
        /// </summary>
        public const string Contains = "contains";

        /// <summary>
        ///     String represents EndWith criteria
        /// </summary>
        public const string EndsWith = "ends with";

        /// <summary>
        ///     String represents Equals criteria
        /// </summary>
        public const string Equals = "equals";

        /// <summary>
        ///     String represents GreaterThan criteria
        /// </summary>
        public const string Greater = "is greater than";

        /// <summary>
        ///     String represents GreaterOrEqual criteria
        /// </summary>
        public const string GreaterOrEqual = "is greater than or equal to";

        /// <summary>
        ///     String represents LessOrEqual criteria
        /// </summary>
        public const string LessOrEqual = "is less than or equal to";

        /// <summary>
        ///     String represents Less criteria
        /// </summary>
        public const string Less = "is less than";

        /// <summary>
        ///     String represents NotBeginWith criteria
        /// </summary>
        public const string NotBeginWith = "does not begin with";

        /// <summary>
        ///     String represents NotContains criteria
        /// </summary>
        public const string NotContains = "does not contain";

        /// <summary>
        ///     String represents NotEndsWith criteria
        /// </summary>
        public const string NotEndsWith = "does not end with";

        /// <summary>
        ///     String represents NotEquals criteria
        /// </summary>
        public const string NotEquals = "does not equal";

        /// <summary>
        ///     Invalid criteria
        /// </summary>
        public const string Invalid = "n/a";

        /// <summary>
        ///     Hide ctor, this class defines only static members, no need to be created
        /// </summary>
        private RuleCriteraNames()
        {
        }

        /// <summary>
        ///     Get all supported criteria(in string) according to StorageType of parameter
        /// </summary>
        /// <param name="paramType">Parameter type.</param>
        /// <returns>String list of criteria supported for specified parameter type.</returns>
        public static ICollection<string> Criterions(StorageType paramType)
        {
            ICollection<string> returns = new List<string>();
            //
            // all parameter supports following criteria
            returns.Add(Equals);
            returns.Add(Greater);
            returns.Add(GreaterOrEqual);
            returns.Add(LessOrEqual);
            returns.Add(Less);
            returns.Add(NotEquals);
            // 
            // Only string parameter support criteria below
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
