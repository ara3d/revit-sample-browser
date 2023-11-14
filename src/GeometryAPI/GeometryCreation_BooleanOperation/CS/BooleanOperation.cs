// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.GeometryCreation_BooleanOperation.CS
{
    internal static class BooleanOperation
    {
        /// <summary>
        ///     Boolean intersect geometric operation, return a new solid as the result
        /// </summary>
        /// <param name="solid1">Operation solid 1</param>
        /// <param name="solid2">Operation solid 2</param>
        /// <returns>The operation result</returns>
        public static Solid BooleanOperation_Intersect(Solid solid1, Solid solid2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Intersect);
        }

        /// <summary>
        ///     Boolean union geometric operation, return a new solid as the result
        /// </summary>
        /// <param name="solid1">Operation solid 1</param>
        /// <param name="solid2">Operation solid 2</param>
        /// <returns>The operation result</returns>
        public static Solid BooleanOperation_Union(Solid solid1, Solid solid2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Union);
        }

        /// <summary>
        ///     Boolean difference geometric operation, return a new solid as the result
        /// </summary>
        /// <param name="solid1">Operation solid 1</param>
        /// <param name="solid2">Operation solid 2</param>
        /// <returns>The operation result</returns>
        public static Solid BooleanOperation_Difference(Solid solid1, Solid solid2)
        {
            return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Difference);
        }

        /// <summary>
        ///     Boolean intersect geometric operation, modify the original solid as the result
        /// </summary>
        /// <param name="solid1">Operation solid 1 and operation result</param>
        /// <param name="solid2">Operation solid 2</param>
        public static void BooleanOperation_Intersect(ref Solid solid1, Solid solid2)
        {
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, solid2,
                BooleanOperationsType.Intersect);
        }

        /// <summary>
        ///     Boolean union geometric operation, modify the original solid as the result
        /// </summary>
        /// <param name="solid1">Operation solid 1 and operation result</param>
        /// <param name="solid2">Operation solid 2</param>
        public static void BooleanOperation_Union(ref Solid solid1, Solid solid2)
        {
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, solid2,
                BooleanOperationsType.Union);
        }

        /// <summary>
        ///     Boolean difference geometric operation, modify the original solid as the result
        /// </summary>
        /// <param name="solid1">Operation solid 1 and operation result</param>
        /// <param name="solid2">Operation solid 2</param>
        public static void BooleanOperation_Difference(ref Solid solid1, Solid solid2)
        {
            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(solid1, solid2,
                BooleanOperationsType.Difference);
        }
    }
}
