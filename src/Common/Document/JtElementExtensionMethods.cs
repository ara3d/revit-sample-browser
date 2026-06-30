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
    public static class JtElementExtensionMethods
        {
            /// <summary>
            ///     Predicate to determine whether given element
            ///     is a physical element, i.e. valid category,
            ///     not view specific, etc.
            /// </summary>
            public static bool IsPhysicalElement(
                this Element e)
            {
                if (e.Category == null)
                    return false;
                // does this produce same result as 
                // WhereElementIsViewIndependent ?
                if (e.ViewSpecific)
                    return false;
                // exclude specific unwanted categories
                if ((BuiltInCategory) e.Category.Id.Value
                    == BuiltInCategory.OST_HVAC_Zones)
                    return false;
                return e.Category.CategoryType == CategoryType.Model
                       && e.Category.CanAddSubcategory;
            }
    
            /// <summary>
            ///     Return the curve from a Revit database Element
            ///     location curve, if it has one.
            /// </summary>
            public static Curve GetCurve(this Element e)
            {
                Debug.Assert(null != e.Location,
                    "expected an element with a valid Location");
    
                var lc = e.Location as LocationCurve;
    
                Debug.Assert(null != lc,
                    "expected an element with a valid LocationCurve");
    
                return lc.Curve;
            }
        }
}
