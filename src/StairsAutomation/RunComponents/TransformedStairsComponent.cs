// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

using Ara3D.RevitSampleBrowser.Common.Geometry;
namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS.RunComponents
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
        public XYZ TransformPoint(XYZ point) => m_transform.OfPoint(point);

        /// <summary>
        ///     Transforms the given curve by the stored transform.
        /// </summary>
        /// <param name="curve">The curve.</param>
        /// <returns>The transformed curve.</returns>
        public Curve Transform(Curve curve) => CurveGeometry.TransformCurve(curve, m_transform);

        public Line Transform(Line line) => Transform(line as Curve) as Line;

        public IList<Curve> Transform(IList<Curve> inputs) => CurveGeometry.TransformCurves(inputs, m_transform);
    }
}
