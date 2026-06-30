using Autodesk.Revit.DB;
using System.IO;

namespace BuildingCoder
{
    internal static partial class Util
    {
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
            return solid == null
                ? null
                : BooleanOperationsUtils
                .ExecuteBooleanOperation(solid, solid,
                    BooleanOperationsType.Union);
        }

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
