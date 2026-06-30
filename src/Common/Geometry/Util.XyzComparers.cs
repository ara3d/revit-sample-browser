// Copyright 2023. See https://github.com/ara3d/revit-sample-browser/LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Color = System.Drawing.Color;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using Rectangle = System.Drawing.Rectangle;
using WinForms = System.Windows.Forms;


namespace BuildingCoder
{
    internal static partial class Util
    {
        #region Consolidated XYZ comparers

        /// <summary>
        ///     Define equality between XYZ objects, ensuring
        ///     that almost equal points compare equal.
        /// </summary>
        public class XyzEqualityComparer : IEqualityComparer<XYZ>
        {
            private readonly double _eps;

            public XyzEqualityComparer()
                : this(0)
            {
            }

            public XyzEqualityComparer(double eps)
            {
                _eps = eps;
            }

            public bool Equals(XYZ p, XYZ q)
            {
                return 0 < _eps
                    ? _eps > p.DistanceTo(q)
                    : p.IsAlmostEqualTo(q);
            }

            public int GetHashCode(XYZ p)
            {
                return PointString(p).GetHashCode();
            }
        }

        #endregion
    }
}
