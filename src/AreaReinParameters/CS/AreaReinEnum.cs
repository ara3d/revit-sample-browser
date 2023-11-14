// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace Revit.SDK.Samples.AreaReinParameters.CS
{
    /// <summary>
    ///     Layout Rules possible values of AreaReinforcement
    /// </summary>
    public enum LayoutRules
    {
        Fixed_Number = 2,
        Maximum_Spacing = 3
    }

    /// <summary>
    ///     Hook Orientation possible values of AreaReinforcement which is on a floor
    /// </summary>
    public enum FloorHookOrientations
    {
        Up = 0,
        Down = 2
    }

    /// <summary>
    ///     Hook Orientation possible values of AreaReinforcement which is on a wall
    /// </summary>
    public enum WallHookOrientations
    {
        Towards_Exterior = 0,
        Towards_Interior = 2
    }
}
