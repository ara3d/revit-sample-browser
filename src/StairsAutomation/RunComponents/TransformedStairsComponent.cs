// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

using Ara3D.RevitSampleBrowser.Common.Geometry;
namespace Ara3D.RevitSampleBrowser.StairsAutomation.CS.RunComponents
{
    public class TransformedStairsComponent
    {
        private readonly Transform m_transform;

        public TransformedStairsComponent()
        {
            m_transform = Autodesk.Revit.DB.Transform.Identity;
        }

        public TransformedStairsComponent(Transform transform)
        {
            m_transform = transform;
        }

        public XYZ TransformPoint(XYZ point) => m_transform.OfPoint(point);

        public Curve Transform(Curve curve) => CurveGeometry.TransformCurve(curve, m_transform);

        public Line Transform(Line line) => Transform(line as Curve) as Line;

        public IList<Curve> Transform(IList<Curve> inputs) => CurveGeometry.TransformCurves(inputs, m_transform);
    }
}
