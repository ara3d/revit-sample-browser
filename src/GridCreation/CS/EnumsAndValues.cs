// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

namespace Revit.SDK.Samples.GridCreation.CS
{
    /// <summary>
    ///     An enumerate type listing the ways to create grids.
    /// </summary>
    public enum CreateMode
    {
        /// <summary>
        ///     Create grids with selected lines/arcs
        /// </summary>
        Select,

        /// <summary>
        ///     Create orthogonal grids
        /// </summary>
        Orthogonal,

        /// <summary>
        ///     Create radial and arc grids
        /// </summary>
        RadialAndArc
    }

    /// <summary>
    ///     An enumerate type listing bubble locations of grids.
    /// </summary>
    public enum BubbleLocation
    {
        /// <summary>
        ///     Place bubble at the start point
        /// </summary>
        StartPoint,

        /// <summary>
        ///     Place bubble at the end point
        /// </summary>
        EndPoint
    }

    /// <summary>
    ///     Class contains common const values
    /// </summary>
    internal static class Values
    {
        public const double PI = 3.1415926535897900;

        // ratio from degree to radian
        public const double DEGTORAD = PI / 180;
    }
}
