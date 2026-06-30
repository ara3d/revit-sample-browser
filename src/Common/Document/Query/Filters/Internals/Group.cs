// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query
{
    public enum LogicalOperator { And, Or }


    public class Group : QueryItem
    {      
        public LogicalOperator Operator { get; init; } = LogicalOperator.Or;
        public IEnumerable<QueryItem> Items { get; init; } = Enumerable.Empty<QueryItem>();


        public Group(IEnumerable<QueryItem> items, LogicalOperator logicalOperator = LogicalOperator.Or)
        {
            Operator = logicalOperator;
            Items = items;

            var filterName = logicalOperator == LogicalOperator.Or ? "LogicalOrFilter" : "LogicalAndFilter";
                          
            FilterSyntax = $"new {filterName}(new [] {{" + String.Join(", ", Items.Select(x => Environment.NewLine + "        " + x.FilterSyntax)) + Environment.NewLine + "    })";
        }

        public override ElementFilter CreateElementFilter(Document document)
        {
            if (Operator == LogicalOperator.Or)
            {
                return new LogicalOrFilter(Items.Select(x => x.CreateElementFilter(document)).ToList());
            }
            else
            {
                return new LogicalAndFilter(Items.Select(x => x.CreateElementFilter(document)).ToList());
            }
        }
    }
}