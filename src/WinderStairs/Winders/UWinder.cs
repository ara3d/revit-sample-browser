// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.WinderStairs.CS.Winders
{
    public class UWinder : Winder
    {
        private WinderSinglePoint m_corner1St;

        private WinderSinglePoint m_corner2Nd;

        private WinderStraight m_straightAtEnd;

        private WinderStraight m_straightAtStart;

        private WinderStraight m_straightInMiddle;

        public uint NumStepsAtStart { get; set; }

        public uint NumStepsInMiddle { get; set; }

        public uint NumStepsAtEnd { get; set; }

        public uint NumStepsInCorner1 { get; set; }

        public double CenterOffsetE1 { get; set; }

        public double CenterOffsetF1 { get; set; }

        public uint NumStepsInCorner2 { get; set; }

        public double CenterOffsetE2 { get; set; }

        public double CenterOffsetF2 { get; set; }

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
            m_corner1St = new WinderSinglePoint(ControlPoints[1], dir1, dir2, NumStepsInCorner1);
            m_corner1St.Construct(RunWidth, CenterOffsetE1, CenterOffsetF1);

            //
            // Construct the second winder corner
            //
            var dir3 = (ControlPoints[3] - ControlPoints[2]).Normalize();
            m_corner2Nd = new WinderSinglePoint(ControlPoints[1], dir2, dir3, NumStepsInCorner2);
            m_corner2Nd.Construct(RunWidth, CenterOffsetF2, CenterOffsetE2);
            var moveDelta = (m_corner1St.Distance2 + m_corner2Nd.Distance1
                                                   + (TreadDepth * NumStepsInMiddle)) * dir2;
            m_corner2Nd.Move(moveDelta);

            //
            // Construct the three straight runs
            //
            var startPnt = m_corner1St.StartPoint - (TreadDepth * NumStepsAtStart * dir1);
            var endPnt = m_corner2Nd.EndPoint + (TreadDepth * NumStepsAtEnd * dir3);
            var bisectDir = (dir2 - dir1).Normalize();
            XYZ perpendicularDir1 = new(-dir1.Y, dir1.X, 0);
            XYZ perpendicularDir2 = new(-dir2.Y, dir2.X, 0);
            XYZ perpendicularDir3 = new(-dir3.Y, dir3.X, 0);
            if (bisectDir.DotProduct(perpendicularDir1) < 0)
            {
                perpendicularDir1 = perpendicularDir1.Negate();
                perpendicularDir2 = perpendicularDir2.Negate();
                perpendicularDir3 = perpendicularDir3.Negate();
            }

            m_straightAtStart = new WinderStraight(
                startPnt, m_corner1St.StartPoint, perpendicularDir1, NumStepsAtStart);
            m_straightInMiddle = new WinderStraight(
                m_corner1St.EndPoint, m_corner2Nd.StartPoint, perpendicularDir2, NumStepsInMiddle);
            m_straightAtEnd = new WinderStraight(
                m_corner2Nd.EndPoint, endPnt, perpendicularDir3, NumStepsAtEnd);
        }

        protected override void GenerateSketch()
        {
            // Instantiate two winer corners and three straight runs.
            Construct();

            // Generate the sketch for two winder corners and three straight runs.
            m_straightAtStart.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
            m_corner1St.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
            m_straightInMiddle.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
            m_corner2Nd.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
            m_straightAtEnd.GenerateSketch(
                RunWidth, OuterBoundary, CenterWalkpath, InnerBoundary, RiserLines);
        }
    }
}
