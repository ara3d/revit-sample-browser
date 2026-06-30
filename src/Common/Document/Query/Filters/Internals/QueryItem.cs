// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Ara3D.RevitSampleBrowser.Common.Infrastructure;
using Ara3D.RevitSampleBrowser.Common.Documents;
using System;
using Autodesk.Revit.DB;


namespace Ara3D.RevitSampleBrowser.Common.Documents.Query
{
    public abstract class QueryItem
    {       
        public string FilterSyntax
        {
            get; 
            init;
        } = String.Empty;
        public string CollectorSyntax
        {
            get 
            {
                if (!string.IsNullOrEmpty(FilterSyntax) && !FilterSyntax.StartsWith("."))
                {
                    return $".WherePasses({FilterSyntax})";
                }
                return FilterSyntax;
            }
        }


        public abstract ElementFilter CreateElementFilter(Document document);
    }
}