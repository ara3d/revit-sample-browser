#region Header

//
// CmdWallDimensions.cs - determine wall dimensions
// by iterating over wall geometry faces
//
// Copyright (C) 2008-2021 by Jeremy Tammik,
// Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    // Max distance between parallel face pairs in each normal direction.
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdWallDimensions : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;

            var msg = string.Empty;

            var walls = new List<Element>();

            if (Util.GetSelectedElementsOrAll(walls, uidoc, typeof(Wall)))
                foreach (Wall wall in walls)
                    msg += Util.ProcessWallDimensions(wall);

            if (0 == msg.Length) msg = "Please select some walls.";

            Util.InfoMsg(msg);

            return Result.Succeeded;
        }
    }
}
