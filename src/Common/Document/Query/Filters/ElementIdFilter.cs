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
    public class ElementIdFilter : Filter
    {
        private readonly List<ElementIdCmdArgument> ids;


        public ElementIdFilter(List<ElementIdCmdArgument> ids)
        {
            this.ids = ids;
            FilterSyntax = "new ElementIdSetFilter(new [] {" + String.Join(", ", ids.Select(x => x.Name)) + "})";
        }


        public static IEnumerable<Filter> Create(IList<ICommand> commands)
        {
            var ids = commands.OfType<ElementIdCmd>().SelectMany(x => x.Arguments).OfType<ElementIdCmdArgument>().ToList();
            if (ids.Any())
            {
                yield return new ElementIdFilter(ids);
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new ElementIdSetFilter(ids.Select(x => x.Value).ToList());
        }
    }
}