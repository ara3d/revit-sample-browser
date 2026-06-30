// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace Ara3D.RevitSampleBrowser.Common.Documents
{
    public static class DocumentExtensions
    {
        public static object GetElementOrCategory(this Document document, ElementId id)
        {
            var element = document.GetElement(id);
            if (element != null)
            {
                return element;
            }
           
            var category = Category.GetCategory(document, id);
            if (category != null)
            {
                return category;
            }
            
            return null;
        }
    }
}