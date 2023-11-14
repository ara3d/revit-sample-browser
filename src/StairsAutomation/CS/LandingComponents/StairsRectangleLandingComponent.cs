// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace RevitMultiSample.StairsAutomation.CS
{
    /// <summary>
    ///     A configuration for creation of a landing with a fixed cross section.
    /// </summary>
    public class StairsRectangleLandingComponent : IStairsLandingComponent
    {
        private readonly double m_elevation;
        private readonly XYZ m_runDirection;

        private readonly Line m_stairsRunBoundary1;
        private readonly Line m_stairsRunBoundary2;
        private readonly double m_width;

        /// <summary>
        ///     Creates a new StairsRectangleLandingConfiguration.
        /// </summary>
        /// <param name="stairsRunBoundary1">The end curve of the lower stair run.</param>
        /// <param name="stairsRunBoundary2">The start curve of the higher stair run.</param>
        /// <param name="runDirection">A vector representing the direction of the lower stair run.</param>
        /// <param name="elevation">The elevation of the landing.</param>
        /// <param name="width">The width of the landing.</param>
        public StairsRectangleLandingComponent(Line stairsRunBoundary1, Line stairsRunBoundary2, XYZ runDirection,
            double elevation, double width)
        {
            // offset to base elevation
            m_stairsRunBoundary1 = LandingComponentUtils.ProjectCurveToElevation(stairsRunBoundary1, elevation) as Line;
            m_stairsRunBoundary2 = LandingComponentUtils.ProjectCurveToElevation(stairsRunBoundary2, elevation) as Line;

            m_runDirection = runDirection;
            m_width = width;
            m_elevation = elevation;
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public CurveLoop GetLandingBoundary()
        {
            // TODO : What if not collinear 
            var boundaryLine =
                LandingComponentUtils.FindLongestEndpointConnection(m_stairsRunBoundary1, m_stairsRunBoundary2);

            // offset by 1/2 of width
            var centerCurve =
                boundaryLine.CreateTransformed(Transform.CreateTranslation(m_runDirection * (m_width / 2.0)));

            // Create by Thicken
            var curveLoop = CurveLoop.CreateViaThicken(centerCurve, m_width, XYZ.BasisZ);

            return curveLoop;
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public double GetLandingBaseElevation()
        {
            return m_elevation;
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public StairsLanding CreateLanding(Document document, ElementId stairsElementId)
        {
            return StairsLanding.CreateSketchedLanding(document, stairsElementId, GetLandingBoundary(),
                GetLandingBaseElevation());
        }
    }
}
