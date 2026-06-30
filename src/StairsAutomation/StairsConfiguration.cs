// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Ara3D.RevitSampleBrowser.StairsAutomation.CS.LandingComponents;
using Ara3D.RevitSampleBrowser.StairsAutomation.CS.RunComponents;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    public class StairsConfiguration : IStairsConfiguration
    {
        protected readonly List<IStairsLandingComponent> LandingConfigurations = new List<IStairsLandingComponent>();

        protected readonly List<IStairsRunComponent> RunConfigurations = new List<IStairsRunComponent>();

        public int GetNumberOfRuns() => RunConfigurations.Count;

        public void CreateStairsRun(Document document, ElementId stairsElementId, int runIndex) =>
            RunConfigurations[runIndex].CreateStairsRun(document, stairsElementId);

        public int GetNumberOfLandings() => LandingConfigurations.Count;

        public void CreateLanding(Document document, ElementId stairsElementId, int landingIndex) =>
            LandingConfigurations[landingIndex].CreateLanding(document, stairsElementId);

        public void SetRunWidth(double width)
        {
            foreach (var config in RunConfigurations)
            {
                config.Width = width;
            }
        }
    }
}
