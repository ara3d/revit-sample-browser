// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS.LandingComponents
{
    public class StairsRectangleLandingComponent : IStairsLandingComponent
    {
        private readonly double m_elevation;
        private readonly XYZ m_runDirection;

        private readonly Line m_stairsRunBoundary1;
        private readonly Line m_stairsRunBoundary2;
        private readonly double m_width;

        public StairsRectangleLandingComponent(Line stairsRunBoundary1, Line stairsRunBoundary2, XYZ runDirection,
            double elevation, double width)
        {
            // offset to base elevation
            m_stairsRunBoundary1 = CurveGeometry.ProjectCurveToElevation(stairsRunBoundary1, elevation) as Line;
            m_stairsRunBoundary2 = CurveGeometry.ProjectCurveToElevation(stairsRunBoundary2, elevation) as Line;

            m_runDirection = runDirection;
            m_width = width;
            m_elevation = elevation;
        }

        public CurveLoop GetLandingBoundary()
        {
            // TODO : What if not collinear 
            var boundaryLine = CurveGeometry.FindLongestEndpointConnection(m_stairsRunBoundary1, m_stairsRunBoundary2);

            // offset by 1/2 of width
            var centerCurve =
                boundaryLine.CreateTransformed(Transform.CreateTranslation(m_runDirection * (m_width / 2.0)));

            // Create by Thicken
            var curveLoop = CurveLoop.CreateViaThicken(centerCurve, m_width, XYZ.BasisZ);

            return curveLoop;
        }

        public double GetLandingBaseElevation()
        {
            return m_elevation;
        }

        public StairsLanding CreateLanding(Document document, ElementId stairsElementId)
        {
            return StairsLanding.CreateSketchedLanding(document, stairsElementId, GetLandingBoundary(), GetLandingBaseElevation());
        }
    }
}
