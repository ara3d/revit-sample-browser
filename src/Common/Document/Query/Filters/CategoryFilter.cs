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
    public class CategoryFilter : Filter
    {
        private readonly List<CategoryCmdArgument> categories;


        public CategoryFilter(List<CategoryCmdArgument> categories)
        {
            this.categories = categories;
            FilterSyntax = categories.Count == 1
                ? $".OfCategory({categories.First().Name})"
                : "new ElementMulticategoryFilter(new [] {" + String.Join(", ", categories.Select(x => x.Name)) + "})";
        }


        public static IEnumerable<Filter> Create(IList<ICommand> commands)
        {
            var categories = commands.OfType<CategoryCmd>().SelectMany(x => x.Arguments).OfType<CategoryCmdArgument>().ToList();
            if (categories.Any())
            {
                yield return new CategoryFilter(categories);
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new ElementMulticategoryFilter(categories.Select(x => x.Value).ToList());
        }
    }
}