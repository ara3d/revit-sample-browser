// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.Common.Documents.Query
{
    public interface IQueryExecutor
    {
        IList<Element> Execute(UIApplication app);
        IList<Element> Execute(Document document, UIDocument uiDocument);
    }
}
