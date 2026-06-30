#region Header

//
// CmdBoundingBox.cs - eplore element bounding box
//
// Copyright (C) 2008-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdBoundingBox : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            var e = Util.SelectSingleElement(
                uidoc, "an element");

            if (null == e)
            {
                message = "No element selected";
                return Result.Failed;
            }

            // Trying to call this property returns the
            // compile time error: Property, indexer, or
            // event 'BoundingBox' is not supported by
            // the language; try directly calling
            // accessor method 'get_BoundingBox( View )'

            //BoundingBoxXYZ b = e.BoundingBox[null];

            View v = null;

            var b = e.get_BoundingBox(v);

            if (null == b)
            {
                v = commandData.View;
                b = e.get_BoundingBox(v);
            }

            if (null == b)
            {
                Util.InfoMsg(
                    $"{Util.ElementDescription(e)} has no bounding box.");
            }
            else
            {
                using var tx = new Transaction(doc);
                tx.Start("Draw Model Line Bounding Box Outline");

                Debug.Assert(b.Transform.IsIdentity,
                    "expected identity bounding box transform");

                var in_view = null == v
                    ? "model space"
                    : $"view {v.Name}";

                Util.InfoMsg(string.Format(
                    "Element bounding box of {0} in "
                    + "{1} extends from {2} to {3}.",
                    Util.ElementDescription(e),
                    in_view,
                    Util.PointString(b.Min),
                    Util.PointString(b.Max)));

                var creator = new Creator(doc);

                creator.DrawPolygon(new List<XYZ>(
                    Util.GetBottomCorners(b)));

                var rotation = Transform.CreateRotation(
                    XYZ.BasisZ, 60 * Math.PI / 180.0);

                b = Util.RotateBoundingBox(b, rotation);

                Util.InfoMsg(string.Format(
                    "Bounding box rotated by 60 degrees "
                    + "extends from {0} to {1}.",
                    Util.PointString(b.Min),
                    Util.PointString(b.Max)));

                creator.DrawPolygon(new List<XYZ>(
                    Util.GetBottomCorners(b)));

                tx.Commit();
            }

            return Result.Succeeded;
        }
    }
}