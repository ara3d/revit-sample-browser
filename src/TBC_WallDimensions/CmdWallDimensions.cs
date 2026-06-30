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
    /// <summary>
    ///     List dimensions for a quadrilateral wall with
    ///     openings. In this algorithm, we collect all
    ///     the faces with parallel normal vectors and
    ///     calculate the maximal distance between any
    ///     two pairs of them. This is the wall dimension
    ///     in that direction.
    /// </summary>
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
