// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    /// <summary>
    ///     A stairs configuration that represents a single curved run (which will be made in Revit as a sketched run).
    /// </summary>
    /// <remarks>Because this is a sketched run, a included angle of greater than 360 degrees will not succeed.</remarks>
    internal class StairsSingleSketchedCurvedRun : StairsConfiguration
    {
        /// <summary>
        ///     Creates a new instance of StairsSingleSketchedCurvedRun at the default location and orientation.
        /// </summary>
        /// <param name="stairs">The stairs element.</param>
        /// <param name="bottomLevel">The bottom level of the configuration.</param>
        /// <param name="innerRadius">The inner radius of the run curvature.</param>
        public StairsSingleSketchedCurvedRun(Stairs stairs, Level bottomLevel, double innerRadius)
        {
            var stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
            RunConfigurations.Add(new SketchedCurvedStairsRunComponent(stairs.DesiredRisersNumber,
                bottomLevel.Elevation,
                stairsType.MinTreadDepth, stairsType.MinRunWidth,
                innerRadius, stairs.Document.Application.Create));
        }

        /// <summary>
        ///     Creates a new instance of StairsSingleSketchedCurvedRun at a specified location and orientation.
        /// </summary>
        /// <param name="stairs">The stairs element.</param>
        /// <param name="bottomLevel">The bottom level of the configuration.</param>
        /// <param name="innerRadius">The inner radius of the run curvature.</param>
        /// <param name="transform">The transform (containing location and orientation).</param>
        public StairsSingleSketchedCurvedRun(Stairs stairs, Level bottomLevel, double innerRadius, Transform transform)
        {
            var stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
            RunConfigurations.Add(new SketchedCurvedStairsRunComponent(stairs.DesiredRisersNumber,
                bottomLevel.Elevation, stairsType.MinTreadDepth,
                stairsType.MinRunWidth, innerRadius,
                stairs.Document.Application.Create,
                transform));
        }
    }
}
