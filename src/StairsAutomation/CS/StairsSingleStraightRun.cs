// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.StairsAutomation.CS.RunComponents;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    /// <summary>
    ///     A stairs configuration representing a single straight run.
    /// </summary>
    public class StairsSingleStraightRun : StairsConfiguration
    {
        /// <summary>
        ///     Creates a new instance of StairsSingleStraightRun at the default location and orientation.
        /// </summary>
        /// <param name="stairs">The stairs element.</param>
        /// <param name="bottomLevel">The bottom level of the configuration.</param>
        public StairsSingleStraightRun(Stairs stairs, Level bottomLevel)
        {
            var stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
            RunConfigurations.Add(new StraightStairsRunComponent(stairs.DesiredRisersNumber, bottomLevel.Elevation,
                stairsType.MinTreadDepth, stairsType.MinRunWidth));
        }

        /// <summary>
        ///     Creates a new instance of StairsSingleStraightRun at a given location and orientation.
        /// </summary>
        /// <param name="stairs">The stairs element.</param>
        /// <param name="bottomLevel">The bottom level of the configuration.</param>
        /// <param name="transform">The transform (containing location and orientation).</param>
        public StairsSingleStraightRun(Stairs stairs, Level bottomLevel, Transform transform)
        {
            var stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
            RunConfigurations.Add(new StraightStairsRunComponent(stairs.DesiredRisersNumber, bottomLevel.Elevation,
                stairsType.MinTreadDepth, stairsType.MinRunWidth, transform));
        }
    }
}
