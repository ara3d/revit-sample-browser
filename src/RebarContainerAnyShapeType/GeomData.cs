// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using Ara3D.RevitSampleBrowser.Common.Geometry;
using Autodesk.Revit.DB;
using System.Collections.Generic;
namespace Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS
{
    public struct RebarGeometry
    {
        // Private members

        public XYZ Normal { get; set; }

        public IList<Curve> Curves { get; set; }

        public int RebarNumber { get; set; }

        public double RebarSpacing { get; set; }

        public RebarGeometry(XYZ normal, IList<Curve> curves, int number, double spacing)
        {
            // initialize the data members
            Normal = normal;
            Curves = curves;
            RebarNumber = number;
            RebarSpacing = spacing;
        }
    }

    public struct BeamRebarData
    {
        /// <summary>
        ///     offset value of the top end reinforcement
        /// </summary>
        public const double TopEndOffset = 0.2;

        /// <summary>
        ///     offset value of the top center reinforcement
        /// </summary>
        public const double TopCenterOffset = 0.23;

        /// <summary>
        ///     offset value of the transverse reinforcement
        /// </summary>
        public const double TransverseOffset = 0.125;

        /// <summary>
        ///     offset value of the end transverse reinforcement
        /// </summary>
        public const double TransverseEndOffset = 1.2;

        public const double TransverseSpaceBetween = 1;

        /// <summary>
        ///     offset value of bottom reinforcement
        /// </summary>
        public const double BottomOffset = 0.271;

        public const int BottomRebarNumber = 5;

        public const int TopRebarNumber = 2;
    }

    public struct ColumnRebarData
    {
        /// <summary>
        ///     offset value of transverse reinforcement
        /// </summary>
        public const double TransverseOffset = 0.125;

        /// <summary>
        ///     offset value of vertical reinforcement
        /// </summary>
        public const double VerticalOffset = 0.234;
    }

    public enum TopRebarLocation
    {
        Start,

        Center,

        End
    }

    public enum TransverseRebarLocation
    {
        Start,

        Center,

        End
    }

    public enum VerticalRebarLocation
    {
        North,

        East,

        South,

        West
    }

    /// <summary>
    ///     A comparer for XYZ, and give a method to sort all the Autodesk.Revit.DB.XYZ points in a array
    /// </summary>
    public class XyzHeightComparer : IComparer<XYZ>
    {
        int IComparer<XYZ>.Compare(XYZ first, XYZ second)
        {
            // first compare z coordinate, then y coordinate, at last x coordinate
            if (XyzMath.IsEqual(first.Z, second.Z))
            {
                return XyzMath.IsEqual(first.Y, second.Y)
                    ? XyzMath.IsEqual(first.X, second.X) ? 0 : first.X > second.X ? 1 : -1
                    : first.Y > second.Y ? 1 : -1;
            }

            return first.Z > second.Z ? 1 : -1;
        }
    }
}
