#region Header

//
// CmdChangeElementColor.cs - Change element colour using OverrideGraphicSettings for active view
//
// Also change its category's material to a random material
//
// Copyright (C) 2020-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    public class CmdChangeElementColor : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            ElementId id;

            try
            {
                var sel = uidoc.Selection;
                var r = sel.PickObject(
                    ObjectType.Element,
                    "Pick element to change its colour");
                id = r.ElementId;
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }

            Util.ChangeElementColor(doc, id);

            Util.ChangeElementMaterial(doc, id);

            return Result.Succeeded;
        }
    }
}
