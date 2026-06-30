// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS.RunComponents
{
    public class StraightStairsRunComponent : TransformedStairsComponent, IStairsRunComponent
    {
        private double m_desiredTreadDepth;

        private int m_riserNumber;
        private readonly XYZ m_runOffset;
        private StairsRun m_stairsRun;
        private double m_width;
        private XYZ m_widthOffset;

        public StraightStairsRunComponent(int riserNumber, double bottomElevation, double desiredTreadDepth,
            double width)
        {
            m_riserNumber = riserNumber;
            RunElevation = bottomElevation;
            m_desiredTreadDepth = desiredTreadDepth;
            m_width = width;
            m_runOffset = new XYZ(0, (riserNumber - 1) * desiredTreadDepth, 0);
            m_widthOffset = new XYZ(m_width, 0, 0);
        }

        public StraightStairsRunComponent(int riserNumber, double bottomElevation, double desiredTreadDepth,
            double width,
            Transform transform) :
            base(transform)
        {
            m_riserNumber = riserNumber;
            RunElevation = bottomElevation;
            m_desiredTreadDepth = desiredTreadDepth;
            m_width = width;
            m_runOffset = new XYZ(0, (riserNumber - 1) * desiredTreadDepth, 0);
            m_widthOffset = new XYZ(m_width, 0, 0);
        }

        public double RunElevation { get; }

        public double TopElevation
        {
            get
            {
                if (m_stairsRun == null) throw new NotSupportedException("Stairs run hasn't been constructed yet.");
                return m_stairsRun.TopElevation;
            }
        }

        public IList<Curve> GetStairsPath()
        {
            if (m_stairsRun == null) throw new NotSupportedException("Stairs run hasn't been constructed yet.");
            var curveLoop = m_stairsRun.GetStairsPath();
            return curveLoop.ToList();
        }

        public Curve GetFirstCurve()
        {
            return GetEndCurve(false);
        }

        public Curve GetLastCurve()
        {
            return GetEndCurve(true);
        }

        public XYZ GetRunEndpoint()
        {
            return GetLastCurve().GetEndPoint(0);
        }

        public StairsRun CreateStairsRun(Document document, ElementId stairsId)
        {
            m_stairsRun = StairsRun.CreateStraightRun(document, stairsId,
                Transform(GetRunStairsPath()), StairsRunJustification.Center);
            Width = m_width;
            document.Regenerate(); // to get updated width
            return m_stairsRun;
        }

        public double Width
        {
            get => m_width;
            set
            {
                m_width = value;
                if (m_stairsRun != null) m_stairsRun.ActualRunWidth = m_width;
            }
        }

        public double GetRunElevation()
        {
            return RunElevation;
        }

        public Line GetRunStairsPath()
        {
            var start = new XYZ(m_width / 2.0, 0, RunElevation);
            var end = start + m_runOffset;
            var curve1 = Line.CreateBound(start, end);

            return curve1;
        }

        private Curve GetEndCurve(bool last)
        {
            if (m_stairsRun == null) throw new NotSupportedException("Stairs run hasn't been constructed yet.");

            // Obtain the footprint boundary of the run.
            var boundary = m_stairsRun.GetFootprintBoundary();

            // Find the first or last point on the path
            var path = m_stairsRun.GetStairsPath();
            var pathCurve = path.First();
            var pathPoint = pathCurve.GetEndPoint(last ? 1 : 0);

            // Walk the footprint boundary, and look for a curve whose projection of the target point is equal to the point.
            foreach (var boundaryCurve in boundary)
            {
                if (boundaryCurve.Project(pathPoint).XYZPoint.IsAlmostEqualTo(pathPoint))
                    return boundaryCurve;
            }

            throw new Exception("Unable to find an intersecting boundary curve in the run.");
        }
    }
}
