// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CustomExporterAdnMeshJson by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CustomExporterAdnMeshJson

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CustomExporter.AdnMeshJsonExporter.CS
{
    internal class CentroidVolume
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

        public void AddTriangle(XYZ[] p)
        {
            var vol
                = p[0].X * (p[1].Y * p[2].Z - p[2].Y * p[1].Z)
                  + p[0].Y * (p[1].Z * p[2].X - p[2].Z * p[1].X)
                  + p[0].Z * (p[1].X * p[2].Y - p[2].X * p[1].Y);

            _centroid += vol * (p[0] + p[1] + p[2]);
            _volume += vol;
        }

        public void Complete()
        {
            _centroid /= 4 * _volume;
            _volume /= 6;
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
