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
    public class RuleBasedFilterCmdDefinition : ICommandDefinition, INeedInitializationWithDocument, IOfferArgumentAutocompletion
    {
        private static readonly AutocompleteItem AutocompleteItem = new("f: ", "f:[filter]", "select elements that pass rule-based filter defined in Revit", AutocompleteItemGroups.Commands);
        private readonly DataBucket<RuleBasedFilterCmdArgument> dataBucket = new(0.61);


        public void Init(Document document)
        {
            dataBucket.Clear();
            foreach (var element in new FilteredElementCollector(document).OfClass(typeof(ParameterFilterElement)))
            {
                dataBucket.Add(new AutocompleteItem(element.Name, element.Name, null), new RuleBasedFilterCmdArgument(element.Id, element.Name), element.Name);

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
            yield return "f";
            yield return "filter";
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
            return new RuleBasedFilterCmd(cmdText, args);
        }
    }


    public class RuleBasedFilterCmdArgument : CommandArgument<ElementId>
    {
        public RuleBasedFilterCmdArgument(ElementId filterId, string name) : base(filterId)
        {
            Name = $"new ElementId({filterId})";
            Label = name;
        }
    }


    public class RuleBasedFilterCmd : Command, ICommandForVisualization
    {
        public string Label => "Rule-based filter: " + String.Join(", ", Arguments.Select(x => x.Label));
        public string Description => "A filter used to match elements which pass rule-based filter defined in Revit.";
        public string APIDescription => "ParameterFilterElement.GetElementFilter()";
        public CmdType Type => CmdType.DocumentSpecific;


        public RuleBasedFilterCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null) : base(text, matchedArguments, null)
        {
        }
    }
}