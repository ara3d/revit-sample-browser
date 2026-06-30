#region Header

//
// CmdSheetSize.cs - list title block element types and title block and view sheet instances and sizes
//
// Copyright (C) 2010-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdSheetSize : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            FilteredElementCollector a;
            Parameter p;
            int n;

            a = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .OfClass(typeof(FamilySymbol));

            n = a.ToElementIds().Count;

            Debug.Print("{0} title block element type{1} "
                        + "retrieved by filtered element collector{2}",
                n,
                1 == n ? "" : "s",
                0 == n ? "." : ":");

            foreach (FamilySymbol symbol in a)
                Debug.Print(
                    "Title block element type {0} {1}",
                    symbol.Name, symbol.Id.Value);

            a = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .OfClass(typeof(FamilyInstance));

            Debug.Print("Title block instances:");

            foreach (FamilyInstance e in a)
            {
                p = e.get_Parameter(
                    BuiltInParameter.SHEET_NUMBER);

                Debug.Assert(null != p,
                    "expected valid sheet number");

                var sheet_number = p.AsString();

                p = e.get_Parameter(
                    BuiltInParameter.SHEET_WIDTH);

                Debug.Assert(null != p,
                    "expected valid sheet width");

                var swidth = p.AsValueString();
                var width = p.AsDouble();

                p = e.get_Parameter(
                    BuiltInParameter.SHEET_HEIGHT);

                Debug.Assert(null != p,
                    "expected valid sheet height");

                var sheight = p.AsValueString();
                var height = p.AsDouble();

                var typeId = e.GetTypeId();
                var type = doc.GetElement(typeId);

                Debug.Print(
                    "Sheet number {0} size is {1} x {2} "
                    + "({3} x {4}), id {5}, type {6} {7}",
                    sheet_number, swidth, sheight,
                    Util.RealString(width),
                    Util.RealString(height),
                    e.Id.Value,
                    type.Name, typeId.Value);
            }

            a = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet));

            Debug.Print("View sheet instances:");

            foreach (ViewSheet vs in a)
            {
                var number = vs.SheetNumber;
                Debug.Print(
                    "View sheet name {0} number {1} id {2}",
                    vs.Name, vs.SheetNumber,
                    vs.Id.Value);
            }

            return Result.Succeeded;
        }
    }
}
