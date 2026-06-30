#region Header

//
// CmdColumnRound.cs - determine whether a
// selected column instance is cylindrical
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
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdColumnRound : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            var rc = Result.Failed;

            var column = Util.SelectSingleElementOfType(
                uidoc, typeof(FamilyInstance), "column", true);

            if (null == column || !Util.IsColumn(column))
            {
                message = "Please select a single column instance";
            }
            else
            {
                var opt = app.Application.Create.NewGeometryOptions();
                var geo = column.get_Geometry(opt);
                GeometryInstance i = null;

                foreach (var obj in geo)
                {
                    i = obj as GeometryInstance;
                    if (null != i) break;
                }

                if (null == i)
                {
                    message = "Unable to obtain geometry instance";
                }
                else
                {
                    var isCylindrical = false;
                    geo = i.SymbolGeometry;

                    foreach (var obj in geo)
                    {
                        var solid = obj as Solid;
                        if (null != solid)
                            foreach (Face face in solid.Faces)
                                if (face is CylindricalFace)
                                {
                                    isCylindrical = true;
                                    break;
                                }
                    }

                    message = $"Selected column instance is{(isCylindrical ? "" : " NOT")} cylindrical";
                }

                rc = Result.Succeeded;
            }

            return rc;
        }
    }
}
