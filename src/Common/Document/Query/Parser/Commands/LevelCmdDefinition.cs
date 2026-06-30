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
    public class LevelCmdDefinition : ICommandDefinition, INeedInitializationWithDocument, IOfferArgumentAutocompletion
    {
        private static readonly AutocompleteItem AutocompleteItem = new("l: ", "l:[level]", "select elements from a given level", AutocompleteItemGroups.Commands);
        private readonly DataBucket<LevelCmdArgument> dataBucket = new(0.61);


        public void Init(Document document)
        {
            dataBucket.Clear();
            foreach (var element in new FilteredElementCollector(document).OfClass(typeof(Level)))
            {
                dataBucket.Add(new AutocompleteItem(element.Name, element.Name, null), new LevelCmdArgument(element.Id, element.Name), element.Name);
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
            yield return "l";
            yield return "lvl";
            yield return "level";
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
            return new LevelCmd(cmdText, args);
        }
    }


    public class LevelCmdArgument : CommandArgument<ElementId>
    {
        public LevelCmdArgument(ElementId levelId, string name) : base(levelId)
        {
            Name = $"new ElementId({levelId})";
            Label = name;
        }
    }


    public class LevelCmd : Command, ICommandForVisualization
    {
        public string Label => "Level: " + String.Join(", ", Arguments.Select(x => x.Label));
        public string Description => "A filter used to match elements by their associated level. A slow filter.";
        public string APIDescription => "new ElementLevelFilter()";
        public CmdType Type => CmdType.DocumentSpecific;


        public LevelCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null) : base(text, matchedArguments, null)
        {
        }
    }
}