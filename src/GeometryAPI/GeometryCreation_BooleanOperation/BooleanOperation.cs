// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.GeometryCreation_BooleanOperation.CS
{
    public static class BooleanOperation
    {
        public static Solid Intersect(this Solid solid1, Solid solid2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Intersect);
        }

        public static Solid Union(this Solid solid1, Solid solid2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Union);
        }

        public static Solid Difference(this Solid solid1, Solid solid2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Difference);
        }

        public static Solid IntersectSelf(this Solid solid1, Solid solid2)
        {
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, solid2,
                BooleanOperationsType.Intersect);
            return solid1;
        }

        public static Solid UnionSelf(this Solid solid1, Solid solid2)
        {
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, solid2,
                BooleanOperationsType.Union);
            return solid1;
        }

        public static Solid DifferenceSelf(this Solid solid1, Solid solid2)
        {
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, solid2,
                BooleanOperationsType.Difference);
            return solid1;
        }
    }
}
