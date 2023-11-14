// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.StairsAutomation.CS
{
    /// <summary>
    ///     A specific implementation of IStairsConfiguration with some default storage included.
    /// </summary>
    public class StairsConfiguration : IStairsConfiguration
    {
        /// <summary>
        ///     The landing configurations.
        /// </summary>
        protected readonly List<IStairsLandingComponent> m_landingConfigurations = new List<IStairsLandingComponent>();

        /// <summary>
        ///     The run configurations.
        /// </summary>
        protected readonly List<IStairsRunComponent> m_runConfigurations = new List<IStairsRunComponent>();

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public int GetNumberOfRuns()
        {
            return m_runConfigurations.Count;
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public void CreateStairsRun(Document document, ElementId stairsElementId, int runIndex)
        {
            m_runConfigurations[runIndex].CreateStairsRun(document, stairsElementId);
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public int GetNumberOfLandings()
        {
            return m_landingConfigurations.Count;
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public void CreateLanding(Document document, ElementId stairsElementId, int landingIndex)
        {
            m_landingConfigurations[landingIndex].CreateLanding(document, stairsElementId);
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public void SetRunWidth(double width)
        {
            foreach (var config in m_runConfigurations) config.Width = width;
        }
    }
}
