// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Documents;
using Autodesk.Revit.DB;


namespace BuildingCoder
{
    public static class JtElementIdExtensionMethods
    {
        public static bool IsInvalid(this ElementId id)
        {
            return ElementIdExtensions.IsInvalid(id);
        }

        public static bool IsValid(this ElementId id)
        {
            return ElementIdExtensions.IsValid(id);
        }
    }
}
