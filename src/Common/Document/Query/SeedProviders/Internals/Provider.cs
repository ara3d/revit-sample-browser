// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Documents;
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Providers.Internals
{
    public abstract class Provider
    {
        public string Syntax
        {
            get;
            init;
        } = String.Empty;


        public abstract IEnumerable<ElementId> GetIds(UIDocument uiDocument);
    }
}