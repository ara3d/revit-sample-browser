// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Filters
{
    public class RuleFilter : Filter
    {
        private readonly RuleBasedFilterCmdArgument arg;


        public RuleFilter(RuleBasedFilterCmdArgument arg)
        {
            this.arg = arg;
            FilterSyntax = $"new LogicalAndFilter((document.GetElement({arg.Name}) as ParameterFilterElement).GetElementFilter()," + Environment.NewLine;
            FilterSyntax += $"       new ElementMulticategoryFilter((document.GetElement({arg.Name}) as ParameterFilterElement).GetCategories()))";
        }


        public static IEnumerable<QueryItem> Create(IList<ICommand> commands)
        {
            var rules = commands.OfType<RuleBasedFilterCmd>().SelectMany(x => x.Arguments).OfType<RuleBasedFilterCmdArgument>().ToList();
            if (rules.Count == 1)
            {
                yield return new RuleFilter(rules.First());
            }
            if (rules.Count > 1)
            {
                yield return new Group(rules.Select(x => new RuleFilter(x)).ToList());
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            var ruleElement = document.GetElement(this.arg.Value) as ParameterFilterElement;
            var elementFilter = ruleElement.GetElementFilter();
            var categories = ruleElement.GetCategories();
            ElementMulticategoryFilter categoryFilter = new(categories);
            var filters = new[] { categoryFilter, elementFilter };
            LogicalAndFilter andFilter = new(filters.Where(x => x != null).ToArray());
            return andFilter;
        }
    }
}