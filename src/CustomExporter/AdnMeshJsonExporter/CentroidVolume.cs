// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt
// Adapted from CustomExporterAdnMeshJson by Jeremy Tammik (MIT).
// https://github.com/jeremytammik/CustomExporterAdnMeshJson

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CustomExporter.AdnMeshJsonExporter.CS
{
    internal class CentroidVolume
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

        public void AddTriangle(XYZ[] p)
        {
            var vol
                = (p[0].X * ((p[1].Y * p[2].Z) - (p[2].Y * p[1].Z)))
                  + (p[0].Y * ((p[1].Z * p[2].X) - (p[2].Z * p[1].X)))
                  + (p[0].Z * ((p[1].X * p[2].Y) - (p[2].X * p[1].Y)));

            Centroid += vol * (p[0] + p[1] + p[2]);
            Volume += vol;
        }

        public void Complete()
        {
            Centroid /= 4 * Volume;
            Volume /= 6;
        }

        public XYZ Centroid { get; private set; }

        public double Volume { get; private set; }

        public override string ToString()
        {
            return BuildingCoder.Util.RealString(Volume) + "@"
                   + BuildingCoder.Util.PointString(Centroid);
        }
    }
}
