// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
//
// Adapted from GetCentroid by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/GetCentroid

using Autodesk.Revit.DB;
using BuildingCoder;

namespace Ara3D.RevitSampleBrowser.GetCentroid.CS
{
    /// <summary>
    ///     Calculate and store the centroid and volume from a set of triangular facets.
    /// </summary>
    public class CentroidVolume
    {
        XYZ _centroid;
        double _volume;

        public CentroidVolume()
        {
            Init();
        }

        public void Init()
        {
            _centroid = XYZ.Zero;
            _volume = 0.0;
        }

        public void AddTriangle(XYZ v0, XYZ v1, XYZ v2)
        {
            var vol
                = v0.X * (v1.Y * v2.Z - v2.Y * v1.Z)
                  + v0.Y * (v1.Z * v2.X - v2.Z * v1.X)
                  + v0.Z * (v1.X * v2.Y - v2.X * v1.Y);

            _centroid += vol * (v0 + v1 + v2);
            _volume += vol;
        }

        /// <summary>
        ///     Add a completed partial centroid, weighted by its volume.
        /// </summary>
        public void AddWeightedContribution(double volume, XYZ centroid)
        {
            _centroid += volume * centroid;
            _volume += volume;
        }

        /// <summary>
        ///     Set centroid coordinates and volume to their final values when completed.
        /// </summary>
        public void Complete()
        {
            if (_volume.Equals(0))
            {
                return;
            }

            _centroid /= 4 * _volume;
            _volume /= 6;
        }

        /// <summary>
        ///     Finalize a volume-weighted average of completed partial centroids.
        /// </summary>
        public void CompleteWeightedAverage()
        {
            if (_volume.Equals(0))
            {
                return;
            }

            _centroid /= _volume;
        }

        public XYZ Centroid => _centroid;

        public double Volume => _volume;

        public override string ToString()
        {
            return Util.RealString(_volume) + "@"
                   + Util.PointString(_centroid);
        }
    }
}
