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
    public class ClassFilter : Filter
    {
        private readonly List<ClassCmdArgument> types;


        public ClassFilter(List<ClassCmdArgument> types)
        {
            this.types = types;
            FilterSyntax = types.Count == 1
                ? $".OfClass({types.First().Name})"
                : "new ElementMulticlassFilter(new [] {" + String.Join(", ", types.Select(x => x.Name)) + "})";
        }


        public static IEnumerable<Filter> Create(IList<ICommand> commands)
        {
            var types = commands.OfType<ClassCmd>().SelectMany(x => x.Arguments).OfType<ClassCmdArgument>().ToList();
            if (types.Any())
            {
                yield return new ClassFilter(types);
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new ElementMulticlassFilter(types.Select(x => x.Value).ToList());
        }
    }
}