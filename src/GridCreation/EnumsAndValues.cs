// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

namespace Ara3D.RevitSampleBrowser.GridCreation.CS
{
    public enum CreateMode
    {
        Select,

        Orthogonal,

        RadialAndArc
    }

    public enum BubbleLocation
    {
        StartPoint,

        EndPoint
    }

    public static class Values
    {
        public const double Pi = 3.1415926535897900;

        public const double Degtorad = Pi / 180;
    }
}
