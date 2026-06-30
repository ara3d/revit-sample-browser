// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Documents;
using Autodesk.Revit.DB;

using System.Collections.Generic;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Autocompletion.Internals;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands
{
    public class OwnerViewFilterCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("owned", "owned", "select elements which are owned by a active view", AutocompleteItemGroups.Commands);


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;


        public IEnumerable<string> GetClassifiers()
        {
            yield break;
        }
        public IEnumerable<string> GetKeywords()
        {
            yield return "owned";
            yield return "owned by view";
            yield return "owned by active view";
        }
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => false;


        public ICommand Create(string cmdText, string argument)
        {
            return new OwnerViewFilterCmd(cmdText);
        }
    }


    public class OwnerViewFilterCmd : Command, ICommandForVisualization
    {
        public string Label => "owned by active view";
        public string Description => "A filter used to match elements which are owned by a particular view.";
        public string APIDescription => "new ElementOwnerViewFilter(document.ActiveView.Id)";
        public CmdType Type => CmdType.WithoutArgument;


        public OwnerViewFilterCmd(string text) : base(text, null, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}