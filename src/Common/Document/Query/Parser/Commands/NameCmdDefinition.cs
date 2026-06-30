// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.Autocompletion.Internals;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.FuzzySearch;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands
{
    public class NameCmdDefinition : ICommandDefinition
    {
        public static readonly NameCmdDefinition Instance = new();
        private static readonly AutocompleteItem AutocompleteItem = new("n: ", "n:[text]", "wildcard search for a given text", AutocompleteItemGroups.Commands);
        private readonly DataBucket<ParameterArgument> dataBucket = new(0.69);

        public IAutocompleteItem GetCommandAutocompleteItem()
        {
            return AutocompleteItem;
        }

        public IEnumerable<string> GetClassifiers()
        {
            yield return "n";
            yield return "name";
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
            return false;
        }

        public ICommand Create(string cmdText, string argument)
        {
            var matchedArguments = NameLikeParameters.Select(x => new ParameterArgument(x)).ToArray();

            var @operator = Operators.Parse($"=%{argument}%");
            return new ParameterCmd(cmdText, dataBucket.CreateMatch(matchedArguments), @operator);
        }

        private static readonly List<BuiltInParameter> NameLikeParameters =
        [
            BuiltInParameter.ALL_MODEL_TYPE_NAME,
            BuiltInParameter.ALL_MODEL_MARK,
            BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM,
            BuiltInParameter.DATUM_TEXT
        ];
    }
}