// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Nice3point.Revit.Extensions;
using Nice3point.Revit.Toolkit.Options;

namespace Ara3D.RevitSampleBrowser.Common.Infrastructure
{
    public static class RevitToolkitSelection
    {
        public static ISelectionFilter ForCategory(BuiltInCategory category)
        {
            return new SelectionConfiguration()
                .Allow.Element(element =>
                    element.Category?.Id != null && element.Category.Id.AreEquals(category))
                .Filter;
        }

        public static ISelectionFilter ForCategories(params BuiltInCategory[] categories)
        {
            return new SelectionConfiguration()
                .Allow.Element(element =>
                {
                    if (element.Category?.Id == null)
                        return false;

                    foreach (var category in categories)
                    {
                        if (element.Category.Id.AreEquals(category))
                            return true;
                    }

                    return false;
                })
                .Filter;
        }

        public static ISelectionFilter ForClass<T>() where T : Element
        {
            return new SelectionConfiguration()
                .Allow.Element(element => element is T)
                .Filter;
        }

        public static SelectionConfiguration Create()
        {
            return new SelectionConfiguration();
        }
    }
}
