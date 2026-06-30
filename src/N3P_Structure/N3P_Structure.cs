using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;
using System.Linq;
using Nice3point.Revit.Extensions.Structure;

namespace Ara3D.RevitSampleBrowser.N3P_Structure.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_Structure : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var doc = data.Application.ActiveUIDocument.Document;


            if (doc.CollectElements()
                .OfClass<FamilyInstance>()
                .OfStructuralType(StructuralType.Beam)
                .FirstOrDefault() is FamilyInstance framing)
            {
                N3POutput.Header("Nice3point Structural framing extensions");
                N3POutput.Line($"Beam {framing.Id}");
                N3POutput.Line("  CanFlipFramingEnds", framing.CanFlipFramingEnds);
                N3POutput.Line("  IsFramingJoinAllowedAtEnd(0)",
                    framing.IsFramingJoinAllowedAtEnd(0));
                return Result.Succeeded;
            }

            if (doc.CollectElements().OfClass<RebarShape>().FirstOrDefault() is RebarShape rebarShape)
            {
                N3POutput.Header("Nice3point Rebar shape extensions");
                var parameters = rebarShape.GetAllParameters();
                N3POutput.Line($"RebarShape '{rebarShape.Name}' parameters", parameters.Count);
                return Result.Succeeded;
            }

            message = "No structural framing or rebar shapes found.";
            return Result.Failed;
        }
    }
}
