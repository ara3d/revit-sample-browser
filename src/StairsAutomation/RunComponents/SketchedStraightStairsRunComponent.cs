// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS.RunComponents
{
    public class SketchedStraightStairsRunComponent : TransformedStairsComponent, IStairsRunComponent
    {
        private readonly double m_desiredTreadDepth;

        private readonly int m_riserNumber;
        private readonly XYZ m_runExtent;
        private StairsRun m_stairsRun;
        private double m_width;
        private readonly XYZ m_widthOffset;

        /// <summary>
        ///     Creates a new sketched run configuration at the default location and orientation.
        /// </summary>
        /// <remarks>Stairs run boundary curves can be calculated before the run is created.</remarks>
        /// <param name="riserNumber">The number of risers.</param>
        /// <param name="bottomElevation">The bottom elevation.</param>
        /// <param name="desiredTreadDepth">The desired tread depth.</param>
        /// <param name="width">The width of the run.</param>
        public SketchedStraightStairsRunComponent(int riserNumber, double bottomElevation, double desiredTreadDepth,
            double width)
        {
            m_riserNumber = riserNumber;
            RunElevation = bottomElevation;
            m_desiredTreadDepth = desiredTreadDepth;
            m_width = width;
            m_runExtent = new XYZ(0, (riserNumber - 1) * desiredTreadDepth, 0);
            m_widthOffset = new XYZ(m_width, 0, 0);
        }

        public SketchedStraightStairsRunComponent(int riserNumber, double bottomElevation, double desiredTreadDepth,
            double width, Transform transform) :
            base(transform)
        {
            m_riserNumber = riserNumber;
            RunElevation = bottomElevation;
            m_desiredTreadDepth = desiredTreadDepth;
            m_width = width;
            // This is the full extent of the run across all treads
            m_runExtent = new XYZ(0, (riserNumber - 1) * desiredTreadDepth, 0);
            m_widthOffset = new XYZ(m_width, 0, 0);
        }

        public double RunElevation { get; }

        public double TopElevation => m_stairsRun == null ? throw new NotSupportedException("Stairs run hasn't been constructed yet.") : m_stairsRun.TopElevation;

        public IList<Curve> GetStairsPath()
        {
            // Proceed up the middle of the run to the run extent
            XYZ start = new(m_width / 2.0, 0, RunElevation);
            var end = start + m_runExtent;
            var curve1 = Line.CreateBound(start, end);

            List<Curve> ret = [curve1];
            return Transform(ret);
        }

        public Curve GetFirstCurve()
        {
            return GenerateRunRiserCurves().First();
        }

        public Curve GetLastCurve()
        {
            return GenerateRunRiserCurves().Last();
        }

        public XYZ GetRunEndpoint()
        {
            return GetLastCurve().GetEndPoint(1);
        }

        public StairsRun CreateStairsRun(Document document, ElementId stairsId)
        {
            m_stairsRun = StairsRun.CreateSketchedRun(document, stairsId, GetRunElevation(),
                GetRunBoundaryCurves(), GenerateRunRiserCurves(),
                GetStairsPath());
            document.Regenerate();

            return m_stairsRun;
        }

        public double Width
        {
            get => m_width;
            set
            {
                if (m_stairsRun != null) throw new InvalidOperationException("Cannot change width of sketched run.");
                m_width = value;
            }
        }

        public double GetRunElevation()
        {
            return RunElevation;
        }

        public IList<Curve> GetRunBoundaryCurves()
        {
            return Transform(GenerateUntransformedRunBoundaryCurves());
        }

        private IList<Curve> GenerateUntransformedRunBoundaryCurves()
        {
            // Start at 0, 0 and extend to the run extent
            XYZ start = new(0, 0, RunElevation);
            var end = start + m_runExtent;
            var curve1 = Line.CreateBound(start, end);

            List<Curve> ret = [curve1];

            // Start offset along the width and extend to the run extent
            start = new XYZ(m_width, 0, RunElevation);
            end = start + m_runExtent;
            var curve2 = Line.CreateBound(start, end);
            ret.Add(curve2);

            return ret;
        }

        private IList<Curve> GenerateRunRiserCurves()
        {
            List<Curve> ret = [];

            // Generate all curves but the last by incrementing along the tread depth
            for (var i = 0; i < m_riserNumber - 1; i++)
            {
                XYZ start = new(0, i * m_desiredTreadDepth, RunElevation);
                var end = start + m_widthOffset;

                ret.Add(Line.CreateBound(start, end));
            }

            // last one handled manually to ensure that it intersects the endpoints of the stairs curves
            // Untransformed curves are used because transformation happens below
            var boundaryCurves = GenerateUntransformedRunBoundaryCurves();

            ret.Add(Line.CreateBound(boundaryCurves[0].Evaluate(1.0, true), boundaryCurves[1].Evaluate(1.0, true)));

            return Transform(ret);
        }
    }
}
