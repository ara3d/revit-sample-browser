// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.WinderStairs.CS
{
    /// <summary>
    ///     It represents a straight run connected to winder-corner.
    /// </summary>
    internal class WinderStraight
    {
        /// <summary>
        ///     Constructor to initialize the basic fields of the straight run.
        /// </summary>
        /// <param name="start">Start point</param>
        /// <param name="end">End point</param>
        /// <param name="offsetDir">Offset direction</param>
        /// <param name="numSteps">Number of steps</param>
        public WinderStraight(XYZ start, XYZ end, XYZ offsetDir, uint numSteps)
        {
            StartPoint = start;
            EndPoint = end;
            NumSteps = numSteps;
            OffsetDirection = offsetDir;
        }
        /*
         *   (StartPoint)-------->--------(EndPoint)
         *                       |
         *                       |
         *                       V (OffsetDirection)
         *                       |
         *                       |
         *               -----------------
         * 
         */

        /// <summary>
        ///     Start delimiter of the straight run.
        /// </summary>
        public XYZ StartPoint { get; }

        /// <summary>
        ///     End delimiter of the straight run.
        /// </summary>
        public XYZ EndPoint { get; }

        /// <summary>
        ///     Perpendicular direction of start-to-end direction.
        /// </summary>
        public XYZ OffsetDirection { get; }

        /// <summary>
        ///     Number of steps in this straight run.
        /// </summary>
        public uint NumSteps { get; }

        /// <summary>
        ///     Generate sketch of the straight run.
        /// </summary>
        public void GenerateSketch(double runWidth,
            IList<Curve> outerBoundary, IList<Curve> walkPath,
            IList<Curve> innerBoundary, IList<Curve> riserLines)
        {
            if (NumSteps == 0)
            {
                // Just Generate one riser line.
                var middleOffset = StartPoint + OffsetDirection * runWidth * 0.5;
                var xOuter = middleOffset - OffsetDirection * runWidth * 0.5;
                var xInner = middleOffset + OffsetDirection * runWidth * 0.5;
                riserLines.Add(Line.CreateBound(xOuter, xInner));
                return;
            }

            // For NumSteps > 0:
            //
            // Generate the outer boundary line.
            var runwidth_2 = runWidth * 0.5;
            var outBounary = Line.CreateBound(StartPoint, EndPoint);
            outerBoundary.Add(outBounary);

            // Generate the middle walk path line.
            var middleStart = StartPoint + OffsetDirection * runWidth * 0.5;
            var middleEnd = EndPoint + OffsetDirection * runwidth_2;
            walkPath.Add(Line.CreateBound(middleStart, middleEnd));

            // Generate the inner boundary line.
            var innerStart = StartPoint + OffsetDirection * runWidth;
            var innerEnd = EndPoint + OffsetDirection * runWidth;
            innerBoundary.Add(Line.CreateBound(innerStart, innerEnd));

            // Generate the NumSteps+1 riser lines.
            var treadDepth = StartPoint.DistanceTo(EndPoint) / NumSteps;
            var dir = (EndPoint - StartPoint).Normalize();
            var currentStep = StartPoint + OffsetDirection * runWidth * 0.5;
            var deltaStep = dir * treadDepth;
            for (var i = 0; i <= NumSteps; i++)
            {
                var xOuter = currentStep - OffsetDirection * runwidth_2;
                var xInner = currentStep + OffsetDirection * runwidth_2;
                riserLines.Add(Line.CreateBound(xOuter, xInner));
                currentStep = currentStep + deltaStep;
            }
        }
    }
}
