// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CurtainSystem.CS.Utility
{
    /// <summary>
    ///     Vector4 is a homogeneous coordinate class used to store vector
    ///     and contain method to handle the vector
    /// </summary>
    public class Vector4
    {
        /// <summary>
        ///     constructor
        /// </summary>
        public Vector4(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     constructor, transfer Autodesk.Revit.DB.XYZ to vector
        /// </summary>
        /// <param name="v">Autodesk.Revit.DB.XYZ structure which needs to be transfered</param>
        public Vector4(XYZ v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        /// <summary>
        ///     X property to get/set x value of Vector4
        /// </summary>
        public double X { get; set; }

        /// <summary>
        ///     Y property to get/set y value of Vector4
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        ///     Z property to get/set z value of Vector4
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        ///     W property to get/set fourth value of Vector4
        /// </summary>
        public double W { get; set; } = 1.0d;

        /// <summary>
        ///     get normal vector of plane contains two vectors
        /// </summary>
        /// <param name="v">second vector</param>
        /// <returns> normal vector of two vectors</returns>
        public Vector4 CrossProduct(Vector4 v)
        {
            return new Vector4(Y * v.Z - Z * v.Y, Z * v.X
                                                  - X * v.Z, X * v.Y - Y * v.X);
        }

        /// <summary>
        ///     get normal vector of two vectors
        /// </summary>
        /// <param name="va">first vector</param>
        /// <param name="vb">second vector</param>
        /// <returns> normal vector of two vectors </returns>
        public static Vector4 CrossProduct(Vector4 va, Vector4 vb)
        {
            return new Vector4(va.Y * vb.Z - va.Z * vb.Y, va.Z * vb.X
                                                          - va.X * vb.Z, va.X * vb.Y - va.Y * vb.X);
        }
    }
}
