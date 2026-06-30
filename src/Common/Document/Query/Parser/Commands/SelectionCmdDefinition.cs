// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.Autocompletion.Internals;
using System.Collections.Generic;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands
{
    public class SelectionCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new("selection", "selection", "retrieve the currently selected Elements in Revit", AutocompleteItemGroups.Commands);


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
            yield return "selection";
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
            return new SelectionCmd(cmdText);
        }
    }


    public class SelectionCmd : Command, ICommandForVisualization
    {
        public string Label => "selection in Revit";
        public string Description => "Retrieve the currently selected Elements in Revit.";
        public string APIDescription => "ActiveUIDocument.Selection.GetElementIds()";
        public CmdType Type => CmdType.WithoutArgument;


        public SelectionCmd(string text) : base(text, null, null)
        {
            IsBasedOnQuickFilter = false;
        }
    }
}
