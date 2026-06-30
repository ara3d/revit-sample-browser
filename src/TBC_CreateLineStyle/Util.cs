using System.Linq;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_CreateLineStyle sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Create a new line style using NewSubcategory
        /// </summary>
        internal static void CreateLineStyle(Document doc)
        {
            var fec
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(LinePatternElement));

            var linePatternElem = fec
                .Cast<LinePatternElement>()
                .First(linePattern
                    => linePattern.Name == "Long Dash");

            var categories = doc.Settings.Categories;

            var lineCat = categories.get_Item(
                BuiltInCategory.OST_Lines);

            using var t = new Transaction(doc);
            t.Start("Create LineStyle");

            var newLineStyleCat = categories
                .NewSubcategory(lineCat, "New LineStyle");

            doc.Regenerate();

            newLineStyleCat.SetLineWeight(8,
                GraphicsStyleType.Projection);

            newLineStyleCat.LineColor = new Color(
                0xFF, 0x00, 0x00);

            newLineStyleCat.SetLinePatternId(
                linePatternElem.Id,
                GraphicsStyleType.Projection);

            t.Commit();
        }
    }
}
