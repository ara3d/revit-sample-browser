// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.StairsAutomation.CS.RunComponents;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    public class StairsSingleStraightRun : StairsConfiguration
    {
        public StairsSingleStraightRun(Stairs stairs, Level bottomLevel)
        {
            var stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
            RunConfigurations.Add(new StraightStairsRunComponent(stairs.DesiredRisersNumber, bottomLevel.Elevation,
                stairsType.MinTreadDepth, stairsType.MinRunWidth));
        }

        public StairsSingleStraightRun(Stairs stairs, Level bottomLevel, Transform transform)
        {
            var stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
            RunConfigurations.Add(new StraightStairsRunComponent(stairs.DesiredRisersNumber, bottomLevel.Elevation,
                stairsType.MinTreadDepth, stairsType.MinRunWidth, transform));
        }
    }
}
