// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Filters;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Providers;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Providers.Internals;
using VisibleInViewFilter = Ara3D.RevitSampleBrowser.Common.Documents.Query.Filters.VisibleInViewFilter;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query
{
    public static class RevitDatabaseQueryService
    {
        public static void Init()
        {
            CommandParser.Init();          
        }


        public static Result ParseAndExecute(Document document, string query)
        {
            RevitDatabaseQueryHost.Document = document;
            if (document is null) return new Result(null, new List<ICommand>(), null);

            CommandParser.LoadDocumentSpecificData(document);
            var commands = QueryParser.Parse(query);
            commands.SelectMany(x => x.Arguments).OfType<ParameterArgument>().ToList().ForEach(x => x.ResolveStorageType(document));

            var pipe = new List<QueryItem>();
            pipe.AddRange(VisibleInViewFilter.Create(commands));
            pipe.AddRange(ElementTypeFilter.Create(commands));
            pipe.AddRange(ElementIdFilter.Create(commands));
            pipe.AddRange(ClassFilter.Create(commands));
            pipe.AddRange(CategoryFilter.Create(commands));
            pipe.AddRange(StructuralTypeFilter.Create(commands));
            pipe.AddRange(LevelFilter.Create(commands));
            pipe.AddRange(OwnerViewFilter.Create(commands));
            pipe.AddRange(RoomFilter.Create(commands));
            pipe.AddRange(RuleFilter.Create(commands));
            pipe.AddRange(ParameterFilter.Create(commands));
            pipe.AddRange(Filters.WorksetFilter.Create(commands));

            string providerSyntax = "";
            string collectorSyntax = "";
            QueryPipeExecutor queryExecutor = null;

            var providers = new List<Provider>();
            providers.AddRange(SelectionProvider.Create(commands));
            providers.AddRange(UniqueIdProvider.Create(commands));

            if (pipe.Any())
            {
                var elementIdsSyntax = "";

                if (providers.Any())
                {
                    providerSyntax = "    var elementIds = new List<ElementId>();";

                    foreach (var provider in providers)
                    {
                        providerSyntax += Environment.NewLine + "    " + provider.Syntax;
                    }

                    providerSyntax += Environment.NewLine + Environment.NewLine;
                    elementIdsSyntax = ", elementIds";
                }

                collectorSyntax = $"    return new FilteredElementCollector(document{elementIdsSyntax})";

                foreach (var filter in pipe)
                {
                   collectorSyntax += Environment.NewLine + "    " + filter.CollectorSyntax;                    
                }
                collectorSyntax += Environment.NewLine + "    .ToElements();";

                queryExecutor = new QueryPipeExecutor(pipe, providers);
            }           

            return new Result(providerSyntax + collectorSyntax, commands, queryExecutor);
        }

        public record Result(string GeneratedCSharpSyntax, IList<ICommand> Commands, IQueryExecutor Executor);

        private class QueryPipeExecutor : IQueryExecutor
        {
            private readonly IReadOnlyList<QueryItem> filterPipe;
            private readonly IReadOnlyList<Provider> seedPipe;

            public QueryPipeExecutor(IReadOnlyList<QueryItem> filterPipe, IReadOnlyList<Provider> seedPipe)
            {
                this.filterPipe = filterPipe;
                this.seedPipe = seedPipe;
            }

            public IList<Element> Execute(UIApplication app)
            {
                var uiDocument = app.ActiveUIDocument;
                if (uiDocument?.Document == null)
                    return Array.Empty<Element>();
                return Execute(uiDocument.Document, uiDocument);
            }

            public IList<Element> Execute(Document document, UIDocument uiDocument)
            {
                if (document == null)
                    return Array.Empty<Element>();

                ICollection<ElementId> elementIds = uiDocument != null
                    ? GatherInitialSeed(uiDocument).ToArray()
                    : Array.Empty<ElementId>();

                if (seedPipe.Any() && elementIds.IsEmpty())
                    return Array.Empty<Element>();

                return BuildCollector(document, elementIds).ToElements();
            }

            private IEnumerable<ElementId> GatherInitialSeed(UIDocument uiDocument)
            {
                foreach (var provider in seedPipe)
                {
                    foreach (var id in provider.GetIds(uiDocument))
                    {
                        yield return id;
                    }
                }
            }
            private FilteredElementCollector BuildCollector(Document document, ICollection<ElementId> elementIds)
            {
                FilteredElementCollector collector;
                if (elementIds?.Any() == true)
                {
                    collector = new FilteredElementCollector(document, elementIds);
                }
                else
                {
                    collector = new FilteredElementCollector(document);
                }

                foreach (var filter in filterPipe)
                {
                    var elementFilter = filter.CreateElementFilter(document);
                    if (elementFilter != null)
                    {
                        collector.WherePasses(elementFilter);                        
                    }
                }

                return collector;
            }
        }
    }
}