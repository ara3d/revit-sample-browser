#region Namespaces

using System;
using System.Diagnostics;
using Autodesk.Revit.DB;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_NewBlend sample.</summary>
    internal static partial class Util
    {
        internal static Blend CreateBlend(Document doc)
        {
            Debug.Assert(doc.IsFamilyDocument,
                "this method will only work in a family document");

            var app = doc.Application;

            var creApp
                = app.Create;

            var factory
                = doc.FamilyCreate;

            double startAngle = 0;
            var midAngle = Math.PI;
            var endAngle = 2 * Math.PI;

            var xAxis = XYZ.BasisX;
            var yAxis = XYZ.BasisY;

            var center = XYZ.Zero;
            var normal = -XYZ.BasisZ;
            var radius = 0.7579;

            var arc1 = Arc.Create(center, radius, startAngle, midAngle, xAxis, yAxis);
            var arc2 = Arc.Create(center, radius, midAngle, endAngle, xAxis, yAxis);

            var baseProfile = new CurveArray();

            baseProfile.Append(arc1);
            baseProfile.Append(arc2);

            var topProfile = new CurveArray();

            var circular_top = false;

            if (circular_top)
            {
                var center2 = new XYZ(0, 0, 1.27);

                var arc3 = Arc.Create(center2, radius, startAngle, midAngle, xAxis, yAxis);
                var arc4 = Arc.Create(center2, radius, midAngle, endAngle, xAxis, yAxis);

                topProfile.Append(arc3);
                topProfile.Append(arc4);
            }
            else
            {
                var pts = new[]
                {
                    new XYZ(0, 0, 3),
                    new XYZ(2, 0, 3),
                    new XYZ(3, 2, 3),
                    new XYZ(0, 4, 3)
                };

                for (var i = 0; i < 4; ++i)
                    topProfile.Append(Line.CreateBound(
                        pts[0 == i ? 3 : i - 1], pts[i]));
            }

            var basePlane = Plane.CreateByNormalAndOrigin(normal, center);

            var sketch = SketchPlane.Create(doc, basePlane);

            var blend = factory.NewBlend(true,
                topProfile, baseProfile, sketch);

            return blend;
        }
    }
}
