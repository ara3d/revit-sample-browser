using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ExportIfc sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Export current view to IFC.
        /// </summary>
        internal static Result ExportToIfc(Document doc)
        {
            var r = Result.Failed;

            using var tx = new Transaction(doc);
            tx.Start("Export IFC");

            var desktop_path = Environment.GetFolderPath(
                Environment.SpecialFolder.Desktop);

            IFCExportOptions opt = null;

            doc.Export(desktop_path, doc.Title, opt);

            tx.RollBack();

            r = Result.Succeeded;

            return r;
        }
    }
}
