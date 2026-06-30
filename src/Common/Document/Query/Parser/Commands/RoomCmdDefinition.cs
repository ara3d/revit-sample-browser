// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Autocompletion.Internals;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.FuzzySearch;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands
{
    public class RoomCmdDefinition : ICommandDefinition, INeedInitializationWithDocument, IOfferArgumentAutocompletion
    {
        private static readonly AutocompleteItem AutocompleteItem = new AutocompleteItem("r: ", "r:[room]", "select elements from a given room", AutocompleteItemGroups.Commands);
        private readonly DataBucket<RoomCmdArgument> dataBucket = new DataBucket<RoomCmdArgument>(0.61);
              

        public void Init(Document document)
        {
            dataBucket.Clear();
            foreach (var room in new FilteredElementCollector(document).WherePasses(new Autodesk.Revit.DB.Architecture.RoomFilter()))
            {
                dataBucket.Add(new AutocompleteItem(room.Name, room.Name, null), new RoomCmdArgument(room.Id, room.Name), room.Name);

            }
            dataBucket.Rebuild();
        }


        public IAutocompleteItem GetCommandAutocompleteItem() => AutocompleteItem;
        public IEnumerable<IAutocompleteItem> GetAutocompleteItems(string prefix)
        {
            return dataBucket.ProvideAutoCompletion(prefix);
        }


        public IEnumerable<string> GetClassifiers()
        {
            yield return "r";
            yield return "room";        
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument) => false;
        public bool CanParticipateInGenericSearch() => true;


        public ICommand Create(string cmdText, string argument)
        {
            var args = dataBucket.FuzzySearch(argument);
            return new RoomCmd(cmdText, args);
        }
    }


    public class RoomCmdArgument : CommandArgument<ElementId>
    {
        public RoomCmdArgument(ElementId roomId, string name) : base(roomId)
        {
          
            Name = $"new ElementId({roomId})";
            Label = name;
        }
    }


    public class RoomCmd : Command, ICommandForVisualization
    {
        public string Label => "Room: " + String.Join(", ", Arguments.Select(x => x.Label));
        public string Description => "A filter used to match elements which are inside a given room shell/solid.  A slow filter.";
        public string APIDescription => "new ElementIntersectsSolidFilter()";
        public CmdType Type => CmdType.DocumentSpecific;


        public RoomCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null) : base(text, matchedArguments, null)
        {
        }
    }
}