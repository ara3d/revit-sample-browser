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
    public class ElementTypeFilter : Filter
    {
        private enum Version { Element, Type, Both }

        private readonly Version version;

        public ElementTypeFilter(bool isElementTypePresent, bool isNotElementTypePresent)
        {           
            if (isElementTypePresent == true && isNotElementTypePresent == false)
            {
                version = Version.Type;
                FilterSyntax = ".WhereElementIsElementType()";
            }
            if (isElementTypePresent == false && isNotElementTypePresent == true)
            {
                version = Version.Element;                
                FilterSyntax = ".WhereElementIsNotElementType()";
            }
            if (isElementTypePresent == true && isNotElementTypePresent == true)
            {
                version = Version.Both;               
                FilterSyntax = "new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false))";
            }
        }


        public static IEnumerable<Filter> Create(IList<ICommand> commands)
        {
            var isElementTypePresent = commands.OfType<ElementTypeCmd>().Any();
            var isNotElementTypePresent = commands.OfType<NotElementTypeCmd>().Any();
            if (isElementTypePresent || isNotElementTypePresent)
            {
                yield return new ElementTypeFilter(isElementTypePresent, isNotElementTypePresent);
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return version switch
            {
                Version.Element => new ElementIsElementTypeFilter(true),
                Version.Type => new ElementIsElementTypeFilter(false),
                Version.Both => new LogicalOrFilter(new ElementIsElementTypeFilter(true), new ElementIsElementTypeFilter(false)),
            };
        }
    }
}