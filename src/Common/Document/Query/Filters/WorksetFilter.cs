// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Filters
{
    public class WorksetFilter : Filter
    {
        private readonly WorksetCmdArgument arg;


        public WorksetFilter(WorksetCmdArgument arg)
        {
            this.arg = arg;
            FilterSyntax = $"new ElementWorksetFilter ({arg.Name})";
        }


        public static IEnumerable<QueryItem> Create(IList<ICommand> commands)
        {
            var worksets = commands.OfType<WorksetCmd>().SelectMany(x => x.Arguments).OfType<WorksetCmdArgument>().ToList();
            if (worksets.Count == 1)
            {
                yield return new WorksetFilter(worksets.First());
            }
            if (worksets.Count > 1)
            {
                yield return new Group(worksets.Select(x => new WorksetFilter(x)).ToList());
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new ElementWorksetFilter(arg.Value);
        }
    }
}
