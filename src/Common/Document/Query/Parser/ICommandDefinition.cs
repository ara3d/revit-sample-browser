// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.Autocompletion.Internals;
using Autodesk.Revit.DB;
using System.Collections.Generic;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser
{
    public interface ICommandDefinition : IAmCommandFactory, IOfferCommandAutocompletion
    {
        bool CanRecognizeArgument(string argument);
        bool CanParticipateInGenericSearch();
        IEnumerable<string> GetClassifiers();
        IEnumerable<string> GetKeywords();
    }

    public interface INeedInitialization
    {
        void Init();
    }
    public interface INeedInitializationWithDocument
    {
        void Init(Document document);
    }

    public interface IAmCommandFactory
    {
        ICommand Create(string cmdText, string argument);
    }

    public interface IOfferCommandAutocompletion
    {
        IAutocompleteItem GetCommandAutocompleteItem();
    }
    public interface IOfferArgumentAutocompletion
    {
        IEnumerable<IAutocompleteItem> GetAutocompleteItems(string prefix);
    }
}