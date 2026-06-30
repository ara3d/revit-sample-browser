// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.Autocompletion.Internals;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.FuzzySearch;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands
{
    public class UniqueIdCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new("u: ", "u:[guid] ", "select elements with given UniqueId", AutocompleteItemGroups.Commands);
        private readonly DataBucket<UniqueIdCmdArgument> dataBucket = new(0.666);


        public IAutocompleteItem GetCommandAutocompleteItem()
        {
            return AutocompleteItem;
        }

        public IEnumerable<string> GetClassifiers()
        {
            yield return "u";
            yield return "uid";
            yield return "unique";
            yield return "unique id";
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument)
        {
            if (argument.Length >= 36)
            {
                var guidPart = argument.Substring(0, 36);

                if (Guid.TryParse(guidPart, out var guidValue))
                {
                    return true;
                }
            }
            return false;
        }
        public bool CanParticipateInGenericSearch()
        {
            return false;
        }

        public ICommand Create(string cmdText, string argument)
        {
            return new UniqueIdCmd(cmdText, dataBucket.CreateMatch(new UniqueIdCmdArgument(argument.Trim())));
        }
    }


    public class UniqueIdCmdArgument : CommandArgument<string>
    {
        public UniqueIdCmdArgument(string value) : base(value)
        {
            Name = $"\"{value}\"";
            Label = value;
        }
    }


    public class UniqueIdCmd : Command, ICommandForVisualization
    {
        public string Label => String.Join(", ", Arguments.Select(x => x.Name));
        public string Description => "A filter used to match elements by their UniqueId.";
        public string APIDescription => "document.GetElement()";
        public CmdType Type => CmdType.ElementId;


        public UniqueIdCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null) : base(text, matchedArguments, null)
        {
            IsBasedOnQuickFilter = false;
        }
    }
}