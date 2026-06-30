// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.Autocompletion.Internals;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.FuzzySearch;
using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands
{
    public class CategoryCmdDefinition : ICommandDefinition, INeedInitializationWithDocument, IOfferArgumentAutocompletion
    {
        private static readonly AutocompleteItem AutocompleteItem = new("c: ", "c:[category]", "select elements of given category", AutocompleteItemGroups.Commands);
        private readonly DataBucket<CategoryCmdArgument> dataBucket = new(0.59);


        public void Init(Document document)
        {
            dataBucket.Clear();

            var categories = document.Settings.Categories;

            foreach (Category category in categories)
            {
                var builtInCategory = category.BuiltInCategory;
                if (builtInCategory == BuiltInCategory.INVALID) continue;

                var label = LabelUtils.GetLabelFor(builtInCategory);

                var strCategory = builtInCategory.ToString();

                if (!Category.IsBuiltInCategoryValid(builtInCategory))
                {
                    continue;
                }
                dataBucket.Add(new AutocompleteItem(strCategory, strCategory, label), new CategoryCmdArgument(builtInCategory), label, strCategory);
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
            yield return "c";
            yield return "cat";
            yield return "category";
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument)
        {
            return argument.StartsWith(nameof(BuiltInCategory), StringComparison.OrdinalIgnoreCase);
        }
        public bool CanParticipateInGenericSearch()
        {
            return true;
        }

        public ICommand Create(string cmdText, string argument)
        {
            var arg = argument.RemovePrefix("BuiltInCategory.");
            var args = dataBucket.FuzzySearch(arg);
            return new CategoryCmd(cmdText, args);
        }
    }


    public class CategoryCmdArgument : CommandArgument<BuiltInCategory>
    {
        public CategoryCmdArgument(BuiltInCategory value) : base(value)
        {
            Name = $"BuiltInCategory.{value}";
            Label = LabelUtils.GetLabelFor(value);
        }
    }


    public class CategoryCmd : Command, ICommandForVisualization
    {
        public string Label => String.Join(", ", Arguments.Select(x => x.Name));
        public string Description => "A filter used to find elements whose category matches any of a given set of categories.";
        public string APIDescription => "collector.OfCategory() or new ElementMulticategoryFilter()";
        public CmdType Type => CmdType.Category;


        public CategoryCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments) : base(text, matchedArguments, null)
        {
            IsBasedOnQuickFilter = true;
        }
    }
}