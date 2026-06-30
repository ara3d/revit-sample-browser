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
    public class VisibleInViewCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("visible", "visible", "select visible elements from the active view", AutocompleteItemGroups.Commands);


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;  
                    

        public IEnumerable<string> GetClassifiers()
        {
            yield break;
        }
        public IEnumerable<string> GetKeywords()
        {            
            yield return "visible";
            yield return "visible in view";       
            yield return "visible in active view";
        }
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => false;


        public ICommand Create(string cmdText, string argument)
        {
            return new VisibleInViewCmd(cmdText);
        }
    }


    public class VisibleInViewCmd : Command, ICommandForVisualization
    {
        public string Label => "visible in active view";
        public string Description => "A quick filter that passes elements that are most likely visible in the given view.";
        public string APIDescription => "new VisibleInViewFilter(document, document.ActiveView.Id)";
        public CmdType Type => CmdType.WithoutArgument;


        public VisibleInViewCmd(string text) : base(text, null, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}