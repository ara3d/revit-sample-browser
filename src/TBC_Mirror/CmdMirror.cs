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

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

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

            using Transaction t = new(doc);
            t.Start("Mirror Elements");

            ElementTransformUtils.MirrorElements(
                doc, elementIds, plane, true);

            t.Commit();

            return Result.Succeeded;
        }
    }


}