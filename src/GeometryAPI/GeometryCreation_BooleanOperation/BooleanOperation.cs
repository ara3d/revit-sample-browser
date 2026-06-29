// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.GeometryAPI.GeometryCreation_BooleanOperation.CS
{
    public static class BooleanOperation
    {
        /// <summary>
        ///     Boolean intersect geometric operation, return a new solid as the result
        /// </summary>
        public static Solid Intersect(this Solid solid1, Solid solid2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Intersect);
        }

        /// <summary>
        ///     Boolean union geometric operation, return a new solid as the result
        /// </summary>
        public static Solid Union(this Solid solid1, Solid solid2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Union);
        }

        /// <summary>
        ///     Boolean difference geometric operation, return a new solid as the result
        /// </summary>
        public static Solid Difference(this Solid solid1, Solid solid2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Difference);
        }

        /// <summary>
        ///     Boolean intersect geometric operation, modify the original solid as the result
        /// </summary>
        public static Solid IntersectSelf(this Solid solid1, Solid solid2)
        {
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, solid2,
                BooleanOperationsType.Intersect);
            return solid1;
        }

        /// <summary>
        ///     Boolean union geometric operation, modify the original solid as the result
        /// </summary>
        public static Solid UnionSelf(this Solid solid1, Solid solid2)
        {
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, solid2,
                BooleanOperationsType.Union);
            return solid1;
        }

        /// <summary>
        ///     Boolean difference geometric operation, modify the original solid as the result
        /// </summary>
        public static Solid DifferenceSelf(this Solid solid1, Solid solid2)
        {
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, solid2,
                BooleanOperationsType.Difference);
            return solid1;
        }
    }
}
