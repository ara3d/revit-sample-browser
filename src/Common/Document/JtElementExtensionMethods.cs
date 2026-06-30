// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Diagnostics;


namespace BuildingCoder
{
    public static class JtElementExtensionMethods
    {
        public static bool IsPhysicalElement(
            this Element e)
        {
            if (e.Category == null)
                return false;
            // does this produce same result as 
            // WhereElementIsViewIndependent ?
            if (e.ViewSpecific)
                return false;
            // exclude specific unwanted categories
            return (BuiltInCategory)e.Category.Id.Value
                    != BuiltInCategory.OST_HVAC_Zones && e.Category.CategoryType == CategoryType.Model
                       && e.Category.CanAddSubcategory;
        }

        public static Curve GetCurve(this Element e)
        {
            Debug.Assert(null != e.Location,
                "expected an element with a valid Location");

            var lc = e.Location as LocationCurve;

            Debug.Assert(null != lc,
                "expected an element with a valid LocationCurve");

            return lc.Curve;
        }
    }
}
