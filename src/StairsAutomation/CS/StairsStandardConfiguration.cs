// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Document = Autodesk.Revit.DB.Document;

namespace Revit.SDK.Samples.StairsAutomation.CS
{
    /// <summary>
    ///     Represents a stairs configuration consisting of straight runs and rectangular landings.  The runs will
    ///     switch based upon the input parameters.  The landings width can be adjusted independently.
    /// </summary>
    public class StairsStandardConfiguration : IStairsConfiguration
    {
        private readonly double m_bottomElevation;
        private readonly List<IStairsLandingComponent> m_landingConfigurations = new List<IStairsLandingComponent>();
        private double m_landingWidth;
        private bool m_landingWidthOverride;
        private readonly int m_numberOfLandings;
        private int m_riserIncrement = 1;
        private readonly List<IStairsRunComponent> m_runConfigurations = new List<IStairsRunComponent>();
        private double m_runOffset;
        private bool m_runOffsetOverride;

        private double m_runWidth;
        private bool m_runWidthOverride;
        private readonly Stairs m_stairs;
        private readonly StairsType m_stairsType;
        private readonly Transform m_transform;

        /// <summary>
        ///     Creates a new StairsStandardConfiguration of runs and landings at the default location and orientation.
        /// </summary>
        /// <param name="stairs">The stairs element.</param>
        /// <param name="bottomLevel">The bottom level.</param>
        /// <param name="numberOfLandings">The number of landings to be included.</param>
        public StairsStandardConfiguration(Stairs stairs, Level bottomLevel, int numberOfLandings)
        {
            m_stairs = stairs;
            m_stairsType = m_stairs.Document.GetElement(m_stairs.GetTypeId()) as StairsType;
            m_bottomElevation = bottomLevel.Elevation;
            RiserNumber = stairs.DesiredRisersNumber;
            m_numberOfLandings = numberOfLandings;
            m_transform = Transform.Identity;
            EqualizeRuns = false;
        }

        /// <summary>
        ///     Creates a new StairsStandardConfiguration of runs and landings at a specified location and orientation.
        /// </summary>
        /// <param name="stairs">The stairs element.</param>
        /// <param name="bottomLevel">The bottom level.</param>
        /// <param name="numberOfLandings">The number of landings to be included.</param>
        /// <param name="transform">The transformation (location and orientation).</param>
        public StairsStandardConfiguration(Stairs stairs, Level bottomLevel, int numberOfLandings, Transform transform)
        {
            m_stairs = stairs;
            m_stairsType = m_stairs.Document.GetElement(m_stairs.GetTypeId()) as StairsType;
            m_bottomElevation = bottomLevel.Elevation;
            RiserNumber = stairs.DesiredRisersNumber;
            m_numberOfLandings = numberOfLandings;
            m_transform = transform;
            EqualizeRuns = false;
        }

        /// <summary>
        ///     The number of risers to include in the stairs.
        /// </summary>
        public int RiserNumber { get; set; }

        /// <summary>
        ///     True to ensure that all runs have the same number of treads (which might require adjustment to the tread height to
        ///     compensate for the number of runs).  False to allow a different number of treads (which practically, might result
        ///     in
        ///     some runs getting one less tread than others).
        /// </summary>
        public bool EqualizeRuns { get; set; }

        /// <summary>
        ///     The width of the landings.  If not explicitly set, the width of the runs is used.
        /// </summary>
        public double LandingWidth
        {
            get
            {
                if (m_landingWidthOverride) return m_landingWidth;
                return RunWidth;
            }
            set
            {
                m_landingWidth = value;
                m_landingWidthOverride = true;
            }
        }

        /// <summary>
        ///     The width of the runs.  If not explicitly set, the stairs type default width is used.
        /// </summary>
        public double RunWidth
        {
            get
            {
                return m_runWidthOverride ? m_runWidth : m_stairsType.MinRunWidth;
            }
            set
            {
                m_runWidth = value;
                m_runWidthOverride = true;
            }
        }

        /// <summary>
        ///     The offset between the corner of one run and the corner of the next during a switchback at a landing.
        /// </summary>
        public double RunOffset
        {
            get
            {
                return m_runOffsetOverride ? m_runOffset : 2.0;
            }
            set
            {
                m_runOffset = value;
                m_runOffsetOverride = true;
            }
        }

