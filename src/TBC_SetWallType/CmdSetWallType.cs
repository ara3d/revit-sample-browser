#region Header

//
// CmdSetWallType.cs - Set the wall type of a selected wall
//
// This answers the Revit API discussion forum thread
// https://forums.autodesk.com/t5/revit-api/change-the-selection-of-a-wall/m-p/5890510
//
// Copyright (C) 2015-2021 by Jeremy Tammik, 
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
using System.Linq;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdSetWallType : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;

            var wall_picked = Util.SelectSingleElementOfType(
                uidoc, typeof(Wall), "wall", true);

            ElementId wall_id = new((long)25122);
            wall_id = wall_picked.Id;

            List<ElementId> ids = new(1)
            {
                wall_id
            };

            var wall
                = new FilteredElementCollector(doc, ids)
                    .OfClass(typeof(Wall))
                    .FirstElement() as Wall;

            var wallType
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(WallType))
                    .Cast<WallType>()
                    .FirstOrDefault();

            using Transaction t = new(doc);
            t.Start("Change Wall Type");
            wall.WallType = wallType;
            t.Commit();

            return Result.Succeeded;
        }
    }
}
