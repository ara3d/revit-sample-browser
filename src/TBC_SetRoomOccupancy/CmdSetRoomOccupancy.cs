#region Header

//
// CmdSetRoomOccupancy.cs - read and set room occupancy
//
// Copyright (C) 2009-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdSetRoomOccupancy : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            var rooms = new List<Element>();
            if (!Util.GetSelectedElementsOrAll(
                rooms, uidoc, typeof(Room)))
            {
                var sel = uidoc.Selection;
                message = 0 < sel.GetElementIds().Count
                    ? "Please select some room elements."
                    : "No room elements found.";
                return Result.Failed;
            }

            using var t = new Transaction(doc);
            t.Start("Bump Room Occupancy");

            foreach (Room room in rooms) Util.BumpOccupancy(room);
            t.Commit();

            return Result.Succeeded;
        }
    }
}
