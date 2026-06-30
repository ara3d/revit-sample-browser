// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from GetCentroid by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/GetCentroid

using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.GetCentroid.CS
{
    public static class SolidCentroidCalculator
    {
        public readonly struct SolidCentroidComparison
        {
            public SolidCentroidComparison(
                CentroidVolume tessellated,
                XYZ nativeCentroid,
                double nativeVolume)
            {
                Tessellated = tessellated;
                NativeCentroid = nativeCentroid;
                NativeVolume = nativeVolume;
            }

            public CentroidVolume Tessellated { get; }

            public XYZ NativeCentroid { get; }

            public double NativeVolume { get; }

            public double CentroidDistance
                => (Tessellated.Centroid - NativeCentroid).GetLength();

            public double VolumeDifference
                => Tessellated.Volume - NativeVolume;

            public double RelativeVolumeDifference
                => Tessellated.Volume.Equals(0)
                    ? 0
                    : VolumeDifference / Tessellated.Volume;
        }

        public static bool TryGetCentroidFromSolid(
            Solid solid,
            out CentroidVolume centroidVolume)
        {
            centroidVolume = null;

            if (solid == null
                || solid.Faces.IsEmpty
                || !SolidUtils.IsValidForTessellation(solid))
            {
                return false;
            }

            SolidOrShellTessellationControls controls = new()
            {
                LevelOfDetail = 0
            };

            TriangulatedSolidOrShell triangulation;
            try
            {
                triangulation = SolidUtils.TessellateSolidOrShell(
                    solid, controls);
            }
            catch (Autodesk.Revit.Exceptions.InvalidOperationException)
            {
                return false;
            }

            CentroidVolume cv = new();

            for (var i = 0; i < triangulation.ShellComponentCount; ++i)
            {
                var component = triangulation.GetShellComponent(i);

                for (var j = 0; j < component.TriangleCount; ++j)
                {
                    var triangle = component.GetTriangle(j);

                    cv.AddTriangle(
                        component.GetVertex(triangle.VertexIndex0),
                        component.GetVertex(triangle.VertexIndex1),
                        component.GetVertex(triangle.VertexIndex2));
                }
            }

            cv.Complete();
            centroidVolume = cv;
            return true;
        }

        /// <summary>
        ///     Calculate centroid for all non-empty solids found for the given element.
        ///     Family instances may have their own non-empty solids; otherwise symbol
        ///     geometry is used. Transformed geometry is requested so solids are in place.
        /// </summary>
        public static bool TryGetCentroidFromElement(
            Element element,
            Options options,
            out CentroidVolume centroidVolume)
        {
            centroidVolume = null;

            var geo = element?.get_Geometry(options);
            if (geo == null)
            {
                return false;
            }

            if (element is FamilyInstance)
            {
                geo = geo.GetTransformed(Transform.Identity);
            }

            List<CentroidVolume> partials = [];
            GeometryInstance geometryInstance = null;

            foreach (var obj in geo)
            {
                if (obj is Solid solid
                    && TryGetCentroidFromSolid(solid, out var partial))
                {
                    partials.Add(partial);
                }

                geometryInstance ??= obj as GeometryInstance;
            }

            if (partials.Count == 0 && geometryInstance != null)
            {
                foreach (var obj in geometryInstance.GetSymbolGeometry())
                {
                    if (obj is Solid solid
                        && TryGetCentroidFromSolid(solid, out var partial))
                    {
                        partials.Add(partial);
                    }
                }
            }

            if (partials.Count == 0)
            {
                return false;
            }

            CentroidVolume combined = new();
            foreach (var partial in partials)
            {
                combined.AddWeightedContribution(
                    partial.Volume,
                    partial.Centroid);
            }

            combined.CompleteWeightedAverage();
            centroidVolume = combined;
            return true;
        }

        public static IEnumerable<Solid> GetSolidsFromElement(
            Element element,
            Options options)
        {
            var geo = element?.get_Geometry(options);
            if (geo == null)
            {
                yield break;
            }

            if (element is FamilyInstance)
            {
                geo = geo.GetTransformed(Transform.Identity);
            }

            var foundSolid = false;
            GeometryInstance geometryInstance = null;

            foreach (var obj in geo)
            {
                if (obj is Solid solid
                    && !solid.Faces.IsEmpty
                    && SolidUtils.IsValidForTessellation(solid))
                {
                    foundSolid = true;
                    yield return solid;
                }

                geometryInstance ??= obj as GeometryInstance;
            }

            if (!foundSolid && geometryInstance != null)
            {
                foreach (var obj in geometryInstance.GetSymbolGeometry())
                {
                    if (obj is Solid solid
                        && !solid.Faces.IsEmpty
                        && SolidUtils.IsValidForTessellation(solid))
                    {
                        yield return solid;
                    }
                }
            }
        }

        public static bool TryCompareSolid(
            Solid solid,
            out SolidCentroidComparison comparison)
        {
            comparison = default;

            if (!TryGetCentroidFromSolid(solid, out var tessellated))
            {
                return false;
            }

            comparison = new SolidCentroidComparison(
                tessellated,
                solid.ComputeCentroid(),
                solid.Volume);
            return true;
        }
    }
}
