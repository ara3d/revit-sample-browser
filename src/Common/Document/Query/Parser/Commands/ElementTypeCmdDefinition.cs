// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.Autocompletion.Internals;
using System.Collections.Generic;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands
{
    public class ElementTypeCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new("type", "type", "select element types", AutocompleteItemGroups.Commands);


        public IAutocompleteItem GetCommandAutocompleteItem()
        {
            return AutocompleteItem;
        }

        public IEnumerable<string> GetClassifiers()
        {
            yield break;
        }
        public IEnumerable<string> GetKeywords()
        {
            yield return "type";
            yield return "types";
            yield return "element type";
            yield return "not element";
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
            return new ElementTypeCmd(cmdText);
        }
    }


    public class ElementTypeCmd : Command, ICommandForVisualization
    {
        public string Label => "element type";
        public string Description => "A filter used to match elements which are ElementTypes.";
        public string APIDescription => "collector.WhereElementIsElementType() or new ElementIsElementTypeFilter(true)";
        public CmdType Type => CmdType.WithoutArgument;


        public ElementTypeCmd(string text) : base(text, null, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}