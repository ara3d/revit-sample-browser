#region Header

//
// CmdWallLayerVolumes.cs - determine
// compound wall layer volumes
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
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.ReadOnly)]
    internal class CmdWallLayerVolumes : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            List<Element> walls = new();
            if (!Util.GetSelectedElementsOrAll(
                walls, uidoc, typeof(Wall)))
            {
                var sel = uidoc.Selection;
                message = 0 < sel.GetElementIds().Count
                    ? "Please select some wall elements."
                    : "No wall elements found.";
                return Result.Failed;
            }

            Util.MapLayerToVolume totalVolumes
                = new();

            foreach (Wall wall in walls) Util.GetWallLayerVolumes(wall, ref totalVolumes);

            var msg
                = "Compound wall layer volumes formatted as '"
                  + "wall type : layer function :"
                  + " volume in cubic meters':\n";

            List<string> keys = new(
                totalVolumes.Keys);

            keys.Sort();

            foreach (var key in keys)
                msg += $"\n{key} : {Util.RealString(Util.CubicFootToCubicMeter(totalVolumes[key]))}";

            Util.InfoMsg(msg);

            return Result.Cancelled;
        }
    }
}
