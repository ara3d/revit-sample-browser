#region Header

//
// CmdSortCurveLoops.cs - Retrieve and sort outer and inner face curve loops
//
// Copyright (C) 2021 by stenci and Jeremy Tammik,
// https://github.com/stenci and Autodesk Inc. 
// All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//

#endregion // Header

#region Namespaces

using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    public class CmdSortCurveLoops : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var app = commandData.Application;
            var uidoc = app.ActiveUIDocument;
            var doc = uidoc.Document;

            Face face;
            try
            {
                var pickedObject = uidoc.Selection.PickObject(ObjectType.Face, "Select a face");
                var element = doc.GetElement(pickedObject);
                face = element.GetGeometryObjectFromReference(pickedObject) as Face;
            }
            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }

            using var tx = new Transaction(doc);
            tx.Start("Sort and Mark Face Curve Loops");

            var lists = Util.SortCurveLoops(face);

            for (var i = 0; i < lists.Count; i++)
            for (var j = 0; j < lists[i].Count; j++)
            {
                var loop = lists[i][j];
                Creator.CreateTextNote($"[{i}][{j}]", loop.First().Evaluate(0.33, true), doc);
            }

            tx.Commit();

            return Result.Succeeded;
        }
    }
}
