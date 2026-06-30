#region Header

//
// CmdBrepBuilder.cs - create DirectShape using BrepBuilder and Boolean difference
//
// Copyright (C) 2018-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
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
    [Transaction(TransactionMode.Manual)]
    internal class CmdBrepBuilder : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            var brepBuilderSolid = Util.CreateBrepSolid();
            var brepBuilderVoid = Util.CreateBrepVoid();

            var cube = brepBuilderSolid.GetResult();
            var cylinder = brepBuilderVoid.GetResult();

            var difference
                = BooleanOperationsUtils.ExecuteBooleanOperation(
                    cube, cylinder, BooleanOperationsType.Difference);

            IList<GeometryObject> list = [difference];

            using Transaction tr = new(doc);
            tr.Start("Create a DirectShape");

            var ds = DirectShape.CreateElement(doc,
                new ElementId(BuiltInCategory.OST_GenericModel));

            ds.SetShape(list);

            tr.Commit();

            return Result.Succeeded;
        }
    }
}
