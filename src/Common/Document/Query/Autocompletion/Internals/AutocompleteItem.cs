// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Autocompletion.Internals
{
    public class AutocompleteItem : IAutocompleteItem
    {
        public string Label { get; init; }
        public string TextToInsert { get; init; }
        public string Description { get; init; }
        public bool IsChosenOne { get; set; }
        public string GroupName { get; init; }

        public AutocompleteItem(string textToInsert, string label, string description, string group = null)
        {
            Label = label;
            TextToInsert = textToInsert;
            Description = description;
            GroupName = group;
        }
    }
}