// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.WinderStairs.CS.Winders
{
    /// <summary>
    ///     It represents an L-shape winder and consists of two straight runs and one winder corner.
    /// </summary>
    internal class LWinder : Winder
    {
        /// <summary>
        ///     Winder Corner, connecting the first and the second straight runs.
        /// </summary>
        private WinderSinglePoint m_corner;

        /// <summary>
        ///     The second straight run at end.
        /// </summary>
        private WinderStraight m_straightAtEnd;

        /// <summary>
        ///     The first straight run at start.
        /// </summary>
        private WinderStraight m_straightAtStart;

        /// <summary>
        ///     Number of straight steps at start.
        /// </summary>
        public uint NumStepsAtStart { get; set; }

        /// <summary>
        ///     Number of straight steps at end.
        /// </summary>
        public uint NumStepsAtEnd { get; set; }

        /// <summary>
        ///     Number of winder steps in corner.
        /// </summary>
        public uint NumStepsInCorner { get; set; }

        /// <summary>
        ///     CenterPoint Offset distance from the first inner boundary line of the corner.
        /// </summary>
        public double CenterOffsetE { get; set; }

        /// <summary>
        ///     CenterPoint Offset distance from the second inner boundary line of the corner.
        /// </summary>
        public double CenterOffsetF { get; set; }

        /// <summary>
        ///     Three points to determine the L-shape of the stairs.
        /// </summary>
        public override IList<XYZ> ControlPoints
        {
            get => base.ControlPoints;
            set
            {
                if (value.Count != 3) throw new ArgumentException("The control points count must be 3 for LWinder.");
                base.ControlPoints = value;
            }
        }

        /// <summary>
        ///     This method constructs the winder corner and two straight runs.
        ///     Please be sure the input properties being set properly before calling this method.
        /// </summary>
        private void Construct()
        {
            //
            // Construct the winder corner.
            //
            var dir1 = (ControlPoints[1] - ControlPoints[0]).Normalize();
            var dir2 = (ControlPoints[2] - ControlPoints[1]).Normalize();
            m_corner = new WinderSinglePoint(ControlPoints[1], dir1, dir2, NumStepsInCorner);
            m_corner.Construct(RunWidth, CenterOffsetE, CenterOffsetF);

            //
            // Construct two straight runs to connect to the winder corner.
            //
            var startPnt = m_corner.StartPoint - TreadDepth * NumStepsAtStart * dir1;
            var endPnt = m_corner.EndPoint + TreadDepth * NumStepsAtEnd * dir2;
            var bisectDir = (dir2 - dir1).Normalize();
            var perpendicularDir1 = new XYZ(-dir1.Y, dir1.X, 0);
            var perpendicularDir2 = new XYZ(-dir2.Y, dir2.X, 0);
            if (bisectDir.DotProduct(perpendicularDir1) < 0)
            {
                perpendicularDir1 = perpendicularDir1.Negate();
                perpendicularDir2 = perpendicularDir2.Negate();
            }

            m_straightAtStart = new WinderStraight(
                startPnt, m_corner.StartPoint, perpendicularDir1, NumStepsAtStart);
            m_straightAtEnd = new WinderStraight(
                m_corner.EndPoint, endPnt, perpendicularDir2, NumStepsAtEnd);
        }

        /// <summary>
        ///     This method generates the sketch for L-shape winder stairs.
        /// </summary>
        protected override void GenerateSketch()
        {
            // Instantiate the corner and the two straight runs.
            Construct();

            // Generate sketch for winder corner and straight runs.
            m_straightAtStart.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
            m_corner.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
            m_straightAtEnd.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
        }
    }
}
