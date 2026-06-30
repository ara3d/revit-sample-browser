using System.Linq;
using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;

namespace Ara3D.RevitSampleBrowser.N3P_Parameters.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_Parameters : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var doc = data.Application.ActiveUIDocument.Document;

            N3POutput.Header("Nice3point Parameter extensions");

            var bipId = BuiltInParameter.WALL_BASE_OFFSET.ToElementId();
            N3POutput.Line("BuiltInParameter.ToElementId()", bipId);

            var isParam = bipId.IsParameter(BuiltInParameter.WALL_BASE_OFFSET);
            N3POutput.Line("ElementId.IsParameter()", isParam);

            var wall = doc.CollectElements().OfClass<Wall>().FirstOrDefault() as Wall;
            if (wall == null)
            {
                message = "No walls found for parameter demo.";
                return Result.Failed;
            }

            var structural = wall.FindParameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT);
            if (structural != null)
                N3POutput.Line("AsBool (structural)", structural.AsBool());

            var structuralWallCount = doc.CollectElements()
                .OfClass<Wall>()
                .WhereParameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT)
                .Equals(1)
                .Count();
            N3POutput.Line("WhereParameter.Equals(1) wall count", structuralWallCount);

            return Result.Succeeded;
        }
    }
}
