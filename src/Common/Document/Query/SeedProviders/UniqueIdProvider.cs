// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Providers.Internals;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Providers
{
    public class UniqueIdProvider : Provider
    {
        private readonly List<UniqueIdCmdArgument> args;

        public UniqueIdProvider(List<UniqueIdCmdArgument> args)
        {
            this.args = args;
            foreach (var arg in args)
            {
                Syntax += $"elementIds.Add(document.GetElement({arg.Name}).Id);";
            }
        }

        public override IEnumerable<ElementId> GetIds(UIDocument uiDocument)
        {
            foreach (var arg in args)
            {
                var element = uiDocument.Document.GetElement(arg.Value);
                if (element != null)
                {
                    yield return element.Id;
                }
            }
        }


        public static IEnumerable<Provider> Create(IList<ICommand> commands)
        {
            var ids = commands.OfType<UniqueIdCmd>().SelectMany(x => x.Arguments).OfType<UniqueIdCmdArgument>().ToList();
            if (ids.Any())
            {
                yield return new UniqueIdProvider(ids);
            }
        }
    }
}