// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.WinderStairs.CS.Winders
{
    public class LWinder : Winder
    {
        private WinderSinglePoint m_corner;

        private WinderStraight m_straightAtEnd;

        private WinderStraight m_straightAtStart;

        public uint NumStepsAtStart { get; set; }

        public uint NumStepsAtEnd { get; set; }

        public uint NumStepsInCorner { get; set; }

        public double CenterOffsetE { get; set; }

        public double CenterOffsetF { get; set; }

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
            var startPnt = m_corner.StartPoint - (TreadDepth * NumStepsAtStart * dir1);
            var endPnt = m_corner.EndPoint + (TreadDepth * NumStepsAtEnd * dir2);
            var bisectDir = (dir2 - dir1).Normalize();
            XYZ perpendicularDir1 = new(-dir1.Y, dir1.X, 0);
            XYZ perpendicularDir2 = new(-dir2.Y, dir2.X, 0);
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
