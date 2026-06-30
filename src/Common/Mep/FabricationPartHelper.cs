// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt


using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.Common.Mep
{
    public static class FabricationPartHelper
    {
        public static bool IsADuct(FabricationPart fabPart)
        {
            return fabPart != null && fabPart.Category.BuiltInCategory == BuiltInCategory.OST_FabricationDuctwork;
        }

        public static bool IsAPipe(FabricationPart fabPart)
        {
            return fabPart != null && fabPart.Category.BuiltInCategory == BuiltInCategory.OST_FabricationPipework;
        }
    }
}