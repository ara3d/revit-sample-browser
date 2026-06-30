// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;


namespace BuildingCoder
{
    public static class JtFamilyInstanceExtensionMethods
    {
        public static string GetColumnLocationMark(
            this FamilyInstance f)
        {
            var p = f.get_Parameter(
                    BuiltInParameter.COLUMN_LOCATION_MARK);

            return p == null
                ? string.Empty
                : p.AsString();
        }
    }
}
