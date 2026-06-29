// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.AreaReinParameters.CS
{
    /// <summary>
    ///     Layout Rules possible values of AreaReinforcement
    /// </summary>
    public enum LayoutRules
    {
        FixedNumber = 2,
        MaximumSpacing = 3
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
        TowardsExterior = 0,
        TowardsInterior = 2
    }
}
