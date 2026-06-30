// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CustomExporterAdnMeshJson by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CustomExporterAdnMeshJson

using System;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CustomExporter.AdnMeshJsonExporter.CS
{
    internal class PointInt : IComparable<PointInt>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        const double FeetToMm = 25.4 * 12;

        static int ConvertFeetToMillimetres(double d)
        {
            return (int)(FeetToMm * d + 0.5);
        }

        public PointInt(XYZ p)
        {
            X = ConvertFeetToMillimetres(p.X);
            Y = ConvertFeetToMillimetres(p.Y);
            Z = ConvertFeetToMillimetres(p.Z);
        }

        public int CompareTo(PointInt a)
        {
            var d = X - a.X;

            if (0 == d)
            {
                d = Y - a.Y;

                if (0 == d)
                {
                    d = Z - a.Z;
                }
            }

            return d;
        }
    }
}
