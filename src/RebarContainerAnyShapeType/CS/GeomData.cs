// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.RebarContainerAnyShapeType.CS
{
    /// <summary>
    ///     a struct to store the geometry information of the reinforcement
    /// </summary>
    public struct RebarGeometry
    {
        // Private members

        /// <summary>
        ///     get and set the value of the normal
        /// </summary>
        public XYZ Normal { get; set; }

        /// <summary>
        ///     get and set the value of curve array
        /// </summary>
        public IList<Curve> Curves { get; set; }

        /// <summary>
        ///     get and set the number of the reinforcement
        /// </summary>
        public int RebarNumber { get; set; }

        /// <summary>
        ///     get and set the value of the reinforcement spacing
        /// </summary>
        public double RebarSpacing { get; set; }

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="normal">the normal information</param>
        /// <param name="curves">the profile of the rebars</param>
        /// <param name="number">the number of the rebar</param>
        /// <param name="spacing">the number of the rebar</param>
        public RebarGeometry(XYZ normal, IList<Curve> curves, int number, double spacing)
        {
            // initialize the data members
            Normal = normal;
            Curves = curves;
            RebarNumber = number;
            RebarSpacing = spacing;
        }
    }

    /// <summary>
    ///     A struct to store the const data which support beam reinforcement creation
    /// </summary>
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

        /// <summary>
        ///     the spacing value between end and center transverse reinforcement
        /// </summary>
        public const double TransverseSpaceBetween = 1;

        /// <summary>
        ///     offset value of bottom reinforcement
        /// </summary>
        public const double BottomOffset = 0.271;

        /// <summary>
        ///     number of bottom reinforcement
        /// </summary>
        public const int BottomRebarNumber = 5;

        /// <summary>
        ///     number of top reinforcement
        /// </summary>
        public const int TopRebarNumber = 2;
    }

    /// <summary>
    ///     A struct to store the const data which support column reinforcement creation
    /// </summary>
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

    /// <summary>
    ///     Indicate location of top reinforcement
    /// </summary>
    public enum TopRebarLocation
    {
        /// <summary>
        ///     locate start
        /// </summary>
        Start,

        /// <summary>
        ///     locate center
        /// </summary>
        Center,

        /// <summary>
        ///     locate end
        /// </summary>
        End
    }

    /// <summary>
    ///     Indicate location of transverse reinforcement
    /// </summary>
    public enum TransverseRebarLocation
    {
        /// <summary>
        ///     locate start
        /// </summary>
        Start,

        /// <summary>
        ///     locate center
        /// </summary>
        Center,

        /// <summary>
        ///     locate end
        /// </summary>
        End
    }

    /// <summary>
    ///     Indicate location of vertical reinforcement
    /// </summary>
    public enum VerticalRebarLocation
    {
        /// <summary>
        ///     locate north
        /// </summary>
        North,

        /// <summary>
        ///     locate east
        /// </summary>
        East,

        /// <summary>
        ///     locate south
        /// </summary>
        South,

        /// <summary>
        ///     locate west
        /// </summary>
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
            if (GeomUtil.IsEqual(first.Z, second.Z))
            {
                if (GeomUtil.IsEqual(first.Y, second.Y))
                {
                    if (GeomUtil.IsEqual(first.X, second.X)) return 0;
                    return first.X > second.X ? 1 : -1;
                }

                return first.Y > second.Y ? 1 : -1;
            }

            return first.Z > second.Z ? 1 : -1;
        }
    }
}
