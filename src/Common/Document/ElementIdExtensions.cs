// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.Common.Documents
{
    public static class ElementIdExtensions
    {
        public static long Value(this ElementId id)
        {
            return id.Value;
        }

        public static bool IsInvalid(this ElementId id)
        {
            return ElementId.InvalidElementId == id;
        }

        public static bool IsValid(this ElementId id)
        {
            return !id.IsInvalid();
        }
    }

    public static class ElementIdFactory
    {
        public static ElementId Create(long id)
        {
            return new ElementId(id);
        }
    }
}
