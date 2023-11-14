// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.WinderStairs.CS
{
    /// <summary>
    ///     It represents a U-shape winder and consists of three straight runs and two winder corners.
    /// </summary>
    internal class UWinder : Winder
    {
        /// <summary>
        ///     The first Winder Corner, connecting the first and the second straight runs.
        /// </summary>
        private WinderSinglePoint m_corner1st;

        /// <summary>
        ///     The second Winder Corner, connecting the second and the third straight runs.
        /// </summary>
        private WinderSinglePoint m_corner2nd;

        /// <summary>
        ///     The third straight run at end.
        /// </summary>
        private WinderStraight m_straightAtEnd;

        /// <summary>
        ///     The first straight run at start.
        /// </summary>
        private WinderStraight m_straightAtStart;

        /// <summary>
        ///     The second straight run in middle.
        /// </summary>
        private WinderStraight m_straightInMiddle;

        /// <summary>
        ///     Number of straight steps at start.
        /// </summary>
        public uint NumStepsAtStart { get; set; }

        /// <summary>
        ///     Number of straight steps in middle.
        /// </summary>
        public uint NumStepsInMiddle { get; set; }

        /// <summary>
        ///     Number of straight steps at end.
        /// </summary>
        public uint NumStepsAtEnd { get; set; }

        /// <summary>
        ///     Number of winder steps in the first corner.
        /// </summary>
        public uint NumStepsInCorner1 { get; set; }

        /// <summary>
        ///     CenterPoint Offset distance from the first inner boundary line of the first corner.
        /// </summary>
        public double CenterOffsetE1 { get; set; }

        /// <summary>
        ///     CenterPoint Offset distance from the second inner boundary line of the first corner.
        /// </summary>
        public double CenterOffsetF1 { get; set; }

        /// <summary>
        ///     Number of winder steps in the second corner.
        /// </summary>
        public uint NumStepsInCorner2 { get; set; }

        /// <summary>
        ///     CenterPoint Offset distance from the first inner boundary line of the second corner.
        /// </summary>
        public double CenterOffsetE2 { get; set; }

        /// <summary>
        ///     CenterPoint Offset distance from the second inner boundary line of the second corner.
        /// </summary>
        public double CenterOffsetF2 { get; set; }

        /// <summary>
        ///     Four Points to determine the U-shape of the stairs.
        /// </summary>
        public override IList<XYZ> ControlPoints
        {
            get => base.ControlPoints;
            set
            {
                if (value.Count != 4) throw new ArgumentException("The control points count must be 4 for UWinder.");
                base.ControlPoints = value;
            }
        }

        /// <summary>
        ///     This method constructs two winder corners and three straight runs.
        ///     Please be sure the input properties being set properly before calling this method.
        /// </summary>
        private void Construct()
        {
            //
            // Construct the first winder corner
            //
            var dir1 = (ControlPoints[1] - ControlPoints[0]).Normalize();
            var dir2 = (ControlPoints[2] - ControlPoints[1]).Normalize();
            m_corner1st = new WinderSinglePoint(ControlPoints[1], dir1, dir2, NumStepsInCorner1);
            m_corner1st.Construct(RunWidth, CenterOffsetE1, CenterOffsetF1);

            //
            // Construct the second winder corner
            //
            var dir3 = (ControlPoints[3] - ControlPoints[2]).Normalize();
            m_corner2nd = new WinderSinglePoint(ControlPoints[1], dir2, dir3, NumStepsInCorner2);
            m_corner2nd.Construct(RunWidth, CenterOffsetF2, CenterOffsetE2);
            var moveDelta = (m_corner1st.Distance2 + m_corner2nd.Distance1
                                                   + TreadDepth * NumStepsInMiddle) * dir2;
            m_corner2nd.Move(moveDelta);

            //
            // Construct the three straight runs
            //
            var startPnt = m_corner1st.StartPoint - TreadDepth * NumStepsAtStart * dir1;
            var endPnt = m_corner2nd.EndPoint + TreadDepth * NumStepsAtEnd * dir3;
            var bisectDir = (dir2 - dir1).Normalize();
            var perpendicularDir1 = new XYZ(-dir1.Y, dir1.X, 0);
            var perpendicularDir2 = new XYZ(-dir2.Y, dir2.X, 0);
            var perpendicularDir3 = new XYZ(-dir3.Y, dir3.X, 0);
            if (bisectDir.DotProduct(perpendicularDir1) < 0)
            {
                perpendicularDir1 = perpendicularDir1.Negate();
                perpendicularDir2 = perpendicularDir2.Negate();
                perpendicularDir3 = perpendicularDir3.Negate();
            }

            m_straightAtStart = new WinderStraight(
                startPnt, m_corner1st.StartPoint, perpendicularDir1, NumStepsAtStart);
            m_straightInMiddle = new WinderStraight(
                m_corner1st.EndPoint, m_corner2nd.StartPoint, perpendicularDir2, NumStepsInMiddle);
            m_straightAtEnd = new WinderStraight(
                m_corner2nd.EndPoint, endPnt, perpendicularDir3, NumStepsAtEnd);
        }

        /// <summary>
        ///     This method generates the sketch for U-shape winder stairs.
        /// </summary>
        protected override void GenerateSketch()
        {
            // Instantiate two winer corners and three straight runs.
            Construct();

            // Generate the sketch for two winder corners and three straight runs.
            m_straightAtStart.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
            m_corner1st.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
            m_straightInMiddle.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
            m_corner2nd.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
            m_straightAtEnd.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
        }
    }
}
