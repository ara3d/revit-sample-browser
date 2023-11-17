// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Document = Autodesk.Revit.DB.Document;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS.RunComponents
{
    /// <summary>
    ///     A stairs run consisting of a single sketched curved run.
    /// </summary>
    /// <remarks>Because this is sketched, the maximum covered angle for the stair is 360 degrees.</remarks>
    public class SketchedCurvedStairsRunComponent : TransformedStairsComponent, IStairsRunComponent
    {
        private Application m_appCreate;
        private readonly XYZ m_center;
        private readonly double m_includedAngle;
        private readonly double m_incrementAngle;
        private readonly double m_innerRadius;
        private double m_outerRadius;

        private readonly int m_riserNumber;
        private StairsRun m_stairsRun;

        /// <summary>
        ///     Creates a new SketchedCurvedStairsRunConfiguration at the default location and orientation.
        /// </summary>
        /// <param name="riserNumber">The number of risers in the run.</param>
        /// <param name="bottomElevation">The bottom elevation.</param>
        /// <param name="desiredTreadDepth">The desired tread depth.</param>
        /// <param name="width">The width.</param>
        /// <param name="innerRadius">The radius of the innermost edge of the run.</param>
        /// <param name="appCreate">The Revit API application creation object.</param>
        public SketchedCurvedStairsRunComponent(int riserNumber, double bottomElevation,
            double desiredTreadDepth, double width,
            double innerRadius, Application appCreate)
        {
            m_riserNumber = riserNumber;
            RunElevation = bottomElevation;
            m_innerRadius = innerRadius;
            m_outerRadius = innerRadius + width;
            m_incrementAngle = desiredTreadDepth / (m_innerRadius + width / 2.0);
            m_includedAngle = m_incrementAngle * (riserNumber - 1);
            if (m_includedAngle > 2 * Math.PI)
                throw new Exception("Arguments provided require an included angle of more than 360 degrees");

            m_center = new XYZ(0, 0, bottomElevation);
            m_appCreate = appCreate;
        }

        /// <summary>
        ///     Creates a new SketchedCurvedStairsRunConfiguration at the specified location and orientation.
        /// </summary>
        /// <param name="riserNumber">The number of risers in the run.</param>
        /// <param name="bottomElevation">The bottom elevation.</param>
        /// <param name="desiredTreadDepth">The desired tread depth.</param>
        /// <param name="width">The width.</param>
        /// <param name="innerRadius">The radius of the innermost edge of the run.</param>
        /// <param name="appCreate">The Revit API application creation object.</param>
        /// <param name="transform">The transformation (location and orientation).</param>
        public SketchedCurvedStairsRunComponent(int riserNumber, double bottomElevation,
            double desiredTreadDepth, double width,
            double innerRadius, Application appCreate,
            Transform transform) :
            base(transform)
        {
            m_riserNumber = riserNumber;
            RunElevation = bottomElevation;
            m_innerRadius = innerRadius;
            m_outerRadius = innerRadius + width;
            m_incrementAngle = desiredTreadDepth / (m_innerRadius + width / 2.0);
            m_includedAngle = m_incrementAngle * (riserNumber - 1);
            if (m_includedAngle > 2 * Math.PI)
                throw new Exception("Arguments provided require an included angle of more than 360 degrees");

            m_center = new XYZ(0, 0, bottomElevation);
            m_appCreate = appCreate;
        }

        /// <summary>
        ///     Implements the interface property.
        /// </summary>
        public double RunElevation { get; }

        /// <summary>
        ///     Implements the interface property.
        /// </summary>
        public double TopElevation
        {
            get
            {
                if (m_stairsRun == null) throw new NotSupportedException("Stairs run hasn't been constructed yet.");
                return m_stairsRun.TopElevation;
            }
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public IList<Curve> GetStairsPath()
        {
            var ret = new List<Curve>();
            var arc = Arc.Create(m_center, (m_innerRadius + m_outerRadius) / 2.0, 0, m_includedAngle, XYZ.BasisX,
                XYZ.BasisY);
            ret.Add(arc);

            return ret;
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public Curve GetFirstCurve()
        {
            return GetRunRiserCurves().First();
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public Curve GetLastCurve()
        {
            return GetRunRiserCurves().Last();
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public XYZ GetRunEndpoint()
        {
            return GetRunBoundaryCurves()[1].GetEndPoint(1);
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public StairsRun CreateStairsRun(Document document, ElementId stairsId)
        {
            m_stairsRun = StairsRun.CreateSketchedRun(document, stairsId, GetRunElevation(),
                Transform(GetRunBoundaryCurves()), Transform(GetRunRiserCurves()),
                Transform(GetStairsPath()));
            document.Regenerate();
            return m_stairsRun;
        }

        /// <summary>
        ///     Implements the interface property.
        /// </summary>
        public double Width
        {
            get => m_outerRadius - m_innerRadius;
            set
            {
                if (m_stairsRun != null)
                    throw new InvalidOperationException("Cannot change width of already existing sketched run.");
                m_outerRadius = value + m_innerRadius;
            }
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public double GetRunElevation()
        {
            return RunElevation;
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public IList<Curve> GetRunRiserCurves()
        {
            var incAngle = 0.0;
            var center = XYZ.Zero;
            var ret = new List<Curve>();

            // Run riser curves are linear and radial with respect to the center of curvature.
            for (var i = 0; i < m_riserNumber - 1; i++)
            {
                var start = center + new XYZ(m_innerRadius * Math.Cos(incAngle), m_innerRadius * Math.Sin(incAngle), 0);
                var end = center + new XYZ(m_outerRadius * Math.Cos(incAngle), m_outerRadius * Math.Sin(incAngle), 0);
                ret.Add(Line.CreateBound(start, end));

                incAngle += m_incrementAngle;
            }

            // last one handled manually to ensure that it intersects end curves
            var boundaryCurves = GetRunBoundaryCurves();

            ret.Add(Line.CreateBound(boundaryCurves[0].Evaluate(1.0, true), boundaryCurves[1].Evaluate(1.0, true)));

            return ret;
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public IList<Curve> GetRunBoundaryCurves()
        {
            var ret = new List<Curve>();
            var arc = Arc.Create(m_center, m_innerRadius, 0, m_includedAngle, XYZ.BasisX, XYZ.BasisY);
            ret.Add(arc);

            arc = Arc.Create(m_center, m_outerRadius, 0, m_includedAngle, XYZ.BasisX, XYZ.BasisY);
            ret.Add(arc);

            return ret;
        }
    }
}
