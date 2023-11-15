// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    /// <summary>
    ///     A stairs configuration that represents a single straight run (which will be made in Revit as a sketched run).
    /// </summary>
    public class StairsSingleSketchedStraightRun : StairsConfiguration
    {
        /// <summary>
        ///     Creates a new instance of StairsSingleSketchedStraightRun at the default location and orientation.
        /// </summary>
        /// <param name="stairs">The stairs element.</param>
        /// <param name="bottomLevel">The bottom level of the configuration.</param>
        public StairsSingleSketchedStraightRun(Stairs stairs, Level bottomLevel)
        {
            var stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
            RunConfigurations.Add(new SketchedStraightStairsRunComponent(stairs.DesiredRisersNumber,
                bottomLevel.Elevation,
                stairsType.MinTreadDepth, stairsType.MinRunWidth));
        }

        /// <summary>
        ///     Creates a new instance of StairsSingleSketchedStraightRun at a given location and orientation.
        /// </summary>
        /// <param name="stairs">The stairs element.</param>
        /// <param name="bottomLevel">The bottom level of the configuration.</param>
        /// <param name="transform">The transform (containing location and orientation).</param>
        public StairsSingleSketchedStraightRun(Stairs stairs, Level bottomLevel, Transform transform)
        {
            var stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
            RunConfigurations.Add(new SketchedStraightStairsRunComponent(stairs.DesiredRisersNumber,
                bottomLevel.Elevation,
                stairsType.MinTreadDepth, stairsType.MinRunWidth,
                transform));
        }
    }
}
