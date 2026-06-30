// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.Autocompletion.Internals;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.FuzzySearch;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands
{
    public class WorksetCmdDefinition : ICommandDefinition, INeedInitializationWithDocument, IOfferArgumentAutocompletion
    {
        private static readonly AutocompleteItem AutocompleteItem = new("w: ", "w:[workset]", "select elements from a given workset", AutocompleteItemGroups.Commands);
        private readonly DataBucket<WorksetCmdArgument> dataBucket = new(0.61);


        public void Init(Document document)
        {
            dataBucket.Clear();
            foreach (var workset in new FilteredWorksetCollector(document).OfKind(WorksetKind.UserWorkset).ToWorksets())
            {
                dataBucket.Add(new AutocompleteItem(workset.Name, workset.Name, null), new WorksetCmdArgument(workset.Id, workset.Name), workset.Name);
            }
            dataBucket.Rebuild();
        }


        public IAutocompleteItem GetCommandAutocompleteItem()
        {
            return AutocompleteItem;
        }

        public IEnumerable<IAutocompleteItem> GetAutocompleteItems(string prefix)
        {
            return dataBucket.ProvideAutoCompletion(prefix);
        }


        public IEnumerable<string> GetClassifiers()
        {
            yield return "w";
            yield return "wrk";
            yield return "workset";
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument)
        {
            return false;
        }

        public bool CanParticipateInGenericSearch()
        {
            return true;
        }

        public ICommand Create(string cmdText, string argument)
        {
            var args = dataBucket.FuzzySearch(argument);
            return new WorksetCmd(cmdText, args);
        }
    }


    public class WorksetCmdArgument : CommandArgument<WorksetId>
    {
        public WorksetCmdArgument(WorksetId worksetId, string name) : base(worksetId)
        {
            Name = $"new WorksetId({worksetId})";
            Label = name;
        }
    }


    public class WorksetCmd : Command, ICommandForVisualization
    {
        public string Label => "Workset: " + String.Join(", ", Arguments.Select(x => x.Label));
        public string Description => "A filter used to match elements which reside in a given workset.";
        public string APIDescription => "new ElementWorksetFilter()";
        public CmdType Type => CmdType.DocumentSpecific;


        public WorksetCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null) : base(text, matchedArguments, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}