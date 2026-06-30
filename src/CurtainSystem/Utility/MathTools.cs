// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;
namespace Ara3D.RevitSampleBrowser.CurtainSystem.CS.Utility
{
    public class Vector4
    {
        public Vector4(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector4(XYZ v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double W { get; set; } = 1.0d;

        public Vector4 CrossProduct(Vector4 v)
        {
            return new Vector4((Y * v.Z) - (Z * v.Y), (Z * v.X)
                                                  - (X * v.Z), (X * v.Y) - (Y * v.X));
        }

        public static Vector4 CrossProduct(Vector4 va, Vector4 vb)
        {
            return new Vector4((va.Y * vb.Z) - (va.Z * vb.Y), (va.Z * vb.X)
                                                          - (va.X * vb.Z), (va.X * vb.Y) - (va.Y * vb.X));
        }
    }
}
