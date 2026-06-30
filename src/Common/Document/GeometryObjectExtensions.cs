// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Portions Copyright Revit Database Explorer (Apache-2.0)
// https://github.com/NeVeSpl/RevitDBExplorer @ 6929da81491a7f9ef69ed4c346afa1c582b830b5

using Autodesk.Revit.DB;

using System.Collections.Generic;


namespace Ara3D.RevitSampleBrowser.Common.Documents
{
    public static class GeometryObjectExtensions
    {
        public static Reference GetReference(this GeometryObject geometryObject)
        {
            var reference = geometryObject switch
            {
                Face face => face.Reference,
                Edge edge => edge.Reference,
                Curve curve => curve.Reference,
                Point point => point.Reference,               
                _ => null,
            };

            return reference;
        }



        public static IEnumerable<Curve> StreamCurves(this GeometryObject geometryObject)
        {            
            if (geometryObject is GeometryElement geometryElement)
            {
                foreach (var geometryObject_ in geometryElement)
                {
                    var result = StreamCurves(geometryObject_);
                    foreach (var item in result) yield return item;
                }
            }
            if (geometryObject is GeometryInstance geometryInstance)
            {
                var result = StreamCurves(geometryInstance.GetInstanceGeometry());
                foreach (var item in result) yield return item;
            }
            if (geometryObject is Solid solid)
            {
                foreach (Face face_ in solid.Faces)
                {
                    var result = StreamCurves(face_);
                    foreach (var item in result) yield return item;
                }
            }
            if (geometryObject is Face face)
            {
                foreach (EdgeArray loop in face.EdgeLoops)
                {
                    foreach (Edge edge_ in loop)
                    {
                        var result = StreamCurves(edge_);
                        foreach (var item in result) yield return item;
                    }
                }
            }
            if (geometryObject is Edge edge)
            {
                var result = StreamCurves(edge.AsCurve());
                foreach (var item in result) yield return item;
            }
            if (geometryObject is Curve curve)
            {
                yield return curve;
            }
        }

        public static IEnumerable<Solid> StreamSolids(this GeometryObject geometryObject)
        {
            if (geometryObject is GeometryElement geometryElement)
            {
                foreach (var geometryObject_ in geometryElement)
                {
                    var result = StreamSolids(geometryObject_);
                    foreach (var item in result) yield return item;
                }
            }
            if (geometryObject is GeometryInstance geometryInstance)
            {
                var result = StreamSolids(geometryInstance.GetInstanceGeometry());
                foreach (var item in result) yield return item;
            }
            if (geometryObject is Solid solid)
            {
                yield return solid;
            }
        }
    }
}