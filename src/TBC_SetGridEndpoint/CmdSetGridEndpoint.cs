#region Header

//
// CmdSetGridEndpoint.cs - move selected grid endpoints in Y direction using SetCurveInView
//
// Copyright (C) 2018-2020 by Ryuji Ogasawara and Jeremy Tammik, Autodesk Inc. All rights reserved.
//
// Keywords: The Building Coder Revit API C# .NET add-in.
//
// Written by Ryuji Ogasawara.
//

#endregion // Header

#region Namespaces

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion // Namespaces

namespace BuildingCoder
{
    [Transaction(TransactionMode.Manual)]
    public class CmdSetGridEndpoint : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uiapp = commandData.Application;
            var uidoc = uiapp.ActiveUIDocument;
            var doc = uidoc.Document;
            var sel = uidoc.Selection;
            var view = doc.ActiveView;

            ISelectionFilter f
                = new JtElementsOfClassSelectionFilter<Grid>();

            var elemRef = sel.PickObject(
                ObjectType.Element, f, "Pick a grid");

            var grid = doc.GetElement(elemRef) as Grid;

            var gridCurves = grid.GetCurvesInView(
                DatumExtentType.Model, view);

            using var tx = new Transaction(doc);
            tx.Start("Modify Grid Endpoints");

            foreach (var c in gridCurves)
            {
                var start = c.GetEndPoint(0);
                var end = c.GetEndPoint(1);

                var newStart = start + 10 * XYZ.BasisY;
                var newEnd = end - 10 * XYZ.BasisY;

                var newLine = Line.CreateBound(newStart, newEnd);

                grid.SetCurveInView(
                    DatumExtentType.Model, view, newLine);
            }

            tx.Commit();

            return Result.Succeeded;
        }
    }
}
