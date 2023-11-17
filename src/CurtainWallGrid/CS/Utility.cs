// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.RevitSampleBrowser.CurtainWallGrid.CS
{
    /// <summary>
    ///     compare 2 wall types and sort them by their type names
    /// </summary>
    public class WallTypeComparer : IComparer<WallType>
    {
        /// <summary>
        ///     compare 2 walltypes by their names
        /// </summary>
        /// <param name="x">
        ///     wall type to be compared
        /// </param>
        /// <param name="y">
        ///     wall type to be compared
        /// </param>
        /// <returns>
        ///     returns the result by comparing their names with CaseInsensitiveComparer
        /// </returns>
        public int Compare(WallType x, WallType y)
        {
            var xName = x.Name;
            var yName = y.Name;

            IComparer comp = new CaseInsensitiveComparer();
            return comp.Compare(xName, yName);
        }
    }

    /// <summary>
    ///     compare 2 views and sort them by their type names
    /// </summary>
    public class ViewComparer : IComparer<View>
    {
        /// <summary>
        ///     compare 2 views by their names
        /// </summary>
        /// <param name="x">
        ///     view to be compared
        /// </param>
        /// <param name="y">
        ///     view to be compared
        /// </param>
        /// <returns>
        ///     returns the result by comparing their names with CaseInsensitiveComparer
        /// </returns>
        public int Compare(View x, View y)
        {
            var xName = x.ViewType + " : " + x.Name;
            var yName = y.ViewType + " : " + y.Name;

            IComparer comp = new CaseInsensitiveComparer();
            return comp.Compare(xName, yName);
        }
    }
}
