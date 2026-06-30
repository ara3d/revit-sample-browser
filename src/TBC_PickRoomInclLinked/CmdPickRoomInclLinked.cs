#region Header

//
// CmdPickRoomInclLinked.cs - selection filter to pick a room either in current project or linked model
//
// Copyright (C) 2020-2021 by Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    public class CmdPickRoomInclLinked : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            Reference r;

            var filter
                = new ElementInLinkSelectionFilter<Room>(
                    doc);

            try
            {
                r = uidoc.Selection.PickObject(
                    ObjectType.PointOnElement,
                    filter,
                    "Please pick a room in current project or linked model");
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }

            Element e;

            if (filter.LastCheckedWasFromLink)
                e = filter.LinkedDocument.GetElement(
                    r.LinkedElementId);
            else
                e = doc.GetElement(r);

            TaskDialog.Show("Picked", e.Name);

            return Result.Succeeded;
        }
    }
}
