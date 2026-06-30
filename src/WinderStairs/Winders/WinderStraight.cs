// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.WinderStairs.CS.Winders
{
    public class WinderStraight
    {
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

        public XYZ StartPoint { get; }

        public XYZ EndPoint { get; }

        public XYZ OffsetDirection { get; }

        public uint NumSteps { get; }

        public void GenerateSketch(double runWidth,
            IList<Curve> outerBoundary, IList<Curve> walkPath,
            IList<Curve> innerBoundary, IList<Curve> riserLines)
        {
            if (NumSteps == 0)
            {
                // Just Generate one riser line.
                var middleOffset = StartPoint + (OffsetDirection * runWidth * 0.5);
                var xOuter = middleOffset - (OffsetDirection * runWidth * 0.5);
                var xInner = middleOffset + (OffsetDirection * runWidth * 0.5);
                riserLines.Add(Line.CreateBound(xOuter, xInner));
                return;
            }

            // For NumSteps > 0:
            //
            // Generate the outer boundary line.
            var runwidth2 = runWidth * 0.5;
            var outBounary = Line.CreateBound(StartPoint, EndPoint);
            outerBoundary.Add(outBounary);

            // Generate the middle walk path line.
            var middleStart = StartPoint + (OffsetDirection * runWidth * 0.5);
            var middleEnd = EndPoint + (OffsetDirection * runwidth2);
            walkPath.Add(Line.CreateBound(middleStart, middleEnd));

            // Generate the inner boundary line.
            var innerStart = StartPoint + (OffsetDirection * runWidth);
            var innerEnd = EndPoint + (OffsetDirection * runWidth);
            innerBoundary.Add(Line.CreateBound(innerStart, innerEnd));

            // Generate the NumSteps+1 riser lines.
            var treadDepth = StartPoint.DistanceTo(EndPoint) / NumSteps;
            var dir = (EndPoint - StartPoint).Normalize();
            var currentStep = StartPoint + (OffsetDirection * runWidth * 0.5);
            var deltaStep = dir * treadDepth;
            for (var i = 0; i <= NumSteps; i++)
            {
                var xOuter = currentStep - (OffsetDirection * runwidth2);
                var xInner = currentStep + (OffsetDirection * runwidth2);
                riserLines.Add(Line.CreateBound(xOuter, xInner));
                currentStep += deltaStep;
            }
        }
    }
}
