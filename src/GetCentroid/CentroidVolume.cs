// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from GetCentroid by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/GetCentroid

using Autodesk.Revit.DB;
using BuildingCoder;

namespace Ara3D.RevitSampleBrowser.GetCentroid.CS
{
    public class CentroidVolume
    {
        public CentroidVolume()
        {
            Init();
        }

        public void Init()
        {
            Centroid = XYZ.Zero;
            Volume = 0.0;
        }

        public void AddTriangle(XYZ v0, XYZ v1, XYZ v2)
        {
            var vol
                = (v0.X * ((v1.Y * v2.Z) - (v2.Y * v1.Z)))
                  + (v0.Y * ((v1.Z * v2.X) - (v2.Z * v1.X)))
                  + (v0.Z * ((v1.X * v2.Y) - (v2.X * v1.Y)));

            Centroid += vol * (v0 + v1 + v2);
            Volume += vol;
        }

        public void AddWeightedContribution(double volume, XYZ centroid)
        {
            Centroid += volume * centroid;
            Volume += volume;
        }

        // Signed tetrahedron volume from origin; scale centroid by 1/(4V) and volume by 1/6.
        public void Complete()
        {
            if (Volume.Equals(0))
            {
                return;
            }

            Centroid /= 4 * Volume;
            Volume /= 6;
        }

        public void CompleteWeightedAverage()
        {
            if (Volume.Equals(0))
            {
                return;
            }

            Centroid /= Volume;
        }

        public XYZ Centroid { get; private set; }

        public double Volume { get; private set; }

        public override string ToString()
        {
            return Util.RealString(Volume) + "@"
                   + Util.PointString(Centroid);
        }
    }
}
