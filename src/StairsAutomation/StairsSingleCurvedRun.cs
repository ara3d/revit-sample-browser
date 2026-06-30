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
        public StairsSingleCurvedRun(Stairs stairs, Level bottomLevel, double innerRadius)
        {
            var stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
            RunConfigurations.Add(new CurvedStairsRunComponent(stairs.DesiredRisersNumber, bottomLevel.Elevation,
                stairsType.MinTreadDepth, stairsType.MinRunWidth,
                innerRadius, stairs.Document.Application.Create));
        }

        public StairsSingleCurvedRun(Stairs stairs, Level bottomLevel, double innerRadius, Transform transform)
        {
            var stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
            RunConfigurations.Add(new CurvedStairsRunComponent(stairs.DesiredRisersNumber, bottomLevel.Elevation,
                stairsType.MinTreadDepth, stairsType.MinRunWidth,
                innerRadius, stairs.Document.Application.Create, transform));
        }
    }
}
