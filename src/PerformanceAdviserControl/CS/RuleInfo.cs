// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.PerformanceAdviserControl.CS
{
    /// <summary>
    ///     A simple data class that holds Performance Adviser rule information for use with
    ///     TestDisplayDialog
    /// </summary>
    public class RuleInfo
    {
        public readonly PerformanceAdviserRuleId Id;
        public readonly bool IsEnabled;
        public readonly bool IsOurRule;
        public readonly string RuleDescription;
        public readonly string RuleName;

        public RuleInfo(PerformanceAdviserRuleId id, bool isOurRule, string name, string description, bool isEnabled)
        {
            Id = id;
            IsOurRule = isOurRule;
            RuleName = name;
            RuleDescription = description;
            IsEnabled = isEnabled;
        }
    }
}
