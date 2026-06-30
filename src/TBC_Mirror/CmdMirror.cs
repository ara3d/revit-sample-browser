#region Header

//
// CmdMirror.cs - mirror some elements.
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdMirror : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;

            var app = uiapp.Application;
            var doc = uidoc.Document;

            // Document.Mirror is obsolete; use ElementTransformUtils.MirrorElements.

            var plane = Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero);

            var elementIds
                = uidoc.Selection.GetElementIds();

            using var t = new Transaction(doc);
            t.Start("Mirror Elements");

            ElementTransformUtils.MirrorElements(
                doc, elementIds, plane, true);

            t.Commit();

            return Result.Succeeded;
        }
    }


}