        /// <summary>
        ///     A helper to obtain the Autodesk.Revit.Creation.Application handle.
        /// </summary>
        protected Application AppCreate => m_stairs.Document.Application.Create;

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public int GetNumberOfRuns()
        {
            return m_numberOfLandings + 1;
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public int GetNumberOfLandings()
        {
            return m_numberOfLandings;
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public void CreateStairsRun(Document document, ElementId stairsElementId, int runIndex)
        {
            // Calculate where the previous run ended.
            var previousRunEndPoint = runIndex == 0 ? XYZ.Zero : m_runConfigurations[runIndex - 1].GetRunEndpoint();
            var elevation = runIndex == 0 ? m_bottomElevation : m_runConfigurations[runIndex - 1].TopElevation;

            // Setup number of risers for the run.
            var runNumberOfRisers = m_riserIncrement;
            if (runIndex == m_numberOfLandings) runNumberOfRisers = RiserNumber - m_numberOfLandings * m_riserIncrement;

            // Setup the transform for the run.  Every second run must be reversed in direction and start point generated from
            // the offet to the previous run.
            var transform = Transform.Identity;
            var pivotPoint = previousRunEndPoint + m_transform.OfPoint(new XYZ(RunWidth + RunOffset, 0, 0));
            if (runIndex % 2 == 1)
            {
                var translation = Transform.CreateTranslation(pivotPoint);
                var rotation = Transform.CreateRotationAtPoint(XYZ.BasisZ, Math.PI, pivotPoint);
                transform = rotation.Multiply(translation);
            }

            transform = transform.Multiply(m_transform);

            // Generate the run configuration and it.
            var configuration = GenerateRun(runNumberOfRisers, elevation,
                m_stairsType.MinTreadDepth, RunWidth, transform);

            m_runConfigurations.Add(configuration);

            // Create the run now (subsequent runs and landings need to use its geometric properties).
            configuration.CreateStairsRun(document, stairsElementId);
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public void CreateLanding(Document document, ElementId stairsElementId, int landingIndex)
        {
            // Get the run configurations for the runs before and after this landing.
            var configuration1 = m_runConfigurations[landingIndex];
            var configuration2 = m_runConfigurations[landingIndex + 1];

            // Get the stairs path from the lower run for run direction.
            var curve = configuration1.GetStairsPath()[0];
            var runDirection = (curve.GetEndPoint(1) - curve.GetEndPoint(0)).Normalize();

            // Generate the landing configuration
            var configuration = GenerateLanding(configuration1.GetLastCurve() as Line,
                configuration2.GetFirstCurve() as Line,
                runDirection,
                configuration2.RunElevation);
            m_landingConfigurations.Add(configuration);

            // Create the landing now
            configuration.CreateLanding(document, stairsElementId);
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public void SetRunWidth(double value)
        {
            //cache width for new runs.
            RunWidth = value;

            //assign to existing run configurations
            foreach (var config in m_runConfigurations) config.Width = value;
        }

        /// <summary>
        ///     Initializes the run and landing data in the configuration.
        /// </summary>
        public void Initialize()
        {
            if (m_numberOfLandings < 0)
                throw new ArgumentOutOfRangeException("numberOfLandings", "Number of landings must be 0 or more");

            if (m_numberOfLandings > RiserNumber)
                throw new ArgumentOutOfRangeException("numberOfLandings",
                    "Number of landings must be less than calculated riser number for the stairs");

            if (EqualizeRuns)
            {
                var remainder = RiserNumber % (m_numberOfLandings + 1);
                if (remainder != 0)
                {
                    m_stairs.DesiredRisersNumber = m_stairs.DesiredRisersNumber + m_numberOfLandings + 1 - remainder;
                    m_stairs.Document.Regenerate();
                    RiserNumber = m_stairs.DesiredRisersNumber;
                }
            }

            // Split as evenly as possible
            m_riserIncrement = RiserNumber / (m_numberOfLandings + 1);
        }

        /// <summary>
        ///     Generates the landing configuration for the end lines of 2 risers.
        /// </summary>
        /// <param name="riser1Line">The end line of the first riser.</param>
        /// <param name="riser2Line">The start line of the second riser.</param>
        /// <param name="runDirection">The run direction.</param>
        /// <param name="landingElevation">The elevation.</param>
        /// <returns>The landing configuration.</returns>
        public virtual IStairsLandingComponent GenerateLanding(Line riser1Line, Line riser2Line, XYZ runDirection,
            double landingElevation)
        {
            IStairsLandingComponent landingConfiguration =
                new StairsRectangleLandingComponent(riser1Line, riser2Line, runDirection, landingElevation,
                    LandingWidth);
            return landingConfiguration;
        }

        /// <summary>
        ///     Generates the run configuration for the given elevation and properties.
        /// </summary>
        /// <param name="numberOfRisers">The number of risers in the run.</param>
        /// <param name="elevation">The start elevation.</param>
        /// <param name="minTreadDepth">The minimum tread depth.</param>
        /// <param name="runWidth">The width of the run.</param>
        /// <param name="transform">The transformation applied to the run start point and orientation.</param>
        /// <returns></returns>
        public virtual IStairsRunComponent GenerateRun(int numberOfRisers, double elevation, double minTreadDepth,
            double runWidth, Transform transform)
        {
            IStairsRunComponent run = new StraightStairsRunComponent(numberOfRisers, elevation,
                minTreadDepth, runWidth, transform);
            return run;
        }

        /// <summary>
        ///     Returns the transform assigned to the configuration.
        /// </summary>
        /// <returns>The transform.</returns>
        protected Transform GetTransform()
        {
            return m_transform;
        }
    }
}
