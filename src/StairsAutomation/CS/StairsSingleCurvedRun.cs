// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.StairsAutomation.CS.RunComponents;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    /// <summary>
    ///     A stairs configuration representing a single curved run.
    /// </summary>
    /// <remarks>Because this run is based on Spiral runs, runs exceeding 360 degrees are possible.</remarks>
    public class StairsSingleCurvedRun : StairsConfiguration
    {
        /// <summary>
        ///     Creates a new instance of StairsSingleCurvedRun at the default location and orientation.
        /// </summary>
        /// <param name="stairs">The stairs element.</param>
        /// <param name="bottomLevel">The bottom level of the configuration.</param>
        /// <param name="innerRadius">The inner radius of the run curvature.</param>
        public StairsSingleCurvedRun(Stairs stairs, Level bottomLevel, double innerRadius)
        {
            var stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
            RunConfigurations.Add(new CurvedStairsRunComponent(stairs.DesiredRisersNumber, bottomLevel.Elevation,
                stairsType.MinTreadDepth, stairsType.MinRunWidth,
                innerRadius, stairs.Document.Application.Create));
        }

        /// <summary>
        ///     Creates a new instance of StairsSingleCurvedRun at a specified location and orientation.
        /// </summary>
        /// <param name="stairs">The stairs element.</param>
        /// <param name="bottomLevel">The bottom level of the configuration.</param>
        /// <param name="innerRadius">The inner radius of the run curvature.</param>
        /// <param name="transform">The transform (containing location and orientation).</param>
        public StairsSingleCurvedRun(Stairs stairs, Level bottomLevel, double innerRadius, Transform transform)
        {
            var stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
            RunConfigurations.Add(new CurvedStairsRunComponent(stairs.DesiredRisersNumber, bottomLevel.Elevation,
                stairsType.MinTreadDepth, stairsType.MinRunWidth,
                innerRadius, stairs.Document.Application.Create, transform));
        }
    }
}
