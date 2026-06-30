using System.Linq;
using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;

namespace Ara3D.RevitSampleBrowser.N3P_Unit.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_Unit : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var uidoc = data.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            var wall = N3PSelection.GetSelectedOrAll(uidoc, typeof(Wall)).Cast<Wall>().FirstOrDefault();
            if (wall == null)
            {
                message = "No walls found for unit conversion demo.";
                return Result.Failed;
            }

            N3POutput.Header("Nice3point Unit extensions");

            var internalFeet = wall.FindParameter(ParameterTypeId.WallBaseOffset)?.AsDouble() ?? 0;
            N3POutput.Line("Internal (feet)", internalFeet);
            N3POutput.Line("ToMillimeters().Round()", internalFeet.ToMillimeters().Round(2));
            N3POutput.Line("ToMeters().Round(3)", internalFeet.ToMeters().Round(3));

            var sampleMm = 2100d;
            N3POutput.Line($"{sampleMm} mm FromMillimeters()", sampleMm.FromMillimeters().Round(4));

            var units = doc.GetUnits();
            var formatted = units.Format(SpecTypeId.Length, internalFeet, forEditing: false);
            N3POutput.Line("GetUnits().Format(Length)", formatted);

            return Result.Succeeded;
        }
    }
}
