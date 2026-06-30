// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using System.Collections.Generic;
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

        public XYZ TransformPoint(XYZ point)
        {
            return m_transform.OfPoint(point);
        }

        public Curve Transform(Curve curve)
        {
            return CurveGeometry.TransformCurve(curve, m_transform);
        }

        public Line Transform(Line line)
        {
            return Transform(line as Curve) as Line;
        }

        public IList<Curve> Transform(IList<Curve> inputs)
        {
            return CurveGeometry.TransformCurves(inputs, m_transform);
        }
    }
}
