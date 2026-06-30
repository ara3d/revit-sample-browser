using Ara3D.RevitSampleBrowser.N3P_Shared.CS;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Nice3point.Revit.Extensions;
using System.Linq;

namespace Ara3D.RevitSampleBrowser.N3P_Category.CS
{
    [Transaction(TransactionMode.ReadOnly)]
    public class N3P_Category : IExternalCommand
    {
        public Result Execute(ExternalCommandData data, ref string message, ElementSet elements)
        {
            var doc = data.Application.ActiveUIDocument.Document;

            N3POutput.Header("Nice3point Category extensions");

            var wallCategory = BuiltInCategory.OST_Walls.ToCategory(doc);
            N3POutput.Line("BuiltInCategory.ToCategory()", wallCategory?.Name);

            var wallCatId = BuiltInCategory.OST_Walls.ToElementId();
            N3POutput.Line("BuiltInCategory.ToElementId()", wallCatId);

            var wall = doc.CollectElements().OfClass<Wall>().FirstOrDefault();
            if (wall != null)
            {
                N3POutput.Line("CategoryId.IsCategory(Walls)",
                    wall.Category.Id.IsCategory(BuiltInCategory.OST_Walls));
                N3POutput.Line("CategoryId.AreEquals(Walls)",
                    wall.Category.Id.AreEquals(BuiltInCategory.OST_Walls));
            }

            return Result.Succeeded;
        }
    }
}
