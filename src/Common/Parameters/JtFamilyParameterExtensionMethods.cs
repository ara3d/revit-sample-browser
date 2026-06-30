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
    public static class JtFamilyParameterExtensionMethods
        {
            public static bool IsShared(
                this FamilyParameter familyParameter)
            {
                var mi = familyParameter
                    .GetType()
                    .GetMethod("getParameter",
                        BindingFlags.Instance
                        | BindingFlags.NonPublic);
    
                if (null == mi)
                    throw new InvalidOperationException(
                        "Could not find getParameter method");
    
                var parameter = mi.Invoke(familyParameter,
                    new object[] { }) as Parameter;
    
                return parameter.IsShared;
            }
        }
}
