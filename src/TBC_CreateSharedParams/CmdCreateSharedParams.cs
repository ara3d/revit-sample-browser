#region Header

//
// CmdSharedParamModelGroup.cs - create a shared
// parameter for the doors, walls, inserted DWG,
// model groups, and model lines.
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
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdCreateSharedParams : IExternalCommand
    {
        private readonly BuiltInCategory[] targets =
        {
            BuiltInCategory.OST_Doors,
            BuiltInCategory.OST_Walls,
            BuiltInCategory.OST_IOSModelGroups,
            BuiltInCategory.OST_Lines
        };

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            using var t = new Transaction(doc);
            t.Start("Create Shared Parameter");
            Category cat;
            var i = 0;

            foreach (var target in targets)
            {
                cat = Util.GetCategoryForSharedParam(doc, target);
                if (null != cat) Util.CreateSharedParameter(doc, cat, ++i, false);
            }

            cat = Util.GetCategoryForSharedParam(doc, BuiltInCategory.OST_Walls);
            Util.CreateSharedParameter(doc, cat, ++i, true);
            t.Commit();

            return Result.Succeeded;
        }
    }
}
