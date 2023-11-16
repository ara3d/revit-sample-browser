// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Ara3D.RevitSampleBrowser.StairsAutomation.CS.LandingComponents;
using Ara3D.RevitSampleBrowser.StairsAutomation.CS.RunComponents;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    /// <summary>
    ///     A specific implementation of IStairsConfiguration with some default storage included.
    /// </summary>
    public class StairsConfiguration : IStairsConfiguration
    {
        /// <summary>
        ///     The landing configurations.
        /// </summary>
        protected readonly List<IStairsLandingComponent> LandingConfigurations = new List<IStairsLandingComponent>();

        /// <summary>
        ///     The run configurations.
        /// </summary>
        protected readonly List<IStairsRunComponent> RunConfigurations = new List<IStairsRunComponent>();

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public int GetNumberOfRuns()
        {
            return RunConfigurations.Count;
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public void CreateStairsRun(Document document, ElementId stairsElementId, int runIndex)
        {
            RunConfigurations[runIndex].CreateStairsRun(document, stairsElementId);
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public int GetNumberOfLandings()
        {
            return LandingConfigurations.Count;
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public void CreateLanding(Document document, ElementId stairsElementId, int landingIndex)
        {
            LandingConfigurations[landingIndex].CreateLanding(document, stairsElementId);
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public void SetRunWidth(double width)
        {
            foreach (var config in RunConfigurations) config.Width = width;
        }
    }
}
