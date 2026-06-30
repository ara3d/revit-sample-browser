using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Ara3D.Bowerbird.RevitSamples;

public static class CategoryExtensions
{
    public static IReadOnlyList<Category> GetAllCategories(
        this Document rootDoc,
        bool includeSubCategories = true)
    {
        if (rootDoc == null) throw new ArgumentNullException(nameof(rootDoc));

        List<Category> result = [];

        void AddCategoryTree(Category cat)
        {
            if (cat == null) return;
            result.Add(cat);

            if (!includeSubCategories) return;

            foreach (Category sub in cat.SubCategories ?? [])
                AddCategoryTree(sub);
        }

        foreach (Category cat in rootDoc.Settings.Categories)
            AddCategoryTree(cat);
        return result;
    }
}