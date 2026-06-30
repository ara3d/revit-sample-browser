// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CustomExporterAdnMeshJson by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CustomExporterAdnMeshJson

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CustomExporter.AdnMeshJsonExporter.CS
{
    /// <summary>
    ///     The data format specifying one solid for the WebGL viewer, defining its centre,
    ///     colour, id, triangular facets, their vertex coordinates, indices and normals.
    /// </summary>
    internal class AdnMeshData
    {
        int FacetCount { get; set; }
        int VertexCount { get; set; }
        int[] VertexCoords { get; set; }
        int[] VertexIndices { get; set; }
        double[] Normals { get; set; }
        int[] NormalIndices { get; set; }
        int[] Center { get; set; }
        int Color { get; set; }
        string Id { get; set; }

        const double ExportFactor = 0.002;

        public AdnMeshData(
            VertexLookupInt vertices,
            List<int> vertexIndices,
            NormalLookupXyz normals,
            List<int> normalIndices,
            PointInt center,
            Color color,
            double transparency,
            string id)
        {
            var n = vertexIndices.Count;

            Debug.Assert(0 == (n % 3),
                "expected triples of 3D point vertex indices");

            Debug.Assert(normalIndices.Count == n,
                "expected a normal for each vertex");

            FacetCount = n / 3;

            n = vertices.Count;
            VertexCount = n;
            VertexCoords = new int[n * 3];
            var i = 0;
            foreach (var p in vertices.Keys)
            {
                VertexCoords[i++] = p.X;
                VertexCoords[i++] = p.Y;
                VertexCoords[i++] = p.Z;
            }

            VertexIndices = vertexIndices.ToArray();

            n = normals.Count;
            Normals = new double[n * 3];
            i = 0;
            foreach (var v in normals.Keys)
            {
                Normals[i++] = v.X;
                Normals[i++] = v.Y;
                Normals[i++] = v.Z;
            }

            NormalIndices = normalIndices.ToArray();

            Center = new int[3];
            i = 0;
            Center[i++] = center.X;
            Center[i++] = center.Y;
            Center[i] = center.Z;

            var alpha = (byte)((100 - transparency) * 2.55555555);

            Color = ConvertClr(
                color.Red, color.Green, color.Blue, alpha);

            Id = id;
        }

        static int ConvertClr(byte r, byte g, byte b, byte a)
        {
            return (r << 24) + (g << 16) + (b << 8) + a;
        }

        public string ToJson()
        {
            var s = string.Format
            ("\n \"FacetCount\":{0},"
             + "\n \"VertexCount\":{1},"
             + "\n \"VertexCoords\":[{2}],"
             + "\n \"VertexIndices\":[{3}],"
             + "\n \"Normals\":[{4}],"
             + "\n \"NormalIndices\":[{5}],"
             + "\n \"Center\":[{6}],"
             + "\n \"Color\":[{7}],"
             + "\n \"Id\":\"{8}\"",
                FacetCount,
                VertexCount,
                string.Join(",", VertexCoords.Select(i => (ExportFactor * i).ToString("0.#")).ToArray()),
                string.Join(",", VertexIndices.Select(i => i.ToString()).ToArray()),
                string.Join(",", Normals.Select(a => a.ToString("0.####")).ToArray()),
                string.Join(",", NormalIndices.Select(i => i.ToString()).ToArray()),
                string.Join(",", Center.Select(i => (ExportFactor * i).ToString("0.#"))),
                Color,
                Id);

            return "\n{" + s + "\n}";
        }
    }
}
