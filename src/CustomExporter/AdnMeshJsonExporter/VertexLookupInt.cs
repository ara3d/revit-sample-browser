// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CustomExporterAdnMeshJson by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CustomExporterAdnMeshJson

using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.CustomExporter.AdnMeshJsonExporter.CS
{
    /// <summary>
    ///     A vertex lookup class to avoid duplicate vertex definitions.
    /// </summary>
    internal class VertexLookupInt : Dictionary<PointInt, int>
    {
        class PointIntEqualityComparer : IEqualityComparer<PointInt>
        {
            public bool Equals(PointInt p, PointInt q)
            {
                return 0 == p.CompareTo(q);
            }

            public int GetHashCode(PointInt p)
            {
                return (p.X.ToString()
                        + "," + p.Y.ToString()
                        + "," + p.Z.ToString())
                    .GetHashCode();
            }
        }

        public VertexLookupInt()
            : base(new PointIntEqualityComparer())
        {
        }

        /// <summary>
        ///     Return the index of the given vertex, adding a new entry if required.
        /// </summary>
        public int AddVertex(PointInt p)
        {
            return ContainsKey(p)
                ? this[p]
                : this[p] = Count;
        }
    }
}
