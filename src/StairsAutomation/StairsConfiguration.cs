// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.StairsAutomation.CS.LandingComponents;
using Ara3D.RevitSampleBrowser.StairsAutomation.CS.RunComponents;
using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    public class StairsConfiguration : IStairsConfiguration
    {
        protected readonly List<IStairsLandingComponent> LandingConfigurations = [];

        protected readonly List<IStairsRunComponent> RunConfigurations = [];

        public int GetNumberOfRuns()
        {
            return RunConfigurations.Count;
        }

        public void CreateStairsRun(Document document, ElementId stairsElementId, int runIndex)
        {
            RunConfigurations[runIndex].CreateStairsRun(document, stairsElementId);
        }

        public int GetNumberOfLandings()
        {
            return LandingConfigurations.Count;
        }

        public void CreateLanding(Document document, ElementId stairsElementId, int landingIndex)
        {
            LandingConfigurations[landingIndex].CreateLanding(document, stairsElementId);
        }

        public void SetRunWidth(double width)
        {
            foreach (var config in RunConfigurations)
            {
                config.Width = width;
            }
        }
    }
}
