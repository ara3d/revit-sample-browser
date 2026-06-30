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
    public static class JtEdgeArrayExtensionMethods
        {
            /// <summary>
            ///     Return a polygon as a list of XYZ points from
            ///     an EdgeArray. If any of the edges are curved,
            ///     we retrieve the tessellated points, i.e. an
            ///     approximation determined by Revit.
            /// </summary>
            public static List<XYZ> GetPolygon(
                this EdgeArray ea)
            {
                var n = ea.Size;
    
                var polygon = new List<XYZ>(n);
    
                foreach (Edge e in ea)
                {
                    var pts = e.Tessellate();
    
                    n = polygon.Count;
    
                    if (0 < n)
                    {
                        Debug.Assert(pts[0]
                                .IsAlmostEqualTo(polygon[n - 1]),
                            "expected last edge end point to "
                            + "equal next edge start point");
    
                        polygon.RemoveAt(n - 1);
                    }
    
                    polygon.AddRange(pts);
                }
    
                n = polygon.Count;
    
                Debug.Assert(polygon[0]
                        .IsAlmostEqualTo(polygon[n - 1]),
                    "expected first edge start point to "
                    + "equal last edge end point");
    
                polygon.RemoveAt(n - 1);
    
                return polygon;
            }
        }
}
