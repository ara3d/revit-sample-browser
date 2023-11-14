// Copyright 2023. See https://github.com/ara3d/revit-samples/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Revit.SDK.Samples.StairsAutomation.CS
{
    /// <summary>
    ///     Description of GeometryUtils.
    /// </summary>
    public static class GeometryUtils
    {
        /// <summary>
        ///     Creates a duplicate of a set of curves, applying the specified transform to each.
        /// </summary>
        /// <param name="inputs">The inputs.</param>
        /// <param name="trf">The transformation.</param>
        /// <returns>The copy of the curves.</returns>
        public static IList<Curve> TransformCurves(IList<Curve> inputs, Transform trf)
        {
            var numCurves = inputs.Count;
            var result = new List<Curve>(numCurves);

            for (var i = 0; i < numCurves; i++) result.Add(TransformCurve(inputs[i], trf));
            return result;
        }

        /// <summary>
        ///     Creates a duplicate curve, applying the specified transform to each.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="trf">The transformation.</param>
        /// <returns>The copy of the curve.</returns>
        public static Curve TransformCurve(Curve input, Transform trf)
        {
            var trfCurve = input.CreateTransformed(trf);
            return trfCurve;
        }
    }
}
