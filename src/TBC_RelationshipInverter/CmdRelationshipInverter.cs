#region Header

//
// CmdRelationshipInverter.cs
//
// Determine door and window to wall relationships,
// i.e. hosted --> host, and invert it to obtain
// a map host --> list of hosted elements.
//
// Copyright (C) 2008-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdRelationshipInverter : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            var fFamInstClass = new ElementClassFilter(typeof(FamilyInstance));
            var fDoorCat = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            var fWindowCat = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            var fCat = new LogicalOrFilter(fDoorCat, fWindowCat);
            var f = new LogicalAndFilter(fCat, fFamInstClass);
            var openings = new FilteredElementCollector(doc);
            openings.WherePasses(f);

            var ids = Util.GetHostedElementIds(doc, openings);

            Util.DumpHostedElements(doc, ids);

            return Result.Succeeded;
        }
    }
}
