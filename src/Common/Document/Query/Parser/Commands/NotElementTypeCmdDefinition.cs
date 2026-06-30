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
    public class NotElementTypeCmdDefinition : ICommandDefinition
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("element", "element", "select elements", AutocompleteItemGroups.Commands);


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;
             

        public IEnumerable<string> GetClassifiers()
        {
            yield break;
        }
        public IEnumerable<string> GetKeywords()
        {
            yield return "element";
            yield return "elements";
            yield return "not element type";
            yield return "not type";       
        }
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => false;


        public ICommand Create(string cmdText, string argument)
        {
            return new NotElementTypeCmd(cmdText);
        }
    }


    public class NotElementTypeCmd : Command, ICommandForVisualization
    {
        public string Label => "element";
        public string Description => "A filter used to match elements which are not ElementTypes.";
        public string APIDescription => "collector.WhereElementIsNotElementType() or or new ElementIsElementTypeFilter(false)";
        public CmdType Type => CmdType.WithoutArgument;


        public NotElementTypeCmd(string text) : base(text, null, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}