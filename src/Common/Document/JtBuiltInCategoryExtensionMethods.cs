// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Diagnostics;


namespace BuildingCoder
{
    public static class JtBuiltInCategoryExtensionMethods
    {
        public static string Description(
            this BuiltInCategory bic)
        {
            var s = bic.ToString().ToLower();
            s = s.Substring(4);
            Debug.Assert(s.EndsWith("s"), "expected plural suffix 's'");
            s = s.Substring(0, s.Length - 1);
            return s;
        }
    }
}
