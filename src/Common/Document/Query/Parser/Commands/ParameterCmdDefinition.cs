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
    public class ParameterCmdDefinition : ICommandDefinition, INeedInitialization, INeedInitializationWithDocument, IOfferArgumentAutocompletion
    {
        private static readonly AutocompleteItem AutocompleteItem = new("p: ", "p:[parametr] = [value]", "search for a parameter (value)", AutocompleteItemGroups.Commands);
        private readonly DataBucket<ParameterArgument> dataBucket = new(0.69);
        private readonly DataBucket<ParameterArgument> dataBucketForUser = new(0.67);

        public void Init()
        {
            var ids = Autodesk.Revit.DB.ParameterUtils.GetAllBuiltInParameters()
                .Select(x => Autodesk.Revit.DB.ParameterUtils.GetBuiltInParameter(x)).ToList();
            foreach (var param in ids)
            {
                var label = LabelUtils.GetLabelFor(param);
                var strParam = param.ToString();

                if (param == BuiltInParameter.INVALID)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(label))
                {
                    continue;
                }


                dataBucket.Add(new AutocompleteItem(strParam, $"{strParam} ({(long)param})", label, AutocompleteItemGroups.BuiltInParameter), new ParameterArgument(param), label, strParam);
            }
            dataBucket.Rebuild();
        }
        public void Init(Document document)
        {
            dataBucketForUser.Clear();
            foreach (var userParam in new FilteredElementCollector(document).OfClass(typeof(ParameterElement)))
            {
                var group = AutocompleteItemGroups.ProjectParameter;
                if (userParam is SharedParameterElement)
                {
                    group = AutocompleteItemGroups.SharedParameter;
                }
                dataBucketForUser.Add(new AutocompleteItem(userParam.Name, userParam.Name, null, group), new ParameterArgument(userParam.Id, userParam.Name), userParam.Name);
            }
            dataBucketForUser.Rebuild();
        }


        public IAutocompleteItem GetCommandAutocompleteItem()
        {
            return AutocompleteItem;
        }

        public IEnumerable<IAutocompleteItem> GetAutocompleteItems(string prefix)
        {
            return dataBucketForUser.ProvideAutoCompletion(prefix).Union(dataBucket.ProvideAutoCompletion(prefix));
        }


        public IEnumerable<string> GetClassifiers()
        {
            yield return "p";
        }
        public IEnumerable<string> GetKeywords()
        {
            yield break;
        }
        public bool CanRecognizeArgument(string argument)
        {
            return argument.StartsWith(nameof(BuiltInParameter), StringComparison.OrdinalIgnoreCase) || Operators.DoesContainAnyValidOperator(argument);
        }
        public bool CanParticipateInGenericSearch()
        {
            return false;
        }

        public ICommand Create(string cmdText, string argument)
        {
            var @operator = Operators.Parse(argument);
            var leftSide = Operators.GetLeftSideOfOperator(argument);
            var bareArgument = leftSide.RemovePrefix("BuiltInParameter.");
            var argsBIP = dataBucket.FuzzySearch(bareArgument);
            var argsUser = dataBucketForUser.FuzzySearch(bareArgument);
            var args = argsBIP.Union(argsUser);

            return new ParameterCmd(cmdText, args, @operator);
        }
    }


    public class ParameterCmd : Command, ICommandForVisualization
    {
        public string Label
        {
            get
            {
                var arguments = Arguments.OfType<ParameterArgument>();
                var firstArg = arguments?.FirstOrDefault();

                if (firstArg != null)
                {
                    var count = "";
                    if (arguments.Count() > 1)
                    {
                        count = $" [+{arguments.Count() - 1} more]";
                    }

                    var name = firstArg?.Name;
                    if (!firstArg.IsBuiltInParameter)
                    {
                        name = firstArg.Label;
                    }

                    var finalName = $"{name}{count} {Operator.ToLabel(firstArg.StorageType, firstArg.DataType)}";

                    return finalName;
                }

                return "";
            }
        }
        public string Description => "A filter used to match elements by one or more parameter filter rules. A slow filter.";
        public string APIDescription => @"new ElementParameterFilter() 

Operators that can be used : !=, <>, >=, <=, =, <, >
?? - HasNoValue, parameter exists but has no value
!! - HasValue, paramater exists and has value
?! - Exists, element has given parameter, with or without value";
        public CmdType Type => CmdType.Parameter;


        public ParameterCmd(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null, OperatorWithArgument @operator = null) : base(text, matchedArguments, @operator)
        {
        }
    }
}