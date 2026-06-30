// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Providers.Internals;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Providers
{
    public class SelectionProvider : Provider
    {


        public SelectionProvider()
        {
            Syntax = "elementIds.AddRange(uia.ActiveUIDocument.Selection.GetElementIds());";
        }

        public override IEnumerable<ElementId> GetIds(UIDocument uiDocument)
        {
            return uiDocument.Selection.GetElementIds();
        }


        public static IEnumerable<Provider> Create(IList<ICommand> commands)
        {

            if (commands.OfType<SelectionCmd>().Any())
            {
                yield return new SelectionProvider();
            }
        }
    }
}