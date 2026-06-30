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
using System.Collections.Generic;

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

            ElementClassFilter fFamInstClass = new(typeof(FamilyInstance));
            ElementCategoryFilter fDoorCat = new(BuiltInCategory.OST_Doors);
            ElementCategoryFilter fWindowCat = new(BuiltInCategory.OST_Windows);
            LogicalOrFilter fCat = new(fDoorCat, fWindowCat);
            LogicalAndFilter f = new(fCat, fFamInstClass);
            FilteredElementCollector openings = new(doc);
            openings.WherePasses(f);

            var ids = Util.GetHostedElementIds(doc, openings);

            Util.DumpHostedElements(doc, ids);

            return Result.Succeeded;
        }
    }
}
