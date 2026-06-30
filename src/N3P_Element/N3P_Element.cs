using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.N3P_Element.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_Element : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var uidoc = data.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            var walls = N3PSelection.GetSelectedOrAll(uidoc, typeof(Wall));
            if (walls.Count == 0)
            {
                message = "No walls found. Select walls or open a model with walls.";
                return Result.Failed;
            }

            N3POutput.Header("Nice3point Element extensions");

            foreach (var wall in walls.Cast<Wall>())
            {
                N3POutput.Line($"Wall {wall.Id}");

                var typed = wall.Id.ToElement<Wall>(doc);
                N3POutput.Line("  ToElement<T>", typed?.Id);

                var offset = wall.FindParameter(ParameterTypeId.WallBaseOffset);
                if (offset != null)
                    N3POutput.Line("  FindParameter(WallBaseOffset)", offset.AsDouble().ToMillimeters());

                var comments = wall.FindParameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                N3POutput.Line("  FindParameter(BuiltInParameter)", comments?.AsString() ?? "(none)");

                N3POutput.Line("  CanBeDeleted", wall.CanBeDeleted);
            }

            return Result.Succeeded;
        }
    }
}
