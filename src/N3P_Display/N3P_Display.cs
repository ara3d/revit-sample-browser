using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;

namespace Ara3D.RevitSampleBrowser.N3P_Display.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_Display : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            N3POutput.Header("Nice3point ForgeTypeId, Label, and Color extensions");

            N3POutput.Line("UnitTypeId.Millimeters ToUnitLabel()",
                UnitTypeId.Millimeters.ToUnitLabel());
            N3POutput.Line("SpecTypeId.Length ToSpecLabel()",
                SpecTypeId.Length.ToSpecLabel());
            N3POutput.Line("BuiltInParameter.WALL_BASE_OFFSET ToLabel()",
                BuiltInParameter.WALL_BASE_OFFSET.ToLabel());
            N3POutput.Line("BuiltInCategory.OST_Walls ToLabel()",
                BuiltInCategory.OST_Walls.ToLabel());

            Color red = new(255, 0, 0);
            N3POutput.Line("Color.ToHex()", red.ToHex());
            N3POutput.Line("Color.ToRgb()", red.ToRgb());

            return Result.Succeeded;
        }
    }
}
