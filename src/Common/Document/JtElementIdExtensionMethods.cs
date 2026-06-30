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
    public static class JtElementIdExtensionMethods
        {
            /// <summary>
            ///     Predicate returning true for invalid element ids.
            /// </summary>
            public static bool IsInvalid(this ElementId id)
            {
                return ElementId.InvalidElementId == id;
            }
    
            /// <summary>
            ///     Predicate returning true for valid element ids.
            /// </summary>
            public static bool IsValid(this ElementId id)
            {
                return !IsInvalid(id);
            }
        }
}
