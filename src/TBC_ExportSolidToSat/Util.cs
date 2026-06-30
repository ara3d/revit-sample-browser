using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.DB;

namespace BuildingCoder
{
    /// <summary>Utilities extracted from TBC_ExportSolidToSat sample.</summary>
    internal static partial class Util
    {
        /// <summary>
        ///     Faster intersected-solid area between host and linked element.
        /// </summary>
        public static double GetIntersectedSolidArea(
            Document host,
            Solid hostElement,
            RevitLinkInstance rins,
            Solid linkedElement)
        {
            var transForm = rins.GetTransform();
            var tmp = SolidUtils.CreateTransformed(linkedElement, transForm);

            var result = BooleanOperationsUtils.ExecuteBooleanOperation(
                hostElement, tmp, BooleanOperationsType.Intersect);

            return result.SurfaceArea;
        }

        /// <summary>
        ///     Clone Solid workaround before Revit 2016 Solid.Clone API.
        /// </summary>
        public static Solid CloneSolid(Solid solid)
        {
            if (solid == null) return null;

            return BooleanOperationsUtils
                .ExecuteBooleanOperation(solid, solid,
                    BooleanOperationsType.Union);
        }

        /// <summary>
        ///     Return the full path of the first file found matching
        ///     the given filename pattern in a recursive search.
        /// </summary>
        internal static string DirSearch(
            string start_dir,
            string filename_pattern)
        {
            foreach (var d in Directory.GetDirectories(start_dir))
            {
                foreach (var f in Directory.GetFiles(d, filename_pattern))
                    return f;

                var f2 = DirSearch(d, filename_pattern);

                if (null != f2) return f2;
            }

            return null;
        }
    }
}
