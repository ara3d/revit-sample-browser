// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS
{
    /// <summary>
    ///     An abstract base class for stairs components which can be moved via a translation and rotation.
    /// </summary>
    public class TransformedStairsComponent
    {
        private readonly Transform m_transform;

        /// <summary>
        ///     Constructs a transformed component with the identity transform.
        /// </summary>
        public TransformedStairsComponent()
        {
            m_transform = Autodesk.Revit.DB.Transform.Identity;
        }

        /// <summary>
        ///     Constructs a transformed component with the specified transform.
        /// </summary>
        public TransformedStairsComponent(Transform transform)
        {
            m_transform = transform;
        }

        /// <summary>
        ///     Transforms the given XYZ point by the stored transform.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The transformed point.</returns>
        public XYZ TransformPoint(XYZ point)
        {
            var xyz = m_transform.OfPoint(point);
            return xyz;
        }

        /// <summary>
        ///     Transforms the given curve by the stored transform.
        /// </summary>
        /// <param name="curve">The curve.</param>
        /// <returns>The transformed curve.</returns>
        public Curve Transform(Curve curve)
        {
            return GeometryUtils.TransformCurve(curve, m_transform);
        }

        /// <summary>
        ///     Transforms the given line by the stored transform.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The transformed line.</returns>
        public Line Transform(Line line)
        {
            return Transform(line as Curve) as Line;
        }

        /// <summary>
        ///     Transforms all members of the given curve array by the stored transform.
        /// </summary>
        /// <param name="inputs">The input curves.</param>
        /// <returns>The transformed curves.</returns>
        public IList<Curve> Transform(IList<Curve> inputs)
        {
            return GeometryUtils.TransformCurves(inputs, m_transform);
        }
    }
}
