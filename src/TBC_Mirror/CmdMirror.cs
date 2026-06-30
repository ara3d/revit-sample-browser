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

            // 'Document.Mirror(ElementSet, Line)' is obsolete:
            // Use one of the replace methods in ElementTransformUtils.
            //
            //Line line = app.Create.NewLine(
            //  XYZ.Zero, XYZ.BasisX, true ); // 2011
            //
            //ElementSet els = uidoc.Selection.Elements; // 2011
            //
            //doc.Mirror( els, line ); // 2011

            //Plane plane = new Plane( XYZ.BasisY, XYZ.Zero ); // added in 2012, used until 2016
            var plane = Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero); // 2017

            var elementIds
                = uidoc.Selection.GetElementIds(); // 2012

            //ElementTransformUtils.MirrorElements(
            //  doc, elementIds, plane ); // 2012-2015

            using var t = new Transaction(doc);
            t.Start("Mirror Elements");

            ElementTransformUtils.MirrorElements(
                doc, elementIds, plane, true); // 2016

            t.Commit();

            return Result.Succeeded;
        }
    }


}