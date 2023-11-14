// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitMultiSample.WinderStairs.CS
{
    /// <summary>
    ///     It represents a winder region used to connect straight runs. For L-shape stairs, there is
    ///     one WinderCorner, For U-shape stairs, there are two such components.
    /// </summary>
    internal abstract class WinderCorner
    {
        /// <summary>
        ///     Constructor to initialize the basic fields of a winder region.
        /// </summary>
        /// <param name="cornerPnt">Corner Point</param>
        /// <param name="dir1">Enter direction</param>
        /// <param name="dir2">Leave direction</param>
        /// <param name="steps">Number of steps</param>
        protected WinderCorner(XYZ cornerPnt, XYZ dir1, XYZ dir2, uint steps)
        {
            CornerPoint = cornerPnt;
            Direction1 = dir1;
            Direction2 = dir2;
            NumSteps = steps;
        }
        /*   
        *   Direction1
        *    |      
        *    |
        *    V    
        *    |
        *    |
        *    |----->---- Direction2
        * CornerPoint   
        */

        /// <summary>
        ///     Corner point of the winder region.
        /// </summary>
        public XYZ CornerPoint { get; private set; }

        /// <summary>
        ///     The enter direction of the winder region.
        /// </summary>
        public XYZ Direction1 { get; }

        /// <summary>
        ///     The leave direction of the winder region.
        /// </summary>
        public XYZ Direction2 { get; }

        /// <summary>
        ///     Number of steps in the winder region.
        /// </summary>
        public uint NumSteps { get; }

        /// <summary>
        ///     The distance from start point to corner point.
        /// </summary>
        public double Distance1 { get; protected set; }

        /// <summary>
        ///     The distance from corner point to end point.
        /// </summary>
        public double Distance2 { get; protected set; }

        /// <summary>
        ///     Start delimiter of the winder region.
        /// </summary>
        public XYZ StartPoint => CornerPoint - Direction1 * Distance1;

        /// <summary>
        ///     End delimiter of the winder region.
        /// </summary>
        public XYZ EndPoint => CornerPoint + Direction2 * Distance2;

        /// <summary>
        ///     Move the whole winder region by the given vector.
        /// </summary>
        /// <param name="vector">Move delta vector</param>
        public virtual void Move(XYZ vector)
        {
            CornerPoint += vector;
        }

        /// <summary>
        ///     Generate the sketch in the winder region including the outer boundary, walk path,
        ///     inner boundary and riser lines. Subclass need to implement this method
        ///     to fill the input sketch using its winder layout algorithm.
        /// </summary>
        /// <param name="runWidth">Runwidth</param>
        /// <param name="outerBoundary">Outer boundary</param>
        /// <param name="walkPath">Walk path</param>
        /// <param name="innerBoundary">Inner boundary</param>
        /// <param name="riserLines">Riser lines</param>
        public abstract void GenerateSketch(double runWidth,
            IList<Curve> outerBoundary, IList<Curve> walkPath,
            IList<Curve> innerBoundary, IList<Curve> riserLines);
    }
}
