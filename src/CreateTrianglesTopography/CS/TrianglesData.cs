// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CreateTrianglesTopography.CS
{
    /// <summary>
    ///     Triangles points and facets data which can be used to create topography surface.
    /// </summary>
    public class TrianglesData
    {
        /// The points represent an enclosed area in the XY plane.
        public IList<XYZ> Points { get; set; }

        ///Triangle faces composing a polygon mesh.
        public IList<IList<int>> Facets { get; set; }

        /// <summary>
        ///     parse all points and facets stored in the TrianglesData.json
        /// </summary>
        /// <returns>an instance of TrianglesData</returns>
        public static TrianglesData Load()
        {
            var assemblyFileFolder = Path.GetDirectoryName(typeof(TrianglesData).Assembly.Location);
            var emmfilePath = Path.Combine(assemblyFileFolder, "TrianglesData.json");
            var emmfileContent = File.ReadAllText(emmfilePath);
            return JsonParse(emmfileContent);
        }

        private static TrianglesData JsonParse(string jsonString)
        {
            var data = JsonSerializer.Deserialize<TrianglesDataDto>(jsonString);
            return new TrianglesData
            {
                Points = data.Points.Select(point => new XYZ(point.X, point.Y, point.Z)).ToList(),
                Facets = data.Facets
            };
        }

        private class TrianglesDataDto
        {
            public IList<PointDto> Points { get; set; }
            public IList<IList<int>> Facets { get; set; }
        }

        private class PointDto
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }
    }
}
