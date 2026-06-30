#region Namespaces

using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

#endregion // Namespaces

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_PurgeLineStyles sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Purge all graphic styles whose name contains
        ///     the given substring. Watch out what you do!
        ///     If your substring is empty, this might delete
        ///     all graphic styles in the entire project!
        /// </summary>
        public static void PurgeGraphicStyles(
            Document doc,
            string name_substring)
        {
            var graphic_styles
                = new FilteredElementCollector(doc)
                    .OfClass(typeof(GraphicsStyle));

            var n1 = graphic_styles.Count();

            var red_line_styles
                = graphic_styles.Where(e
                    => e.Name.Contains(name_substring));

            var n2 = red_line_styles.Count();

            if (0 < n2)
            {
                using var tx = new Transaction(doc);
                tx.Start("Delete Line Styles");

                doc.Delete(red_line_styles
                    .Select(e => e.Id)
                    .ToArray());

                tx.Commit();

                TaskDialog.Show("Purge line styles",
                    $"Deleted {n2} graphic style{(1 == n2 ? "" : "s")} named '*{name_substring}*' from {n1} total graohic styles.");
            }
        }
    }
}
