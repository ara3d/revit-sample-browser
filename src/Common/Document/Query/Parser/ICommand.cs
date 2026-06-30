// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.FuzzySearch;
using System.Collections.Generic;
using System.Linq;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser
{
    public interface ICommand
    {
        string Text { get; }
        IEnumerable<IFuzzySearchResult> MatchedArguments { get; }
        OperatorWithArgument Operator { get; }
        bool IsBasedOnQuickFilter { get; init; }

        IEnumerable<ICommandArgument> Arguments { get; }

        double Score { get; }
    }

    public interface ICommandForVisualization
    {
        public string Label { get; }
        public string Description { get; }
        public string APIDescription { get; }
        CmdType Type { get; }
    }


    public class Command : ICommand
    {
        public string Text { get; init; } = "";
        public IEnumerable<IFuzzySearchResult> MatchedArguments { get; init; } = Enumerable.Empty<IFuzzySearchResult>();
        public OperatorWithArgument Operator { get; init; } = new OperatorWithArgument();
        public bool IsBasedOnQuickFilter { get; init; } = false;


        public double Score => MatchedArguments.Any() ? MatchedArguments.First().LevensteinScore : 0;

        public IEnumerable<ICommandArgument> Arguments => MatchedArguments.Select(x => x.Argument);


        public Command(string text, IEnumerable<IFuzzySearchResult> matchedArguments = null, OperatorWithArgument @operator = null)
        {
            Text = text;
            MatchedArguments = matchedArguments?.ToArray() ?? MatchedArguments;
            Operator = @operator ?? Operator;
        }
    }
}