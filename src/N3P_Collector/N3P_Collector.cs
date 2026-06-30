using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;
using System.Collections.Generic;

namespace Ara3D.RevitSampleBrowser.N3P_Collector.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_Collector : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var doc = data.Application.ActiveUIDocument.Document;

            N3POutput.Header("Nice3point FilteredElementCollector extensions");

            var walls = doc.CollectElements()
                .OfClass<Wall>()
                .ToElements();
            N3POutput.Line("OfClass<Wall>()", walls.Count);

            var wallsAndFloors = doc.CollectElements()
                .OfClasses(typeof(Wall), typeof(Floor))
                .ToElements();
            N3POutput.Line("OfClasses(Wall, Floor)", wallsAndFloors.Count);

            var rooms = doc.CollectElements()
                .Rooms()
                .ToElements();
            N3POutput.Line("Rooms()", rooms.Count);

            var noGrids = doc.CollectElements()
                .ExcludingCategory(BuiltInCategory.OST_Grids)
                .OfCategories(BuiltInCategory.OST_Walls, BuiltInCategory.OST_Floors)
                .ToElements();
            N3POutput.Line("OfCategories minus grids context", noGrids.Count);

            return Result.Succeeded;
        }
    }
}
