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
    public static class JtLineExtensionMethods
        {
            public static bool Contains(
                this Line line,
                XYZ p,
                double tolerance = Util._eps)
            {
                var a = line.GetEndPoint(0); // line start point
                var b = line.GetEndPoint(1); // line end point
                var f = a.DistanceTo(b); // distance between focal points
                var da = a.DistanceTo(p);
                var db = p.DistanceTo(b);
                // da + db is always greater or equal f
                return (da + db - f) * f < tolerance;
            }
        }
}
