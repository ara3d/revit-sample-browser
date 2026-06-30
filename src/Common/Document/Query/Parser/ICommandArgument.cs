// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Documents;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser
{
    public interface ICommandArgument
    {
        string Name { get;  }
        string Label { get; }       
       
    }

    public abstract class CommandArgument<T> : ICommandArgument
    {
        public T Value { get; init; }
        public string Name { get; init; }
        public string Label { get; init; }     
        


        public CommandArgument(T value)
        {
            Value = value;           
        }
    }
}