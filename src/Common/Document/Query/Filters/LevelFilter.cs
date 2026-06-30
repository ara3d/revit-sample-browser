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
    public class LevelFilter : Filter
    {
        private readonly LevelCmdArgument levelArg;


        public LevelFilter(LevelCmdArgument level)
        {
            this.levelArg = level;
            FilterSyntax = $"new ElementLevelFilter({level.Name})";
        }


        public static IEnumerable<QueryItem> Create(IList<ICommand> commands)
        {
            var levels = commands.OfType<LevelCmd>().SelectMany(x => x.Arguments).OfType<LevelCmdArgument>().ToList();
            if (levels.Count == 1)
            {
                yield return new LevelFilter(levels.First());
            }
            if (levels.Count > 1)
            {
                yield return new Group(levels.Select(x => new LevelFilter(x)).ToList());
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new ElementLevelFilter(levelArg.Value);
        }
    }
}