#region Header

//
// CmdNewRailing.cs - insert a new railing instance,
// in response to queries from Berria at
// http://thebuildingcoder.typepad.com/blog/2009/02/list-railing-types.html#comments
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>
    ///     Currently, it is not possible to create a new railing instance:
    ///     http://thebuildingcoder.typepad.com/blog/2009/02/list-railing-types.html#comments
    ///     SPR #134260 [API - New Element Creation: Railing]
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    internal class CmdNewRailing : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            var levels = Util.GetElementsOfType(
                doc, typeof(Level), BuiltInCategory.OST_Levels);

            if (levels.FirstElement() is not Level level)
            {
                message = "No level found.";
                return Result.Failed;
            }

            BuiltInCategory bic;
            Type t;

            // Baluster FamilySymbols cannot create railings; OST_StairsRailing yields ElementType, not FamilySymbol.

            bic = BuiltInCategory.OST_StairsRailingBaluster;
            t = typeof(FamilySymbol);

            bic = BuiltInCategory.OST_StairsRailing;
            t = typeof(ElementType);

            var symbols
                = Util.GetElementsOfType(doc, t, bic);

            FamilySymbol sym = null;

            foreach (ElementType s in symbols)
            {
                Debug.Print(
                    "Family name={0}, symbol name={1},"
                    + " category={2}",
                    s is not FamilySymbol fs ? "<none>" : fs.Family.Name,
                    s.Name,
                    s.Category.Name);

                if (null == sym && s is not null)
                    sym = s as FamilySymbol;
            }

            if (null == sym)
            {
                message = "No railing family symbols found.";
                return Result.Failed;
            }

            using Transaction tx = new(doc);
            tx.Start("Create New Railing");

            XYZ p1 = new(17, 0, 0);
            XYZ p2 = new(33, 0, 0);
            var line = Line.CreateBound(p1, p2);


            var Railing1
                = doc.Create.NewFamilyInstance(
                    line, sym, level, StructuralType.NonStructural);

            tx.Commit();

            return Result.Succeeded;
        }
    }
}