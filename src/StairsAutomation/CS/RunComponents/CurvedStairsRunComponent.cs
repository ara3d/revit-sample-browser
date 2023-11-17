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
    ///     A stairs component consisting of a single curved run.
    /// </summary>
    public class CurvedStairsRunComponent : TransformedStairsComponent, IStairsRunComponent
    {
        private Application m_appCreate;
        private readonly XYZ m_center;
        private readonly double m_desiredTreadDepth;
        private readonly double m_includedAngle;
        private double m_incrementAngle;
        private readonly double m_innerRadius;
        private double m_outerRadius;

        private int m_riserNumber;
        private StairsRun m_stairsRun;

        /// <summary>
        ///     Creates a new CurvedStairsRunConfiguration at the default location and orientation.
        /// </summary>
        /// <param name="riserNumber">The number of risers in the run.</param>
        /// <param name="bottomElevation">The bottom elevation.</param>
        /// <param name="desiredTreadDepth">The desired tread depth.</param>
        /// <param name="width">The width.</param>
        /// <param name="innerRadius">The radius of the innermost edge of the run.</param>
        /// <param name="appCreate">The Revit API application creation object.</param>
        public CurvedStairsRunComponent(int riserNumber, double bottomElevation,
            double desiredTreadDepth, double width,
            double innerRadius, Application appCreate)
        {
            m_riserNumber = riserNumber;
            RunElevation = bottomElevation;
            m_innerRadius = innerRadius;
            m_outerRadius = innerRadius + width;
            m_incrementAngle = desiredTreadDepth / (m_innerRadius + width / 2.0);
            m_desiredTreadDepth = desiredTreadDepth;
            m_includedAngle = m_incrementAngle * (riserNumber - 1);

            m_center = new XYZ(0, 0, bottomElevation);
            m_appCreate = appCreate;
        }

        /// <summary>
        ///     Creates a new CurvedStairsRunConfiguration at the specified location and orientation.
        /// </summary>
        /// <param name="riserNumber">The number of risers in the run.</param>
        /// <param name="bottomElevation">The bottom elevation.</param>
        /// <param name="desiredTreadDepth">The desired tread depth.</param>
        /// <param name="width">The width.</param>
        /// <param name="innerRadius">The radius of the innermost edge of the run.</param>
        /// <param name="appCreate">The Revit API application creation object.</param>
        /// <param name="transform">The transformation (location and orientation).</param>
        public CurvedStairsRunComponent(int riserNumber, double bottomElevation,
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
            m_desiredTreadDepth = desiredTreadDepth;
            m_center = new XYZ(0, 0, bottomElevation);
            m_appCreate = appCreate;
        }

        /// <summary>
        ///     Implements the interface property.
        /// </summary>
        private double Radius => (m_innerRadius + m_outerRadius) / 2.0;

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
            if (m_stairsRun == null) throw new NotSupportedException("Stairs run hasn't been constructed yet.");
            var curveLoop = m_stairsRun.GetStairsPath();
            return curveLoop.ToList();
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public Curve GetFirstCurve()
        {
            return GetEndCurve(false);
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public Curve GetLastCurve()
        {
            return GetEndCurve(true);
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public XYZ GetRunEndpoint()
        {
            return GetLastCurve().GetEndPoint(0);
        }

        /// <summary>
        ///     Implements the interface method.
        /// </summary>
        public StairsRun CreateStairsRun(Document document, ElementId stairsId)
        {
            m_stairsRun = StairsRun.CreateSpiralRun(document, stairsId, TransformPoint(m_center),
                Radius, 0, m_includedAngle, true, StairsRunJustification.Center);
            Width = m_outerRadius - m_innerRadius;
            document.Regenerate(); // to get updated width
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
                m_outerRadius = value + m_innerRadius;
                if (m_stairsRun != null)
                {
                    m_stairsRun.ActualRunWidth = m_outerRadius - m_innerRadius;
                    m_incrementAngle = m_desiredTreadDepth / (m_innerRadius + value / 2.0);
                }
            }
        }

        /// <summary>
        ///     Gets the first or last riser curve of the run.
        /// </summary>
        /// <param name="last">True to get the last curve, false to get the first.</param>
        /// <returns>The curve.</returns>
        private Curve GetEndCurve(bool last)
        {
            if (m_stairsRun == null) throw new NotSupportedException("Stairs run hasn't been constructed yet.");

            // Obtain the footprint boundary
            var boundary = m_stairsRun.GetFootprintBoundary();

            // Obtain the endpoint of the stairs path matching the desired end,
            // and find out which curve contains this point.
            var path = m_stairsRun.GetStairsPath();
            var pathCurve = path.First();
            var pathPoint = pathCurve.GetEndPoint(last ? 1 : 0);

            foreach (var boundaryCurve in boundary)
                if (boundaryCurve.Project(pathPoint).XYZPoint.IsAlmostEqualTo(pathPoint))
                    return boundaryCurve;

            throw new Exception("Unable to find an intersecting boundary curve in the run.");
        }
    }
}
