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
    public class StructuralTypeFilter : Filter
    {
        private readonly StructuralTypeCmdArgument structuralType;


        public StructuralTypeFilter(StructuralTypeCmdArgument structuralType)
        {
            this.structuralType = structuralType;  
            FilterSyntax = $"new ElementStructuralTypeFilter({structuralType.Name})";
        }


        public static IEnumerable<QueryItem> Create(IList<ICommand> commands)
        {
            var structuralTypes = commands.OfType<StructuralTypeCmd>().SelectMany(x => x.Arguments).ToList();
            if (structuralTypes.Count == 1)
            {
                yield return new StructuralTypeFilter(structuralTypes.First());
            }
            if (structuralTypes.Count > 1)
            {
                yield return new Group(structuralTypes.Select(x => new StructuralTypeFilter(x)).ToList());
            }
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            return new ElementStructuralTypeFilter(structuralType.Value);
        }
    }   
}