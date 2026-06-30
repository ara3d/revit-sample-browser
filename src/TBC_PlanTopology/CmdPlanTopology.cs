#region Header

//
// CmdPlanTopology.cs - test PlanTopology class
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
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    internal class CmdPlanTopology : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var doc = app.ActiveUIDocument.Document;

            var levels = Util.GetElementsOfType(
                doc, typeof(Level), BuiltInCategory.OST_Levels);

            var level = levels.FirstElement() as Level;

            var pt = doc.get_PlanTopology(level);

            var output = $"Rooms on {level.Name}:\n  Name and Number : Area";

            foreach (var id in pt.GetRoomIds())
            {
                var r = doc.GetElement(id) as Room;

                output += $"\n  {r.Name} : {Util.RealString(r.Area)} sqf";
            }

            Util.InfoMsg(output);

            output = "Circuits without rooms:"
                     + "\n  Number of Sides : Area";

            using (Transaction t = new(doc))
            {
                t.Start("Create New Rooms");

                foreach (PlanCircuit pc in pt.Circuits)
                    if (!pc.IsRoomLocated)
                    {
                        output += $"\n  {pc.SideNum} : {Util.RealString(pc.Area)} sqf";

                        doc.Create.NewRoom(null, pc);
                    }

                t.Commit();
            }

            Util.InfoMsg(output);

            return Result.Succeeded;
        }
    }
}
