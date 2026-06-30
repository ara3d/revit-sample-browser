// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Documents;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser;
using Ara3D.RevitSampleBrowser.Common.Documents.Query.Parser.Commands;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query.Filters
{
    public class OwnerViewFilter : Filter
    {       
        public OwnerViewFilter()
        {   
            FilterSyntax = "new ElementOwnerViewFilter(document.ActiveView.Id)";
        }


        public static IEnumerable<Filter> Create(IList<ICommand> commands)
        {
            if (commands.OfType<OwnerViewFilterCmd>().Any())
            {
                yield return new OwnerViewFilter();
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new Autodesk.Revit.DB.ElementOwnerViewFilter(document.ActiveView.Id);
        }
    }
}