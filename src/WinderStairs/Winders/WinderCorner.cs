// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.WinderStairs.CS.Winders
{
    /// <summary>
    ///     It represents a winder region used to connect straight runs. For L-shape stairs, there is
    ///     one WinderCorner, For U-shape stairs, there are two such components.
    /// </summary>
    public abstract class WinderCorner
    {
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

        public XYZ CornerPoint { get; private set; }

        public XYZ Direction1 { get; }

        public XYZ Direction2 { get; }

        public uint NumSteps { get; }

        public double Distance1 { get; protected set; }

        public double Distance2 { get; protected set; }

        public XYZ StartPoint => CornerPoint - Direction1 * Distance1;

        public XYZ EndPoint => CornerPoint + Direction2 * Distance2;

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
