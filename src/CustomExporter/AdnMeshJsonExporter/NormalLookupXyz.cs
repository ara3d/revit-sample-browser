// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CustomExporterAdnMeshJson by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CustomExporterAdnMeshJson

using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.CustomExporter.AdnMeshJsonExporter.CS
{
    /// <summary>
    ///     A facet normal vector lookup class to avoid duplicate normal vector definitions.
    /// </summary>
    internal class NormalLookupXyz : Dictionary<XYZ, int>
    {
        class XyzVectorEqualityComparer : IEqualityComparer<XYZ>
        {
            const double Eps = 1.0e-9;

            public bool Equals(XYZ v, XYZ w)
            {
                return v.IsAlmostEqualTo(w, Eps);
            }

            public int GetHashCode(XYZ v)
            {
                return BuildingCoder.Util.PointString(v).GetHashCode();
            }
        }

        public NormalLookupXyz()
            : base(new XyzVectorEqualityComparer())
        {
        }

        public int AddNormal(XYZ v)
        {
            return ContainsKey(v)
                ? this[v]
                : this[v] = Count;
        }
    }
}
